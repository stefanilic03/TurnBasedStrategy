using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro gridCoordinatesText;

    private object gridObject;

    protected virtual void Update()
    {
        gridCoordinatesText.text = gridObject.ToString();       
    }

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

}
