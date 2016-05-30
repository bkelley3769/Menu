using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public static int seed;
    public static string worldName;

    public static bool server = false;
    public static bool host = false;

    public static void SetGame(bool SERVER, bool HOST)
    {
        server = SERVER;
        host = HOST;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
