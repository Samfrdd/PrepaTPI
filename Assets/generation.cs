using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class generation : MonoBehaviour
{
    // Script de g�n�ration de la carte

    //Bool pour le mode debugage
    public bool debug;

    private bool _mapCree = false;

    //Liste des blocs pour former le terrain
    [SerializeField]
    private List<GameObject> _lstBlockMaze;
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
    private bool allCollisionDetected = false;
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
        // Position de d�part des blocs
        float startX = -(terrainSize.x / 2) + (blockSize.x / 2);
        float startZ = -(terrainSize.y / 2) + (blockSize.y / 2);

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

                        // On instancie le nous block qui va �tre ajout� 
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

                        // On ajoute dans la liste toute les variables pour savoir si le bloc est autoris� a �tre connect�
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
                        print("PREMIER BLOCK");
                        // Connecter le point de sortie du bloc � un point de sortie en dehors de la zone de jeu
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




        // D�truire tous les gameObject vide de la scene 
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "New Game Object" && obj.transform.childCount == 0)
            {
                Destroy(obj);
                // Ou utilisez DestroyImmediate(obj) pour une destruction imm�diate
            }
        }




        // On ajoute dans une liste tous les blocks de la carte
        int nombreEnfants = dossierBlocParent.childCount;

        for (int i = 0; i < nombreEnfants; i++)
        {
            Transform enfant = dossierBlocParent.GetChild(i);
            allBlock.Add(enfant.gameObject); // Ajoutez l'enfant � la liste.
        }

        for (int i = 0; i < allBlock.Count; i++)
        {
            int childCount = allBlock[i].transform.childCount;

            for (int y = 0; y < childCount; y++)
            {
                Transform child = allBlock[i].transform.GetChild(y);
                if (child.CompareTag("Connecteur") || child.CompareTag("mauvais"))
                {
                    if(child.GetComponent<connecteur>().Connected == "pasConnecte")
                    {
                        LstEntre.Add(child.gameObject);
                    }
                    
                }
            }
        }


        MapCree = true;
        print("La map a �t� cr�e !");
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

}
