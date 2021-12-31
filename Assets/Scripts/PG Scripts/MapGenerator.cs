using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Plane size
    public int mapWidth;
    public int mapHeight;
    public enum DrawMode {NoiseMap, FogMap}
    public DrawMode drawMode;

    public Color fogColor;
    
    public AnimationCurve opacity;

    public AnimationCurve edgeFade;

    // Noise values
    public float noiseScale;
    public int octaves;
    [Range(0,1)] public float persistance;
    public float lacunarity;
    public Vector2 offset;

    // Random seed values
    public bool randomSeed;
    public int seed;
    public Renderer textureRenderer;
    
    // To allow fog to move
    public int delay;
    private int currentDelay;
    public int maxTextures;
    public float stepSize;
    private bool right;
    private int position = 0;

    private Texture2D[] textures;
    public bool editMode;

    // Start is called before the first frame update
    void Start()
    {
        if (randomSeed)
        {
            seed = Random.Range(0,1000);
        }
        if (editMode)
        {
            GenerateMap();
        }
        else
        {
            textures = new Texture2D[maxTextures];
            GenerateMaps();
            right = true;
            currentDelay = delay;
            DrawMap(textures[position]);
        }
    }
        

    // Update is called once per frame
    void Update()
    {
        if (editMode)
        {
            GenerateMap();
        }
        else
        {
            if (right)
            {
                if (position==maxTextures-1)
                {
                    if (currentDelay>0)
                    {
                        currentDelay--;
                    }
                    else
                    {
                        currentDelay = delay;
                        right = false;
                    }
                
                }
                else
                {
                    DrawMap(textures[position]);
                    position++;
                }
                
            }
            else
            {
                if (position==0)
                {
                    if (currentDelay>0)
                    {
                        currentDelay--;
                    }
                    else
                    {
                        currentDelay = delay;
                        right = true;
                    }
                }
                else
                {
                    DrawMap(textures[position]);
                    position--;
                }
            }
        }
        
        
        //GenerateMaps();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoise(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset);
        //Debug.Log("Noise Map"+ noiseMap[0,0]);

        DrawMap(TextureGenerator.FogFromNoiseMap(noiseMap, opacity, edgeFade,fogColor));
    }
    public void GenerateMaps()
    {
        //Texture2D[] textures = new Texture2D[max_textures];
        Vector2 offsetMoving = new Vector2();
        //Debug.Log("Noise Map"+ noiseMap[0,0]);
        for (int i = 0; i < maxTextures; i++)
        {
            offsetMoving.x = i*stepSize;
            float[,] noiseMap = Noise.GenerateNoise(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset+offsetMoving);
            textures[i] = TextureGenerator.FogFromNoiseMap(noiseMap, opacity, edgeFade,fogColor);
        }
        
        //return textures;       
    }
    public void DrawMap(Texture2D texture)
    {   
        //Vector3 size = textureRenderer.transform.localScale;
        //textureRenderer.transform.localScale = new Vector3(texture.width/100f,1,texture.height/100f);
        //return;
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                float[,] noiseMap = Noise.GenerateNoise(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset);
                textureRenderer.material.mainTexture = TextureGenerator.TextureFromNoiseMap(noiseMap);
                break;
            case DrawMode.FogMap:
                textureRenderer.material.mainTexture = texture;
                break;
            default:
                Debug.Log("Draw mode not recognized");
                break;
        }   
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}
