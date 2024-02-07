using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;
using Button = UnityEngine.UI.Button;

public class generation : MonoBehaviour
{
    // Script de génération de la carte

    //Bool pour le mode debugage
    public bool debug;

    private bool _mapCree = false;

    //Liste des blocs pour former le terrain
    [SerializeField]
    private List<GameObject> _lstBlockMaze;

    //Liste des blocs pour former le terrain
    [SerializeField]
    private GameObject _blockEnter;

    //Liste des blocs pour former le terrain
    [SerializeField]
    private GameObject _blockExit;
    //Liste des rotations possible

    [SerializeField]
    private List<float> _lstRotationBlock;

    //Taille des blocs 
    [SerializeField]
    private Vector2 terrainSize = new Vector2(100f, 100f);

    [SerializeField]
    private Vector2 blockSize = new Vector2(10f, 10f);


    //Nombre de colonne et de ligne pour former notre grille
    [SerializeField]
    private int rows = 10;
    [SerializeField]
    private int columns = 10;

    [SerializeField]
    private bool pause;

    [SerializeField]
    private GameObject _btnPrefab;




    public Button _btnRestartGenerator;



    [SerializeField]
    private Canvas canvas; // La toile UI sur laquelle placer les boutons


    [SerializeField]
    private Transform dossierBlocParent;
    [SerializeField]
    private List<GameObject> allBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> allCubeCollision;
    [SerializeField]
    private List<GameObject> _lstEntre;

  


    public bool MapCree { get => _mapCree; private set => _mapCree = value; }
    public List<GameObject> LstEntre { get => _lstEntre; set => _lstEntre = value; }
    public Button BtnRestartGenerator { get => _btnRestartGenerator; set => _btnRestartGenerator = value; }

