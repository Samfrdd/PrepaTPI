using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class saveMap : MonoBehaviour
{

    [SerializeField]
    private GameObject objectToSave;
   

    public void SaveObject(string filename)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameObject));
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + filename + ".xml", FileMode.Create);
        serializer.Serialize(stream, objectToSave);
        stream.Close();
        Debug.Log("Object saved as: " + filename);
    }
}
