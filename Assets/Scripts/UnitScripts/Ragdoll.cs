using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private GameObject ragdollPrefab;
    [SerializeField] private SkinnedMeshRenderer unitSkinnedMeshRenderer;
    [SerializeField] private Transform originalRootBone;

    private HealthSystem healthSystem;

    private SkinnedMeshRenderer unitRagdollSkinnedMeshRenderer;
    private Material unitMaterial;
    private Material ragdollMaterial;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void Start()
    {
        unitMaterial = unitSkinnedMeshRenderer.material;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        RagdollPosition unitRagdollPosition = ragdoll.GetComponent<RagdollPosition>();
        unitRagdollPosition.Setup(originalRootBone);
        unitRagdollSkinnedMeshRenderer = ragdoll.GetComponent<UnitRagdoll>().GetSkinnedMeshRenderer();
        unitRagdollSkinnedMeshRenderer.material = unitMaterial;
    }
}
