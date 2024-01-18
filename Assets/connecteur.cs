using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connecteur : MonoBehaviour
{
    [SerializeField]
    private bool _connected;

    public bool Connected { get => _connected; set => _connected = value; }

    // Start is called before the first frame update
    void Start()
    {
        _connected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if (other.gameObject.CompareTag("Connecteur")){
            _connected = true;
        }
    }


}
