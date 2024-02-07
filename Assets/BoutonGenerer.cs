using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonGenerer : MonoBehaviour
{
    private const string sceneName = "generationMap"; // Le nom de la scène vers laquelle vous souhaitez changer

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
