﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Blocks 
{
	public static int numberOfTypes = 6;
	//	Enum indices correspond to fixed block attribute arrays below
	public enum Types {		AIR = 0,
							DIRT = 1,
							STONE = 2,
							GRASS = 3,
							LIGHTGRASS = 4,
							WATER = 5
							};

	//	Block type is see-through
	public static bool[] seeThrough = new bool[] {		true,		//	0	//	AIR
														false,		//	1	//	DIRT
														false,		//	2	//	STONE
														false,		//	3	//	GRASS
														false,		//	4	//	LIGHTGRASS
														false		//	5	//	WATER
														};

	//	Block type color
	public static Color32[] colors = new Color32[]{	Color.white,					//	0	//	AIR
													new Color32(80, 71, 10, 255),	//	1	//	DIRT
													new Color32(60, 60, 60, 255),	//	2	//	STONE
													new Color32(11, 110, 35, 255),	//	3	//	GRASS
													new Color32(50, 110, 70, 255),	//	4	//	LIGHTGRASS
													new Color32(10, 10, 200, 255)	//	5	//	WATER
													};

	//	Block type color
	public static Vector2[] tileOffset = new Vector2[]{			new Vector2(0,0),	//	0	//	AIR
																new Vector2(0,0),	//	1	//	DIRT
																new Vector2(1,0),	//	2	//	STONE
																new Vector2(0,0),	//	3	//	GRASS
																new Vector2(0,0),	//	4	//	LIGHTGRASS
																new Vector2(0,0)	//	5	//	WATER
													};		

	public static bool[] smoothSurface = new bool[] {	false,		//	0	//	AIR
														false,		//	1	//	DIRT
														false,		//	2	//	STONE
														false,		//	3	//	GRASS
														true,		//	4	//	LIGHTGRASS
														false		//	5	//	WATER
													};			
}
