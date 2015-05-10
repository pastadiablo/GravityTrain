using UnityEngine;
using UnityEditor;
using UnityEngine.Sprites;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMap : MonoBehaviour {

	public Texture2D Tileset;

	public int mapWidth, mapHeight, tilePixelSize;

	public float tileUnitSize = 1;

	private int[,] map; //TODO: change to enum

	// Use this for initialization
	void Start () {


		map = new int[mapWidth,mapHeight];

		int k = 0;
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				map[x,y] = k;
				k++;
			}
		}

		BuildMesh ();
	}
	
	// Update is called once per frame
	void Update () {
		//spriteRenderer.Render(0);


	}

	//Build Mesh
	void BuildMesh() {

		int tileCount = mapWidth * mapHeight;
		int triCount = tileCount * 2;

		int vertCountX = mapWidth  * 2;
		int vertCountY = mapHeight * 2;
		int vertCount = vertCountX * vertCountY;

		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector2[] uvs = new Vector2[vertCount];

		int[] triangles = new int[triCount * 3];

		int x, y;

		for(y = 0; y < mapHeight; y++) {
			for(x = 0; x < mapWidth; x++) {

				//Index of map tile in row-major order, on the tilemap
				int tileIndex = y * mapWidth + x;

				//Create vertices for tile
				float upperLeftX = x*tileUnitSize;
				float upperLeftY = -y*tileUnitSize;

				//Grab the indices of the 4 vertices of the tile
				int ulVertIndex = vertCountX*y + 2*tileIndex;
				int urVertIndex = ulVertIndex+1;
				int llVertIndex = ulVertIndex+vertCountX;
				int lrVertIndex = llVertIndex+1;

				//Assign vertices
				vertices[ulVertIndex]	= new Vector3(upperLeftX, 				upperLeftY);
				vertices[urVertIndex] 	= new Vector3(upperLeftX + tileUnitSize, upperLeftY);
				vertices[llVertIndex] 	= new Vector3(upperLeftX, 				upperLeftY - tileUnitSize);
				vertices[lrVertIndex] 	= new Vector3(upperLeftX + tileUnitSize, upperLeftY - tileUnitSize);

				//Assign normals
				normals[ulVertIndex] 	= Vector3.up;
				normals[urVertIndex] = Vector3.up;
				normals[llVertIndex] 	= Vector3.up;
				normals[lrVertIndex] = Vector3.up;

				//Assign triangles
				int triIndex = tileIndex * 6; // 6 indices for each square, each 3 indices stores the indices of verts that form a tri

				triangles[triIndex + 0] = ulVertIndex;//	|\
				triangles[triIndex + 1] = lrVertIndex;//	| \
				triangles[triIndex + 2] = llVertIndex;//	|__\

				triangles[triIndex + 3] = ulVertIndex;//	---
				triangles[triIndex + 4] = urVertIndex;//	\ |
				triangles[triIndex + 5] = lrVertIndex;//	 \|


				//Assign UVs	
				int tileType = map[x,y];

				int column = tileType % ((int)Tileset.height / tilePixelSize);
				int row = ((int)Tileset.width / tilePixelSize) - tileType / ((int)Tileset.width / tilePixelSize);

				float uvLeftX 	= (float)tilePixelSize * column / Tileset.height;
				float uvRightX 	= (float)tilePixelSize * (column+1) / Tileset.height;
				float uvTopY 	= (float)tilePixelSize * row / Tileset.width;
				float uvBottomY = (float)tilePixelSize * (row-1) / Tileset.width;

				Vector2 p1 = uvs[ulVertIndex] = new Vector2(uvLeftX, uvTopY);
				Vector2 p2 = uvs[urVertIndex] = new Vector2(uvRightX, uvTopY);
				Vector2 p3 = uvs[llVertIndex] = new Vector2(uvLeftX, uvBottomY);
				Vector2 p4 = uvs[lrVertIndex] = new Vector2(uvRightX, uvBottomY);
				
				Debug.Log ("Tile: " + tileIndex + 
				           "\nID: " + tileType +
				           "\nul: " + ulVertIndex +
				           "\nur: " + urVertIndex +
				           "\nll: " + llVertIndex +
				           "\nlr: " + lrVertIndex + 
				           "\nP1: " + p1 +
				           "\nP2: " + p2 +
				           "\nP3: " + p3 +
				           "\nP4: " + p4);

			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uvs;


		MeshFilter meshFilter = GetComponent<MeshFilter>();

		meshFilter.mesh = mesh;
	}
}
