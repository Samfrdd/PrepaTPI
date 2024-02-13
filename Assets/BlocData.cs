using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class BlocData 
{
    public Vector3 position;
    public Quaternion rotation;
    public string type;
}
