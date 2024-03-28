/** 
***********************************************************************
Auteur : Sam Freddi
Date : 26.03.2024
Description. BoutonGenerer.cs CLass qui me permets de changer de scene
version 1.0
***********************************************************************
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAlreadyPass : MonoBehaviour
{
   [SerializeField]
   private GameObject _pathfinder;

   [SerializeField]
   private bool _isBlockEnding;

    public GameObject Pathfinder { get => _pathfinder; set => _pathfinder = value; }
    public bool IsBlockEnding { get => _isBlockEnding; set => _isBlockEnding = value; }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "pathfinder"){
            if(Pathfinder == null){
                Pathfinder = other.gameObject;
            }

            if(IsBlockEnding){
               // other.gameObject.GetComponent<Pathfinding1>().BlockPathfinder();
            }
        }
    }
}
