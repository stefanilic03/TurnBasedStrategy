using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int maximumActionPoints = 2;
    [SerializeField] private bool isEnemy;

    //public static event EventHandler OnAnyActionPointsChanged;
    public event EventHandler OnActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    #region Variables
    private GridPosition currentGridPosition;
    private BaseAction[] baseActionArray;
    private HealthSystem healthSystem;
    private int actionPoints = 2;
    #endregion

    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(currentGridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != currentGridPosition)
        {
            GridPosition oldGridPosition = currentGridPosition;
            currentGridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return currentGridPosition;
    }

    public BaseAction[] GetBaseActions()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPoints(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true; 
        }
        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        return false;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = maximumActionPoints;
            OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }   
    }

    private void SpendActionPoints(int amount)
    {
        //OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        actionPoints -= amount;
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public int GetActionPointsAmount()
    {
        //OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);

        return actionPoints;
    }
    
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(currentGridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
