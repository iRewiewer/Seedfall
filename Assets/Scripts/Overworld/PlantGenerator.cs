using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
	[Header("General")]
	[SerializeField] private Material material;

	[Header("Plant Generation")]
	[SerializeField] private float step = 1f;
	[SerializeField] private float thickness = 0.08f;

	[Header("Fake 3D")]
	[SerializeField] private int sliceCount = 2;
	[SerializeField] private float sliceRotationStep = 90f;

	private Mesh cylinderMesh;

	private readonly List<LSystemDefinition> plantSystems = new List<LSystemDefinition>()
	{
		new LSystemDefinition(
			"Plant_A",
			"F",
			4,
			22.5f,
			new Dictionary<char, string>
			{
				{ 'F', "FF-[-F+F+F]+[+F-F-F]" }
			}
		),
		new LSystemDefinition(
			"Plant_B",
			"F",
			4,
			25f,
			new Dictionary<char, string>
			{
				{ 'F', "F[+F]F[-F][F]" }
			}
		),
		new LSystemDefinition(
			"Plant_C",
			"F",
			5,
			20f,
			new Dictionary<char, string>
			{
				{ 'F', "FF" },
				{ 'X', "F[+X][-X]FX" }
			}
		),
		new LSystemDefinition(
			"Plant_D",
			"X",
			5,
			22.5f,
			new Dictionary<char, string>
			{
				{ 'X', "F[+X]F[-X]+X" },
				{ 'F', "FF" }
			}
		)
	};

	private struct TurtleState
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public TurtleState(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}

	private class LSystemDefinition
	{
		public string Name { get; }
		public string Axiom { get; }
		public int Iterations { get; }
		public float Angle { get; }
		public Dictionary<char, string> Rules { get; }

		public LSystemDefinition(string name, string axiom, int iterations, float angle, Dictionary<char, string> rules)
		{
			Name = name;
			Axiom = axiom;
			Iterations = iterations;
			Angle = angle;
			Rules = rules;
		}
	}

	private void Awake()
	{
		// reuse same primitive mesh for all segments to save memory and reduce GC pressure
		GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

		Collider collider = temp.GetComponent<Collider>();
		if (collider != null)
		{
			DestroyImmediate(collider);
		}

		cylinderMesh = temp.GetComponent<MeshFilter>().sharedMesh;
		DestroyImmediate(temp);
	}

	public GameObject GeneratePlant()
	{
		// pick random system
		LSystemDefinition selectedSystem = plantSystems[Random.Range(0, plantSystems.Count)];
		string sequence = GenerateLSystem(selectedSystem.Axiom, selectedSystem.Rules, selectedSystem.Iterations);

		// mesh merger
		List<CombineInstance> allCombineInstances = new List<CombineInstance>();

		for (int i = 0; i < sliceCount; i++)
		{
			float yRotation = i * sliceRotationStep;
			Quaternion sliceRotation = Quaternion.Euler(0f, yRotation, 0f);

			List<CombineInstance> sliceInstances = BuildSlice(
				sequence,
				selectedSystem.Angle,
				sliceRotation
			);

			allCombineInstances.AddRange(sliceInstances);
		}

		Mesh combinedMesh = new Mesh
		{
			name = $"PlantMesh_{selectedSystem.Name}",
			indexFormat= UnityEngine.Rendering.IndexFormat.UInt32
		};

		combinedMesh.CombineMeshes(allCombineInstances.ToArray(), true, true);
		combinedMesh.RecalculateBounds();
		combinedMesh.RecalculateNormals();

		GameObject root = new GameObject($"GeneratedPlant_{selectedSystem.Name}");

		MeshFilter meshFilter = root.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = root.AddComponent<MeshRenderer>();

		meshFilter.sharedMesh = combinedMesh;
		meshRenderer.sharedMaterial = material;

		return root;
	}

	private string GenerateLSystem(
		string axiom,
		Dictionary<char, string> rules,
		int iterations)
	{
		string current = axiom;

		for (int i = 0; i < iterations; i++)
		{
			StringBuilder next = new StringBuilder();

			foreach (char ch in current)
			{
				if (rules.TryGetValue(ch, out string replacement))
				{
					next.Append(replacement);
				}
				else
				{
					next.Append(ch);
				}
			}

			current = next.ToString();
		}

		return current;
	}

	private List<CombineInstance> BuildSlice(string sequence, float angle, Quaternion sliceRotation)
	{
		List<CombineInstance> combineInstances = new List<CombineInstance>();
		Stack<TurtleState> stack = new Stack<TurtleState>();

		Vector3 currentPosition = Vector3.zero;
		Quaternion currentRotation = sliceRotation * Quaternion.Euler(0f, 0f, 0f);

		foreach (char ch in sequence)
		{
			switch (ch)
			{
				case 'F':
					{
						Vector3 direction = currentRotation * Vector3.up;
						Vector3 nextPosition = currentPosition + direction * step;

						combineInstances.Add(
							CreateCylinderSegment(currentPosition, nextPosition, thickness)
						);

						currentPosition = nextPosition;
						break;
					}

				case '+':
					{
						currentRotation *= Quaternion.Euler(0f, 0f, angle);
						break;
					}

				case '-':
					{
						currentRotation *= Quaternion.Euler(0f, 0f, -angle);
						break;
					}

				case '[':
					{
						stack.Push(new TurtleState(currentPosition, currentRotation));
						break;
					}

				case ']':
					{
						if (stack.Count > 0)
						{
							TurtleState state = stack.Pop();
							currentPosition = state.Position;
							currentRotation = state.Rotation;
						}

						break;
					}
			}
		}

		return combineInstances;
	}

	private CombineInstance CreateCylinderSegment(Vector3 start, Vector3 end, float radius)
	{
		Vector3 delta = end - start;
		float length = delta.magnitude;

		if (length <= 0.0001f)
		{
			return new CombineInstance
			{
				mesh = cylinderMesh,
				transform = Matrix4x4.identity
			};
		}

		Vector3 midPoint = (start + end) * 0.5f;

		// translate, rotate, scale
		Matrix4x4 matrix = Matrix4x4.TRS(
			midPoint,
			Quaternion.FromToRotation(Vector3.up, delta.normalized),
			new Vector3(radius * 2f, length * 0.5f, radius * 2f)
		);

		return new CombineInstance
		{
			mesh = cylinderMesh,
			transform = matrix
		};
	}
}