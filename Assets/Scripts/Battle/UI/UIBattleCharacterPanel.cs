using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIBattleCharacterPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSlotPrefab;
    [SerializeField]
    private GameObject[] characterSlots;
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Button settingBtn;
    [SerializeField]
    private Button specialSkillBtn;
    [SerializeField]
    private Button endTurnBtn;

    [SerializeField]
    private GameObject[] characterPrefabList;
    [SerializeField]
    private GameObject[] enemyPrefabList;

    private List<UIBattleCharacterSlot> characterSlotsList;
    private MiniBattleCoreController controller;
    //Setting
    private const int maxSlotPerRow = 6;
    private Vector2? waitSelectPos = null;
    private Vector2 cancelSelectPos = new Vector2(-99,-99);
    // Start is called before the first frame update
    void Start()
    {
        settingBtn.onClick.AddListener(OpenSetting);
        specialSkillBtn.onClick.AddListener(ClickSpecialSkill);
        endTurnBtn.onClick.AddListener(ClickEndTurn);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            waitSelectPos = cancelSelectPos;
        }
    }

    public void Init(MiniBattleCoreController _controller)
    {
        controller = _controller;
        characterSlotsList = new List<UIBattleCharacterSlot>();
        for (int i = 0; i < characterSlots.Length; i++)
        {
            for (int index = 0; index < maxSlotPerRow; index++)
            {
                UIBattleCharacterSlot slot = Instantiate(characterSlotPrefab, characterSlots[i].transform).GetComponent<UIBattleCharacterSlot>();
                int slotIndex = i * maxSlotPerRow + index ;
                slot.Init(slotIndex, SelectSlot);
                characterSlotsList.Add(slot);
            }
        }

        Debug.Log("characterSlotsList Count:" + characterSlotsList.Count);
    }

    public void StartBattle(List<BattleCharacter> battleCharacters, List<Vector2> charactersPos)
    {

        for (int i = 0;i < battleCharacters.Count;i++)
        {
            var data = battleCharacters[i].GetCharacterData();
            if(battleCharacters[i].IsPlayerCharacter() ) 
                characterSlotsList[ConvertPosToSlotID(charactersPos[i])].ApplyCharacter(characterPrefabList[data.ID - 1], true, data.HP);
            else
                characterSlotsList[ConvertPosToSlotID(charactersPos[i])].ApplyCharacter(enemyPrefabList[data.ID - 1], false, data.HP);
        }
    }

    public async Task<bool> MoveCharacter(Vector2 pos, Vector2 newpos, CharacterData characterData)
    {
        if(pos == newpos) return false;

        int slotID = ConvertPosToSlotID(pos);
        int targetSlotID = ConvertPosToSlotID(newpos);

        UIBattleCharacterSlot orginSlot = characterSlotsList.Find(x => x.slotid == slotID);
        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == targetSlotID);
        if (targetSlot == null)
        {
            targetSlot = orginSlot;
            Debug.Log("targetSlot null Can't Move");
            return false;
        }
        orginSlot.HideInformation();
        bool isSuccess = await targetSlot.MoveCharacterToSlot(orginSlot.characterObj, orginSlot.isPlayerCharacter, characterData);
        if (!isSuccess) 
            return false;

        orginSlot.RemoveCharacter(false);
        return true;
    }

    private void CharacterAttack(Vector2  pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.attack);    
    }

    private void CharacterHurt(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        if(slotID < 0 ||  slotID >= characterSlotsList.Count) 
            return;

        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.hurt);
    }

    public async Task CharacterDie(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
        {
            Debug.LogError("CharacterDie SlotID error");
            return;
        }

        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.die);
        await Task.Delay(1000);
        characterSlotsList[slotID].RemoveCharacter(true);
    }

    public void CharacterHeal(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.casting);
    }

    private void CharacterBeHeal(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.idle);
    }

    public async Task Attack(Vector2 pos, Vector2 targetPos, bool needAttackAnimation)
    {
        if (needAttackAnimation)
        {
            CharacterAttack(pos);
            await Task.Delay(200);
        }

        CharacterHurt(targetPos);
    }

    public async Task Heal(Vector2 pos, Vector2 targetPos)
    {
        CharacterHeal(pos);
        await Task.Delay(200);
        CharacterBeHeal(targetPos);
    }

    public async Task Victory()
    {
        for (int i = 0; i < characterSlotsList.Count; i++)
        {
            if (characterSlotsList[i].isPlayerCharacter)
            {
                PlayCharacterAnimation(characterSlotsList[i], UIBattleCharacterSlot.CharacterAnimationEnum.victory);
            }
        }

        await Task.Delay(1000);
    }

    public async Task Lose()
    {
        for (int i = 0; i < characterSlotsList.Count; i++)
        {
            if (!characterSlotsList[i].isPlayerCharacter)
            {
                PlayCharacterAnimation(characterSlotsList[i], UIBattleCharacterSlot.CharacterAnimationEnum.victory);
            }
        }

        await Task.Delay(1000);
    }

    public async Task UpdateHP(Vector2 pos, int hp, bool needAnimation)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        characterSlotsList[slotID].UpdateHP(hp);
    }

    public async Task UpdateStatus(Vector2 pos, Dictionary<CharacterStatus, float> statusDic, bool needAnimation)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        characterSlotsList[slotID].UpdateStatus(statusDic);
    }

    public void UpdateCountdown(Vector2 pos, int coundown)
    {
        int slotID = ConvertPosToSlotID(pos);
        if (slotID < 0 || slotID >= characterSlotsList.Count)
            return;

        characterSlotsList[slotID].UpdateCountdown(coundown);
    }

    private void PlayCharacterAnimation(UIBattleCharacterSlot targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum characterAnimationEnum)
    {
        targetSlot.PlayCharacterAnimation(characterAnimationEnum);
    }

    private int ConvertPosToSlotID(Vector2 vector)
    {
        if ((vector.x < 0) || (vector.y < 0))
            return -1;
        return Mathf.RoundToInt(vector.x  + vector.y * maxSlotPerRow);
    }

    private Vector2 ConvertSlotIDToPos(int  slotID)
    {
        return new Vector2(slotID % maxSlotPerRow, slotID / maxSlotPerRow);
    }

    public async Task<Vector2> ShowEffectArea(List<Vector2> vectors, bool needSelect, bool isAreaEffect, bool isPlayerAction)
    {
        try
        {
            bool hasEffectSlot = false;
            for (int i = 0; i < vectors.Count; i++)
            {
                int slotID = ConvertPosToSlotID(vectors[i]);
                if (slotID >= 0 && slotID < characterSlotsList.Count)
                {
                    characterSlotsList[slotID].ShowEffectArea(needSelect, isAreaEffect, isPlayerAction);
                    hasEffectSlot = true;
                }
            }

            if (needSelect && hasEffectSlot)
            {
                waitSelectPos = null;
                while (!waitSelectPos.HasValue)
                {
                    await Task.Yield();
                }

                for (int i = 0; i < vectors.Count; i++) //remove show effects
                {
                    int slotID = ConvertPosToSlotID(vectors[i]);
                    if (slotID >= 0 && slotID < characterSlotsList.Count)
                    {
                        characterSlotsList[slotID].Reset();
                    }
                }

                return waitSelectPos.Value;
            }
            else if (hasEffectSlot)
            {
                return vectors[0];
            }
            else
                return vectors[0];

        }
        catch (System.Exception e)
        {
            Debug.LogError("ShowEffectArea Error: " + e.Message);
            return vectors[0];
        }
    }

    public void ClearEffectArea(List<Vector2> vectors)
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            int slotID = ConvertPosToSlotID(vectors[i]);
            if (slotID >= 0 && slotID < characterSlotsList.Count)
            {
                characterSlotsList[slotID].Reset();
            }
        }
    }

    private void SelectSlot(int slotid)
    {
        waitSelectPos = ConvertSlotIDToPos(slotid);
        for(int i = 0;i < characterSlotsList.Count;i++)
        {
            characterSlotsList[i].Reset();
        }
        Debug.Log("SelectSlot: " + slotid + " waitSelectPos: " + waitSelectPos);

    }

    private void OpenSetting()
    {

    }

    private void ClickSpecialSkill() { }
    private void ClickEndTurn() {
        controller.PlayerSelectEndTurn();
    }

}
