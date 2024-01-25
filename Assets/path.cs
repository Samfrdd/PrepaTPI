using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class path : MonoBehaviour
{
    [SerializeField]
    private GameObject _nextPoint;
    [SerializeField]
    private GameObject _prevPoint;
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
                if (_nextPoint.transform == gameObject.transform)
                {
                    // On arrive a la fin du bloc et on veut passer au suivant
                    Debug.Log("On arrive a la fin d'ub bloc");
                    other.gameObject.GetComponent<pathfinding2>().AddWaypoint(gameObject.GetComponent<connecteur>().BlockConnecte.transform);
                }
                else
                {
                    // On viens d'arriver sur ce bloc

                    Debug.Log("On arrive sur un bloc");
                    other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_nextPoint.transform);
                    _alreadyPass = true;
                }

            }
            else
            {
                Debug.Log("On est deja passé par la");
            }

        }

    }
}
