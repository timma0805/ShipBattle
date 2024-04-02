using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardController : MonoBehaviour
{
    [SerializeField]
    private UIBattleCardsPanel uIBattleCardsPanel;

    private BattleEnemy[] enemyDatas;
    private BattlePlayerCharacter playerData;
    private MiniBattleCoreController battleController;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(MiniBattleCoreController _battleController, List<CardData> cardDb)
    {
        battleController = _battleController;
        uIBattleCardsPanel.Init(this, cardDb);
    }

    public void StartPlayerTurn()
    {
        uIBattleCardsPanel.DrawCard();
    }

    public void UpdateCharacterData(BattlePlayerCharacter player, BattleEnemy[] battleEnemies)
    {
        playerData = player;
        enemyDatas = battleEnemies;
    }

    public bool CheckCardCondition(Card card)
    {
        Debug.Log("CheckCardCondition:" + card.data.Name);
        bool canUse = true;

        Vector2 comparePos;
        int compareInt = -1;
        bool compareBool = false;
        string compareStr = "";

        //check condition
        //for (int i = 0; i < card.data.conditionList.Count; i++)
        //{
        //    //process condition
        //    CardCondition condition = card.data.conditionList[i];

        //    //Use on Player
        //    if (condition.effectTarget == CardCondition.EffectTarget.Self)
        //    {
        //        if (condition.conditionValue == CardCondition.ConditionValue.HP)
        //        {
        //            compareInt = playerData.playerData.MaxShipBodyHP;
        //        }
        //        else if (condition.conditionValue == CardCondition.ConditionValue.MP)
        //        {
        //            //compareInt = playerData.playerData.MP;
        //        }
        //        else if (condition.conditionValue == CardCondition.ConditionValue.Position)
        //        {
        //            comparePos = playerData.currentPos;
        //        }
        //        else if (condition.conditionValue == CardCondition.ConditionValue.CharacterStatus)
        //        {
        //            compareBool = playerData.playerStatusList.FindIndex(x => x == (CharacterStatus)int.Parse(condition.conditionCompareValueStr)) > 0;
        //        }
        //        else if (condition.conditionValue == CardCondition.ConditionValue.FaceDirection)
        //        {
        //            compareInt = (int)playerData.currentDirection;
        //        }
        //        else
        //            compareStr = "Not Found";
        //    }

        //    if (condition.conditionCompareValueType == CardCondition.ConditionCompareValueType.Boolean)
        //    {
        //        if (compareBool != (bool.TrueString == condition.conditionCompareValueStr))
        //            return false;
        //    }
        //    else if (condition.conditionCompareValueType == CardCondition.ConditionCompareValueType.Integer)
        //    {
        //        if (compareInt != int.Parse(condition.conditionCompareValueStr))
        //            return false;
        //    }
        //    else if (condition.conditionCompareValueType == CardCondition.ConditionCompareValueType.String)
        //    {
        //        if (compareStr != condition.conditionCompareValueStr)
        //            return false;
        //    }
        //    else if (condition.conditionCompareValueType == CardCondition.ConditionCompareValueType.Other)
        //    {
        //        if (condition.conditionValue == CardCondition.ConditionValue.CharacterStatus && !compareBool)
        //            return false;
        //    }
        //}

        return canUse;
    }

    public void ShowEffectRange(Card card)
    {
        //for(int i = 0; i < card.data.effectList.Count; i++)
        //{
        //    CardEffect effect = card.data.effectList[i];
        //    //if(effect.haveDirection)
        //    //    coreController.ShowCardEffectRange(effect.rangeX, effect.rangeY, effect.direction, Color.red);
        //}
    }

    public bool UsePlayerCard(Card card)
    {
        return true;
        //return coreController.UsePlayerCard(card);
    }

    public void ResetMapTiles()
    {
        //coreController.ResetMapTiles();
    }
}
