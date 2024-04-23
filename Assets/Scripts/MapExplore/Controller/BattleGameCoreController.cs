using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BattleGameCoreController : MonoBehaviour
{
    // Static instance variable for the Singleton pattern
    private static BattleGameCoreController instance;

    // Public property to access the instance
    public static BattleGameCoreController Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private JsonManager jsonManager;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject playerPanelUIPrefab;
    private PlayerPanelUI playerPanelUI;
    [SerializeField]
    private GameObject oceanMapPrefab;
    private MapController mapController;
    [SerializeField]
    private GameObject villagePanelPrefab;
    private VillageController villageController;
    [SerializeField]
    private GameObject eventPanelPrefab;
    private SpecialEventController eventController;
    [SerializeField]
    private GameObject miniBattlePrefab;
    private MiniBattleCoreController miniBattleController;

    [SerializeField]
    private Camera uiCamera;

    //Current Data
    private BattleStage curBattleStage;
    private EntireMapData currentEntireMapData;
    private BattlePlayerCharacterData currentCaptainData;
    private MapData currentMapData;

    #region Unity Activities
    private void Awake()
    {
        // If an instance already exists and it's not this instance, destroy this instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance to this instance
        instance = this;

        // Ensure that this GameObject persists between scenes
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    async void Start()
    {
        await InitPanels();
        LoadBattleStage(0);
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlayerStats();
    }

    #endregion

    private async Task InitPanels()
    {
        //Read Data from Json
        await jsonManager.Init();

        //Load Player Data
        playerController.Init(jsonManager.characterDB, jsonManager.itemDB);
        playerPanelUI = Instantiate(playerPanelUIPrefab).GetComponent<PlayerPanelUI>();
        playerPanelUI.Init(playerController);

        mapController = Instantiate(oceanMapPrefab).GetComponent<MapController>();
        mapController.GetComponent<Canvas>().worldCamera = uiCamera;
        mapController.gameObject.SetActive(false);
        mapController.Init(jsonManager.mapDB);

        villageController = Instantiate(villagePanelPrefab).GetComponent<VillageController>();
        villageController.GetComponent<Canvas>().worldCamera = uiCamera;
        villageController.gameObject.SetActive(false);
        villageController.Init(
            playerController, 
            jsonManager.characterDB.characters,
            () => { LoadBattleStage(BattleStage.EndVillage);}
            , uiCamera);

        eventController = Instantiate(eventPanelPrefab).GetComponent<SpecialEventController>();
        eventController.GetComponent<Canvas>().worldCamera = uiCamera;
        eventController.Init(() => {
            LoadBattleStage(BattleStage.EndEvent);
        });
        eventController.gameObject.SetActive(false);

        miniBattleController = Instantiate(miniBattlePrefab).GetComponent<MiniBattleCoreController>();
        miniBattleController.GetComponent<Canvas>().worldCamera = uiCamera;
        miniBattleController.Init(() => {
            LoadBattleStage(BattleStage.EndBattle);
        });
        miniBattleController.gameObject.SetActive(false);
    }

    public Camera GetUICamera()
    {
        return uiCamera;
    }

    private void ShowPlayerStats()
    {
        //BattlePlayerData playerData = playerController.GetBattlePlayerData();
        //string str = "";
        //str += "Food: " + playerData.CurFoodAmount;
        //str += " | EG HP: " + playerData.CurEnergyHP;
        //str += " | Ship HP: " + playerData.CurShipBodyHP;
    }

    private void LoadBattleStage(BattleStage nextStage)
    {
        if (curBattleStage == nextStage && nextStage != 0)
            return;

        curBattleStage = nextStage;
        Debug.Log($"LoadBattleStage: {curBattleStage}");

        switch (curBattleStage)
        {
            case BattleStage.InitVillage:
                StartInitVillage();
                break;
            case BattleStage.InitRun:
                InitRun();
                break;
            case BattleStage.StartRun:
                StartRun();
                break;
            case BattleStage.StartEvent:
                StartEvent();
                break;
            case BattleStage.EndEvent:
                EndEvent();
                break;
            case BattleStage.StartBattle:
                StartEnemyBattle();
                break;
            case BattleStage.EndBattle: 
                EndEnemyBattle();
                break;
            case BattleStage.StartVillage:
                StartVillage();
                break;
            case BattleStage.EndVillage:
                EndVillage();
                break;
            case BattleStage.EndRun:
                EndRun();
                break;

            default:
                break;
        }
    }

    private void InitRun()
    {
        LoadBattleStage(BattleStage.StartRun);
    }

    private async Task StartRun()
    {
        mapController.gameObject.SetActive(true);
        mapController.GoToStartPoint();
    }
    private async Task EndRun()
    {
        Debug.Log("EndRun");
    }

    public void ProcessMapData(MapData mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("mapData null");
            return;
        }

        currentMapData = mapData;

        switch (currentMapData.eventType)
        {
            case MapEventType.Enemy:
                LoadBattleStage(BattleStage.StartBattle);
                break;
            case MapEventType.Village:
                LoadBattleStage(BattleStage.StartVillage);
                break;
            case MapEventType.SpecialEvent:
                LoadBattleStage(BattleStage.StartEvent);
                break;
            case MapEventType.MiniBoss:
                LoadBattleStage(BattleStage.StartBattle);
                break;
            case MapEventType.FinalBoss:
                LoadBattleStage(BattleStage.StartBattle);
                break;
        }
    }

    private async Task StartInitVillage()
    {
        villageController.StartInitVillage(EndInitVillage);
    }

    private void EndInitVillage(int mapIndex, BattlePlayerCharacterData captainData)
    {
        currentEntireMapData = mapController.SetupMapEvents(mapIndex);
        currentCaptainData = captainData;
        LoadBattleStage(BattleStage.InitRun);
    }

    private async Task StartEvent()
    {
        eventController.StartEvent(currentMapData.specialEventType);
    }

    private async Task EndEvent()
    {
        eventController.gameObject.SetActive(false);
        LoadBattleStage(BattleStage.Choosing);
    }
    private async Task StartVillage()
    {
        villageController.StartVillage(false);
    }
    private async Task EndVillage()
    {
        villageController.gameObject.SetActive(false);
        LoadBattleStage(BattleStage.Choosing);
    }

    private async Task StartEnemyBattle()
    {
        miniBattleController.StartBattle(playerController.GetBattlePlayerData(), currentEntireMapData);
    }
    private async Task EndEnemyBattle()
    {
        miniBattleController.gameObject.SetActive(false);
        LoadBattleStage(BattleStage.Choosing);
    }


}
