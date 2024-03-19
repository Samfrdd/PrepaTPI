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

    private bool isStarted = false;
    // Start is called before the first frame update

    void Start()
    {

        _prefabParent = GameObject.FindWithTag("IA");
        _gameManager = GameObject.FindWithTag("gameManager");
        _prefabParent = GameObject.FindWithTag("IA");
    }

    public void StartPathfinder()
    {
        isStarted = true;
        SpawnPathfinder();
        _gameManager.GetComponent<ManagerUI>().RemoveButtonGeneration();
        _gameManager.GetComponent<ManagerUI>().RemoveButtonSave();
        _gameManager.GetComponent<ManagerUI>().RemoveButtonStart();
    }
    // Update is called once per frame
    public void SpawnPathfinder()
    {
        GameObject pathfinder = Instantiate(_prefabPathfinder, transform.position, transform.rotation);
        pathfinder.transform.parent = _prefabParent.transform;
        pathfinder.GetComponent<Pathfinding1>().SetOriginal(true);

    }






}
