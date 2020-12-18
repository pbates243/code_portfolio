using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class World : MonoBehaviour
{
    //public int seed;

    public bool isCreatingChunks;

    [Range(0,100)]
    public int randomFillPercent;
 
    //public bool useRandomSeed;
    
    public BiomeAttribute biome;

    public Transform player;
    public Vector3 spawnPosition;

    public Material material;

    //public Material transparentMaterial;


    public BlockType[] blockTypes;

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.Chunkheigth, VoxelData.ChunkWidth];

    

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks]; // store chunks

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    List<Chunk> chunksToUpdate = new List<Chunk>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkChord; // last chunk player was known to be on

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();

    List<VoxelMod> modifications = new List<VoxelMod>(); //modifications to the basic structure of a chunk



    private void Start()

    {
        //Random.InitState(seed);
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.Chunkheigth - 50, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkChord = GetChunkCoordFromVector3(player.position);
        

    }

    private void Update()
    {

        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if (!playerChunkCoord.Equals(playerLastChunkChord)) // to make sure player has moved far enough to move the view.
        {
            CheckViewDistance();

            playerLastChunkChord = playerChunkCoord;

            if (chunksToCreate.Count > 0 && !isCreatingChunks)
            {
                StartCoroutine("createChunks");
            }
        }

    }


    void GenerateWorld()
    {
       
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.viewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.viewDistanceInChunks; x++) // generating the world
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.viewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.viewDistanceInChunks; z++)
            {

                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                //CreateNewChunk(x, z);
                activeChunks.Add(new ChunkCoord(x, z));
            }
        }

        while (modifications.Count > 0) // loop til no more mods
        {
            VoxelMod v = modifications[0]; // global list
            modifications.RemoveAt(0);

            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null) // if something crosses over chunks we need to add it
            {
                chunks[c.x, c.z] = new Chunk(c, this, true);
                activeChunks.Add(c); // all chunks in game must be here
            }

            chunks[c.x, c.z].modifications.Insert(0,v);

            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
                chunksToUpdate.Add(chunks[c.x, c.z]);
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].UpdateChunk();
            chunksToUpdate.RemoveAt(i);
        }

        player.position = spawnPosition;
    }

    IEnumerator createChunks()
    {
        isCreatingChunks = true;
        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }
        isCreatingChunks = false;

    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        //playerLastChunkCoord = playerChunkCoord;


        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks); // whatever chunks are active will be added here

        activeChunks.Clear();

        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++) // works from wherever the player is standing
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                if (isChunkInWorld(new ChunkCoord(x, z))) // check that chunk is in world
                {
                    if (chunks[x, z] == null) // means chunk has not been generated before
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        
                    }
                    activeChunks.Add(new ChunkCoord(x, z));
                }
                //CreateNewChunk(x, z);   // it is in view distance generate chunk

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i); // any chunks left were previously in view distance but now are no longer
                }

            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
        }
    }

    public bool checkForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!isChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Chunkheigth)
        {
            return false;
        }
        
        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
        {
            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
        }

        return blockTypes[GetVoxel(pos)].isSolid;
    }

    


    public byte GetVoxel(Vector3 pos)
    {

        ChunkCoord thisChunk = new ChunkCoord(pos);

        int yPos = Mathf.FloorToInt(pos.y);
        int xPos = Mathf.FloorToInt(pos.x);

        /* IMMUTABLE PASS */

        if (!isChunkInWorld(thisChunk))
            return 0; // air if outside

        // if bottom block of chunk, return bedrock
        if (yPos < 3)
            return 1;

        /* Basic Terrain Pass */

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale) + biome.solidGroundHeight); // this translates a 0-1 value to an actual height
        byte voxelValue = 0;

        if (yPos == terrainHeight)
        {
            voxelValue = 3; // grass

        }


        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 5;
        else if (yPos > terrainHeight)
            return 0;

        else
            voxelValue = 2;
     

        /* SECOND PASS */
        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    {
                        voxelValue = lode.blockID;
                        if(lode.blockID == 4)
                        {
                            voxelValue = 0; // places air block
                        }
                    }
                }
            }
        }

        /* Biome */

        if (yPos == terrainHeight)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treeZoneScale) > biome.treeZoneThreshold)
            {
                voxelValue = 1;
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treePlacementScale) > biome.treePlacementThreshold)
                {
                    voxelValue = 8;
                    Structure.MakeTree(pos, modifications, biome.minTreeHeight, biome.maxTreeHeight);
                }
            }
            //else if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.rockZoneScale) > biome.rockZoneThreshold)
            //{
            //    voxelValue = 2;
            //}




        }


        return voxelValue;
    }



    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
        activeChunks.Add(new ChunkCoord(x, z));

    }

    bool isChunkInWorld(ChunkCoord coord)
        {
            if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1)
                return true;
            else
                return false;
        }

        bool isVoxelInWorld(Vector3 pos)
        {
            if (pos.x >= 0 && pos.x < VoxelData.worldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.Chunkheigth && pos.z >= 0 && pos.z < VoxelData.worldSizeInVoxels)
                return true;
            else
                return false;
        }
    
}






    [System.Serializable]
    public class BlockType
    {
        public string blockName;
        public bool isSolid;
        //public bool isTransparent;


        // back,front,top,bottom,left,right
        [Header("Texture Values")]
        public int backFaceTexture;
        public int frontFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;



        public int GetTextureID(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0:
                    return backFaceTexture;
                case 1:
                    return frontFaceTexture;
                case 2:
                    return topFaceTexture;
                case 3:
                    return bottomFaceTexture;
                case 4:
                    return leftFaceTexture;
                case 5:
                    return rightFaceTexture;
                default:
                    Debug.Log("Error in GetTextureID; invalid face index");
                    return 0;

            }
        }
     
    }



public class VoxelMod
{
    public Vector3 position;
    public byte id;

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }

    public VoxelMod(Vector3 _position, byte _id)
    {
        position = _position;
        id = _id;
    }
}




