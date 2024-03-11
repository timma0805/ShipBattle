using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : MonoBehaviour
{
    private VillagePanelUI panelUI;
    private Action endCallback;
    private void Awake()
    {
        panelUI = GetComponent<VillagePanelUI>();
        panelUI.Init(EndVillage);
    }

    public void Init(Action _endCallback)
    {
        endCallback = _endCallback;
    }


    public void StartVillage()
    {
        gameObject.SetActive(true);
        panelUI.ShowVillageWithAccessPoints(new Vector2[0], new Action[0]);
    }
    
    private void EndVillage()
    {
        if (endCallback != null)
        {
            endCallback();
        }
    }
}
