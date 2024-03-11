using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapExploreUI : MonoBehaviour
{
    [SerializeField]
    private Sprite enemyAlertSprite;
    [SerializeField]
    private Sprite eventAlertSprite;
    [SerializeField]
    private Sprite villageAlertSprite;
    [SerializeField]
    private Sprite startAlertSprite;
    [SerializeField]
    private Sprite miniBossAlertSprite;
    [SerializeField]
    private Sprite finalBossAlertSprite;

    [SerializeField]
    private Image[] alertImages;
    [SerializeField]
    private GameObject[] lines;
    [SerializeField]
    private GameObject ship;

    [SerializeField]
    private GameObject fullMapPopup;
    [SerializeField]
    private Button fullMapBtn;
    [SerializeField]
    private GameObject mapPointObj;
    [SerializeField]
    private RectTransform fullMapContent;
    [SerializeField]
    private ScrollRect fullMapScrollRect;
    [SerializeField]
    private Sprite fullMapLineSprite;
    [SerializeField]
    private Button fullMapCloseBtn;
    [SerializeField]
    private Image fullMapShipImg;

    private MapController mapController;
    private Dictionary<MapData, GameObject> mapPoints = new Dictionary<MapData, GameObject>();


    const int maxline = 3;
    const float distance = 200.0f;
    const float leftDistance = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < alertImages.Length; i++) {
            int index = i;
            alertImages[i].gameObject.SetActive(true);
            alertImages[i].GetComponent<Button>().onClick.AddListener(() => SelectNextMapPoint(index));
        }

        fullMapBtn.onClick.AddListener(DisplayFullMap);
        fullMapCloseBtn.onClick.AddListener(() => {
            fullMapPopup.SetActive(false);
        });

        //Default Closed
        fullMapPopup.SetActive(false);
    }

    public void Init(MapController _mapController )
    {
        mapController = _mapController;
    }

    public void ShowAlerts(List<MapEventType> alerts)
    {
        Debug.Log("ShowAlerts: " + alerts.Count);
        for(int i = 0;i < alerts.Count;i++)
        {
            DisplayMapEventImage(alertImages[i], alerts[i]);
        }
    }
    public void HideAllAlerts()
    {
        foreach (Image image in alertImages)
        {
            image.gameObject.SetActive(false);
        }
    }
    private void DisplayMapEventImage(Image image, MapEventType alert)
    {
        switch(alert)
        {
            case MapEventType.Village:
                image.sprite = villageAlertSprite;
                break;
            case MapEventType.Enemy:
                image.sprite = enemyAlertSprite;
                break;
            case MapEventType.SpecialEvent:
                image.sprite = eventAlertSprite;
                break;
            case MapEventType.Start:
                image.sprite = startAlertSprite;
                break;
            case MapEventType.MiniBoss:
                image.sprite = miniBossAlertSprite;
                break;
            case MapEventType.FinalBoss:
                image.sprite = finalBossAlertSprite;
                break;
            default: break;
        }

        image.gameObject.SetActive(alert != MapEventType.NA);
    }

    public async Task MoveShipToTargetPoint(MapData mapData)
    {
        Debug.Log("MoveShipToTargetPoint: " + mapData.currentMapID);

        ship.transform.SetParent(lines[mapData.line].transform);
        ship.transform.localPosition = new Vector3(-780,0,0) ;

        List<MapEventType> eventlist = new List<MapEventType>();
        for (int i = 0; i < maxline; i++)
        {
            var nextMapData = mapData.nextMapDatas.Find(x => x.line == i);
            if (nextMapData != null)
                eventlist.Add(nextMapData.eventType);
            else
                eventlist.Add(MapEventType.NA);
        }

        ShowAlerts(eventlist);

        //Full Map
        GoToFullMapTargetPoint(mapData);
    }

    #region Full Map Pop-up

    private void DisplayFullMap()
    {
        Debug.Log("DisplayFullMap");

        if (mapPoints.Count > 0 ) //Generated the map
        {
            fullMapPopup.SetActive(true);
            return;
        }

        //Gen Map
        var mapList = mapController.GetMapList();
       //Create points
        for (int i = 0; i < mapList.Count; i++)
        {
            var lineMap = mapList[i];
            for (int j = 0; j < lineMap.Count; j++)
            {
                var mapData = lineMap[j];
                if (mapData != null)
                {
                    GameObject newMapPoint = Instantiate(mapPointObj, fullMapContent.transform);
                    newMapPoint.name = "MapPoint" + mapData.currentMapID;
                    newMapPoint.gameObject.SetActive(true);

                    if (mapData.line == 0) //always middle
                        newMapPoint.transform.localPosition = new Vector2(leftDistance + mapData.mapDepth * distance, 0);
                    else if(mapData.line%2 == 0)
                        newMapPoint.transform.localPosition = new Vector2(leftDistance + mapData.mapDepth * distance,  Mathf.Ceil(mapData.line/2f) * distance );
                    else
                        newMapPoint.transform.localPosition = new Vector2(leftDistance + mapData.mapDepth * distance, -Mathf.Ceil(mapData.line / 2f) * distance);

                    Debug.Log(newMapPoint.transform.localPosition);

                    DisplayMapEventImage(newMapPoint.GetComponent<Image>(), mapData.eventType);

                    mapPoints.Add(mapData, newMapPoint);

                    if(mapData.eventType == MapEventType.FinalBoss) //Farest point
                    {
                        fullMapContent.sizeDelta = new Vector2(leftDistance + newMapPoint.transform.localPosition.x, fullMapContent.sizeDelta.y );
                    }
                }
            }
        }

        List < MapData > mapDataList = mapPoints.Keys.ToList();
        foreach (var mapData in mapPoints.Keys)
        {
            var startPos = mapPoints[mapData].transform.localPosition;
            for (int i = 0; i < mapData.nextMapDatas.Count; i++)
            {
                var nextMapData = mapData.nextMapDatas[i];
                var nextPos = mapPoints[nextMapData].transform.localPosition;
                MakeLine(startPos.x, startPos.y, nextPos.x, nextPos.y, Color.black);
            }
        }

        GoToFullMapTargetPoint(mapController.GetCurrentMapData());
        fullMapPopup.SetActive(true);
    }

    private void MakeLine(float ax, float ay, float bx, float by, Color col)
    {
        const float lineWidth = 5.0f;

        GameObject NewObj = new GameObject();
        NewObj.name = "line from " + ax + " to " + bx;
        Image NewImage = NewObj.AddComponent<Image>();
        NewImage.sprite = fullMapLineSprite;
        NewImage.color = col;
        RectTransform rect = NewObj.GetComponent<RectTransform>();
        rect.SetParent(fullMapContent);
        rect.localScale = Vector3.one;

        Vector3 a = new Vector3(ax , ay , 0);
        Vector3 b = new Vector3(bx , by , 0);


        rect.localPosition = (a + b) / 2;
        Vector3 dif = a - b;
        rect.sizeDelta = new Vector3(dif.magnitude, lineWidth);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
    }

    private void GoToFullMapTargetPoint(MapData mapData)
    {
        //Full Map Display change
        if(mapPoints.ContainsKey(mapData))
            fullMapShipImg.transform.position = mapPoints[mapData].transform.position;

        fullMapShipImg.transform.SetAsLastSibling();

        fullMapScrollRect.horizontalNormalizedPosition = fullMapShipImg.transform.localPosition.x / fullMapContent.sizeDelta.x;
    }

    #endregion

    private void SelectNextMapPoint(int lineIndex)
    {
        Debug.Log("SelectNextMapPoint: " + lineIndex);
        mapController.GoToNextLine(lineIndex);

    }
}
