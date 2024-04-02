using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour
{
    private BattlePlayerData playerData;
    private MiniBattleCoreController battleController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task Init(MiniBattleCoreController controller, BattlePlayerData _playerData)
    {
        battleController = controller;

        playerData = _playerData;
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
