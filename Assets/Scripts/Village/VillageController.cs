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


    public void StartVillage(bool needCaptainChoosing)
    {
        Vector2[] vector2s = new Vector2[needCaptainChoosing ? 4 : 3];
        string[] strings = new string[needCaptainChoosing ? 4 : 3];
        Action[] actions = new Action[needCaptainChoosing ? 4 : 3];

        int index = 0;
        if(needCaptainChoosing)
        {
            vector2s[index] = new Vector2(-368,-126);
            strings[index] = "Captain Choose";
            actions[index] = OpenCaptainChoosePanel;

            index++;
        }

        vector2s[index] = new Vector2(848, 67);
        strings[index] = "SupplyShop";
        actions[index] = OpenSupplyShop;

        vector2s[index+1] = new Vector2(270, 139);
        strings[index+1] = "Bar";
        actions[index+1] = OpenBarPanel;

        vector2s[index+2] = new Vector2(541, -334);
        strings[index+2] = "Repair";
        actions[index+2] = OpenRepairPanel;


        gameObject.SetActive(true);
        panelUI.ShowVillageWithAccessPoints(vector2s, strings, actions);
    }

    public void OpenCaptainChoosePanel()
    {

    }

    public void OpenBarPanel()
    {

    }

    public void OpenSupplyShop()
    {

    }

    public void OpenRepairPanel()
    {

    }


    private void EndVillage()
    {
        if (endCallback != null)
        {
            endCallback();
        }
    }
}
