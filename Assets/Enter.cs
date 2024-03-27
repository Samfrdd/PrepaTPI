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
