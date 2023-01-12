using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer unitRagdollSkinnedMeshRenderer;

    public SkinnedMeshRenderer GetSkinnedMeshRenderer()
    {
        return unitRagdollSkinnedMeshRenderer;
    }
}
