using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    public void LoadScene(string name, string nameMap)
    {

        PlayerPrefs.SetString("nameMap", nameMap); // Stockez le paramètre dans PlayerPrefs
        SceneManager.LoadScene(name);
    }

    public void MoveScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ExitApp()
    {

    }

}
