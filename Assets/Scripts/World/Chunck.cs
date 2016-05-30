using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using System.Threading;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunck : MonoBehaviour
{
    public Block[, ,] map;

    public static int Width = 16, Height = 16;

    public static Dictionary<Vector3, Block> blocosAdicionais = new Dictionary<Vector3, Block>();
    public static Dictionary<Vector3, Chunck> Chuncks = new Dictionary<Vector3, Chunck>();
    public static Dictionary<Vector2, int> heightMap = new Dictionary<Vector2, int>();

    Chunck left;
    Chunck right;
    Chunck back;
    Chunck front;
    Chunck bottom;
    Chunck top;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangulos = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    List<Vector3> NOCOLvertices = new List<Vector3>();
    List<int> NOCOLtriangulos = new List<int>();
    List<Vector2> NOCOLuvs = new List<Vector2>();
    List<Color> NOCOLcolors = new List<Color>();

    public Color shadowColors;
    public GameObject Mesh2;

    float TextureOffset = 1F / 16F;
    Mesh mesh;
    Mesh mesh2;

    public static bool working = false;
    public static bool Blockworking = false;
    public bool ready = false;
    public bool generatedMap = false;

    public bool iamWorking = false;

    void Start()
    {
        Mesh2 = new GameObject("Mesh 2 - NO COLLISION");
        Mesh2.transform.parent = transform;

        Mesh2.transform.localPosition = Vector3.zero;

        pos = transform.position;
        if(Chuncks.ContainsKey(pos))
        {
            Destroy(gameObject);
            return;
        }
        Chuncks.Add(pos, this);

        if(Time.time < 1 && GameManager.host)
        {
            StartFunction();
        }
    }

    GameObject pl;
    public Vector3 pos;
    public static bool spawningChuncksS = false;
    public bool IamspawningChuncksS = false;
    void Update()
    {
        if(!ready && generatedMap)
        {
            if (left != null && right != null && front != null && back != null &&
                bottom != null && top != null)
            {
                if (left.generatedMap && right.generatedMap && front.generatedMap && back.generatedMap &&
                bottom.generatedMap && top.generatedMap)
                    StartCoroutine(CalculateMesh());
            }
        }

        if (pl == null)
            pl = GameObject.FindGameObjectWithTag("Player");

        if (ready)
        {
            if (pl != null)
            {
                if (Vector3.Distance(this.transform.position, pl.transform.position) > PlayerController.viewRange)
                {
                    if(generatedMap)
                        SaveToFile(this);

                    Chuncks.Remove(transform.position);

                    if (iamWorking == true)
                        working = false;
                    if (IamspawningChuncksS == true)
                        spawningChuncks = false;
                    Destroy(this.gameObject);
                }
            }
        }

        if (Time.time < 1f || spawningChuncksS == true || spawningChuncks == true) return;
        if (Vector3.Distance(pos, pl.transform.position) > PlayerController.viewRange - Width) return;
                
        if (left == null || right == null || front == null || back == null ||
        bottom == null || top == null)
        {
            if (left == null)
            {
                int x = Mathf.FloorToInt((transform.position.x - Chunck.Width) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        left = GetChunck(new Vector3(x, y, z));
                    }
                }
            }
            if (right == null)
            {
                int x = Mathf.FloorToInt((transform.position.x + Chunck.Width) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        right = GetChunck(new Vector3(x, y, z));
                    }
                }
            }
            if (top == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y + Chunck.Height) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        top = GetChunck(new Vector3(x, y, z));
                    }
                }
            }
            if (bottom == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y - Chunck.Height) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        bottom = GetChunck(new Vector3(x, y, z));
                    }
                    else
                    {
                        StartCoroutine(SpawnChunck(new Vector3(x, y, z)));
                    }
                }
            }
            if (back == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z - Chunck.Width) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        back = GetChunck(new Vector3(x, y, z));
                    }
                }
            }
            if (front == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Chunck.Width) * Chunck.Width;
                int y = Mathf.FloorToInt((transform.position.y) / Chunck.Height) * Chunck.Height;
                int z = Mathf.FloorToInt((transform.position.z + Chunck.Width) / Chunck.Width) * Chunck.Width;

                if (Vector3.Distance(new Vector3(x, y, z), pl.transform.position) <= PlayerController.viewRange)
                {
                    if (ChunckExists(new Vector3(x, y, z)))
                    {
                        front = GetChunck(new Vector3(x, y, z));
                    }
                }
            }
        }
    }
    public void StartFunction()
    {
        mesh = new Mesh();
        StartCoroutine(CalculateMap());
    }
    public void StartFunction(Block[, ,] MAP)
    {
        mesh = new Mesh();
        map = MAP;

        generatedMap = true;
        //StartCoroutine(CalculateMesh());

        Chunck.working = false;
    }

    public void SaveToFile(Chunck c)
    {
        if (!GameManager.host) return;

        string[] blocks = File.ReadAllLines(Application.dataPath + "\\" + GameManager.worldName + "\\" + c.pos.ToString());
        int i = 0;
        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (c.map[x, y, z] == null)
                        blocks[i] = "-1";
                    else
                        blocks[i] = c.map[x, y, z].BlockID.ToString();
                    i++;
                }
            }
        }

        File.WriteAllLines(Application.dataPath + "\\" + GameManager.worldName + "\\" + c.pos.ToString(), blocks);
    }

    public static bool spawningChuncks = false;
    public IEnumerator SpawnChunck(Vector3 pos)
    {
        spawningChuncks = true;

        GameObject.Instantiate(PlayerController.ChunckPrefab, pos, Quaternion.identity);

        yield return 0;

        spawningChuncks = false;
    }

    public static Thread tmap;

    public IEnumerator CalculateMap()
    {
        while(working || iamWorking)
        {
            yield return 0;
        }

        if (tmap != null && tmap.IsAlive)
        {

        }
        else
        {
            bool createFile = true;
            if (System.IO.File.Exists(Application.dataPath + "\\"+ GameManager.worldName +"\\" + pos.ToString()))
            {
                createFile = false;
            }
            else
            {
                
            }

            working = true;
            iamWorking = true;

            if (createFile == false)
            {
                map = new Block[Width, Height, Width];

                string[] linhas = File.ReadAllLines(Application.dataPath + "\\" + GameManager.worldName + "\\" + pos.ToString());

                int i = 0;
                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            Vector2 key = new Vector2(pos.x + x, pos.z + z);
                            if(!heightMap.ContainsKey(key))
                            {
                                heightMap.Add(key, (int)pos.y + y);
                            }
                            else
                            {
                                if(heightMap[key] < pos.y + y)
                                {
                                    heightMap[key] = (int)pos.y + y;
                                }
                            }

                            int blockID = -1;

                            blockID = int.Parse(linhas[i]);

                            if (blockID == -1)
                                map[x, y, z] = null;
                            else
                                map[x, y, z] = Block.getBlock(blockID);

                            i++;
                        }
                    }
                }
            }
            else if(createFile == true)
            {
                while (tmap != null && tmap.IsAlive)
                {
                    yield return 0;
                }

                tmap = new Thread(CMap);
                tmap.Start();

                while (tmap.IsAlive)
                {
                    yield return 0;
                }

                tmap.Abort();

                if (!Directory.Exists(Application.dataPath + "\\" + GameManager.worldName))
                {
                    if (!Directory.Exists(Application.dataPath + "\\" + GameManager.worldName.Split('\\')[0]))
                        Directory.CreateDirectory(Application.dataPath + "\\" + GameManager.worldName.Split('\\')[0]);
                    Directory.CreateDirectory(Application.dataPath + "\\" + GameManager.worldName.Split('\\')[0] + "\\" + GameManager.worldName.Split('\\')[1]);
                }

                if (!System.IO.File.Exists(Application.dataPath + "\\" + GameManager.worldName + "\\" + "seed"))
                {
                    StreamWriter f = File.CreateText(Application.dataPath + "\\" + GameManager.worldName + "\\" + "seed");
                    f.Write(GameManager.seed);
                    f.Close();
                }

                File.Create(Application.dataPath + "\\" + GameManager.worldName + "\\" + pos.ToString()).Close();
                List<string> linhas = new List<string>();

                TextWriter c = new StreamWriter(Application.dataPath + "\\" + GameManager.worldName + "\\" + pos.ToString());

                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (map[x, y, z] == null)
                                c.WriteLine("-1");
                                //linhas.Add(-1 + ";");
                            else
                                c.WriteLine(map[x,y,z].BlockID);
                                //linhas.Add(map[x,y,z].BlockID + ";");
                        }
                    }
                }

                c.Close();
                //File.WriteAllLines(Application.dataPath + "\\" + pos.ToString(), linhas.ToArray());
            }

            generatedMap = true;

            yield return 0;

            working = false;
            iamWorking = false;
            //StartCoroutine(CalculateMesh());
        }
    }

    public void CMap()
    {
        {
            System.Random r = new System.Random();
            working = true;

            map = new Block[Width, Height, Width];

            for (int z = 0; z < Width; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Vector2 key = new Vector2(pos.x + x, pos.z + z);
                        if (!heightMap.ContainsKey(key))
                        {
                            heightMap.Add(key, (int)pos.y + y);
                        }
                        else
                        {
                            if (heightMap[key] < pos.y + y)
                            {
                                heightMap[key] = (int)pos.y + y;
                            }
                        }

                        Block bloc = GetTheoreticalBlock(pos + new Vector3(x, y, z));
                        if(bloc == Block.getBlock(2))
                        {
                            if ((pos + new Vector3(x, y, z)).y < 100 && r.Next(0, 10000) == 1)
                            {
                                CreateOreNode(pos + new Vector3(x, y, z));
                            }
                        }
                        map[x, y, z] = bloc;

                        if (map[x, y, z] == Block.getBlock("Dirt") && GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                        {
                            map[x, y, z] = Block.getBlock("Grass");
                            if(r.Next(0, 500) == 1)
                            {
                                CreateTree(pos + new Vector3(x, y + 1, z));
                            }
                            else if(r.Next(0, 70) == 1)
                            {
                                if (!blocosAdicionais.ContainsKey(pos + new Vector3(x, y + 1, z)))
                                    blocosAdicionais.Add(pos + new Vector3(x, y + 1, z), Block.getBlock("Grass_"));
                            }
                            else if (r.Next(0, 150) == 1)
                            {
                                if (!blocosAdicionais.ContainsKey(pos + new Vector3(x, y + 1, z)))
                                    blocosAdicionais.Add(pos + new Vector3(x, y + 1, z), Block.getBlock("Yellow Flower"));
                            }
                            else if (r.Next(0, 150) == 1)
                            {
                                if (!blocosAdicionais.ContainsKey(pos + new Vector3(x, y + 1, z)))
                                    blocosAdicionais.Add(pos + new Vector3(x, y + 1, z), Block.getBlock("Red Flower"));
                            }
                        }
                        else if (map[x, y, z] == Block.getBlock("Sand") && GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                        {
                            if (r.Next(0, 180) == 1)
                            {
                                CreateCactus(pos + new Vector3(x, y + 1, z));
                            }
                        }
                    }
                }
            }
        }
    }
    public void CreateTree(Vector3 key)
    {
        System.Random r = new System.Random();
        int tronco = r.Next(5, 7);
        int widthFolhas = 6;

        for(int i = 0; i < tronco; i ++)
        {
            Vector3 keyI = key + new Vector3(0, i, 0);
            if (blocosAdicionais.ContainsKey(keyI))
                blocosAdicionais[keyI] = Block.getBlock("Log");
            else
                blocosAdicionais.Add(keyI, Block.getBlock("Log"));
        }
        for (int z = -(widthFolhas / 2); z < (widthFolhas / 2); z++)
        {
            for (int x = -(widthFolhas / 2); x < (widthFolhas / 2); x++)
            {
                for (int y = -(widthFolhas / 2); y < (widthFolhas / 2); y++)
                {
                    Vector3 keyI = key + new Vector3(x, tronco + y, z);

                    if (Vector3.Distance(key + new Vector3(0, tronco, 0), keyI) < widthFolhas / 2f)
                    {
                        if (blocosAdicionais.ContainsKey(keyI))
                            blocosAdicionais[keyI] = Block.getBlock("Leaves");
                        else
                            blocosAdicionais.Add(keyI, Block.getBlock("Leaves"));
                    }
                }
            }
        }
    }
    public void CreateCactus(Vector3 key)
    {
        System.Random r = new System.Random();
        int tronco = r.Next(2, 4);

        for (int i = 0; i < tronco; i++)
        {
            Vector3 keyI = key + new Vector3(0, i, 0);
            if (blocosAdicionais.ContainsKey(keyI))
                blocosAdicionais[keyI] = Block.getBlock("Cactus");
            else
                blocosAdicionais.Add(keyI, Block.getBlock("Cactus"));
        }
    }
    public void CreateOreNode(Vector3 key)
    {
        System.Random r = new System.Random();
        Block ore = Block.getBlock("Iron Ore");

        for (int x = 0; x < 4; x ++ )
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    if ((x == 0 && r.Next(0, 20) == 1) &&
                        (z == 1 && r.Next(0, 20) == 1) &&
                        (y == 3 && r.Next(0, 20) == 1))
                    {
                        AddAdBlock(key + new Vector3(x, y, z), ore);
                    }
                    else if(r.Next(0, 20) == 3)
                    {
                        AddAdBlock(key + new Vector3(x, y, z), ore);
                    }
                }
            }
        }
    }

    public void AddAdBlock(Vector3 key, Block b)
    {
        if (!blocosAdicionais.ContainsKey(key))
            blocosAdicionais.Add(key, b);
    }

    public IEnumerator CalculateMesh()
    {
        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (blocosAdicionais.ContainsKey(pos + new Vector3(x, y, z)))
                    {
                        map[x, y, z] = blocosAdicionais[pos + new Vector3(x, y, z)];
                    }
                }
            }
        }
        SaveToFile(this);

        iamWorking = true;
        working = true;

        mesh = new Mesh();
        mesh2 = new Mesh();

        /* tmap = new Thread(CMesh);
         tmap.Start();

         while (tmap != null && tmap.IsAlive)
         {
             yield return 0;
         }

         tmap = null;*/
        CMesh();

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(triangulos.ToArray(), 0);
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        {
            mesh2.vertices = NOCOLvertices.ToArray();
            mesh2.triangles = NOCOLtriangulos.ToArray();
            mesh2.colors = NOCOLcolors.ToArray();
            mesh2.RecalculateNormals();
            mesh2.uv = NOCOLuvs.ToArray();

            GetComponent<MeshCollider>().sharedMesh = mesh;
            GetComponent<MeshFilter>().sharedMesh = mesh;

            if (Mesh2.GetComponent<MeshRenderer>() == null)
                Mesh2.AddComponent<MeshRenderer>();
            if (Mesh2.GetComponent<MeshFilter>() == null)
                Mesh2.AddComponent<MeshFilter>();

            Mesh2.GetComponent<MeshFilter>().sharedMesh = mesh2;
            Mesh2.GetComponent<MeshRenderer>().material = PlayerController.m;

            if (Mesh2.GetComponent<MeshCollider>() == null)
                Mesh2.AddComponent<MeshCollider>();

            Mesh2.layer = 8;

            //Physics.IgnoreLayerCollision(9, 8);
        }

        yield return new WaitForEndOfFrame();

        ready = true;
        working = false;
        iamWorking = false;
    }
    public void CMesh()
    {
        vertices.Clear();
        triangulos.Clear();
        colors.Clear();
        uvs.Clear();

        NOCOLvertices.Clear();
        NOCOLtriangulos.Clear();
        NOCOLcolors.Clear();
        NOCOLuvs.Clear();

        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    if (map[x, y, z] != null)
                    {
                        if (!map[x, y, z].Collide)
                        {
                            AddGrass(x, y, z, map[x, y, z]);
                        }
                        else
                        {
                            if (isBlockTransparent(x, y, z + 1))
                                AddCubeFront(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y, z - 1))
                                AddCubeBack(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y + 1, z))
                                AddCubeTop(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y - 1, z))
                                AddCubeBottom(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x + 1, y, z))
                                AddCubeRight(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x - 1, y, z))
                                AddCubeLeft(x, y, z, map[x, y, z]);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator RecalculateMesh()
    {
        Blockworking = true;
        ready = true;

        mesh = new Mesh();
        vertices.Clear();
        triangulos.Clear();
        colors.Clear();
        uvs.Clear();

        CMesh();

        mesh2 = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(triangulos.ToArray(), 0);
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        {
            mesh2.vertices = NOCOLvertices.ToArray();
            mesh2.triangles = NOCOLtriangulos.ToArray();
            mesh2.colors = NOCOLcolors.ToArray();
            mesh2.RecalculateNormals();
            mesh2.uv = NOCOLuvs.ToArray();

            GetComponent<MeshCollider>().sharedMesh = mesh;
            GetComponent<MeshFilter>().sharedMesh = mesh;

            if (Mesh2.GetComponent<MeshRenderer>() == null)
            Mesh2.AddComponent<MeshRenderer>();
            if (Mesh2.GetComponent<MeshFilter>() == null)
            Mesh2.AddComponent<MeshFilter>();

            Mesh2.GetComponent<MeshFilter>().sharedMesh = mesh2;
            Mesh2.GetComponent<MeshRenderer>().material = PlayerController.m;

            if (Mesh2.GetComponent<MeshCollider>() == null)
                Mesh2.AddComponent<MeshCollider>();

            Mesh2.GetComponent<MeshCollider>().sharedMesh = mesh2;

            //Physics.IgnoreLayerCollision(9, 8);
        }

        yield return 0;

        Blockworking = false;
        yield return 0;
    }

    static LibNoise.Generator.Perlin noise = new LibNoise.Generator.Perlin(1f, 1f, 1f, 8, GameManager.seed, QualityMode.High);
    static LibNoise.Generator.Perlin biome = new LibNoise.Generator.Perlin(0.1f, 2f, 0.5f, 1, GameManager.seed, QualityMode.High);
    public static Block GetTheoreticalBlock(Vector3 pos)
    {
        noise.Seed = GameManager.seed;
        biome.Seed = GameManager.seed;

        System.Random r = new System.Random(GameManager.seed);

        Vector3 offset = new Vector3((float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000);

        double noiseX = (double)Mathf.Abs((float)(pos.x + offset.x) / 20);
        double noiseY = (double)Mathf.Abs((float)(pos.y + offset.y) / 20);
        double noiseZ = (double)Mathf.Abs((float)(pos.z + offset.z) / 20);

        double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);
        double biomeValue = biome.GetValue(noiseX, 50, noiseZ);
        
        noiseValue += (200 - (float)pos.y) / 18f;
        noiseValue /= (float)pos.y / 8f;

        if (noiseValue > 0.5f)
        {
            if (noiseValue > 0.6f)
                return Block.getBlock("Stone");

            if (biomeValue > 0.1f)
                return Block.getBlock("Sand");
            else
                return Block.getBlock("Dirt");
        }

        return null;
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        //x = x /*+ Mathf.FloorToInt(transform.position.x)*/;
        //y = y /*+ Mathf.FloorToInt(transform.position.y)*/;
        //z = z /*+ Mathf.FloorToInt(transform.position.z*/);

        z++;

        int offset = 1;
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        float shrinkSize = 0.005f;
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));

        CalculateLightFront(x, y, z - 1, b);

        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeBack(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        float shrinkSize = 0.005f;
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));
        CalculateLightBack(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeTop(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        float shrinkSize = 0.005f;
        uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));

        CalculateLightTop(x, y, z, b);

        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeBottom(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        y--;

        int offset = 1;
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        float shrinkSize = 0.005f;
        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));
        CalculateLightTop(x, y - 1, z, b);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeRight(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        float shrinkSize = 0.005f;
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));
        CalculateLightRight(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeLeft(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
       // y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        x--;

        int offset = 1;

        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        float shrinkSize = 0.005f;

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));

        CalculateLightLeft(x + 1, y, z, b);

        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    public void AddGrass(int x, int y, int z, Block b)
    {
        GameObject d = Instantiate(PlayerController.blockColPrefab, pos + new Vector3(x, y, z), Quaternion.identity) as GameObject;
        d.transform.parent = transform;

        x--;

        NOCOLtriangulos.Add(1 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(0 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(0 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(3 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(0 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(1 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(3 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(0 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(4 + 0 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 1 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(4 + 3 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 0 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(4 + 1 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 0 + NOCOLvertices.Count);

        NOCOLtriangulos.Add(4 + 0 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 2 + NOCOLvertices.Count);
        NOCOLtriangulos.Add(4 + 3 + NOCOLvertices.Count);

        ///////////////////////////////////////////////////

        float shrinkSize = 0.005f;

        NOCOLuvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        NOCOLuvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        NOCOLuvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        NOCOLuvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));

        NOCOLuvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide) + new Vector2(shrinkSize, shrinkSize));
        NOCOLuvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide) + new Vector2(-shrinkSize, shrinkSize));
        NOCOLuvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        NOCOLuvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset) + new Vector2(shrinkSize, -shrinkSize));

        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);
        NOCOLcolors.Add(b.BlockColor);

        NOCOLvertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        NOCOLvertices.Add(new Vector3(x + 1, y + 0, z + 1)); // 2
        NOCOLvertices.Add(new Vector3(x + 1, y + 1, z + 1)); // 3
        NOCOLvertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4

        NOCOLvertices.Add(new Vector3(x + 1, y + 0, z + 0)); // 1
        NOCOLvertices.Add(new Vector3(x + 0, y + 0, z + 1)); // 2
        NOCOLvertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 3
        NOCOLvertices.Add(new Vector3(x + 1, y + 1, z + 0)); // 4
    }

    void CalculateLightTop(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        Color colorToAdd = new Color(0,0,0);
        Vector2 key = new Vector2(pos.x + x, pos.y + y);
        if(heightMap.ContainsKey(key))
        {
            if(pos.y + y < heightMap[key])
            {
                //colorToAdd = Color.red;
            }
        }

        bool blockLight = true;

        if (!blockLight)
        {
            colorToAdd = Color.black;
        }

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        {
            if (!isBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] = shadowColors;
                colors[index + 1] = shadowColors;
            }

            if (!isBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 0] = shadowColors;
                colors[index + 3] = shadowColors;
            }

            if (!isBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors;
                colors[index + 0] = shadowColors;
            }

            if (!isBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors ;
                colors[index + 3] = shadowColors;
            }

            if (!isBlockTransparent(x + 1, y + 1, z + 1))
            {
                colors[index + 3] = shadowColors;
            }

            if (!isBlockTransparent(x - 1, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors ;
            }

            if (!isBlockTransparent(x - 1, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors ;
            }

            if (!isBlockTransparent(x + 1, y + 1, z - 1))
            {
                colors[index + 0] = shadowColors ;
            }

            colors[index + 0] += colorToAdd;
            colors[index + 1] += colorToAdd;
            colors[index + 2] += colorToAdd;
            colors[index + 3] += colorToAdd;
        }
    }
    void CalculateLightRight(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x + 1, y - 1, z) && isBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y , z + 1) && isBlockTransparent(x + 1, y, z))
            {
                colors[index + 1] *= shadowColors;
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y, z - 1) && isBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!isBlockTransparent(x + 1, y - 1, z + 1) &&
                isBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y - 1, z - 1) &&
                isBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y + 1, z + 1) &&
                isBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y + 1, z - 1) &&
                isBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
    }
    void CalculateLightLeft(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        //SideShadows
        {
            if (!isBlockTransparent(x - 1, y - 1, z) && isBlockTransparent(x - 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y, z + 1) && isBlockTransparent(x - 1, y, z))
            {
                colors[index + 1] *= shadowColors;
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y, z - 1) && isBlockTransparent(x - 1, y, z))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!isBlockTransparent(x - 1, y - 1, z + 1) &&
                isBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y - 1, z - 1) &&
                isBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y + 1, z + 1) &&
                isBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y + 1, z - 1) &&
                isBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
    }
    void CalculateLightBack(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x, y - 1, z - 1) && isBlockTransparent(x , y, z - 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y, z - 1) && isBlockTransparent(x, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y, z - 1) && isBlockTransparent(x, y, z - 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!isBlockTransparent(x + 1, y - 1, z - 1) &&
                isBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y - 1, z - 1) &&
                isBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y + 1, z - 1) &&
                isBlockTransparent(x + 1, y, z - 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y + 1, z - 1) &&
                isBlockTransparent(x - 1, y, z - 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
    }
    void CalculateLightFront(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!isBlockTransparent(x, y - 1, z + 1) && isBlockTransparent(x, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y, z + 1) && isBlockTransparent(x, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y, z + 1) && isBlockTransparent(x, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
                colors[index + 1] *= shadowColors;
            }
        }

        //4Sides
        {
            if (!isBlockTransparent(x + 1, y - 1, z + 1) &&
                isBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 0] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y - 1, z + 1) &&
                isBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 1] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x + 1, y + 1, z + 1) &&
                isBlockTransparent(x + 1, y, z + 1))
            {
                colors[index + 3] *= shadowColors;
            }
        }
        {
            if (!isBlockTransparent(x - 1, y + 1, z + 1) &&
                isBlockTransparent(x - 1, y, z + 1))
            {
                colors[index + 2] *= shadowColors;
            }
        }
    }

    bool isBlockTransparent(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width
            || x < 0 || y < 0 || z < 0)
        {
            Vector3 chunck = pos + new Vector3(x, y, z);
            Chunck c = Chunck.GetChunck(chunck);

            Vector3 localPos = chunck - c.pos;

            if(c == null || c.map == null)
            {
                return false;
            }
            if (c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z] == null ||
                !c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z].Collide)
                return true;
            return c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z] == null;
        }
        else
        {
            if (map[x, y, z] == null || !map[x, y, z].Collide)
                return true;
            return map[x, y, z] == null;
        }
    }
    bool InitialisBlockTransparent(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width
            || x < 0 || y < 0 || z < 0)
        {
            if (GetTheoreticalBlock(pos + new Vector3(x, y, z)) == null)
                return true;
            else
                return false;
        }

        if (map[x, y, z] == null)
            return true;

        if (map[x, y, z].Trasnsparent)
            return true;

        return false;
    }

    public static Chunck GetChunck(Vector3 wpos)
    {
        int xx = Mathf.FloorToInt(wpos.x / Chunck.Width) * Chunck.Width;
        int zz = Mathf.FloorToInt(wpos.z / Chunck.Width) * Chunck.Width;
        int yy = Mathf.FloorToInt(wpos.y / Chunck.Height) * Chunck.Height;

        if (Chuncks.ContainsKey(new Vector3(xx, yy, zz)))
            return Chuncks[new Vector3(xx, yy, zz)];
        return null;
    }
    public static bool ChunckExists(Vector3 wpos)
    {
        int xx = Mathf.FloorToInt(wpos.x / Chunck.Width) * Chunck.Width;
        int zz = Mathf.FloorToInt(wpos.z / Chunck.Width) * Chunck.Width;
        int yy = Mathf.FloorToInt(wpos.y / Chunck.Height) * Chunck.Height;

        if (Chuncks.ContainsKey(new Vector3(xx, yy, zz)))
            return true;
        return false;
    }

    public void SetBlock(Vector3 worldPos, Block b)
    {
        Vector3 localPos;
        localPos = worldPos - transform.position;

        if (localPos.x > (Width))
        {
            return;
        }

        if (Mathf.FloorToInt(localPos.x) >= Width || Mathf.FloorToInt(localPos.y) >= Height || Mathf.FloorToInt(localPos.z) >= Width
            || Mathf.FloorToInt(localPos.x) < 0 || Mathf.FloorToInt(localPos.y) < 0 || Mathf.FloorToInt(localPos.z) < 0)
        {
        }
        else
        {
            if (b == null)
            {
                Block mb = map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
                if(mb != null)
                {
                    GameObject g = new GameObject("DropedI" + mb.BlockName) as GameObject;
                    CubeDropGenerator cdg = g.gameObject.AddComponent<CubeDropGenerator>();
                    cdg.StartCube(mb);

                    cdg.transform.position = worldPos - new Vector3(1,0,0);
                    cdg.gameObject.AddComponent<Rigidbody>();
                    cdg.renderer.material = this.gameObject.renderer.material;

                    cdg.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
            map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = b;

            if (GameManager.host)
            {
                string[] blocosNoC = File.ReadAllLines(Application.dataPath + "\\" + GameManager.worldName + "\\" + pos.ToString());
                int i = 0;
                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (x == Mathf.FloorToInt(localPos.x) && y == Mathf.FloorToInt(localPos.y) && z == Mathf.FloorToInt(localPos.z))
                            {
                                if (b == null)
                                    blocosNoC[i] = "-1";
                                else
                                    blocosNoC[i] = b.BlockID.ToString();
                            }
                            i++;
                        }
                    }
                }
                File.WriteAllLines(Application.dataPath + "\\" + GameManager.worldName + "\\" + pos.ToString(), blocosNoC);
            }
        }

        StartCoroutine(RecalculateMesh());

        if (b != null) return;
        if (Mathf.FloorToInt(localPos.x) >= Width - 1)
        {
            if (right == null)
            {
                right = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x + 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z)));
            }
            StartCoroutine(right.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.x) <= 1)
        {
            if (left == null)
            {
                left = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x - 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z)));
            }
            StartCoroutine(left.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.z) >= Width - 1)
        {
            if (front == null)
            {
                front = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z + 1)));
            }
            StartCoroutine(front.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.z) <= 1)
        {
            if (back == null)
            {
                back = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z - 1)));
            }
            StartCoroutine(back.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.y) >= Height - 1)
        {
            if (top == null)
            {
                top = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y + 1), Mathf.FloorToInt(worldPos.z - 1)));
            }
            StartCoroutine(top.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.y) <= 1)
        {
            if (bottom == null)
            {
                bottom = GetChunck(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y - 1), Mathf.FloorToInt(worldPos.z - 1)));
                /*bottom2 = GetChunck(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y - 4), Mathf.FloorToInt(worldPos.z - 1));
                if (bottom2 == null)
                {

                }
                else
                {
                    int zCoordinate = Mathf.FloorToInt((worldPos.z / Width)) * Width;
                    int xCoordinate = Mathf.FloorToInt((worldPos.x / Width)) * Width;
                    int yCoordinate = Mathf.FloorToInt((worldPos.y - 4 / Height)) * Height;
                    GameObject go = Instantiate(chunckPrefab, new Vector3(xCoordinate, yCoordinate, zCoordinate), Quaternion.identity) as GameObject;
                    go.GetComponent<Chunck>().StartFunction();
                }*/
            }
            StartCoroutine(bottom.RecalculateMesh());
        }
    }
    public Block GetBlock(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
    }
}

