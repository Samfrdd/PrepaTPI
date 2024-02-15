using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class path : MonoBehaviour
{
    [SerializeField]
    private GameObject _nextPoint;

    [SerializeField]
    private bool _alreadyPass;
    void Start()
    {
        _alreadyPass = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("pathfinder"))
        {
            if (!_alreadyPass)
            {

                _alreadyPass = true;


                if (other.gameObject.GetComponent<pathfinding2>().Waypoints.Contains(_nextPoint.transform))
                {
                    other.gameObject.GetComponent<pathfinding2>().AddWaypoint(gameObject.GetComponent<connecteur>().BlockConnecte.transform);
                    
                }
                else
                {
                    other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_nextPoint.transform);
              

                }
            }
            else
            {
                other.gameObject.GetComponent<pathfinding2>().BlockPathfinder();

            }

        }

    }
}
