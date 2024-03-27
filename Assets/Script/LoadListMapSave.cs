using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;
using Button = UnityEngine.UI.Button;
using System.IO;
public class LoadListMapSave : MonoBehaviour
{
    private string _folderPath; // Chemin du dossier dont vous voulez récupérer les noms de fichiers

    [SerializeField]
    private GameObject _parentFolder;

    [SerializeField]
    private GameObject _prefabButton;

    private float _contentHeight;

    public string FolderPath { get => _folderPath;private set => _folderPath = value; }
    public GameObject ParentFolder { get => _parentFolder;private set => _parentFolder = value; }
    public GameObject PrefabButton { get => _prefabButton;private set => _prefabButton = value; }
    public float ContentHeight { get => _contentHeight;private set => _contentHeight = value; }

    void Start()
    {
        FolderPath = Application.persistentDataPath + "/Maps/";
        FetchFileNames();
    }

    void FetchFileNames()
    {
        if (Directory.Exists(FolderPath))
        {
            string[] fileNames = Directory.GetFiles(FolderPath);

            foreach (string fileName in fileNames)
            {
                // Instancier le bouton à partir du prefab
                GameObject buttonGO = Instantiate(PrefabButton, new Vector3(0, 0, 0), Quaternion.identity, ParentFolder.transform);

                // D�finir le parent du bouton
                buttonGO.transform.SetParent(ParentFolder.transform, false); // Ne pas conserver la rotation et l'échelle du parent

                Text buttonText = buttonGO.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = Path.GetFileName(fileName);

                    // Ajuster la taille du bouton pour correspondre à la taille du texte
                    RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                    buttonRect.sizeDelta = new Vector2(buttonText.preferredWidth + 20, buttonText.preferredHeight + 20);
                     ContentHeight += buttonRect.sizeDelta.y + 5;

                }
                // Acc�der au composant Button du bouton et ajouter une fonction à appeler avec un paramètre
                Button button = buttonGO.GetComponent<Button>();
                if (button != null)
                {
                    // Ajouter un écouteur d'évènement au bouton avec une méthode à appeler et un paramètre
                    button.onClick.AddListener(() => gameObject.GetComponent<ManagerScene>().LoadScene("Main_NewPathFinder", Path.GetFileName(fileName)));
                }

               
            }

            Vector2 currentSize = ParentFolder.GetComponent<RectTransform>().sizeDelta;

            // Modifier la hauteur
            currentSize.y = ContentHeight;

            // Appliquer la nouvelle taille
            ParentFolder.GetComponent<RectTransform>().sizeDelta = currentSize;
        
        }
        else
        {
            Debug.LogError("Le dossier spécifié n'existe pas : " + FolderPath);
        }
    }

}
