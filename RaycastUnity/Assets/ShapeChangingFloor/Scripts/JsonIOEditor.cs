using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
[CustomEditor(typeof(JsonIO))]
public class JsonIOEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();
    JsonIO jsonIO = (JsonIO)target;
 
    GUILayout.Label("JSON file:");
    GUILayout.BeginHorizontal();
    if(GUILayout.Button("Load"))
    {
        jsonIO.Load();      
    }
    if(GUILayout.Button("Save"))
    {

    }
    GUILayout.EndHorizontal();

    GUILayout.Label("Game Objects:");
    GUILayout.BeginHorizontal();
    if(GUILayout.Button("Create"))
    {
        jsonIO.Create();
    }
    if(GUILayout.Button("Erase"))
    {
        jsonIO.Erase();
    }
    GUILayout.EndHorizontal();
  }
}