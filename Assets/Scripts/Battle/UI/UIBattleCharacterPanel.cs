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
    private GameObject[] characterPrefabList;
    [SerializeField]
    private GameObject[] enemyPrefabList;

    private List<UIBattleCharacterSlot> characterSlotsList;

    //Setting
    private const int maxSlotPerRow = 6;

    // Start is called before the first frame update
    void Start()
    {
        settingBtn.onClick.AddListener(OpenSetting);
        specialSkillBtn.onClick.AddListener(ClickSpecialSkill);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            characterSlotsList[0].PlayCharacterAnimation(UIBattleCharacterSlot.CharacterAnimationEnum.attack);
            characterSlotsList[5].PlayCharacterAnimation(UIBattleCharacterSlot.CharacterAnimationEnum.attack);
        }
    }

    public void Init()
    {
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

    public void StartBattle(List<BattlePlayerCharacter> battlePlayerCharacters, List<Vector2> charactersPos,  List<BattleEnemy> battleEnemies, List<Vector2> enemiesPos)
    {
        const float scaleSize = 100.0f;

        for (int i = 0;i < battlePlayerCharacters.Count;i++)
        {
           characterSlotsList[ConvertPosToSlotID(charactersPos[i])].ApplyCharacter(characterPrefabList[battlePlayerCharacters[i].characterData.ID-1], true);
        }

        for (int i = 0; i < battleEnemies.Count; i++)
        {
            characterSlotsList[ConvertPosToSlotID(enemiesPos[i])].ApplyCharacter(enemyPrefabList[battleEnemies[i].enemyData.ID-1], false);
        }
    }

    public async Task<bool> MoveCharacter(Vector2 pos, Vector2 newpos)
    {
        int slotID = ConvertPosToSlotID(pos);
        int targetSlotID = ConvertPosToSlotID(newpos);

        UIBattleCharacterSlot orginSlot = characterSlotsList.Find(x => x.slotid == slotID);
        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == targetSlotID);
        if (targetSlot == null)
        {
            targetSlot = orginSlot;
            Debug.Log("Can't Move");
            return false;
        }

        await targetSlot.MoveCharacterToSlot(orginSlot.characterObj, orginSlot.isPlayerCharacter);
        orginSlot.RemoveCharacter();
        return true;
    }

    public void CharacterAttack(Vector2  pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.attack);    
    }

    public void CharacterHurt(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.hurt);
    }

    public void CharacterDie(Vector2 pos)
    {
        int slotID = ConvertPosToSlotID(pos);
        UIBattleCharacterSlot targetSlot = characterSlotsList.Find(x => x.slotid == slotID);
        PlayCharacterAnimation(targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum.die);
    }

    public void Victory()
    {
        for (int i = 0; i < characterSlotsList.Count; i++)
        {
            if (characterSlotsList[i].isPlayerCharacter)
            {
                PlayCharacterAnimation(characterSlotsList[i], UIBattleCharacterSlot.CharacterAnimationEnum.victory);
            }
        }
    }

    public void Lose()
    {
        for (int i = 0; i < characterSlotsList.Count; i++)
        {
            if (!characterSlotsList[i].isPlayerCharacter)
            {
                PlayCharacterAnimation(characterSlotsList[i], UIBattleCharacterSlot.CharacterAnimationEnum.victory);
            }
        }
    }

    private void PlayCharacterAnimation(UIBattleCharacterSlot targetSlot, UIBattleCharacterSlot.CharacterAnimationEnum characterAnimationEnum)
    {
        targetSlot.PlayCharacterAnimation(characterAnimationEnum);
    }

    private int ConvertPosToSlotID(Vector2 vector)
    {
        return Mathf.RoundToInt(vector.x  + vector.y * maxSlotPerRow);
    }

    private void SelectSlot(int slotid)
    {

    }

    private void OpenSetting()
    {

    }

    private void ClickSpecialSkill() { }

}
