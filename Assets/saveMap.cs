
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;


public class MapManager : MonoBehaviour
{

    private string saveFolderPath; // Chemin où les fichiers seront sauvegardés

    [SerializeField]
    private ModalWindowSave modalWindow;

    public static MapManager instance;
    private void Awake()
    {
        // Assurez-vous d'appeler get_persistentDataPath dans Awake ou Start
        saveFolderPath = Application.persistentDataPath + "/Maps/";
    }

    public static MapManager GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            return new MapManager();

        }
    }

    public void BtnSaveClicked()
    {

        modalWindow.OpenModal();

        
    }
    public void SaveMap(string mapName, MapData mapData)
    {
        // Créer le dossier de sauvegarde s'il n'existe pas
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);

        string filePath = saveFolderPath + mapName + ".xml";

        // Sérialiser les données de la carte en XML
        XmlSerializer serializer = new XmlSerializer(typeof(MapData));
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            serializer.Serialize(streamWriter, mapData);
        }

        Debug.Log("Map saved at: " + filePath);
    }


    public string GenerateUniqueMapName()
    {
        // Générez un nom unique basé sur le timestamp actuel
        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        return "Map_" + timeStamp;
    }
    public void AddBlocksToMapData(MapData mapData)
    {


        // Récupérez tous les blocs de la scène
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Bloc");
        foreach (GameObject blockObject in blocks)
        {
            // Créez un nouvel objet BlockData pour chaque bloc de la scène
            BlocData blockData = new BlocData
            {
                position = blockObject.transform.position,
                rotation = blockObject.transform.rotation,
                type = blockObject.name // Adapter en fonction de votre structure de blocs
            };
            // Ajoutez le bloc à la liste de blocs de la carte
            mapData.blocks.Add(blockData);
        }



    }

    public MapData LoadMap(string mapName)
    {
        string filePath = saveFolderPath + mapName;

        // Vérifier si le fichier existe
        if (!File.Exists(filePath))
        {
            Debug.LogError("Map file not found: " + filePath);
            return null;
        }

        // Désérialiser les données de la carte depuis le fichier XML
        XmlSerializer serializer = new XmlSerializer(typeof(MapData));
        using (StreamReader streamReader = new StreamReader(filePath))
        {
            return (MapData)serializer.Deserialize(streamReader);
        }
    }
}
