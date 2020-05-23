using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            ((TileMap)target).GenerateMap(false);
        }
    }
}
