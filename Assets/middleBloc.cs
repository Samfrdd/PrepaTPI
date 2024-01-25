using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class middleBloc : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _lstExit;

    private bool _alreadyPass;

    public bool AlreadyPass { get => _alreadyPass;private set => _alreadyPass = value; }

    // Start is called before the first frame update
    void Start()
    {
        _alreadyPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {
            if (!_alreadyPass)
            {
                  
                if(other.gameObject.GetComponent<pathfinding2>().Waypoints[other.gameObject.GetComponent<pathfinding2>().Waypoints.Length - 1 ] == _lstExit[0] || other.gameObject.GetComponent<pathfinding2>().Waypoints[other.gameObject.GetComponent<pathfinding2>().Waypoints.Length - 1] == _lstExit[1])
                {
                    if(_lstExit.Count == 2)
                    {
                        Debug.Log("1 sortie");
                        if (other.gameObject.GetComponent<pathfinding2>().Waypoints[other.gameObject.GetComponent<pathfinding2>().Waypoints.Length - 1] == _lstExit[0])
                        {
                            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_lstExit[0]);
                        }
                        else
                        {
                            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_lstExit[1]);

                        }
                    }
                    else
                    {
                        // dupliquer les personnage et crée l'arborésence
                        Debug.Log("2 if");

                    }


                }
                else
                {
                    Debug.Log("1 if");

                    // dupliquer les personnage et crée l'arborésence


                }

            }

        }
    }
}
