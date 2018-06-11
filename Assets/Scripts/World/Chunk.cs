﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	//	Chunk local space and game components
	public GameObject gameObject;
	//	All blocks in this chunk
	public Block[,,] blocks;

	public enum Status {GENERATED, DRAWN}
	public Status status;

	World world;

	//	Chunk size and position in world
	int size;
	public Vector3 position;

	public Chunk(Vector3 _position, World _world)
	{
		//	Create GameObject
		gameObject = new GameObject(_position.ToString());
		gameObject.layer = 9;

		world = _world;	//	for testing
		position = _position;
		
		//	Apply chunk size
		size = World.chunkSize;
		blocks = new Block[size,size,size];

		//	Set transform
		gameObject.transform.parent = world.gameObject.transform;
		gameObject.transform.position = position;
		
		GenerateBlocks();		
	}

	//	Choose types of all blocks in the chunk based on Perlin noise
	void GenerateBlocks()
	{
		for(int x = 0; x < size; x++)
			for(int z = 0; z < size; z++)
			{
				//	Get height of ground in this column
				int groundHeight = NoiseUtils.GroundHeight( x + (int)position.x,
															z + (int)position.z,
															World.maxGroundHeight);
				//	Generate column
				for(int y = 0; y < size; y++)
				{
					//	Position of block in chunk
					Vector3 blockPosition = new Vector3(x, y, z);
					//	Position of block in world
					Vector3 blockPositionInWorld = blockPosition + this.position;

					Block.BlockType type;

					//	Set block type
					if (blockPositionInWorld.y > groundHeight)
					{
						type = Block.BlockType.AIR;
					}
					else
					{
						type = Block.BlockType.GROUND;
					}

					//	Store new block in 3D array
					blocks[x,y,z] = new Block(type, blockPosition, this);
				}
			}
	}

	//	Create all block face meshes in Block.Draw() then merge into one mesh attached to chunk GameObject
	public void DrawBlocks()
	{
		//	Meshes of all blocks in the chunk
		List<MeshFilter> blockMeshes = new List<MeshFilter>();

		//	Iterate over all block locations in chunk		
		for(int x = 0; x < size; x++)
			for(int z = 0; z < size; z++)
				for(int y = 0; y < size; y++)
				{
					//	Create meshes for block and add to list to be merged
					blockMeshes.AddRange(blocks[x,y,z].DrawBlock());
				}
		
		//	Merge quad meshes
		MergeQuads(blockMeshes.ToArray());

		MeshCollider collider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = gameObject.transform.GetComponent<MeshFilter>().mesh;
	}

	//	Merge individual cube faces into one model represending entire chunk
	public void MergeQuads(MeshFilter[] blockMeshes)
	{
		//	Combine quad meshes
        CombineInstance[] combine = new CombineInstance[blockMeshes.Length];
        for(int i = 0; i < blockMeshes.Length; i++)
		{
            combine[i].mesh = blockMeshes[i].sharedMesh;
            combine[i].transform = blockMeshes[i].transform.localToWorldMatrix;
        }

		//	Combine meshes to new mesh attached to chunk
        MeshFilter mf = (MeshFilter) gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);

        //	Add mesh renderer to chunk
		MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = world.defaultMaterial;

		//	Delete quad GameObjects
		foreach (Transform quad in gameObject.transform) {
     		GameObject.Destroy(quad.gameObject);
 		}
	}
}
