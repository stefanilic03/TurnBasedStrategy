using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsBlockedUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChange;
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChange(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Show();
            return;
        }
        Hide();
    }
}
