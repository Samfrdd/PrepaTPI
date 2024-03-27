using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonGenerer : MonoBehaviour
{
    private const string SceneName = "generationMap"; // Le nom de la sc�ne vers laquelle vous souhaitez changer

    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
