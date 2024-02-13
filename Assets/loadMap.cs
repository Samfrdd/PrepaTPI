
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
    private List<GameObject> _lstBlockMaze;

    [SerializeField]
    private Transform folderBlocParent;



    //manager.instance.LoadMap("Map_20240213144955.xml");

    private void Start()
    {

        MapData _mapToLoad = manager.LoadMap("Map_20240213161326");

        GenerateMapFromSave(_mapToLoad.blocks);
    }

    public void GenerateMapFromSave(List<BlocData> map)
    {
        foreach (BlocData blockData in map)
        {
            GameObject desiredElement;
            string fullString = blockData.type;
            string[] parts = fullString.Split('('); // Diviser la chaîne en fonction de '('
            string extractedString = parts[0]; // Prendre la première partie



            desiredElement = _lstBlockMaze.Find(item => item.name == extractedString);
            Debug.Log(extractedString);

            GameObject block = Instantiate(desiredElement, blockData.position, blockData.rotation);

             block.transform.parent = folderBlocParent;
        }
    }

}
