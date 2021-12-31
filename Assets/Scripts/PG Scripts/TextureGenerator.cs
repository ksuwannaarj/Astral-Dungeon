using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    private enum Quadrant{topLeft,topRight,bottomLeft,bottomRight};
    // To render the noise map
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = new Color[width*height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y*width+x] = Color.Lerp(Color.black, Color.white,noiseMap[x,y]);
            }
        }
        return ColorTexture(colorMap,width,height);
    }

    public static Texture2D FogFromNoiseMap(float[,] noiseMap, AnimationCurve opacity, AnimationCurve edge, Color fogColor)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colorMap = new Color[width*height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color fog = fogColor;
                fog.a = opacity.Evaluate(noiseMap[x,y])* edge.Evaluate(findDistanceFromEdge(x,y, noiseMap));
                colorMap[y*width+x] = fog;
            }
        }
        
        return ColorTexture(colorMap,width,height);
    }

    public static Texture2D ColorTexture(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width,height);
        texture.wrapMode =TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    private static float findDistanceFromEdge(int x, int y, float[,] noiseMap)
    {
        
        float distance = 0;
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        float midWidth = width/2f;
        float midHeight = height/2f;

        Quadrant quadrant;

        if (y < midHeight)
        {
            if (x < midWidth)
            {
                quadrant = Quadrant.topLeft;
            }
            else
            {
                quadrant = Quadrant.topRight;
            }
        }
        else
        {
            if (x < midWidth)
            {
                quadrant = Quadrant.bottomLeft;
            }
            else
            {
                quadrant = Quadrant.bottomRight;
            }
        }

        switch (quadrant)
        {
            case Quadrant.topLeft:
                distance = (float)((x-0f)/midWidth) * (float)((y-0f)/midHeight);
                break;
            case Quadrant.topRight:
                distance = (float)(((width-1)-x)/midWidth) * (float)((y-0f)/midHeight);
                break;
            case Quadrant.bottomLeft:
                distance = (float)((x-0f)/midWidth) * (float)(((height-1)-y)/midHeight);
                break;
            case Quadrant.bottomRight:
                distance = (float)(((width-1)-x)/midWidth) * (float)(((height-1)-y)/midHeight);
                break;
            default:
                Debug.Log("quadrant error");
                break;
        }
        distance *= 2f;

        return distance;
    }
}
