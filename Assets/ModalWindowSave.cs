using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;
using Button = UnityEngine.UI.Button;

using System;

using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;


public class ModalWindowSave : MonoBehaviour
{
    [SerializeField]
    private GameObject modalWindow;
    [SerializeField]
    private GameObject inputField;

    [SerializeField]
    private GameObject lblErreur;
    [SerializeField]
    private ManagerUI managerUI;

    [SerializeField]
    private MapManager managerMap;

    public void OpenModal()
    {
        modalWindow.SetActive(true);
        lblErreur.SetActive(false);

    }

    public void CloseModal()
    {
        modalWindow.SetActive(false);
    }

    public void ValidateInput()
    {
        string userInput = inputField.GetComponent<InputField>().text;
        Debug.Log("Input validé : " + userInput);
        // Ici, vous pouvez traiter ou utiliser la valeur entrée par l'utilisateur
        string folderPath = Application.persistentDataPath + "/Maps/"; ;
        string mapName;
        MapData mapData = new MapData();
        bool nameValide = true;
        // Ajoutez tous les blocs de la scène à la liste de blocs

        if (userInput == "")
        {
            mapName = managerMap.GenerateUniqueMapName();
        }
        else
        {
            mapName = userInput;
        }


        if (Directory.Exists(folderPath))
        {
            string[] fileNames = Directory.GetFiles(folderPath);

            foreach (string fileName in fileNames)
            {
                if ((mapName.ToLower() + ".xml") == Path.GetFileName(fileName).ToLower())
                {
                    nameValide = false;
                }
            }
        }

        if (nameValide)
        {
            managerMap.AddBlocksToMapData(mapData);
            // Sauvegardez la carte
            managerMap.SaveMap(mapName, mapData);

            StartCoroutine(managerUI.MapSavedConfirmed());

            CloseModal(); // Fermez la fenêtre modale après la validationF
        }
        else
        {
            // Erreur nom pas valide ou deja existant
            lblErreur.SetActive(true);
        }




    }

    public void CancelInput()
    {
        Debug.Log("Input annulé.");
        CloseModal(); // Fermez la fenêtre modale
    }
}
