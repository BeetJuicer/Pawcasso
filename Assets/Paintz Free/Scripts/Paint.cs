﻿using UnityEngine;

public struct ChannelMask
{
    int one;
    int two;
    int three;
    int four;
    int five;
    int six;
    int seven;
}

public class Paint
{
    public Matrix4x4 paintMatrix;
    public Vector4  channelMask;
    public Vector4  channelMask2;
    public Vector4 scaleBias;
    public Brush brush;
}

[System.Serializable]
public class Brush
{
    public Texture2D splatTexture;
    public int splatsX = 1;
    public int splatsY = 1;
    public int splatIndex = -1;

    public float splatScale = 1.0f;
    public float splatRandomScaleMin = 1.0f;
    public float splatRandomScaleMax = 1.0f;

    public float splatRotation = 0f;
    public float splatRandomRotation = 180f;

    public int splatChannel = 0;

    public Vector4 getMask()
    {
        if (this.splatChannel == 0) return new Vector4(1, 0, 0, 0);
        if (this.splatChannel == 1) return new Vector4(0, 1, 0, 0);
        if (this.splatChannel == 2) return new Vector4(0, 0, 1, 0);
        if (this.splatChannel == 3) return new Vector4(0, 0, 0, 1);

        if (this.splatChannel == 4) return new Vector4(1, 0, 0, 1);
        if (this.splatChannel == 5) return new Vector4(1, 1, 0, 1);
        if (this.splatChannel == 6) return new Vector4(1, 1, 1, 1);
        if (this.splatChannel == 7) return new Vector4(0, 0, 1, 1);
        return new Vector4(0, 0, 0, 0);
    }

    public Vector4 getTile()
    {
        float splatscaleX = 1.0f / splatsX;
        float splatscaleY = 1.0f / splatsY;

        int index = splatIndex;
        if (index >= splatsX * splatsY)
        {
            splatIndex = 0;
            index = 0;
        }

        if (splatIndex == -1) index = Random.Range(0, splatsX * splatsY);

        float splatsBiasX = splatscaleX * (index % splatsX);
        float splatsBiasY = splatscaleY * (index / splatsX);

        return new Vector4(splatscaleX, splatscaleY, splatsBiasX, splatsBiasY);
    }
}