using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensUnique : MonoBehaviour
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

            other.gameObject.GetComponent<pathfinding2>().BlockPathfinder();

            other.gameObject.GetComponent<TrailRenderer>().material = _material;
            




        }
    }
}
