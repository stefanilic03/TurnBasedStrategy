using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    //public delegate void SpinCompleteDelegate();

    [SerializeField] private float spinningSpeed = 300f;

    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        transform.eulerAngles += new Vector3(0, spinningSpeed * Time.deltaTime, 0);

        totalSpinAmount += spinningSpeed * Time.deltaTime;
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    //public void Spin(Action onSpinComplete)
    //{
    //    this.onActionComplete = onSpinComplete;
    //    isActive = true;
    //    totalSpinAmount = 0f;
    //}

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //isActive = true;
        totalSpinAmount = 0f;

        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 0
        };
    }
}
