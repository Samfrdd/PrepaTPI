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
    private Vector3 _position;
    private Quaternion _rotation;
    private string _type;


    // Propriétés avec setters privés et attributs XML pour la sérialisation
    [XmlElement("Position")]
    public Vector3 Position { get => _position;  set => _position = value; }
    [XmlElement("Rotation")]
    public Quaternion Rotation { get => _rotation;  set => _rotation = value; }
    [XmlElement("Type")]
    public string Type { get => _type;  set => _type = value; }


    public BlocData(Vector3 pos, Quaternion rot, string type)
    {
        Position = pos;
        Rotation = rot;
        Type = type;
    }

    public BlocData()
    {

    }


}
