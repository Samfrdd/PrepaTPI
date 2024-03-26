
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class loadMap : MonoBehaviour
{
    [SerializeField]
    private MapManager manager;

    [SerializeField]
    private ManagerUI managerUI;

    [SerializeField]
    private List<GameObject> _lstBlockMaze;

    [SerializeField]
    private Transform folderBlocParent;


    //manager.instance.LoadMap("Map_20240213144955.xml");

    private void Start()
    {

        Debug.Log(" load playerPref : " + PlayerPrefs.HasKey("nameMap"));

        if (PlayerPrefs.HasKey("nameMap"))
        {
          
            // Faites ce que vous devez faire avec le paramètre
        }
        else
        {
            Debug.LogWarning("Aucun paramètre trouvé !");
        }




    }

    public void GenerateMapFromSave(List<BlocData> map)
    {

        foreach (BlocData blockData in map)
        {
            GameObject desiredElement;
            string fullString = blockData.Type;
            string[] parts = fullString.Split('('); // Diviser la chaîne en fonction de '('
            string extractedString = parts[0]; // Prendre la première partie



            desiredElement = _lstBlockMaze.Find(item => item.name == extractedString);
            // Debug.Log(extractedString);

            GameObject block = Instantiate(desiredElement, blockData.Position, blockData.Rotation);

            block.transform.parent = folderBlocParent;
        }

        managerUI.SetBtnStart();
    }

}
