using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static JsonManager;

public class PlayerController : MonoBehaviour
{
    private BasePlayerData basePlayerData;
    private BattlePlayerData battlePlayerData;

    private List<BattlePlayerCharacterData> unlockCharacterDataList;
    private List<ItemData> unlockItemDataList;
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
        unlockCharacterDataList = new List<BattlePlayerCharacterData>();
        unlockItemDataList = new List<ItemData>();

        //Temp: Make fake save
        basePlayerData.UnlockMemberIndex.Add(1);
        basePlayerData.UnlockMemberIndex.Add(2);
        basePlayerData.UnlockMemberIndex.Add(3);
        basePlayerData.UnlockMemberIndex.Add(4);
        basePlayerData.UnlockMemberIndex.Add(5);
        basePlayerData.UnlockMemberIndex.Add(10);

        basePlayerData.UnlockItemIndex.Add(1);
        basePlayerData.UnlockItemIndex.Add(2);
        basePlayerData.UnlockItemIndex.Add(3);
        basePlayerData.UnlockItemIndex.Add(4);
        basePlayerData.UnlockItemIndex.Add(5);
        basePlayerData.UnlockItemIndex.Add(9);

        //Update unlock list
        for (int i = 0; i < basePlayerData.UnlockMemberIndex.Count; i++)
        {
            unlockCharacterDataList.Add(characterDB.characters.Find(x => x.ID == basePlayerData.UnlockMemberIndex[i]));
        }
        for (int i = 0; i < basePlayerData.UnlockItemIndex.Count; i++)
        {
            unlockItemDataList.Add(itemDB.items.Find(x => x.ID == basePlayerData.UnlockItemIndex[i]));
        }

        //Fake battle data
        battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 10));
        //battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 1));
        //battlePlayerData.battlePlayerCharacterList.Add(characterDB.characters.Find(x => x.ID == 2));

        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 1));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 2));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 3));
        battlePlayerData.itemList.Add(itemDB.items.Find(x => x.ID == 9));

    }

    public List<BattlePlayerCharacterData> GetUnlockCharacterList()
    {
        return unlockCharacterDataList;
    }

    public List<ItemData> GetUnlockItemList()
    {
        return unlockItemDataList;
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
