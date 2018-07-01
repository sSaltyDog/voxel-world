﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseUtils
{
	public static void noisetest()
	{
		float frequency = 1;
		float amplitude = 1;

		/*for(int x = 0; x < 10; x++)
			for(int z = 0; z < 10; z++)
			{	
				float _x = x*0.01f;
				float _z = z*0.01f;

				float value = Mathf.PerlinNoise(_x * frequency, _z * frequency) * amplitude;
				Debug.Log(value);
			}*/
	}

	public static int TestGround(int x, int z)
	{
		float main = Mathf.Lerp(0, 50, BrownianMotion(x * 0.01f, z * 0.01f, 3, 0.2f));
		float subtract = Mathf.Lerp(0, 50, BrownianMotion(x * 0.012f, z * 0.012f, 3, 0.25f));
		float difference = 0;
		if(subtract > main)
		{
			difference = subtract - main;
		}

		return (int)(main - difference);	
	}

	public static int BrownianGround(float x, float z, int octaves, float persistance, float factor, int maxHeight)
	{
		float value = BrownianMotion(x * factor, z * factor, octaves, persistance);

		return (int)Mathf.Lerp(0, maxHeight, value);	
	}

	public static int PerlinGround(int x, int z, float frequency, float amplitude, float factor, int maxHeight)
	{
		float _x = x*factor;
		float _z = z*factor;

		float value = Mathf.PerlinNoise(_x * frequency, _z * frequency) * amplitude;

		return (int)Mathf.Lerp(0, maxHeight, value);	
	}

	public static int LowLands(int x, int z)
	{
		int maxHeight = 50;
		int levelHeight = 15;

		float source = BrownianMotion(x * 0.01f,z * 0.01f, 5, 0.5f);
		float microHeight = Mathf.Lerp(10, maxHeight, source);
		float factor = Mathf.Lerp(0, 1, BrownianMotion(x * 0.01f,z * 0.01f));

		int height = (int) (microHeight * factor);

		return (int) LevelOutAtMin(levelHeight, 5, height, source * BrownianMotion(x * 0.001f, z * 0.001f));
	}

	static float LevelOutAtMin(int min, int preservedNoise, int height, float source)
	{
		if(height < min)
		{
			float factor = (min - height) * 0.75f;
			return height + factor;
		}

		return height;
	}

	public static Blocks.Types Biome(int x, int z)
	{
		float biomeRoll = (float)Util.RoundToDP(BrownianMotion(x * 0.001f, z * 0.001f, 10), 2);
		if(biomeRoll < .5f)
		{
			return Blocks.Types.STONE;
		}
			return Blocks.Types.DIRT;
	}
	

	//	Brownian Motion (Perlin Noise)
	public static float BrownianMotion(float x, float z, int octaves = 2, float persistance = 0.2f)
	{
		float total = 0;
		float frequency = 1;
		float amplitude = 1;
		float maxValue = 0;
		
		//	Layer noise to create more complex/natural waves
		for(int i = 0; i < octaves ; i++)
		{
			total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

			maxValue += amplitude;
			amplitude *= persistance;
			frequency += 2;
		}

		return total/maxValue;
	}

	public static int GroundHeight(int x, int z, int maxHeight)
	{
		float height;
		height = BrownianMotion(x * 0.05f,z * 0.05f);

		return (int)Map(0, maxHeight, 0, 1, height);;
	}
	static float Map(float newmin, float newmax, float origmin, float origmax, float value)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(origmin, origmax, value));
    }
}
