using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{

    [SerializeField]
    private Transform _nextPoint;
    // Start is called before the first frame update


    // Update is called once per frame


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {

            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(gameObject.transform);


            other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_nextPoint);





        }
    }
}
