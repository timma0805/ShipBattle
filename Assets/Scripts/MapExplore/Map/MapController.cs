using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class MapController : MonoBehaviour
{
    private MapExploreUI mapExploreUI;

    // Start is called before the first frame update
    [SerializeField]
    private Dictionary<int, List<MapData>> mapList;

    private List<EntireMapData> entireMapDataList;
    private EntireMapData currentEntireMapData;
    private MapData currentMapData;

    private void Awake()
    {
        mapExploreUI = GetComponent<MapExploreUI>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(JsonManager.MapDB mapList)
    {
        entireMapDataList = mapList.maps;
        mapExploreUI.Init(this);
    }

    public EntireMapData SetupMapEvents(int entireMapIndex)
    {
        currentEntireMapData = entireMapDataList[entireMapIndex];

        /** Rule to generate
         * 1. 3 lines need to generate
         * 2. Have one middle boss and one final boss shared with three lines
         * 3. Each line separates into four small map parts
         * 4. Each small part must have at least 1 Village, 2 special event and 2 enemy battle
         * 
         */
        int smallMapDepth = currentEntireMapData.SmallMapDepth;
        int mapDepth = 1+ (currentEntireMapData.SmallMapDepth +1)* currentEntireMapData.SmallMapCount; // Start point + small map with boss
        int mapWidth = currentEntireMapData.MapWidth;
        float doConnectionBtwLinePer = currentEntireMapData.ConnectionBtwLinePer;
        float doConnectionWithTwoLine = currentEntireMapData.ConnectionWithTwoLine;

        mapList = new Dictionary<int, List<MapData>>(); // Key: line index
        int mapID = 0;

        //generate line individually
        for (int l = 0; l < mapWidth; l++)
        {
            List<MapData> mapDataList = new List<MapData>();

            var (villiageCount, specialEventCount, enemyCounts) = GetMapSmallPartCounts(smallMapDepth);

            MapData previousMapData = null;
            List<SpecialEventType> specialEvents = GetRandomSpecialEventList();

            for (int depth = 0; depth < mapDepth; depth++)
            {
                MapData mapData = new MapData();
                mapData.currentMapID = mapID;
                mapData.mapDepth = depth;
                mapData.line = l;

                if (depth == 0) // start point
                {
                    //Create Start Point;
                    mapData.eventType = MapEventType.Start;
                }
                else if (depth == 1) // first point
                {
                    mapData.eventType = MapEventType.Enemy;
                    enemyCounts--;
                }
                else if (villiageCount == 0 && specialEventCount == 0 && enemyCounts == 0)  //finish small part
                {
                    (villiageCount, specialEventCount, enemyCounts) = GetMapSmallPartCounts(smallMapDepth);

                    if (depth < mapDepth-1)// middle point, MiniBoss
                    {
                        mapData.eventType = MapEventType.MiniBoss;
                    }
                    else//Final Boss
                    {
                        mapData.eventType = MapEventType.FinalBoss;
                    }
                }

                if (mapData.eventType == MapEventType.NA) // Normal Gen
                {
                    List<MapEventType> randomList = new List<MapEventType>();
                    if (villiageCount > 0 && previousMapData.eventType != MapEventType.Start && previousMapData.eventType != MapEventType.Village)
                        randomList.Add(MapEventType.Village);
                    if (specialEventCount > 0)
                        randomList.Add(MapEventType.SpecialEvent);
                    if (enemyCounts > 0)
                        randomList.Add(MapEventType.Enemy);

                    int randomCount = Random.Range(0, randomList.Count);
                    mapData.eventType = randomList[randomCount];

                    if (mapData.eventType == MapEventType.Village)
                        villiageCount--;
                    else if (mapData.eventType == MapEventType.SpecialEvent)
                        specialEventCount--;
                    else if (mapData.eventType == MapEventType.Enemy)
                        enemyCounts--;
                }

                if(l != 0)
                {
                    if(mapData.eventType == MapEventType.Start || mapData.eventType == MapEventType.MiniBoss || mapData.eventType == MapEventType.FinalBoss) //Only line 0 would generate Shared point
                        mapData = mapList[0].Find(x => x.eventType == mapData.eventType);
                }

                if(mapData.eventType != MapEventType.Start)  //connect mapping except Start Point
                {
                    //same line
                    previousMapData.nextMapDatas.Add(mapData);
                }

                previousMapData = mapData;

                Debug.Log($"Line{mapData.line} Depth{mapData.mapDepth} mapData {mapData.currentMapID} {mapData.eventType} {mapData.specialEventType}");

                if (mapData.line == l)
                {
                    mapDataList.Add(mapData);
                    mapID++;
                }
            }

            for (int i = 0; i < mapDataList.Count; i++)
            {
                var mapData = mapDataList[i];
            }

            mapList.Add(l, mapDataList);
        }

        //set connection with other line previous point
        for (int l = 0; l < mapWidth; l++)
        {
            for (int i = 1; i < mapList[l].Count - 1; i++)
            {
                var mapData = mapList[l][i];
                List<MapData> previousLineDataList = new List<MapData>();
                for (int line = 0; line < mapWidth; line++)
                {
                    if(line != l)
                    {
                        var previousMapData = mapList[line].Find(x => x.mapDepth == mapData.mapDepth - 1);
                        if(previousMapData != null)
                            previousLineDataList.Add(previousMapData);
                    }
                }

                if (mapData.eventType == MapEventType.MiniBoss || mapData.eventType == MapEventType.FinalBoss || previousLineDataList.Count == 0)
                    continue;

                float randomNum = Random.Range(0.0f, 100.0f) / 100.0f;
                if (randomNum > doConnectionWithTwoLine)
                {
                    for (int index = 0; index < previousLineDataList.Count; index++)
                    {
                        previousLineDataList[index].nextMapDatas.Add(mapData);
                    }                
                }
                else if (randomNum > doConnectionBtwLinePer)
                {
                    int index = Random.Range(0, previousLineDataList.Count);
                    previousLineDataList[index].nextMapDatas.Add(mapData);
                }
            }
        }

        return currentEntireMapData;
    }

    private (int, int , int) GetMapSmallPartCounts(int smallMapDepth)
    {
        //random small part
        int villageCount = currentEntireMapData.VillageCount;
        int specialEventCount = currentEntireMapData.SpecialEventCount;
        int enemyCount = smallMapDepth - villageCount - specialEventCount;

        return (villageCount, specialEventCount, enemyCount);
    }

    public void GoToStartPoint()
    {
        GoToNextLine(0);
    }

    public async Task GoToNextLine(int lineIndex)
    {
        MapData nextMapData = new MapData();
        if (currentMapData == null)
            nextMapData = mapList[0][0];
        else
        {
            nextMapData = currentMapData.nextMapDatas.Find(x => x.line == lineIndex) ;       
        }

        //TODO: process Can Move to next map or not

        //Confirmed Can Move
        currentMapData = nextMapData;

        //Do animation to next map point
        await mapExploreUI.MoveShipToTargetPoint(currentMapData);

        //process MapPoint Event
        BattleGameCoreController.Instance.ProcessMapData(currentMapData);
    }

    private List<SpecialEventType> GetRandomSpecialEventList()
    {
        List<SpecialEventType> shuffledEventList = currentEntireMapData.SpecialEventTypeList.OrderBy(_ => System.Guid.NewGuid()).ToList();
        return shuffledEventList;
    }

    public Dictionary<int, List<MapData>> GetMapList()
    {
        return mapList;
    }

    public MapData GetCurrentMapData()
    {
        return currentMapData;
    }
}
