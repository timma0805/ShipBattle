using Newtonsoft.Json;
using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public CardsDB cardDB { get; private set; }
    public CharacterDB characterDB { get; private set; }
    public EnemyDB enemyDB { get; private set; }
    public MapDB mapDB { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public async Task Init()
    {
        await ParseCardJson();
        await ParsePlayerCharacterJson();
        await ParseEnemyJson();
        await ParseEnemySkillJson();
        await ParseMapJson();
    }

    private async Task ParseCardJson()
    {
        try
        {
            string json = Resources.Load("JSON/CardJson").ToString();
            cardDB = JsonConvert.DeserializeObject<CardsDB>("{\"cards\":" + json + "}");
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }
    private async Task ParsePlayerCharacterJson()
    {
        try
        {
            string json = Resources.Load("JSON/CharacterJson").ToString();
            characterDB = JsonConvert.DeserializeObject<CharacterDB>("{\"characters\":" + json + "}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task ParseEnemyJson()
    {
        try
        {
            string json = Resources.Load("JSON/EnemyJson").ToString();
            enemyDB = JsonConvert.DeserializeObject<EnemyDB>("{\"enemies\":" + json + "}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task ParseEnemySkillJson()
    {
        try
        {
            string json = Resources.Load("JSON/EnemySkillJson").ToString();
            enemyDB = JsonConvert.DeserializeObject<EnemyDB>("{\"skills\":" + json + "}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private async Task ParseMapJson()
    {
        try
        {
            string json = Resources.Load("JSON/MapJson").ToString();
            mapDB = JsonConvert.DeserializeObject<MapDB>("{\"maps\":" + json + "}");

            for(int i = 0; i < mapDB.maps.Count; i++)
            {
                mapDB.maps[i].SpecialEventTypeList = new List<SpecialEventType>();
                string[] setStrs = mapDB.maps[i].SpecialEventSetStr.Split(',');
                for (int j = 0; j < setStrs.Length; j++)
                {
                    int eventTypeInt = int.Parse(setStrs[j]);
                    mapDB.maps[i].SpecialEventTypeList.Add((SpecialEventType)eventTypeInt);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [Serializable]
    public class CardsDB
    {
        public List<CardData> cards;
    }

    [Serializable]
    public class CharacterDB
    {
        public List<BattlePlayerCharacterData> characters;
    }

    [Serializable]
    public class EnemyDB
    {
        public List<EnemyData> enemies;
    }

    [Serializable]
    public class EnemySkillDB
    {
        public List<EnemySkillData> skills;
    }

    [Serializable]
    public class MapDB
    {
        public List<EntireMapData> maps;
    }
}


