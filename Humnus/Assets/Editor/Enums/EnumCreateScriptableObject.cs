using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnumCreateScriptableObject : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string _savePath = "";
    public string savePath
    {
        get { return _savePath; }
        set { _savePath = value; }
    }
}