    public static generation instance;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Au start de la scene
        StartCoroutine(generationMap());
    }

    IEnumerator generationMap()
    {
        BtnRestartGenerator.enabled = false;
        BtnRestartGenerator.gameObject.SetActive(false);


        // Position de départ des blocs
        float startX = -(terrainSize.x / 2) + (blockSize.x / 2);
        float startZ = -(terrainSize.y / 2) + (blockSize.y / 2);
        allBlock = new List<GameObject>();
        LstEntre = new List<GameObject>();

        GameObject previousBlock = null;
        int nbTentative = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                bool blockGood = false;
                while (!blockGood)
                {
                    nbTentative++;
                    float x = startX + col * blockSize.x;
                    float z = startZ + row * blockSize.y;
                    Vector3 position = new Vector3(x, 0f, z);
                    GameObject block = new GameObject();
                    List<GameObject> lstConnecteur = new List<GameObject>();
                   

                    if (previousBlock != null)
                    {
                        bool ConnecteurAllFalse;
                        List<string> listString = new List<string>();
                        List<string> listStringAllCol = new List<string>();

                        // On instancie le nous block qui va être ajouté 
                        GameObject selectedRoadBlockPrefab = _lstBlockMaze[Random.Range(0, _lstBlockMaze.Count)];
                        block = Instantiate(selectedRoadBlockPrefab, position, Quaternion.identity);
                        block.transform.parent = dossierBlocParent;
                        block.transform.Rotate(0f, _lstRotationBlock[Random.Range(0, _lstRotationBlock.Count)], 0f);
                        block.name = block.name + "_Row:" + row + "_Col:" + col;

                        // Pause de 1 seconde
                        yield return new WaitForSeconds(0.02f);


                        // On ajoute les connecteurs dans la liste
                        int childCount = block.transform.childCount;

                        for (int i = 0; i < childCount; i++)
                        {
                            Transform child = block.transform.GetChild(i);
                            if (child.CompareTag("Connecteur") || child.CompareTag("mauvais"))
                            {
                                lstConnecteur.Add(child.gameObject);
                            }
                        }

                        // On ajoute dans la liste toute les variables pour savoir si le bloc est autorisé a être connecté
                        foreach (GameObject connectedCube in lstConnecteur)
                        {
                            if (connectedCube.CompareTag("Connecteur") || connectedCube.CompareTag("mauvais"))
                            {
                                // On verifie les blocs
                     
                                listString.Add(connectedCube.GetComponent<connecteur>().Connected);
                            }
                            
                        }

                        print(gameObject.name + " " + lstConnecteur.Count + " " + listString.Count);


                        // Test si 1 des connecteurs a une mauvaise connection
                        ConnecteurAllFalse = TestIfBadConnection(listString);



                        // Verifie le bloc est legit on le pose sinon ou le détruit
                        if (ConnecteurAllFalse)
                        {
                            previousBlock = block;
                            blockGood = true;
                            nbTentative = 0;
                        }
                        else
                        {
                            DestroyImmediate(block);
                        }
                        
                    }
                    else
                    {
                        print("PREMIER BLOCK");
                        // Connecter le point de sortie du bloc à un point de sortie en dehors de la zone de jeu
                        GameObject selectedRoadBlockPrefab = _lstBlockMaze[0];
                        block = Instantiate(selectedRoadBlockPrefab, position, Quaternion.identity);
                        block.transform.Rotate(0f, 90f, 0f);
                        block.transform.parent = dossierBlocParent;
                        block.name = block.name + "_" + row + "_" + col + " FIRST";
                        previousBlock = block;
                        blockGood = true;
                        yield return new WaitForSeconds(0.02f);
                    }
                   

                }
            }

        }




        // Détruire tous les gameObject vide de la scene 
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "New Game Object" && obj.transform.childCount == 0)
            {
                Destroy(obj);
                // Ou utilisez DestroyImmediate(obj) pour une destruction immédiate
            }
        }




        // On ajoute dans une liste tous les blocks de la carte
        int nombreEnfants = dossierBlocParent.childCount;

        for (int i = 0; i < nombreEnfants; i++)
        {
            Transform enfant = dossierBlocParent.GetChild(i);
            allBlock.Add(enfant.gameObject); // Ajoutez l'enfant à la liste.
        }

        for (int i = 0; i < allBlock.Count; i++)
        {
            int childCount = allBlock[i].transform.childCount;

            for (int y = 0; y < childCount; y++)
            {
                Transform child = allBlock[i].transform.GetChild(y);
                if (child.CompareTag("Connecteur"))
                {
                    if(child.GetComponent<connecteur>().Connected == "pasConnecte")
                    {
                        LstEntre.Add(child.gameObject);
                    }
                    
                }
            }
        }


        MapCree = true;
        generateBtnEnter();
        BtnRestartGenerator.enabled = true;
        BtnRestartGenerator.gameObject.SetActive(true);


        print("La map a été crée !");
    }

    private bool TestIfBadConnection(List<string> listString)
    {
        bool ConnecteurAllFalse;

        if (listString.Any(b => b == "mauvaiseConnection"))
        {
            ConnecteurAllFalse = false;
        }
        else
        {

            ConnecteurAllFalse = true;
        }

        return ConnecteurAllFalse;
    }

    public void generateBtnEnter()
    {

        int index = 0;


        // Créer et placer les boutons dynamiquement sur les GameObjects existants
        foreach (GameObject targetObject in _lstEntre)
        {
            index++;
            // Convertir la position du GameObject en coordonnées d'écran
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);

            // Instancier le bouton à partir du prefab
            GameObject buttonGO = Instantiate(_btnPrefab, screenPos, Quaternion.identity, canvas.transform);

            // Définir le parent du bouton
            buttonGO.transform.SetParent(canvas.transform, false); // Ne pas conserver la rotation et l'échelle du parent



            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "N:" + index;

                // Ajuster la taille du bouton pour correspondre à la taille du texte
                RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonText.preferredWidth + 20, buttonText.preferredHeight + 20);

            }

            // Accéder au composant Button du bouton et ajouter une fonction à appeler avec un paramètre
            Button button = buttonGO.GetComponent<Button>();
            if (button != null)
            {
                // Ajouter un écouteur d'événement au bouton avec une méthode à appeler et un paramètre

                button.onClick.AddListener(() => AjouterEntre(targetObject.transform));
            }

        }

        
    }

    public void AjouterEntre(Transform entre)
    {
        GameObject block;
        Debug.Log(entre.gameObject.name);

        Vector3 position = entre.position + new Vector3(0,-1,0);

        // Placer le bloc au bonne endroit Nord sud est ouest

        block = Instantiate(_blockEnter, position, Quaternion.identity);

    }

    public void ClearMap()
    {
        foreach (Transform child in dossierBlocParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenerateMapFromSave()
    {

    }

    public void RestartGeneration()
    {
        ClearMap();
        StartCoroutine(generationMap());

    }

}
