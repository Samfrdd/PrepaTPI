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

    [SerializeField]
    private GameObject _blocSensUnique;


    [SerializeField]
    private GameObject _blocFermer;
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
    private GameObject _entreChoisi;

    [SerializeField]
    private GameObject _sortiChoisi;

    [SerializeField]
    private GameObject _btnPrefab;




    public Button _btnRestartGenerator;

    [SerializeField]
    private GameObject textBox;

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

    [SerializeField]
    private List<GameObject> _lstAllBlocNotConnected;



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

        textBox.GetComponent<Text>().text = "Génération en cours...";

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

                     //   print(gameObject.name + " " + lstConnecteur.Count + " " + listString.Count);


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

        GetAllBlocNotConnected();


        MapCree = true;
        generateBtnEnter();
        BtnRestartGenerator.enabled = true;
        BtnRestartGenerator.gameObject.SetActive(true);


        print("La map a été crée !");
    }

    public void GetAllBlocNotConnected()
    {
        LstEntre.Clear();
        _lstAllBlocNotConnected.Clear();

        for (int i = 0; i < allBlock.Count; i++)
        {
            int childCount = allBlock[i].transform.childCount;

            for (int y = 0; y < childCount; y++)
            {
                Transform child = allBlock[i].transform.GetChild(y);
                if (child.CompareTag("Connecteur"))
                {
                    if (child.GetComponent<connecteur>().Connected == "pasConnecte")
                    {
                        LstEntre.Add(child.gameObject);
                        _lstAllBlocNotConnected.Add(child.gameObject);

                    }

                }
                else if (child.CompareTag("mauvais"))
                {
                    if (child.GetComponent<connecteur>().Connected == "pasConnecte")
                    {
                        _lstAllBlocNotConnected.Add(child.gameObject);
                    }

                }
            }
        }
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
        textBox.GetComponent<Text>().text = "Veuillez sélectionner une entré !";

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

    public void GenerateBtnExit()
    {
        textBox.GetComponent<Text>().text = "Veuillez sélectionner une sortie !";

        int index = 0;

        List<GameObject> _lstExit = _lstEntre;

        _lstExit.Remove(_entreChoisi);

        // Créer et placer les boutons dynamiquement sur les GameObjects existants
        foreach (GameObject targetObject in _lstExit)
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

                button.onClick.AddListener(() => AjouterExit(targetObject.transform));
            }
        }
    }

    public void AjouterEntre(Transform entre)
    {
        GameObject block;
        Debug.Log(entre.gameObject.name);


        _entreChoisi = entre.gameObject;

          Vector3 position = new Vector3();
        float rotation = 0f;

        // NE MARCHE PAS POUR LES ANGLES

        
        if (entre.parent.gameObject.transform.position.z == -45) // SUD
        {
            position = entre.position + new Vector3(0, -1, -5);
            rotation = 180f;
        }
        else if(entre.parent.gameObject.transform.position.z == 45) // NORD
        {
            position = entre.position + new Vector3(0, -1, 5);

        }
        else if (entre.parent.gameObject.transform.position.x == 45) // EST
        {
            position = entre.position + new Vector3(5, -1, 0);
            rotation = 90;

        }
        else if (entre.parent.gameObject.transform.position.x == -45) // OUEST
        {
            position = entre.position + new Vector3(-5, -1, 0);
            rotation = -90;

        }


        block = Instantiate(_blockEnter, position, Quaternion.identity);
        block.name = "Entre";
        block.transform.parent = dossierBlocParent;
        block.transform.Rotate(0f, rotation, 0f);


        RemoveButton();
        GenerateBtnExit();
    }

    public void AjouterExit(Transform exit)
    {
        GameObject block;
        Debug.Log(exit.gameObject.name);

        List<GameObject> _lstExit = _lstEntre;


        _sortiChoisi = exit.gameObject;

        _lstExit.Remove(_sortiChoisi);

        this._lstEntre = _lstExit;

        Vector3 position = new Vector3();
        float rotation = 0f;

        // NE MARCHE PAS POUR LES ANGLES


        if (exit.parent.gameObject.transform.position.z == -45) // SUD
        {
            position = exit.position + new Vector3(0, -1, -5);
            rotation = 180f;
        }
        else if (exit.parent.gameObject.transform.position.z == 45) // NORD
        {
            position = exit.position + new Vector3(0, -1, 5);

        }
        else if (exit.parent.gameObject.transform.position.x == 45) // EST
        {
            position = exit.position + new Vector3(5, -1, 0);
            rotation = 90;

        }
        else if (exit.parent.gameObject.transform.position.x == -45) // OUEST
        {
            position = exit.position + new Vector3(-5, -1, 0);
            rotation = -90;

        }


        block = Instantiate(_blockExit, position, Quaternion.identity);
        block.transform.parent = dossierBlocParent;
        block.name = "Sortie";
        block.transform.Rotate(0f, rotation, 0f);


        RemoveButton();

        CompleteMap();

    }

    public void AjouterBlocFermeture(Transform blocNotConnected, GameObject prefab)
    {
        GameObject block;
      
        Vector3 position = new Vector3();
        float rotation = 0f;

        // NE MARCHE PAS POUR LES ANGLES

        

        if (blocNotConnected.parent.gameObject.transform.position.z == -45) // SUD
        {
            position = blocNotConnected.position + new Vector3(0, -1, -5);
            rotation = 0;
        }
        else if (blocNotConnected.parent.gameObject.transform.position.z == 45) // NORD
        {
            position = blocNotConnected.position + new Vector3(0, -1, 5);
            rotation = 180;

        }
        else if (blocNotConnected.parent.gameObject.transform.position.x == 45) // EST
        {
            position = blocNotConnected.position + new Vector3(5, -1, 0);
            rotation = -90;

        }
        else if (blocNotConnected.parent.gameObject.transform.position.x == -45) // OUEST
        {
            position = blocNotConnected.position + new Vector3(-5, -1, 0);
            rotation = +90;

        }


        block = Instantiate(prefab, position, Quaternion.identity);
        block.name = "BlocFermeture";
        block.transform.parent = dossierBlocParent;
        block.transform.Rotate(0f, rotation, 0f);


 

    }

    public void CompleteMap()
    {
        textBox.GetComponent<Text>().text = "Génération terminer ! ";


        GetAllBlocNotConnected();


        // ?
        _lstAllBlocNotConnected.Remove(_sortiChoisi);


        for (int i = 0; i < _lstAllBlocNotConnected.Count; i++)
        {
            Debug.Log("tag : " + _lstAllBlocNotConnected[i].tag);
            if (_lstAllBlocNotConnected[i].tag == "mauvais")
            {
                Debug.Log("On mets un bloc ferme");
                AjouterBlocFermeture(_lstAllBlocNotConnected[i].transform, _blocFermer);
            }
            else
            {
                AjouterBlocFermeture(_lstAllBlocNotConnected[i].transform, _blocSensUnique );
            }
                   
        }


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
        RemoveButton();
        ClearMap();
        StartCoroutine(generationMap());

    }

    public void RemoveButton()
    {
        List<GameObject> objectsWithTag = new List<GameObject>();

        GameObject[] btnObjects = GameObject.FindGameObjectsWithTag("btn");
        foreach (GameObject btnObject in btnObjects)
        {
            objectsWithTag.Add(btnObject);
        }

        // Détruire chaque objet dans la liste
        foreach (GameObject obj in objectsWithTag)
        {
            Destroy(obj);
        }

        // Effacer la liste après destruction des objets
        objectsWithTag.Clear();
    }

}
