using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static JsonManager;

public class PlayerController : MonoBehaviour
{
    private BasePlayerData basePlayerData;
    private BattlePlayerData battlePlayerData;

    private void Awake()
    {
        //init
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(CharacterDB characterDB, ItemDB itemDB)
    {
        basePlayerData = new BasePlayerData();
        battlePlayerData = new BattlePlayerData();

        //Temp: Make fake save
        battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 10));
        battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 1));
        battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 2));

        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 1));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 2));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 3));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 9));

    }


    public BasePlayerData GetBasePlayerData()
    {
        return basePlayerData;
    }

    public BattlePlayerData GetBattlePlayerData()
    {
        return battlePlayerData;
    }
}
