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


    // Pas de private set car la methode de sérialization demande a acceder au variable et a les modifiers

    public Vector3 Position { get => _position;  set => _position = value; }

    public Quaternion Rotation { get => _rotation;  set => _rotation = value; }

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
