using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyRailManager : MonoBehaviour
{
    public GameObject SafetyRailGameObject;
    public bool SafetyRailIsEnabled = true;

    void Start()
    {
        SafetyRailGameObject.SetActive(SafetyRailIsEnabled);
    }

    void Update()
    {
        
    }

    public void ToggleSafetyRail()
    {
        SafetyRailIsEnabled = !SafetyRailIsEnabled;
        SafetyRailGameObject.SetActive(SafetyRailIsEnabled);
    }
}
