using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData 
{
    public int currentMapID;
    public List<MapData> nextMapDatas;
    public MapEventType eventType;
    public SpecialEventType specialEventType;
    public int mapDepth;
    public int line;

    public MapData() { 
        nextMapDatas = new List<MapData>();
        specialEventType = SpecialEventType.NA;
    }
}
