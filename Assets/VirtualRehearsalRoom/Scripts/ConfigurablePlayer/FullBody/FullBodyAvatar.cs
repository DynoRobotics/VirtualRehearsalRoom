using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBodyAvatar : MonoBehaviour
{
    public SkinnedMeshRenderer[] m_HeadMeshRenderers;
    public SkinnedMeshRenderer[] m_BodyMeshRenderers;

    public void DisableMeshRenderers()
    {
        foreach (SkinnedMeshRenderer meshRenderer in m_HeadMeshRenderers)
        {
            meshRenderer.enabled = false;
        }

        foreach (SkinnedMeshRenderer meshRenderer in m_BodyMeshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
}
