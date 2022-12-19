using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform shootPoint;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnEndMoving += MoveAction_OnEndMoving;
        };
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShootActionStart += ShootAction_OnStartShooting;
        };
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", true);
    }

    private void MoveAction_OnEndMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", false);
    }

    private void ShootAction_OnStartShooting(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("shoot");

        Transform bulletProjectileTransform = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPoint.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
}
