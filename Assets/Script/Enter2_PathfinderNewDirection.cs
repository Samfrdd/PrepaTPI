using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter2_PathfinderNewDirection : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabPathfinder;

    [SerializeField]
    private GameObject _prefabParent;

    [SerializeField]
    private GameObject _gameManager;


    public GameObject PrefabPathfinder { get => _prefabPathfinder; set => _prefabPathfinder = value; }
    public GameObject PrefabParent { get => _prefabParent; set => _prefabParent = value; }
    public GameObject GameManager { get => _gameManager; set => _gameManager = value; }

    // Start is called before the first frame update

    void Start()
    {

        PrefabParent = GameObject.FindWithTag("IA");
        GameManager = GameObject.FindWithTag("gameManager");
        PrefabParent = GameObject.FindWithTag("IA");
    }

    public void StartPathfinder()
    {
        SpawnPathfinder();
        GameManager.GetComponent<ManagerUI>().RemoveButtonGeneration();
        GameManager.GetComponent<ManagerUI>().RemoveButtonSave();
        GameManager.GetComponent<ManagerUI>().RemoveButtonStart();
    }
    // Update is called once per frame
    public void SpawnPathfinder()
    {
        GameObject pathfinder = Instantiate(PrefabPathfinder, transform.position, transform.rotation);
        pathfinder.transform.parent = PrefabParent.transform;
        pathfinder.GetComponent<Pathfinding1>().SetOriginal(true);

    }






}