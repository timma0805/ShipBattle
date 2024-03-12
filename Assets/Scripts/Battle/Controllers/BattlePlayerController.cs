using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour
{
    private BattlePlayerData battlePlayerData;
    private BattlePlayer battlePlayer;
    private MiniBattleCoreController battleController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task Init(MiniBattleCoreController controller, GameObject playerObj)
    {
        battleController = controller;

        if (playerObj.GetComponent<BattlePlayer>() == null)
            battlePlayer = playerObj.AddComponent<BattlePlayer>();
        else
            battlePlayer = playerObj.GetComponent<BattlePlayer>();

        battlePlayer.Init(battlePlayerData);
    }
    public BattlePlayerData GetBattlePlayerData()
    {
        return battlePlayerData;
    }

    public BattlePlayer GetBattlePlayer()
    {
        return battlePlayer;
    }

    public GameObject GetPlayerPrefab()
    {
        //   return playerPrefab;
        return null;
    }

    public int GetPlayerWidth()
    {
        return 0;
        //return playerData.Width;
    }

    public int GetPlayerLength()
    {
        return 0;
        //return playerData.Length;
    }

    public void StartPlayerTurn()
    {

    }

    public bool UseMP(int cost)
    {
        return false;
        //int currentMP = battlePlayer.playerData.MP;

        //Debug.Log("UseMP:" + cost + "|" + currentMP);

        //if (cost <= currentMP)
        //    battlePlayer.UsePlayerMP(cost);
        //else
        //    return false;

        //return true;
    }

}
