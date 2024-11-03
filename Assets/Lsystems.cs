using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lsystems : MonoBehaviour
{
    [SerializeField] public int iterations;
    [SerializeField] public float angle;
    [SerializeField] public float length;
    [SerializeField] public GameObject branchPrefab;
    [SerializeField] public GameObject leafPrefab;

    [SerializeField] private InputField angleInputField;
    [SerializeField] private InputField lengthInputField;
    [SerializeField] private InputField iterationsInputField;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button growAllButton;
    [SerializeField] private Text angleValueText;
    [SerializeField] private Text lengthValueText;
    [SerializeField] private Text iterationsValueText;

    private string axiom = "X";
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;
    private List<TransformInfo> branchInstructions = new List<TransformInfo>();
    private int currentBranchIndex = 0;
    private List<GameObject> branchesAndLeaves = new List<GameObject>();

    private struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
        public bool isBranch;
    }

    void Start()
    {
        rules.Add('X', "F[-FXY][+FXY][FXY]");
        rules.Add('F', "FF");
        rules.Add('Y', "F[-L]");

        generateButton.onClick.AddListener(GenerateTree);
        growAllButton.onClick.AddListener(GrowAllBranches);

        angleInputField.text = angle.ToString();
        lengthInputField.text = length.ToString();
        iterationsInputField.text = iterations.ToString();

        iterationsInputField.onEndEdit.AddListener(delegate { UpdateIterations(); });
    }

    void UpdateIterations()
    {
        if (int.TryParse(iterationsInputField.text, out int newIterations))
        {
            iterations = newIterations;
            iterationsValueText.text = "Iterations: " + iterations.ToString();
        }
        else
        {
            Debug.LogError("Invalid input for iterations");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GrowNextBranch();
        }
    }

    void GenerateTree()
    {
        DestroyPreviousObjects();
        if (float.TryParse(angleInputField.text, out angle) && float.TryParse(lengthInputField.text, out length))
        {
            angleValueText.text = "Angle: " + angle.ToString("F2");
            lengthValueText.text = "Length: " + length.ToString("F2");
            iterationsValueText.text = "Iterations: " + iterations.ToString();

            currentString = axiom;
            for (int i = 0; i < iterations; i++)
            {
                currentString = GenerateNextString(currentString);
            }

            PrepareBranches(currentString);
        }
        else
        {
            Debug.LogError("Invalid input for angle or length");
        }
    }

    void DestroyPreviousObjects()
    {
        foreach (var obj in branchesAndLeaves)
        {
            Destroy(obj);
        }
        branchesAndLeaves.Clear();
        branchInstructions.Clear();
        currentBranchIndex = 0;
    }

    string GenerateNextString(string input)
    {
        string result = "";
        foreach (char c in input)
        {
            result += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return result;
    }

    void PrepareBranches(string input)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.up;

        foreach (char c in input)
        {
            if (c == 'F')
            {
                Vector3 newPosition = position + direction * length;
                branchInstructions.Add(new TransformInfo
                {
                    position = (position + newPosition) / 2,
                    direction = newPosition - position,
                    isBranch = true
                });
                position = newPosition;
            }
            else if (c == 'L')
            {
                branchInstructions.Add(new TransformInfo
                {
                    position = position,
                    direction = Vector3.zero,
                    isBranch = false
                });
            }
            else if (c == '+')
            {
                direction = Quaternion.Euler(Random.Range(-angle, angle), Random.Range(-angle, angle), Random.Range(-angle, angle)) * direction;
            }
            else if (c == '-')
            {
                direction = Quaternion.Euler(Random.Range(-angle, angle), Random.Range(-angle, angle), Random.Range(-angle, angle)) * direction;
            }
            else if (c == '[')
            {
                transformStack.Push(new TransformInfo { position = position, direction = direction });
            }
            else if (c == ']')
            {
                var ti = transformStack.Pop();
                position = ti.position;
                direction = ti.direction;
            }
        }
    }

    void GrowNextBranch()
    {
        if (currentBranchIndex < branchInstructions.Count)
        {
            TransformInfo info = branchInstructions[currentBranchIndex];
            InstantiateBranchOrLeaf(info);
            currentBranchIndex++;
        }
    }

    void GrowAllBranches()
    {
        while (currentBranchIndex < branchInstructions.Count)
        {
            TransformInfo info = branchInstructions[currentBranchIndex];
            InstantiateBranchOrLeaf(info);
            currentBranchIndex++;
        }
    }

    void InstantiateBranchOrLeaf(TransformInfo info)
    {
        GameObject newObject;
        if (info.isBranch)
        {
            newObject = Instantiate(branchPrefab);
            newObject.transform.position = info.position;
            newObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, info.direction);
            newObject.transform.localScale = new Vector3(1, info.direction.magnitude, 1);
        }
        else
        {
            newObject = Instantiate(leafPrefab);
            newObject.transform.position = info.position;
        }
        branchesAndLeaves.Add(newObject);
    }
}
