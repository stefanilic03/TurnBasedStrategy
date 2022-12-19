using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler OnSelectedActionStarted;
    public event EventHandler<bool> OnBusyChanged;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        //If the mouse is on top of a UI element, return
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        HandleUnitSelection();

        HandleSelectedAction();
    }

    //private void HandleSelectedAction()
    //{
    //    if (Input.GetMouseButtonDown(1) && selectedUnit != null)
    //    {
    //        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

    //        switch (selectedAction)
    //        {
    //            case MoveAction moveAction:
    //                if (moveAction.IsValidActionGridPosition(mouseGridPosition))
    //                {
    //                    SetBusy();
    //                    moveAction.Move(mouseGridPosition, ClearBusy);
    //                }
    //                break;
    //            case SpinAction spinAction:
    //                SetBusy();
    //                spinAction.Spin(ClearBusy);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnit != null)
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (selectedUnit.TrySpendActionPoints(selectedAction))
            {
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                OnSelectedActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void HandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycasthit, float.MaxValue, unitLayerMask))
            {
                if (raycasthit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        //Unit is already selected
                        return;
                    }
                    if (unit.IsEnemy())
                    {
                        return;
                    }
                    SetSelectedUnit(unit);
                }
            }
        }
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(selectedUnit.GetMoveAction());
        //The next line does the same thing as the commented code below
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        //if (OnSelectedUnitChanged != null)
        //{
        //    OnSelectedUnitChanged(this, EventArgs.Empty);
        //}
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
