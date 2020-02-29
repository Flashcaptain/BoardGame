using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(Frame))]
public class FrameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();
        Frame thisFrame = (Frame)this.target;

        if (thisFrame._unit != null)
        {
            thisFrame._text[0].text = thisFrame._unit.gameObject.name;
            thisFrame._text[1].text = thisFrame._amount.ToString();
        }
        else
        {
            thisFrame._text[0].text = "UnitName";
        }

        serializedObject.ApplyModifiedProperties();
    }
}

