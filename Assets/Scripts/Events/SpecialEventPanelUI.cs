using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialEventPanelUI : MonoBehaviour
{
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Image npcImg;
    [SerializeField]
    private GameObject optionPanel;
    [SerializeField] 
    private Button[] optionBtns;
    [SerializeField] 
    private GameObject shopPanel;
    [SerializeField]
    private Button[] shopBtns;
    [SerializeField]
    private GameObject messagePanel;
    [SerializeField] 
    private TMP_Text messageText;
    [SerializeField]
    private Button closeBtn;

    //=============================
    private Action nextAction;
    private bool waitToClick = false;
    private Action closeCallback;
    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() => { LeaveEventPanel(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && waitToClick)
        {
            if(nextAction != null)
            {
                nextAction();
                nextAction = null;
            }

            waitToClick = false;
        }
    }

    public void ShowEventPanel(Action _closeCallback)
    {
        closeCallback = _closeCallback;
        gameObject.SetActive(true);
    }

    private void HideEventPanel()
    {
        gameObject.SetActive(false);
    }

    private async Task LeaveEventPanel()
    {
        await ShowMessage("Good bye");
        if(closeCallback  != null)
        {
            closeCallback();
        }
        HideEventPanel();
    }

    private async Task ShowMessage(string text)
    {
        messagePanel.SetActive(true);

        string displayText = "";
        int i = 0;
        waitToClick = true; //show entire message when input any key
        while(waitToClick && displayText.Length < text.Length)  //display word one by one
        {
            displayText += text[i];
            messageText.text = displayText;
            i++;
            await Task.Delay(100);
        }

        messageText.text = text;

        await Task.Delay(100);
        waitToClick = true;
        while (waitToClick)
        {
            await Task.Yield();
        }
        messagePanel.SetActive(false);
    }

    private async Task ShowOptions(string[] optionTexts, Action[] optionCallbacks)
    {
        //Disable all options before show panel
        for (int i = 0; i < optionBtns.Length; i++)
        {
            optionBtns[i].gameObject.SetActive(false);
        }
        optionPanel.SetActive(true);

        for (int i = 0; i < optionBtns.Length; i++)
        {
            if (i < optionTexts.Length)
            {
                await Task.Delay(100);  //Show option one by one
                optionBtns[i].GetComponent<TMP_Text>().text = optionTexts[i];
                optionBtns[i].onClick.RemoveAllListeners();
                optionBtns[i].onClick.AddListener(() => { optionCallbacks[i](); });
                optionBtns[i].gameObject.SetActive(true) ;
            }
        }
    }

    private async Task ShowShopItems(string[] itemStrs, Sprite[] itemSprite, Action[] itemCallbacks) //TODO: using shop item class
    {
        //Disable all options before show panel
        for (int i = 0; i < shopBtns.Length; i++)
        {
            shopBtns[i].gameObject.SetActive(false);
        }
        shopPanel.SetActive(true);

        for (int i = 0; i < itemStrs.Length; i++)
        {
            if (i < optionBtns.Length)
            {
                await Task.Delay(100);  //Show option one by one
                shopBtns[i].GetComponent<TMP_Text>().text = itemStrs[i];
                shopBtns[i].GetComponentInChildren<Image>().sprite = itemSprite[i];
                shopBtns[i].onClick.RemoveAllListeners();
                shopBtns[i].onClick.AddListener(() => { itemCallbacks[i](); });
                shopBtns[i].gameObject.SetActive(true);
            }
        }
    }
}
