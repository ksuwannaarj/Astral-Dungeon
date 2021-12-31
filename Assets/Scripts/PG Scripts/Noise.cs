using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    // Perlin noise gets weird when range is higher
    private const int RANGE = 100000;

    // Completely arbitrary number for the noise scale minimum that isn't 0
    private const float MIN_SCALE = .0001f;
    public static float[,] GenerateNoise(int width, int height, int seed, float scale, 
            int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[width,height];

        System.Random nRandom = new System.Random(seed);
        
        // Gets random places within the perlin noise
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float oX = nRandom.Next(-RANGE, RANGE) + offset.x;
            float oY = nRandom.Next(-RANGE, RANGE) + offset.y;
            octaveOffsets[i] = new Vector2(oX,oY);
        }

        if (scale <= 0)
        {
            scale = MIN_SCALE;
        }

        // Centers map when adujusting noise scale
        float halfW = width/2f;
        float halfH = height/2f;

        // For normalizing the height map
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        // Generates noise map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float X = (x-halfW) / scale * frequency + octaveOffsets[i].x;
                    float Y = (y-halfH) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(X, Y)*2-1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                // Gets the max and min of noise
                if (noiseHeight > maxHeight)
                {
                    maxHeight = noiseHeight;
                }
                if (noiseHeight < minHeight)
                {
                    minHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        //Debug.Log("Max noise height: "+maxHeight);
        //Debug.Log("Min noise height: "+minHeight);

        // Normalizes to the max and min noise heights
        // This is done because it causes a visually pleasing effect when the noise moves
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minHeight,maxHeight, noiseMap[x,y]);
                
            }
        }
        int debugx = Random.Range(0,99);
        int debugy = Random.Range(0,99);
        //Debug.Log("noiseMap["+debugx+","+debugy+"] from Noise: "+ noiseMap[debugx,debugy].ToString());
        return noiseMap;
    }
}


