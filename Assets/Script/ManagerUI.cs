/** 
***********************************************************************
Auteur : Sam Freddi
Date : 26.03.2024
Description : ManagerScene.cs permets de manager tous l'affichage de notre scene
version 1.0
***********************************************************************
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;
using Button = UnityEngine.UI.Button;

public class ManagerUI : MonoBehaviour
{

    [SerializeField]
    private GameObject _btnPrefab; // prefab du button
    [SerializeField]
    private Button _btnSave; // Btn save qui est dans la scene
    [SerializeField]
    private Button _btnStart; // Btn qui permet de lancer le pathfinder
    [SerializeField]
    private Button _btnRestartGenerator; // Btn regénerer qui est dans la scene
    [SerializeField]
    private GameObject _textBox; // textbox qui est en haut de la scene

    [SerializeField]
    private GameObject _lblConfirmationSave; // textbox qui est en haut de la scene

    [SerializeField]
    private GameObject _infoPathfinder; // textbox qui est en haut de la scene

    private int _nbPathfinder;


    public Button BtnRestartGenerator { get => _btnRestartGenerator; set => _btnRestartGenerator = value; }
    public GameObject BtnPrefab { get => _btnPrefab; set => _btnPrefab = value; }
    public Button BtnSave { get => _btnSave; set => _btnSave = value; }
    public Button BtnStart { get => _btnStart; set => _btnStart = value; }
    public GameObject TextBox { get => _textBox; set => _textBox = value; }
    public GameObject InfoPathfinder { get => _infoPathfinder; set => _infoPathfinder = value; }
    public int NbPathfinder { get => _nbPathfinder; set => _nbPathfinder = value; }
    public GameObject LblConfirmationSave { get => _lblConfirmationSave; set => _lblConfirmationSave = value; }

    // Start is called before the first frame update


    public void Start()
    {
        Debug.Log(" random Gen player : " + PlayerPrefs.HasKey("nameMap"));

        if (PlayerPrefs.HasKey("nameMap"))
        {
            string nameMap = PlayerPrefs.GetString("nameMap"); // Récupérez le paramètre de PlayerPrefs
            Debug.Log("Paramètre récupéré : " + nameMap);
            MapData _mapToLoad = gameObject.GetComponent<MapManager>().LoadMap(nameMap);
            Debug.Log(_mapToLoad);
            gameObject.GetComponent<LoadMap>().GenerateMapFromSave(_mapToLoad.Blocks);
            SetTexBoxText("Map telechargé : " + nameMap);
            PlayerPrefs.DeleteKey("nameMap");
        }
        else
        {
            gameObject.GetComponent<RandomGeneration>().StartGeneation();
        }
    }
    public void UpdateView()
    {
        InfoPathfinder.GetComponent<Text>().text = " Nombre de pathfinder : " + _nbPathfinder;

    }
    public void RemoveButtonGeneration()
    {
        BtnRestartGenerator.gameObject.SetActive(false);
    }

    public void ClearInfo()
    {
        NbPathfinder = 0;
        InfoPathfinder.GetComponent<Text>().text = "";
        UpdateView();

    }
    public void RemoveButtonSave()
    {
        BtnSave.gameObject.SetActive(false);

    }

    public void RemoveButtonStart()
    {
        BtnStart.gameObject.SetActive(false);

    }

    public IEnumerator MapSavedConfirmed()
    {
        LblConfirmationSave.SetActive(true);
        RemoveButtonSave();
        yield return new WaitForSeconds(5f);
        LblConfirmationSave.SetActive(false);

    }

    public void SetTexBoxText(string text)
    {
        TextBox.GetComponent<Text>().text = text;
    }

    public void SetBtnStart()
    {
        BtnStart.onClick.RemoveAllListeners();
        BtnStart.onClick.AddListener(() => GameObject.FindWithTag("Enter").GetComponent<Enter2_PathfinderNewDirection>().StartPathfinder());
        BtnStart.gameObject.SetActive(true);
    }
}
