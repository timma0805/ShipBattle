using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyDataListSO enemyDataListSO;

    private int[] enemyIndexList;
    private EnemyData[] enemyDataList;
    private BattleEnemy[] battleEnemiesList;
    private MiniBattleCoreController battleController;

    public GameObject[] GetEnemiesPrefabs(int[] _enemyIndexList)
    {
        enemyIndexList = _enemyIndexList;

        enemyDataList = new EnemyData[enemyIndexList.Length];
        GameObject[] enemyPrefabs = new GameObject[enemyIndexList.Length];

        for (int i = 0; i < enemyIndexList.Length; i++)
        {
            //Read Data from enemyDataListSO
            EnemyData enemyData = enemyDataListSO.enemyDataList[enemyIndexList[i]];
            enemyPrefabs[i] = enemyData.prefab;
            enemyDataList[i] = enemyData;
        }

        return enemyPrefabs;
    }

    public async Task Init(MiniBattleCoreController controller,  GameObject[] enemiesObjs)
    {
        battleController = controller;

        battleEnemiesList = new BattleEnemy[enemiesObjs.Length];

        for (int i = 0; i < enemiesObjs.Length; i++)
        {
            if (enemiesObjs[i].GetComponent<BattleEnemy>() == null)
                battleEnemiesList[i] = enemiesObjs[i].AddComponent<BattleEnemy>();
            else
                battleEnemiesList[i] = enemiesObjs[i].GetComponent<BattleEnemy>();
        }
    }

    public void StartEnemyTurn()
    {
        for(int i = 0; i < battleEnemiesList.Length; i++)
        {
            //coreController.DoEnemyAction(); //temp
            battleEnemiesList[i].DoAction();
        }
    }

    public BattleEnemy GetTargetEnemy(int index = 0)
    {
        return battleEnemiesList[index];
    }

    public BattleEnemy[] GetAllEnemies()
    {
        return battleEnemiesList;
    }

    public int[] GetTargetEnemiesWidth()
    {
        int[] widthArr = new int[enemyIndexList.Length];
        for(int i = 0; i < enemyDataList.Length; i++)
        {
            widthArr[i] = enemyDataList[i].width;
        }
        return widthArr;
    }

    public int[] GetTargetEnemiesLength()
    {
        int[] lengthArr = new int[enemyIndexList.Length];
        for (int i = 0; i < enemyDataList.Length; i++)
        {
            lengthArr[i] = enemyDataList[i].length;
        }
        return lengthArr;
    }

    public bool CheckIsAllEnemyDead()
    {
        for(int i = 0; i < battleEnemiesList.Length; i++)
        {
            if (!battleEnemiesList[i].IsDead())
                return false;
        }

        return true;
    }

}
