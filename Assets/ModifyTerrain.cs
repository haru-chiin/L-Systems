using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyTerrain : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int depth = 20;
    public float scale = 20f;
    public float brushSize = 5f;
    public float brushStrength = 0.1f;

    public InputField widthInputField;
    public InputField heightInputField;
    public InputField depthInputField;
    public InputField scaleInputField;
    public InputField brushSizeInputField;
    public InputField brushStrengthInputField;
    public Button generateButton;

    public Texture2D[] textures;
    private Terrain terrain;

    private void Start()
    {
        terrain = CreateTerrain();
        GenerateTerrain(terrain.terrainData);
        generateButton.onClick.AddListener(GenerateNewTerrain);
        SetDefaultInputValues();
    }

    private void SetDefaultInputValues()
    {
        widthInputField.text = width.ToString();
        heightInputField.text = height.ToString();
        depthInputField.text = depth.ToString();
        scaleInputField.text = scale.ToString();
        brushSizeInputField.text = brushSize.ToString();
        brushStrengthInputField.text = brushStrength.ToString();
    }

    private void GenerateNewTerrain()
    {
        width = int.Parse(widthInputField.text);
        height = int.Parse(heightInputField.text);
        depth = int.Parse(depthInputField.text);
        scale = float.Parse(scaleInputField.text);
        brushSize = float.Parse(brushSizeInputField.text);
        brushStrength = float.Parse(brushStrengthInputField.text);

        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        GenerateTerrain(terrainData);
    }

    Terrain CreateTerrain()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        return terrainObject.GetComponent<Terrain>();
    }

    void GenerateTerrain(TerrainData terrainData)
    {
        terrainData.SetHeights(0, 0, GenerateHeights());
        ApplyRandomTextures(terrainData);
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    void ApplyRandomTextures(TerrainData terrainData)
    {
        TerrainLayer[] terrainLayers = new TerrainLayer[textures.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            TerrainLayer layer = new TerrainLayer();
            layer.diffuseTexture = textures[i];
            terrainLayers[i] = layer;
        }

        terrainData.terrainLayers = terrainLayers;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ModifyHeight(brushStrength);
        }
        if (Input.GetMouseButton(1))
        {
            ModifyHeight(-brushStrength);
        }
    }

    void ModifyHeight(float strength)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 terrainPosition = hit.point;
            TerrainData terrainData = terrain.terrainData;
            int terrainX = (int)((terrainPosition.x / terrainData.size.x) * terrainData.heightmapResolution);
            int terrainZ = (int)((terrainPosition.z / terrainData.size.z) * terrainData.heightmapResolution);

            float[,] heights = terrainData.GetHeights(terrainX - (int)brushSize / 2, terrainZ - (int)brushSize / 2, (int)brushSize, (int)brushSize);
            for (int x = 0; x < brushSize; x++)
            {
                for (int z = 0; z < brushSize; z++)
                {
                    heights[x, z] += strength;
                }
            }

            terrainData.SetHeights(terrainX - (int)brushSize / 2, terrainZ - (int)brushSize / 2, heights);
        }
    }
}
