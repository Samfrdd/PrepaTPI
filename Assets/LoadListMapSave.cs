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
    private string folderPath; // Chemin du dossier dont vous voulez récupérer les noms de fichiers

    [SerializeField]
    private GameObject _parentFolder;

    [SerializeField]
    private GameObject _prefabButton;

    private float _contentHeight;
    void Start()
    {

        folderPath = Application.persistentDataPath + "/Maps/";
        FetchFileNames();
    }

    void FetchFileNames()
    {
        if (Directory.Exists(folderPath))
        {
            string[] fileNames = Directory.GetFiles(folderPath);

            foreach (string fileName in fileNames)
            {
                // Instancier le bouton à partir du prefab
                GameObject buttonGO = Instantiate(_prefabButton, new Vector3(0, 0, 0), Quaternion.identity, _parentFolder.transform);

                // D�finir le parent du bouton
                buttonGO.transform.SetParent(_parentFolder.transform, false); // Ne pas conserver la rotation et l'échelle du parent

                Text buttonText = buttonGO.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = Path.GetFileName(fileName);

                    // Ajuster la taille du bouton pour correspondre à la taille du texte
                    RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                    buttonRect.sizeDelta = new Vector2(buttonText.preferredWidth + 20, buttonText.preferredHeight + 20);
                     _contentHeight += buttonRect.sizeDelta.y + 5;

                }
                // Acc�der au composant Button du bouton et ajouter une fonction à appeler avec un paramètre
                Button button = buttonGO.GetComponent<Button>();
                if (button != null)
                {
                    // Ajouter un écouteur d'évènement au bouton avec une méthode à appeler et un paramètre
                    button.onClick.AddListener(() => gameObject.GetComponent<ManagerScene>().LoadScene("Scene_Main", Path.GetFileName(fileName)));
                }

               
            }

            Vector2 currentSize = _parentFolder.GetComponent<RectTransform>().sizeDelta;

            // Modifier la hauteur
            currentSize.y = _contentHeight;

            // Appliquer la nouvelle taille
            _parentFolder.GetComponent<RectTransform>().sizeDelta = currentSize;
        
        }
        else
        {
            Debug.LogError("Le dossier spécifié n'existe pas : " + folderPath);
        }
    }

}
