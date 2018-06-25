﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public Chunk currentChunk;
	public World world;
	public Shapes.Types blockPlaceShape;
	public Blocks.Types blockPlaceType;
	//	First person controls
	public int speed = 25;
	int sensitivity = 1;
	Rigidbody rigidBody;
	Camera mycam;

	//	Chunk checks
	float updateTimer;
	int chunkLayerMask;
	

	void Start ()
	{
		mycam = GetComponentInChildren<Camera>();
		rigidBody = GetComponent<Rigidbody>();

		updateTimer = Time.fixedTime;
		chunkLayerMask = LayerMask.GetMask("Chunk");
	}

	//	The ray we are using for selection
	Ray Ray()
	{
		//return Camera.main.ScreenPointToRay(Input.mousePosition);
		return mycam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
	}

	void CursorOff()
	{
		if(Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;
	}
	
	void CursorOn()
	{
		if(Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
	}

	void Update ()
	{
		Movement();
		
		GetCurrentChunk();

		//	Break block
		if(Input.GetButtonDown("Fire1") && !Input.GetKeyDown(KeyCode.LeftControl))
		{
			if(Input.GetKey(KeyCode.LeftShift))
				DebugChunk(Ray());
				//DebugVertColor(Ray());
			else if(Input.GetKey(KeyCode.LeftAlt))
				Redraw(Ray());
			else
				RemoveBlock(Ray());
		}

		//	Break block
		if(Input.GetButtonDown("Fire2"))
		{
			AddBlock(Ray());
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			CursorOn();
		}
		
	}
	
	//	Raycast down to find current chunk
	//	Generate more chunks if player has moved to a new chunk
	void GetCurrentChunk()
	{
		//	Only do this twice per second
		if(Time.fixedTime - updateTimer < 0.5f)
		{
			return;
		}
		updateTimer = Time.fixedTime;

		//	Raycast to chunk below player
        RaycastHit hit;
        if (Physics.Raycast(transform.position,
							transform.TransformDirection(Vector3.down),
							out hit, 1000f,
							chunkLayerMask))
        {
			Chunk chunk;			
			//	Check if chunk exists
            if(World.chunks.TryGetValue(hit.transform.position, out chunk))
			{
				//	Initialise currentChunk
				if(currentChunk == null){ currentChunk = chunk; }

				//	Check if player has moved to a different chunk
				else if(chunk.position != currentChunk.position)
				{
					currentChunk = chunk;

					//	Generate chunks around player location
					world.LoadChunks(currentChunk.gameObject.transform.position, World.viewDistance);
				}
			}
			else
			{
				//	Something strange happened
				Debug.Log("chunk at "+hit.transform.position+" not found! ("+hit.transform.gameObject.name+")");
			}
        }
	}

	//	Simple flying first person movement
	//	Probably temporary
	void Movement()
	{
		if(Input.GetKey(KeyCode.W))	//	forward
		{
			transform.Translate((Vector3.forward * speed) * Time.deltaTime);
			CursorOff();
		}
		if(Input.GetKey(KeyCode.S))	//	back
		{
			transform.Translate((Vector3.back * speed) * Time.deltaTime);
			CursorOff();
		}
		if(Input.GetKey(KeyCode.A))	//	left
		{
			transform.Translate((Vector3.left * speed) * Time.deltaTime);
			CursorOff();
		}
		if(Input.GetKey(KeyCode.D))	//	right
		{
			transform.Translate((Vector3.right * speed) * Time.deltaTime);
			CursorOff();
		}
		if(Input.GetKey(KeyCode.LeftControl))	//	down
		{
			transform.Translate((Vector3.down * speed) * Time.deltaTime);
			CursorOff();
		}
		if(Input.GetKey(KeyCode.Space))	//	up
		{
			transform.Translate((Vector3.up * speed) * Time.deltaTime);
			CursorOff();
		}

		if(Cursor.lockState != CursorLockMode.Locked) return;
		
		float horizontal = sensitivity * Input.GetAxis("Mouse X");
        float vertical = -(sensitivity * Input.GetAxis("Mouse Y"));
        transform.Rotate(0, horizontal, 0);
		mycam.gameObject.transform.Rotate(vertical, 0, 0);		
	}

	void RemoveBlock(Ray ray)
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			//	get voxel position
			Vector3 pointInCube = hit.point - (hit.normal * 0.1f);
			Vector3 voxel = BlockUtils.RoundVector3(pointInCube);

			world.ChangeBlock(voxel, Blocks.Types.AIR);
		}
	}

	void AddBlock(Ray ray)
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			//	get voxel position
			Vector3 pointInCube = hit.point + (hit.normal * 0.1f);
			Vector3 voxel = BlockUtils.RoundVector3(pointInCube);

			world.ChangeBlock(voxel, blockPlaceType, blockPlaceShape);
		}
	}

	void DebugBitMask(Ray ray)
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			//	get voxel position
			Vector3 pointInCube = hit.point - (hit.normal * 0.1f);
			Vector3 voxel = BlockUtils.RoundVector3(pointInCube);
			Debug.Log(World.GetBitMask(voxel));
		}
	}

	void DebugChunk(Ray ray)
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			//	get voxel position
			Vector3 pointInCube = hit.point - (hit.normal * 0.1f);
			Vector3 voxel = BlockUtils.RoundVector3(pointInCube);
			Chunk chunk = World.chunks[World.BlockOwner(voxel)];
			Debug.Log(chunk.position + " " + chunk.hidden);

			Vector3[] offsets = Util.CubeFaceDirections();
			for(int i = 0; i < 6; i++)
			{
				Debug.Log((Shapes.CubeFace)i);
				Debug.Log(World.chunks[chunk.position + (offsets[i] * World.chunkSize)].position);
			}
		}
	}

	void DebugVertColor(Ray ray)
	{
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			//	get voxel position
			Vector3 pointInCube = hit.point - (hit.normal * 0.1f);
			Vector3 voxel = BlockUtils.RoundVector3(pointInCube);
			Chunk chunk = World.chunks[World.BlockOwner(voxel)];
			chunk.DebugMarkerColor(Color.red);		
		}
	}

	void Redraw(Ray ray)
	{
		RaycastHit hit;
		Chunk chunk;
		if (Physics.Raycast(ray, out hit))
		{
			World.chunks.TryGetValue(hit.transform.position, out chunk);

			chunk.Redraw();
		}
	}
}
