using System;
using System.Collections.Generic;

[Serializable]
public class EntireMapData 
{
    public int ID;
    public string Name;
    public string Description;
    public int SmallMapDepth;
    public int SmallMapCount;
    public int MapWidth;
    public float ConnectionBtwLinePer;
    public float ConnectionWithTwoLine;
    public string EnemySetStr;
    public string SpecialEventSetStr;
    public int VillageCount;
    public int SpecialEventCount;

    public List<SpecialEventType> SpecialEventTypeList;

    public EntireMapData() { }
}
