using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update

   [SerializeField]
    private Material _material;

    public Material Material { get => _material; set => _material = value; }


    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {
            other.gameObject.GetComponent<TrailRenderer>().material = Material;
            other.gameObject.GetComponent<Pathfinding1>().FindExit();
            other.gameObject.GetComponent<Pathfinding1>().StopMovement();
        }
    }
}
