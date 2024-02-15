using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter : MonoBehaviour
{

    [SerializeField]
    private Transform _nextPoint;

    [SerializeField]
    private GameObject _gameManager;

    [SerializeField]
    private GameObject _prefabPathfinder;

    [SerializeField]
    private GameObject _prefabParent;

    private bool isStarted = false;
    // Start is called before the first frame update

    void Start()
    {
        _gameManager = GameObject.FindWithTag("gameManager");
        _prefabParent = GameObject.FindWithTag("IA");

    }

    private void Update()
    {
        
    }

    public void StartPathfinder()
    {
        isStarted = true;
        SpawnPathfinder();
        _gameManager.GetComponent<generation>().RemoveButtonGeneration();
        _gameManager.GetComponent<generation>().RemoveButtonSave();
        _gameManager.GetComponent<generation>().RemoveButtonStart();
    
    }
    // Update is called once per frame
    public void SpawnPathfinder()
    {
        GameObject pathfinder = Instantiate(_prefabPathfinder, transform.position, transform.rotation);
        pathfinder.transform.parent = _prefabParent.transform;
        pathfinder.GetComponent<pathfinding2>().IsOriginal = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {

            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(gameObject.transform);


            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_nextPoint);





        }
    }
}
