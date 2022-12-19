using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    public event EventHandler<OnShootEventArgs> OnShootActionStart;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }


    [SerializeField] private float aimingStateTime = 1f;
    [SerializeField] private float shootingStateTime = 0.1f;
    [SerializeField] private float coolOffStateTime = 0.5f;
    [SerializeField] private float turnSpeed = 200f;

    private Unit targetUnit;
    private bool canShootBullet;

    private enum State
    {
        Aiming, 
        Shooting,
        CoolOff
    }
    private State state;

    private int maxShootDistance = 3;
    private float stateTimer;

    public override string GetActionName()
    {
        return "Shoot";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = targetUnit.GetWorldPosition() - unit.GetWorldPosition().normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, turnSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.CoolOff:
                break;
            default:
                break;
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.CoolOff;
                stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                ActionComplete();
                break;
            default:
                break;
        }
    }

    private void Shoot()
    {
        OnShootActionStart?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });
        targetUnit.Damage();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        stateTimer = aimingStateTime;

        canShootBullet = true;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                int testDistance = Mathf.Abs(x + z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                //if (unitGridPosition == testGridPosition)
                //{
                //    continue;
                //}
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

}
