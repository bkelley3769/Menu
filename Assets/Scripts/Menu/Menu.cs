using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Menu : MonoBehaviour 
{
    public GameObject mainM;
    public GameObject joinM;
    public GameObject createW;

	public UnityEngine.UI.Text us;
	public UnityEngine.UI.Text usa;

    public UnityEngine.UI.InputField nM;
    public UnityEngine.UI.InputField sM;

    public UnityEngine.UI.InputField nJ;


    public void GoToMainMenu()
    {
        createW.SetActive(false);
        mainM.SetActive(true);
        joinM.SetActive(false);
    }
	public void JoinServer()
	{
		createW.SetActive(false);
		mainM.SetActive(false);
		
		GameManager.SetGame(true, false);
		Application.LoadLevel(1);
	}

    public void JoinWorld()
    {
        createW.SetActive(false);
        mainM.SetActive(false);
        joinM.SetActive(true);
    }
	public void CreateNewWorld(string name, string seeds)
	{
		us.text = "";
		if (System.IO.Directory.Exists(Application.dataPath + "\\" + name))
		{
			us.text = "World already exists with this name ..";
			return;
		}
		
		int seed = Convert.ToInt32(seeds);
		
		GameManager.seed = seed;
		GameManager.worldName = "SAVES\\" + name;
		
		GameManager.SetGame(true, true);
		
		Application.LoadLevel(1);
	}

	public void CreateWorld()
	{
		createW.SetActive(true);
		mainM.SetActive(false);
		joinM.SetActive(false);
		
		nM.text = "";
		sM.text = "";
		
		us.text = "";
	}
    public void Exit()
    {
        Application.Quit();
    }

    public void GenerateMap()
    {
        CreateNewWorld(nM.text, sM.text);
    }

    public void JoinMap()
    {
        GameManager.SetGame(true, true);
        string wName = nJ.text;

        GameManager.worldName = "SAVES\\" + wName;

        usa.text = "";
        if (!System.IO.Directory.Exists(Application.dataPath + "\\" + GameManager.worldName))
        {
			usa.text = "World does not exist ..";
            return;
        }

        GameManager.seed = int.Parse(System.IO.File.ReadAllText(Application.dataPath + "\\" + GameManager.worldName + "\\seed"));
        Application.LoadLevel(1);
    }
}
