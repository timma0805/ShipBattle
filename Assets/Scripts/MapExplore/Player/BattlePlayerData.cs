using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class is player data for Each run
 */
public class BattlePlayerData 
{
    public int money;
    public int score;
    public int foodSupply;

    public List<BattlePlayerCharacterData> battlePlayerCharacterDatas;

    public BattlePlayerData ()
    {
        money = 0;
        score = 0;
        foodSupply = 100;
    }
}
