using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro gridCoordinatesText;
    private GridObject gridObject;

    private void Update()
    {
        gridCoordinatesText.text = gridObject.ToString();       
    }

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

}
