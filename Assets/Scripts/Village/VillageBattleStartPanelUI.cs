using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillageBattleStartPanelUI : MonoBehaviour
{
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Button startBtn;
    [SerializeField]
    private Button[] captainBtns;

    private List<BattlePlayerCharacterData> captainDatas;
    private BattlePlayerCharacterData selectedCaptain;
    private Action<BattlePlayerCharacterData> _startBattleCallback;
    private Action _closeCallback;
    // Start is called before the first frame update
    void Awake()
    {
        closeBtn.onClick.AddListener(Close);
        startBtn.onClick.AddListener(StartBattle);
    }

    public void Init(List<BattlePlayerCharacterData> datas, Action<BattlePlayerCharacterData> startBattleCallback, Action closeCallback)
    {
        captainDatas = datas;
        _startBattleCallback = startBattleCallback;
        _closeCallback = closeCallback;

        for (int i = 0;i < captainBtns.Length;i++)
        {
            if (i < datas.Count)
            {
                int index = i;
                TMP_Text nameTxt = captainBtns[i].GetComponentInChildren<TMP_Text>();
                nameTxt.text = datas[i].Name;

                captainBtns[i].name = datas[i].ID.ToString();
                captainBtns[i].onClick.RemoveAllListeners();
                captainBtns[i].onClick.AddListener(() => { SelectCaptain(datas[index]); });

                captainBtns[i].gameObject.SetActive(true);
            }
            else
            {
                captainBtns[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenPanel(List<BattlePlayerCharacterData> optionDatas)
    {
        ShowCaptainOptions(optionDatas);
        gameObject.SetActive(true);
    }

    private void ShowCaptainOptions(List<BattlePlayerCharacterData> optionDatas)
    {
        for(int i = 0;i < captainBtns.Length;i++)
        {
            int index = i;
            Image[] imgs =  captainBtns[i].GetComponents<Image>();
          
            if (optionDatas.FindIndex(x => x.ID.ToString() == captainBtns[index].name) ==-1)//need to lock
            {
                foreach(Image img in imgs)
                {
                    img.color = Color.gray;
                }
                captainBtns[index].enabled = false;
            }
            else
            {
                foreach (Image img in imgs)
                {
                    img.color = Color.white;
                }

                captainBtns[index].enabled = true;

                if (selectedCaptain == null)
                    SelectCaptain(optionDatas[index]);
            }
        }
    }

    private void Close()
    {
        if(_closeCallback  != null)
            _closeCallback();

        selectedCaptain = null;
        gameObject.SetActive(false);
    }

    private void StartBattle()
    {
        if (_startBattleCallback != null)
            _startBattleCallback(selectedCaptain);

        Close();
    }

    private void SelectCaptain(BattlePlayerCharacterData data)
    {
        selectedCaptain = data;
    }
}
