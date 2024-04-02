using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

    public void Init(JsonManager.CharacterDB characterDB)
    {
        basePlayerData = new BasePlayerData();
        battlePlayerData = new BattlePlayerData();

        battlePlayerData.battlePlayerCharacterDatas = new List<BattlePlayerCharacterData>();

        //Temp: Make fake save
        battlePlayerData.battlePlayerCharacterDatas.Add(characterDB.characters.Find(x => x.ID == 10));
        battlePlayerData.battlePlayerCharacterDatas.Add(characterDB.characters.Find(x => x.ID == 1));
        battlePlayerData.battlePlayerCharacterDatas.Add(characterDB.characters.Find(x => x.ID == 2));
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
