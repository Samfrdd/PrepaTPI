using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainWindows;
    [SerializeField]
    private GameObject _mainContent;
    
    [SerializeField]
    private GameObject _loadWindows;
        [SerializeField]
    private GameObject loadContent;

    private void Start() {
        _mainWindows.SetActive(true);
        _loadWindows.SetActive(true);


        _mainContent.SetActive(true);
        loadContent.SetActive(false);
    }

    public void AfficherLoadList(){
        _mainContent.SetActive(false);
        loadContent.SetActive(true);
    }

        public void AfficherMain(){
        _mainContent.SetActive(true);
        loadContent.SetActive(false);
    }
}
