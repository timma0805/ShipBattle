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
    public EnemySkillDB enemySkillDB { get; private set; }
    public MapDB mapDB { get; private set; }

    public ItemDB itemDB { get; private set; }

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
        await ParseItemJson();
        await ParsePlayerCharacterJson();
        await ParseEnemySkillJson();
        await ParseEnemyJson();
        await ParseMapJson();
    }

    private async Task ParseItemJson()
    {
        try
        {
            string json = Resources.Load("JSON/ItemJson").ToString();
            itemDB = JsonConvert.DeserializeObject<ItemDB>("{\"items\":" + json + "}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
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

            for (int i = 0; i < characterDB.characters.Count; i++)
            {
                characterDB.characters[i].CardDataList = new List<CardData>();
                string[] setStrs = characterDB.characters[i].CardSetStr.Split(',');
                for (int j = 0; j < setStrs.Length; j++)
                {
                    int cardID = int.Parse(setStrs[j]);
                    CardData cardData = cardDB.cards.Find(x => x.ID == cardID);
                    if (cardData != null)
                        characterDB.characters[i].CardDataList.Add(cardData);
                }
            }
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
            enemySkillDB = JsonConvert.DeserializeObject<EnemySkillDB>("{\"skills\":" + json + "}");
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

            for (int i = 0; i < enemyDB.enemies.Count; i++)
            {
                enemyDB.enemies[i].SkillList = new List<EnemySkillData>();
                string[] setStrs = enemyDB.enemies[i].SkillSetStr.Split(',');
                for (int j = 0; j < setStrs.Length; j++)
                {
                    int skillID = int.Parse(setStrs[j]);
                    EnemySkillData skillData = enemySkillDB.skills.Find(x => x.ID == skillID);
                    if (skillData != null)
                        enemyDB.enemies[i].SkillList.Add(skillData);
                }
            }
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

                mapDB.maps[i].EnemyDataList = new List<EnemyData>();
                string[] enemySetStrs = mapDB.maps[i].EnemySetStr.Split(',');
                for (int j = 0; j < enemySetStrs.Length; j++)
                {
                    int enemyID = int.Parse(enemySetStrs[j]);
                    EnemyData enemyData = enemyDB.enemies.Find(x => x.ID == enemyID);
                    if(enemyData != null)
                        mapDB.maps[i].EnemyDataList.Add(enemyData);
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

    [Serializable]
    public class ItemDB
    {
        public List<ItemData> items;
    }
}