public class Block
{
    public string BlockName;
    public Texture ItemView;
    public bool Collide = true;
    public int BlockID;
    public bool Trasnsparent = false;
    public int TextureX;
    public int TextureY;

    public int TextureXSide;
    public int TextureYSide;

    public int TextureXBottom;
    public int TextureYBottom;

    public bool BlockGlow;
    public Color BlockColor = Color.white;

    public int BlockMaxStack = 64;
    
    public Block()
    {
        BlockID = -1;
        Trasnsparent = true;
    }
    public Block(string name, bool transparent, int tX, int tY)
    {
        Trasnsparent = transparent;
        BlockName = name;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = tX;
        TextureYSide = tY;
        TextureXBottom = tX;
        TextureYBottom = tY;

        ItemView = Resources.Load<Texture>(name);
    }
    public Block(string name, bool transparent, int tX, int tY, int sX, int sY)
    {
        Trasnsparent = transparent;
        BlockName = name;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = sX;
        TextureYSide = sY;
        TextureXBottom = tX;
        TextureYBottom = tY;
        ItemView = Resources.Load<Texture>(name);
    }
    public Block(string name, bool transparent, int tX, int tY, int sX, int sY, int bX, int bY)
    {
        Trasnsparent = transparent;
        BlockName = name;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = sX;
        TextureYSide = sY;
        TextureXBottom = bX;
        TextureYBottom = bY;
        ItemView = Resources.Load<Texture>(name);
    }

    public void SetColor(Color color, bool glow)
    {
        BlockGlow = glow;
        BlockColor = color;
    }
    public void SetTexture(Texture i)
    {
        ItemView = i;
    }

    public static Block getBlock(string name)
    {
        foreach(Block b in BlockList.Blocks)
        {
            if (b.BlockName == name)
                return b;
        }
        return new Block();
    }
    public static Block getBlock(int id)
    {
        if (id < 0)
            return null;
        return BlockList.Blocks[id];
    }

    public void SetMaxStack(int maxStack)
    {
        BlockMaxStack = maxStack;
    }
}