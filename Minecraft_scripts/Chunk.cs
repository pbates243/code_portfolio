using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    public ChunkCoord coord;
    GameObject chunkObject;
    public MeshRenderer meshRenderer;
    public MeshFilter MeshFilter;

	public MeshCollider meshCollider;


    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();   //voxel details
    //List<int> transparentTriangles = new List<int>();
    //Material[] materials = new Material[2];
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.Chunkheigth, VoxelData.ChunkWidth];
    

    public List<VoxelMod> modifications = new List<VoxelMod>(); 

    World world;
    public bool isVoxelMapPopulated = false;
    private bool _isActive;
    


    public bool isActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if (chunkObject != null)
            {
                chunkObject.SetActive(value);
            }

        }
    }

    public Chunk (ChunkCoord _coord, World _world, bool generateOnLoad)
    {

        
        coord = _coord;
        world = _world;
        isActive = true;

        if (generateOnLoad)
        {
            Init();
        }
     
    }

    public void Init()
    {
        chunkObject = new GameObject();
        MeshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();

        //materials[0] = world.material;
        //materials[1] = world.transparentMaterial;
        //meshRenderer.materials = materials;

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;




        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

        meshCollider.sharedMesh = MeshFilter.mesh;
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }

 


    bool IsVoxelInChunk (int x, int y, int z) // checks to see if the voxel is part of a chunk, ifes returns true
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.Chunkheigth - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
    }


   
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x,y,z)) // if voxel is not already in chunk
        {
            return world.checkForVoxel(pos + position); // get the block type 
        }
        
        return world.blockTypes[voxelMap[x, y, z]].isSolid; 


    }

    public byte GetVoxelFromGlobalVector3 (Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        return voxelMap[xCheck, yCheck, zCheck];

    }



    public void UpdateChunk()
    {


        while (modifications.Count > 0)
        {
            VoxelMod v = modifications[0]; //removes from list and take
            modifications.RemoveAt(0);
            Vector3 pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = v.id;
        }
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
        //bool isTransparent = world.blockTypes[blockID].isSolid;

        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p])) // only draws voxel if the face can be seen
            {
                

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockTypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                
                
                
                vertexIndex += 4;

            }
            
        }
    } // this puts voxel info into data lists

    void PopulateVoxelMap()
    {
        //float terrainScale = 0.25f; // turn terrain scale into int
        //int new_scale = (int)terrainScale;

        for (int y = 0; y < VoxelData.Chunkheigth; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position); // populates the voxel map
                }

            }
        }

        isVoxelMapPopulated = true;

    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.Chunkheigth; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (world.blockTypes[voxelMap[x,y,z]].isSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }


    void CreateMesh()
    {
        Mesh mesh = new Mesh(); // builds the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        //mesh.subMeshCount = 2;

        //mesh.SetTriangles(triangles.ToArray(), 0);
        //mesh.SetTriangles(transparentTriangles.ToArray(), 1);

        //mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals(); // built in unity function

        MeshFilter.mesh = mesh;
    }

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y+ VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x+ VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x+ VoxelData.NormalizedBlockTextureSize, y+ VoxelData.NormalizedBlockTextureSize));
    }
    
}


public class ChunkCoord
{
    public int x; // position of the chunk but not in world space
    public int z; // different than abs position

    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }

    public ChunkCoord (int _x, int _z)
    {
        x = _x;
        z = _z;
    }

   

    public ChunkCoord (Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null) // checks to see if they are equal
            return false;
            else if (other.x == x && other.z == z)
                return true;
            else
                return false;

    }
}
    


