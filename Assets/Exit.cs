using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update

   [SerializeField]
    private Material _material;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {

           

            other.gameObject.GetComponent<TrailRenderer>().material = _material;

            other.gameObject.GetComponent<Pathfinding1>().FindExit();
            other.gameObject.GetComponent<Pathfinding1>().StopMovement();




        }
    }
}
