using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Metadata;
using Button = UnityEngine.UI.Button;

public class generation : MonoBehaviour
{
    // Script de génération de la carte
    #region variable
    public bool debug;
    private bool _mapCree = false;
    [SerializeField]
    private bool pause;

    #endregion

    #region dossierParent

    [SerializeField]
    private Transform folderBlocParent;
    [SerializeField]
    private Transform folderIaParent;
    [SerializeField]
    private Canvas canvas;

    #endregion

    #region mazeBloc

    [SerializeField]
    private List<GameObject> _lstBlockMaze;     //Liste des blocs pour former le terrain
    [SerializeField]
    private GameObject _blockEnter;     //Liste des blocs pour former le terrain
    [SerializeField]
    private GameObject _blockExit; //Liste des blocs pour former le terrain
    [SerializeField]
    private GameObject _blocSensUnique; // ?? Nom anglais 
    [SerializeField]
    private GameObject _blocFermer; // ?? Nom anglais 
    [SerializeField]
    private List<float> _lstRotationBlock; //Liste des rotations possible
    [SerializeField]
    private Vector2 terrainSize = new Vector2(100f, 100f); // Taille du terrain
    [SerializeField]
    private Vector2 blockSize = new Vector2(10f, 10f); //Taille des blocs 
    [SerializeField]
    private int rows = 10; // Nombre de lignes
    [SerializeField]
    private int columns = 10; // Nombre de colonnes
    [SerializeField]
    private GameObject _entreChoisi; // Entré choisi par l'utilisateur
    [SerializeField]
    private GameObject _sortiChoisi; // Entré choisi par l'utilisateur
    private List<GameObject> allBlock = new List<GameObject>(); // List de tous les bloc instancié
    [SerializeField]
    private List<GameObject> _lstEntre; // Liste de toute les entré et sorti possible
    [SerializeField]
    private List<GameObject> _lstAllBlocNotConnected; // Une list pour tester tous les blocs qui n'arrive pas a se connecter

    #endregion

    #region displayUser

    [SerializeField]
    private GameObject _btnPrefab; // prefab du button
    [SerializeField]
    private Button _btnSave; // Btn save qui est dans la scene
    [SerializeField]
    private Button _btnStart; // Btn qui permet de lancer le pathfinder
    public Button _btnRestartGenerator; // Btn regénerer qui est dans la scene
    [SerializeField]
    private GameObject textBox; // textbox qui est en haut de la scene

    #endregion

    public bool MapCree { get => _mapCree; private set => _mapCree = value; }
    public List<GameObject> LstEntre { get => _lstEntre; set => _lstEntre = value; }
    public Button BtnRestartGenerator { get => _btnRestartGenerator; set => _btnRestartGenerator = value; }

    // public static generation instance;

    // void Awake()
    // {
    //     instance = this;
    // }

    void Start()
    {
        // Au start de la scene
        folderIaParent = GameObject.FindWithTag("IA").transform;
        StartCoroutine(generationMap());
    }

    IEnumerator generationMap()
    {
        _btnStart.gameObject.SetActive(false);
        BtnRestartGenerator.enabled = false;
        BtnRestartGenerator.gameObject.SetActive(false);
        _btnSave.gameObject.SetActive(false);

        SetTexBoxText("Génération en cours...");



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
                    GameObject block;
                    List<GameObject> lstConnecteur = new List<GameObject>();


                    if (previousBlock != null)
                    {
                        bool ConnecteurAllFalse;
                        List<string> listString = new List<string>();

                        // On instancie le nous block qui va �tre ajout� 
                        GameObject selectedRoadBlockPrefab = _lstBlockMaze[Random.Range(0, _lstBlockMaze.Count)];
                        block = Instantiate(selectedRoadBlockPrefab, position, Quaternion.identity);
                        block.transform.parent = folderBlocParent;
                        block.transform.Rotate(0f, _lstRotationBlock[Random.Range(0, _lstRotationBlock.Count)], 0f);
                        block.name = block.name + "_Row:" + row + "_Col:" + col;

                        // Pause de 0.02 seconde pour que les connections ont le temps de s'être faite 
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
                                // Ajout de la variable Connected dans la list de string
                                listString.Add(connectedCube.GetComponent<connecteur>().Connected);
                            }

                        }

                        // Test si 1 des connecteurs a une mauvaise connection
                        ConnecteurAllFalse = TestIfBadConnection(listString);

