using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
    public GameObject playerPrefab;

    void Awake()
    {
        if( GameManager.server && GameManager.host)
        {
            Network.InitializeServer(16, 25565, false);
        }
        else if (!GameManager.server && !GameManager.host)
        {
            Network.InitializeServer(16, 20000, false);
        }
        else
        {
            Network.Connect("localhost", 25565);
        }
    }

    void OnServerInitialized()
    {
        SpawnPlayer();
    }
    void OnConnectedToServer()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
         Network.Instantiate(playerPrefab, new Vector3(0, 110, 0), Quaternion.identity, 0);
    }
}
