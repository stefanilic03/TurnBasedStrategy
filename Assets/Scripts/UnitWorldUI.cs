using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        unit.OnActionPointsChanged += Unit_OnActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPointsAmount().ToString();  
    }

    private void Unit_OnActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void OnDestroy()
    {
        unit.OnActionPointsChanged -= Unit_OnActionPointsChanged;  
    }

    private void OnDisable()
    {
        unit.OnActionPointsChanged -= Unit_OnActionPointsChanged;        
    }
}
