using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class pathfinding1 : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<generation>().MapCree)
        {
            print("Debut de la recherche de pathfinding");
            List<GameObject> listEntre = gameObject.GetComponent<generation>().LstEntre;

            
        }   
    }
}