                        // Verifie le bloc est legit on le pose sinon ou le d�truit
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
                        // Connecter le point de sortie du bloc à un point de sortie en dehors de la zone de jeu
                        GameObject selectedRoadBlockPrefab = _lstBlockMaze[0];
                        block = Instantiate(selectedRoadBlockPrefab, position, Quaternion.identity);
                        block.transform.Rotate(0f, 90f, 0f);
                        block.transform.parent = folderBlocParent;
                        block.name = block.name + "_" + row + "_" + col + " FIRST";
                        previousBlock = block;
                        blockGood = true;
                        yield return new WaitForSeconds(0.02f);
                    }


                }
            }

        }


        DestroyAllGameObjectEmpty();

        AddAllBlocFromMaze();

        GetAllBlocNotConnected();

        generateBtnEnter();
        BtnRestartGenerator.enabled = true;
        BtnRestartGenerator.gameObject.SetActive(true);

        print("La map a été générer !");
    }

    // Fonctions qui ajoute tous les blocs de la map dans la liste AllBlock
    public void AddAllBlocFromMaze()
    {
        int nombreEnfants = folderBlocParent.childCount;
        for (int i = 0; i < nombreEnfants; i++)
        {
            Transform enfant = folderBlocParent.GetChild(i);
            allBlock.Add(enfant.gameObject);
        }
    }

    // Fonctions qui permets de détruire tous les gameObject vide
    public void DestroyAllGameObjectEmpty()
    {
        // Détruire tous les gameObject vide de la scene 
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "New Game Object" && obj.transform.childCount == 0)
            {
                Destroy(obj);
            }
        }
    }

    // Fonctions qui nous pemets de récuéperer toute les entrés 
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



    // Fonctions qui test si les blocs sont connecté
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

    // Fonctions qui génère tous les boutons pour placer une entre
    public void generateBtnEnter()
    {
        SetTexBoxText("Veuillez sélectionner une entré !");


        int index = 0;

        // Créer et placer les boutons dynamiquement sur les GameObjects existants
        foreach (GameObject targetObject in _lstEntre)
        {
            index++;
            // Convertir la position du GameObject en coordonnées d'écran
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);

            // Instancier le bouton à partir du prefab
            GameObject buttonGO = Instantiate(_btnPrefab, screenPos, Quaternion.identity, canvas.transform);

            // D�finir le parent du bouton
            buttonGO.transform.SetParent(canvas.transform, false); // Ne pas conserver la rotation et l'échelle du parent

            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "N:" + index;

                // Ajuster la taille du bouton pour correspondre à la taille du texte
                RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonText.preferredWidth + 20, buttonText.preferredHeight + 20);
            }
            // Acc�der au composant Button du bouton et ajouter une fonction à appeler avec un paramètre
            Button button = buttonGO.GetComponent<Button>();
            if (button != null)
            {
                // Ajouter un écouteur d'évènement au bouton avec une méthode à appeler et un paramètre
                button.onClick.AddListener(() => AjouterEntre(targetObject.transform));
            }
        }
    }


    // Fonctions qui génère tous les boutons pour placer une sortie
    public void GenerateBtnExit()
    {
        SetTexBoxText("Veuillez sélectionner une sortie !");

        int index = 0;
        List<GameObject> _lstExit = _lstEntre;
        _lstExit.Remove(_entreChoisi);


        foreach (GameObject targetObject in _lstExit)
        {
            index++;
            // Convertir la position du GameObject en coordonn�es d'�cran
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);

            // Instancier le bouton � partir du prefab
            GameObject buttonGO = Instantiate(_btnPrefab, screenPos, Quaternion.identity, canvas.transform);

            // D�finir le parent du bouton
            buttonGO.transform.SetParent(canvas.transform, false); // Ne pas conserver la rotation et l'�chelle du parent

            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "N:" + index;

                // Ajuster la taille du bouton pour correspondre � la taille du texte
                RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonText.preferredWidth + 20, buttonText.preferredHeight + 20);
            }

            // Acc�der au composant Button du bouton et ajouter une fonction � appeler avec un param�tre
            Button button = buttonGO.GetComponent<Button>();
            if (button != null)
            {
                // Ajouter un �couteur d'�v�nement au bouton avec une m�thode � appeler et un param�tre
                button.onClick.AddListener(() => AjouterExit(targetObject.transform));
            }
        }
    }


    //Fonctions qui ajoute l'entré a la carte
    public void AjouterEntre(Transform entre)
    {
        GameObject block;
        _entreChoisi = entre.gameObject;

        Vector3 position = new Vector3();
        float rotation = 0f;

        // NE MARCHE PAS POUR LES ANGLES
        if (entre.parent.gameObject.transform.position.z == -45) // SUD
        {
            position = entre.position + new Vector3(0, -1, -5);
            rotation = 180f;
        }
        else if (entre.parent.gameObject.transform.position.z == 45) // NORD
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
        block.name = _blockEnter.name;
        block.transform.parent = folderBlocParent;
        block.transform.Rotate(0f, rotation, 0f);

        RemoveButton();
        GenerateBtnExit();
    }

    //Foncions qui ajoute la sortie
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
        block.transform.parent = folderBlocParent;
        block.name = _blockExit.name;
        block.transform.Rotate(0f, rotation, 0f);

        RemoveButton();
        CompleteMap();
    }


    // Fonctions qui ajoute les blocs pour fermé la map
    public void AjouterBlocFermeture(Transform blocNotConnected, GameObject prefab)
    {
        GameObject block;

        Vector3 position = new Vector3();
        float rotation = 0f;

        if (blocNotConnected.parent.gameObject.transform.position.z == 45 && blocNotConnected.parent.gameObject.transform.position.x == 45)
        {

            // Obtenez la position de l'objet et du carr� parent dans le r�f�rentiel mondial
            Vector3 objectPosition = blocNotConnected.transform.position;
            Vector3 parentPosition = blocNotConnected.parent.gameObject.transform.position;

            // Calculez la diff�rence de position entre l'objet et le carr� parent
            Vector3 difference = objectPosition - parentPosition;

            // Calculez l'angle entre le vecteur de diff�rence et l'axe X (droite)
            float angle = Vector3.SignedAngle(difference, Vector3.right, Vector3.forward);

            // D�terminez la position relative en fonction de l'angle
            if (angle > -45 && angle <= 45)
            {
                // Debug.Log("L'objet est � droite du carr� parent.");
                position = blocNotConnected.position + new Vector3(5, -1, 0);
                rotation = -90;
            }
            else if (angle > 45 && angle <= 135)
            {
                // Debug.Log("L'objet est en haut du carr� parent.");
            }
            else if (angle > 135 || angle <= -135)
            {
                // Debug.Log("L'objet est � gauche du carr� parent.");
            }
            else
            {
                // Debug.Log("L'objet est en bas du carr� parent.");

                position = blocNotConnected.position + new Vector3(0, -1, 5);
                rotation = 180;
            }

        }
        else if (blocNotConnected.parent.gameObject.transform.position.z == 45 && blocNotConnected.parent.gameObject.transform.position.x == -45)
        {
            // Obtenez la position de l'objet et du carr� parent dans le r�f�rentiel mondial
            Vector3 objectPosition = blocNotConnected.transform.position;
            Vector3 parentPosition = blocNotConnected.parent.gameObject.transform.position;

            // Calculez la diff�rence de position entre l'objet et le carr� parent
            Vector3 difference = objectPosition - parentPosition;

            // Calculez l'angle entre le vecteur de diff�rence et l'axe X (droite)
            float angle = Vector3.SignedAngle(difference, Vector3.right, Vector3.forward);

            // D�terminez la position relative en fonction de l'angle
            if (angle > -45 && angle <= 45)
            {

            }
            else if (angle > 45 && angle <= 135)
            {
                // Debug.Log("L'objet est en haut du carr� parent.");
            }
            else if (angle > 135 || angle <= -135)
            {
                position = blocNotConnected.position + new Vector3(-5, -1, 0);
                rotation = +90;
            }
            else
            {
                //  Debug.Log("L'objet est en bas du carr� parent.");

                position = blocNotConnected.position + new Vector3(0, -1, 5);
                rotation = 180;
            }

        }
        else if (blocNotConnected.parent.gameObject.transform.position.z == -45 && blocNotConnected.parent.gameObject.transform.position.x == -45)
        {
            // Obtenez la position de l'objet et du carr� parent dans le r�f�rentiel mondial
            Vector3 objectPosition = blocNotConnected.transform.position;
            Vector3 parentPosition = blocNotConnected.parent.gameObject.transform.position;

            // Calculez la diff�rence de position entre l'objet et le carr� parent
            Vector3 difference = objectPosition - parentPosition;

            // Calculez l'angle entre le vecteur de diff�rence et l'axe X (droite)
            float angle = Vector3.SignedAngle(difference, Vector3.right, Vector3.forward);

            // D�terminez la position relative en fonction de l'angle
            if (angle > -45 && angle <= 45)
            {

            }
            else if (angle > 45 && angle <= 135)
            {
                // Debug.Log("L'objet est en haut du carr� parent.");
                position = blocNotConnected.position + new Vector3(0, -1, -5);
                rotation = 0;
            }
            else if (angle > 135 || angle <= -135)
            {
                position = blocNotConnected.position + new Vector3(-5, -1, 0);
                rotation = +90;
            }
            else
            {
                // Debug.Log("L'objet est en bas du carr� parent.");
                position = blocNotConnected.position + new Vector3(0, -1, -5);
                rotation = 0;

            }
        }
        else if (blocNotConnected.parent.gameObject.transform.position.z == -45 && blocNotConnected.parent.gameObject.transform.position.x == 45)
        {
            // Obtenez la position de l'objet et du carr� parent dans le r�f�rentiel mondial
            Vector3 objectPosition = blocNotConnected.transform.position;
            Vector3 parentPosition = blocNotConnected.parent.gameObject.transform.position;

            // Calculez la diff�rence de position entre l'objet et le carr� parent
            Vector3 difference = objectPosition - parentPosition;

            // Calculez l'angle entre le vecteur de diff�rence et l'axe X (droite)
            float angle = Vector3.SignedAngle(difference, Vector3.right, Vector3.forward);

            // D�terminez la position relative en fonction de l'angle
            if (angle > -45 && angle <= 45)
            {
                position = blocNotConnected.position + new Vector3(5, -1, 0);
                rotation = -90;
            }
            else if (angle > 45 && angle <= 135)
            {
                // Debug.Log("L'objet est en haut du carr� parent.");
                position = blocNotConnected.position + new Vector3(0, -1, -5);
                rotation = 0;
            }
            else if (angle > 135 || angle <= -135)
            {
                position = blocNotConnected.position + new Vector3(-5, -1, 0);
                rotation = +90;
            }
            else
            {
                //  Debug.Log("L'objet est en bas du carr� parent.");
                position = blocNotConnected.position + new Vector3(0, -1, -5);
                rotation = 0;

            }
        }
        else
        {
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
        }

        block = Instantiate(prefab, position, Quaternion.identity);
        block.name = prefab.name;
        block.transform.parent = folderBlocParent;
        block.transform.Rotate(0f, rotation, 0f);
    }

    public void CompleteMap()
    {
        SetTexBoxText("Génération terminer ! ");


        GetAllBlocNotConnected();

        _lstAllBlocNotConnected.Remove(_sortiChoisi);

        for (int i = 0; i < _lstAllBlocNotConnected.Count; i++)
        {
            if (_lstAllBlocNotConnected[i].tag == "mauvais")
            {
                //  Debug.Log("On mets un bloc ferme");
                AjouterBlocFermeture(_lstAllBlocNotConnected[i].transform, _blocFermer);
            }
            else
            {
                AjouterBlocFermeture(_lstAllBlocNotConnected[i].transform, _blocSensUnique);
            }
        }

        Debug.Log("Map termnimé");

        _btnSave.gameObject.SetActive(true);

        SetBtnStart();

        MapCree = true;
    }

    public void SetBtnStart()
    {
        _btnStart.onClick.RemoveAllListeners();
        _btnStart.onClick.AddListener(() => GameObject.FindWithTag("Enter").GetComponent<Enter2_PathfinderNewDirection>().StartPathfinder());
        _btnStart.gameObject.SetActive(true);
    }

    //Fonctions qui efface la carte de la scene
    public void ClearMap()
    {
        foreach (Transform child in folderBlocParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in folderIaParent)
        {
            Destroy(child.gameObject);
        }
    }


    // Fonctions qui appele toute les fonctions pour régénerer une carte
    public void RestartGeneration()
    {
        RemoveButton();
        ClearMap();
        StartCoroutine(generationMap());

    }

    // Fonctions qui remove les buttons
    public void RemoveButton()
    {
        List<GameObject> objectsWithTag = new List<GameObject>();

        GameObject[] btnObjects = GameObject.FindGameObjectsWithTag("btn");
        foreach (GameObject btnObject in btnObjects)
        {
            objectsWithTag.Add(btnObject);
        }

        foreach (GameObject obj in objectsWithTag)
        {
            Destroy(obj);
        }

        objectsWithTag.Clear();
    }

    public void RemoveButtonGeneration()
    {
        _btnRestartGenerator.gameObject.SetActive(false);
    }

    public void RemoveButtonSave()
    {
        _btnSave.gameObject.SetActive(false);

    }

    public void RemoveButtonStart()
    {
        _btnStart.gameObject.SetActive(false);

    }

    public void SetTexBoxText(string text)
    {
        textBox.GetComponent<Text>().text = text;

    }

    public void NoPathFound(){
        SetTexBoxText("Aucun chemin trouvé !");
        _btnRestartGenerator.gameObject.SetActive(true);
    }


    public void ExitFound(){
        SetTexBoxText("Le pathFinder a trouvé la sortie !");
        _btnRestartGenerator.gameObject.SetActive(true);
    }
}
