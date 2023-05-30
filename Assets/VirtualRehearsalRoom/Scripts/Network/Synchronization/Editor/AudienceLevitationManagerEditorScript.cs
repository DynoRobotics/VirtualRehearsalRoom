using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudienceLevitationManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        AudienceLevitationManager avatarLevitationManager = (AudienceLevitationManager)target;
    }
}
