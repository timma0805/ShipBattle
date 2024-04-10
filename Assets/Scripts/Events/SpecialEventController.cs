using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Presets;
using UnityEngine;

public class SpecialEventController : MonoBehaviour
{
    private SpecialEventPanelUI panelUI;
    private SpecialEventType currentEventType;
    private Action endEventCallback;

    private void Awake()
    {
        panelUI = GetComponent<SpecialEventPanelUI>();
    }
    
    public void Init(Action _endEventCallback)
    {
        endEventCallback = _endEventCallback;
    }

    public void StartEvent(SpecialEventType eventType)
    {
        currentEventType = eventType;

        switch (currentEventType)
        {  
            case SpecialEventType.Priest:
            case SpecialEventType.Blacksmith:
            case SpecialEventType.Tailor:
            case SpecialEventType.Scholar:
            case SpecialEventType.Chronicler:
            case SpecialEventType.Musician:
                ProcessNPCEvent(eventType);
                break;
            case SpecialEventType.Merchant:
                ProcessShopEvent(eventType);
            break;
            default:
                ProcessOptionEvent(eventType);
                break;
        }

        gameObject.SetActive(true);
    }

    private async Task ProcessNPCEvent(SpecialEventType eventType)
    {
        switch (eventType)
        {

        }

        panelUI.ShowEventPanel(EndEvent);

    }

    private async Task ProcessShopEvent(SpecialEventType eventType)
    {
        switch (eventType)
        {

        }

        panelUI.ShowEventPanel(EndEvent);

    }

    private async Task ProcessOptionEvent(SpecialEventType eventType)
    {
        switch (eventType)
        {

        }

        panelUI.ShowEventPanel(EndEvent);

    }

    private void EndEvent()
    {
        if(endEventCallback != null)
        {
            endEventCallback();
        }

        gameObject.SetActive(false);
    }
}
