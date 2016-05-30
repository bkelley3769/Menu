using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject chunckPrefab;
    public static GameObject ChunckPrefab;
    public static int viewRange = Chunck.Width * 8;

	public static Material m;
	public Material mf;

    public GameObject BlockHighlight;
    public GameObject camera;

    public static GameObject blockColPrefab;
    public GameObject blockColPrefafb;

    public CharacterController cc;

	public LayerMask lm;

    bool once = true;
    void Awake()
    {
        Physics.IgnoreLayerCollision(0, 8);
        Physics.IgnoreLayerCollision(1, 8);
        Physics.IgnoreLayerCollision(9, 9);
        blockColPrefab = blockColPrefafb;
        m = mf;
        if(!networkView.isMine)
        {
            Destroy(GetComponent<FPSInputController>());
            Destroy(GetComponent<CharacterMotor>());
            Destroy(GetComponent<CharacterController>());
            Destroy(GetComponent<MouseLook>());
            Destroy(GetComponent<PlayerInventory>());

            Destroy(GetComponentInChildren<Camera>().gameObject);

            return;
        }

        BlockHighlight.transform.parent = null;

        if(Network.isServer)
        {
            networkView.RPC("All_GetSeed", RPCMode.AllBuffered, GameManager.seed);
        }

        Physics.IgnoreLayerCollision(9, 8);

        //cc.enabled = false;
        ChunckPrefab = chunckPrefab;
        for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunck.Width)
        {
            for (float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunck.Height)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunck.Width)
                {
                    int xx = Mathf.FloorToInt(x / Chunck.Width) * Chunck.Width;
                    int zz = Mathf.FloorToInt(z / Chunck.Width) * Chunck.Width;
                    int yy = Mathf.FloorToInt(y / Chunck.Height) * Chunck.Height;

                    if (!Chunck.ChunckExists(new Vector3(xx, yy, zz)))
                    {
                        Instantiate(chunckPrefab, new Vector3(xx, yy, zz), Quaternion.identity);
                    }
                }
            }
        }
    }


    void ChunckSpawn()
    {
        if (Chunck.working) return;
        float lastDistance = 9999999f;
        Chunck c = null;

        Dictionary<Vector3, Chunck> copyC = Chunck.Chuncks;

        foreach(var dc in copyC)
        {
            float d = Vector3.Distance(this.transform.position, dc.Value.pos);

            if(d < lastDistance)
            {
                Chunck cc = dc.Value.gameObject.GetComponent<Chunck>();
                if (cc.generatedMap == false)
                {
                    lastDistance = d;
                    c = cc;
                }
            }
        }

        if(c != null)
        {
            if(GameManager.host)
                c.StartFunction();
            else
            {
                networkView.RPC("Server_RequestChunck", RPCMode.Server, c.transform.position, Network.player);
                Chunck.working = true;
            }
        }
    }
	void Update ()
	{
		if (!networkView.isMine) return;
		
		/*if (Time.time > 5)
            cc.enabled = true;*/
		
		for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunck.Width)
		{
			for (float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunck.Height)
			{
				for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunck.Width)
				{
					int xx = Mathf.FloorToInt(x / Chunck.Width) * Chunck.Width;
					int zz = Mathf.FloorToInt(z / Chunck.Width) * Chunck.Width;
					int yy = Mathf.FloorToInt(y / Chunck.Height) * Chunck.Height;
					
					if (!Chunck.ChunckExists(new Vector3(xx, yy, zz)))
					{
						Instantiate(chunckPrefab, new Vector3(xx, yy, zz), Quaternion.identity);
					}
				}
			}
		}
		
		Chunck c = Chunck.GetChunck(transform.position);
		if (c == null || !c.ready)
		{
			cc.enabled = false;
		}
		else
		{
			cc.enabled = true;
		}
	}
	
	void FixedUpdate()
	{
		BlockController();
		ChunckSpawn();
	}
    void BlockController()
    {
        if (Chunck.Blockworking || PlayerInventory.showInventory) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7f, lm))
        {
            Vector3 worldBlockPosPL = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
            Vector3 p = hit.point - hit.normal / 2;


            if (BlockHighlight != null)
                BlockHighlight.transform.position = new Vector3(Mathf.Floor(p.x), Mathf.Floor(p.y), Mathf.Floor(p.z));

            if (Input.GetMouseButtonDown(0))
            {
                //SetBlock(p, null);
                networkView.RPC("All_SetBlock", RPCMode.All, p, -1, (hit.transform.gameObject.layer == 8 && !hit.transform.name.Contains("Mesh 2")));
            }
            if (Input.GetMouseButtonDown(1))
            {
                p = hit.point + hit.normal / 2;

                Vector3 newBlockPos = new Vector3(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z));
     
                if (worldBlockPosPL == newBlockPos)
                {
                    return;
                }
                if (worldBlockPosPL + new Vector3(0, 1, 0) == newBlockPos)
                {
                    return;
                }

                Block selectedBlock = PlayerInventory.HotbarItems[PlayerInventory.SelectedBlock];
                if (selectedBlock != null)
                {
                    PlayerInventory.HotbarNum[PlayerInventory.SelectedBlock] -= 1;
                    if (PlayerInventory.HotbarNum[PlayerInventory.SelectedBlock] <= 0)
                    {
                        PlayerInventory.HotbarItems[PlayerInventory.SelectedBlock] = null;
                    }
                    networkView.RPC("All_SetBlock", RPCMode.All, p, selectedBlock.BlockID, (hit.transform.gameObject.layer == 8 && !hit.transform.name.Contains("Mesh 2")));
                }
            }
        }
        else
        {
            if (BlockHighlight != null)
                BlockHighlight.transform.position = new Vector3(0, -1000, 0);
        }
    }

    [RPC]
    public void All_GetSeed(int seed)
    {
        GameManager.seed = seed;
    }

    [RPC]
    public void Server_RequestChunck(Vector3 pos, NetworkPlayer pl)
    {
        Chunck c = Chunck.GetChunck(pos);
        
        Block[, ,] Cmap;

        if(c == null || c.map == null)
        {
            Cmap = new Block[Chunck.Width, Chunck.Height, Chunck.Width];

            for (int z = 0; z < Chunck.Width; z++)
            {
                for (int x = 0; x < Chunck.Width; x++)
                {
                    for (int y = 0; y < Chunck.Height; y++)
                    {
                        Block bloc = Chunck.GetTheoreticalBlock(pos + new Vector3(x, y, z));
                        Cmap[x, y, z] = bloc;

                        if (Cmap[x, y, z] == Block.getBlock("Dirt") && Chunck.GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                        {
                            Cmap[x, y, z] = Block.getBlock("Grass");
                        }
                    }
                }
            }
            
            if( c != null)
            {
                c.map = Cmap;
            }
        }
        else
        {
            Cmap = c.map;
        }

        byte[] map = new byte[Chunck.Width * Chunck.Width * Chunck.Height];

        int i = 0;
        for (int z = 0; z < Chunck.Width; z++)
        {
            for (int x = 0; x < Chunck.Width; x++)
            {
                for (int y = 0; y < Chunck.Height; y++)
                {
                    if (c.map[x, y, z] == null)
                    {
                        map[i] = (byte)0;
                    }
                    else
                        map[i] = (byte)(Cmap[x, y, z].BlockID + 1);
                    i++;
                }
            }
        }

        byte[] mapCompressed = CLZF2.Compress(map);
        networkView.RPC("Client_ReceiveChunck", pl, pos, mapCompressed);
    }
    [RPC]
    public void Client_ReceiveChunck(Vector3 pos, byte[] map)
    {
        Chunck c = Chunck.GetChunck(pos);
        Block[, ,] MAP = new Block[Chunck.Width, Chunck.Height, Chunck.Width];
        byte[] mapDecompressed = CLZF2.Decompress(map);

        int i = 0;
        for (int z = 0; z < Chunck.Width; z++)
        {
            for (int x = 0; x < Chunck.Width; x++)
            {
                for (int y = 0; y < Chunck.Height; y++)
                {
                    MAP[x, y, z] = Block.getBlock(mapDecompressed[i] - 1);
                    i++;
                }
            }
        }
        
        c.StartFunction(MAP);
    }

    [RPC]
    public void All_SetBlock(Vector3 pos, int blockID, bool nocol)
    {
        SetBlock(pos, Block.getBlock(blockID), nocol);
    }

    void SetBlock(Vector3 p, Block b, bool nocol)
    {
        if(b == null && nocol)
        {
            Collider[] cc = Physics.OverlapSphere(p, 0.5f);
            for(int i= 0; i < cc.Length; i++)
            {
                if (cc[i].gameObject.layer != 8 ||
                    cc[i].transform.name.Contains("Mesh 2")) continue;
                Destroy(cc[i].gameObject);
            }
        }

        Chunck chunck = Chunck.GetChunck(new Vector3(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
        Vector3 localPos = chunck.transform.position - p;

        if ((Mathf.FloorToInt(localPos.x) * -1) == (Chunck.Width))
        {
            Chunck c = Chunck.GetChunck(new Vector3(Mathf.FloorToInt(p.x + 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }
        else
        {
            Chunck c = Chunck.GetChunck(new Vector3(Mathf.FloorToInt(p.x - 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }


        if (b == null)
        {

        }
        chunck.SetBlock(p + new Vector3(+1, 0, 0), b);
    }
}
