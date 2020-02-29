using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor
{
    private List<ClassEnum> _canKill = new List<ClassEnum>();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();
        Unit thisUnit = (Unit)this.target;

        if (thisUnit._classBool == null || thisUnit._classBool.Length < ClassEnum.GetNames(typeof(ClassEnum)).Length)
        {
            thisUnit._classBool = new bool[ClassEnum.GetNames(typeof(ClassEnum)).Length];
        }

        if (GUILayout.Button("UpdateList"))
        {
            thisUnit._canKill = new List<ClassEnum>(_canKill);
        }

            _canKill.Clear();
        foreach (ClassEnum classEnum in ClassEnum.GetValues(typeof(ClassEnum)))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(classEnum.ToString());
            GUILayout.FlexibleSpace();
            thisUnit._classBool[(int)classEnum] = EditorGUILayout.Toggle(thisUnit._classBool[(int)classEnum]);
            if (thisUnit._classBool[(int)classEnum])
            {
                _canKill.Add(classEnum);
            }
            GUILayout.Space(100);
            EditorGUILayout.EndHorizontal();

        }

        serializedObject.ApplyModifiedProperties();
    }
}

