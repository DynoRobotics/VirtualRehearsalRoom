using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPerformanceImprover : MonoBehaviour
{
    private Mirror _mirror;
    void Awake()
    {
        _mirror = GetComponent<Mirror>();
        _mirror.enabled = false;
    }

    private void OnBecameInvisible()
    {
        Debug.Log("Mirror became invisible");
        if (_mirror == null)
        {
            Debug.LogError("_mirror is null");
        }
        _mirror.enabled = false;
    }

    private void OnBecameVisible()
    {
        Debug.Log("Mirror became visible");
        _mirror.enabled = true;
    }
}
