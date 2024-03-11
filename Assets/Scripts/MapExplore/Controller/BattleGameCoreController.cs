using System;
using System.Collections;
using System.Collections.Generic;
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
    private PlayerController playerController;
    [SerializeField]
    private GameObject oceanMapPrefab;
    private MapController mapController;
    [SerializeField]
    private GameObject villagePanelPrefab;
    private VillageController villageController;
    [SerializeField]
    private GameObject eventPanelPrefab;
    private SpecialEventController eventController;

    //Current Data
    private BattleStage curBattleStage;
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
    void Start()
    {
        InitPanels();
        LoadBattleStage(BattleStage.InitRun);
    }

    // Update is called once per frame
    void Update()
    {
        ShowPlayerStats();
    }

    #endregion

    private void InitPanels()
    {
        mapController = Instantiate(oceanMapPrefab, transform).GetComponent<MapController>();
        mapController.gameObject.SetActive(false);
        villageController = Instantiate(villagePanelPrefab, transform).GetComponent<VillageController>();
        villageController.gameObject.SetActive(false);
        villageController.Init(() => {
            LoadBattleStage(BattleStage.EndVillage);
        });
        eventController = Instantiate(eventPanelPrefab, transform).GetComponent<SpecialEventController>();
        eventController.Init(() => {
            LoadBattleStage(BattleStage.EndEvent);
        });
        eventController.gameObject.SetActive(false);
    }

    private void ShowPlayerStats()
    {
        BattlePlayerData playerData = playerController.GetBattlePlayerData();
        string str = "";
        str += "Food: " + playerData.CurFoodAmount;
        str += " | EG HP: " + playerData.CurEnergyHP;
        str += " | Ship HP: " + playerData.CurShipBodyHP;
    }

    private void LoadBattleStage(BattleStage nextStage)
    {
        if (curBattleStage == nextStage && nextStage != BattleStage.InitRun)
            return;

        curBattleStage = nextStage;
        Debug.Log($"LoadBattleStage: {curBattleStage}");

        switch (curBattleStage)
        {
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
                break;
            case BattleStage.EndBattle: 
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
        //Load Event Map
        mapController.SetupMapEvents();
        LoadBattleStage(BattleStage.StartRun);
    }



    private async Task StartRun()
    {
        mapController.gameObject.SetActive(true);
        mapController.GoToStartPoint();
    }
    private async Task EndRun()
    {

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

    private async Task StartEvent()
    {
        eventController.StartEvent(currentMapData.specialEventType);
    }

    private async Task EndEvent()
    {
        eventController.gameObject.SetActive(false);
    }
    private async Task StartVillage()
    {
        villageController.StartVillage();
    }
    private async Task EndVillage()
    {
        villageController.gameObject.SetActive(false);
    }


}
