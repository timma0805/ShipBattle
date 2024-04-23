using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : MonoBehaviour
{
    [SerializeField]
    private VillagePanelUI panelUI;
    [SerializeField]
    private VillageBattleStartPanelUI villageBattleStartPanelUI;
    private PlayerController _playerController;
    private Action endCallback;
    private Camera _uiCamera;

    private Action<int, BattlePlayerCharacterData> _startBattleCallback;
    private int selectedMapIndex = 0;


    public void Init(PlayerController playerController, List<BattlePlayerCharacterData> characterDatas, Action _endCallback, Camera uiCamera)
    {
         _playerController = playerController;
        _uiCamera = uiCamera;
        endCallback = _endCallback;

        List<BattlePlayerCharacterData> captainList = characterDatas.FindAll(x => x.Type == CharacterType.Captain);
        panelUI.Init(EndVillage, uiCamera);
        villageBattleStartPanelUI.Init(captainList, StartBattle, panelUI.CloseSubPanel);
    }

    public void StartInitVillage(Action<int, BattlePlayerCharacterData> startBattleCallback)
    {
        _startBattleCallback = startBattleCallback;
        StartVillage(true);
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
        panelUI.ShowVillageWithAccessPoints(vector2s, strings, actions, !needCaptainChoosing);
    }

    public void OpenCaptainChoosePanel()
    {
        villageBattleStartPanelUI.OpenPanel( _playerController.GetUnlockCharacterList());
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

    public async void StartBattle(BattlePlayerCharacterData data)
    {
        Debug.Log("StartBattle:" + data.ID);

        if ( _startBattleCallback == null)
            return;

        await panelUI.CloseMapAnimation();
        _startBattleCallback(selectedMapIndex, data);

        EndVillage();
    }


    private void EndVillage()
    {
        if (endCallback != null)
        {
            endCallback();
        }
    }
}
