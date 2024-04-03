using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private List<GameObject> characterObjList;
    private List<GameObject> enemyObjList;

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

    public void StartBattle(List<BattlePlayerCharacter> battlePlayerCharacters, List<BattleEnemy> battleEnemies)
    {
        const float scaleSize = 100.0f;
        characterObjList = new List<GameObject>();
        enemyObjList = new List<GameObject>();

        for (int i = 0;i < battlePlayerCharacters.Count;i++)
        {
            GameObject newCharacter = Instantiate(characterPrefabList[battlePlayerCharacters[i].characterData.ID], characterSlotsList[i].transform);
            newCharacter.transform.localScale = new Vector3(-newCharacter.transform.localScale.x* scaleSize, newCharacter.transform.localScale.y* scaleSize, newCharacter.transform.localScale.z);
            newCharacter.transform.localPosition = new Vector3(newCharacter.transform.localPosition.x, newCharacter.transform.localPosition.y - 100.0f, newCharacter.transform.localPosition.z);
            characterObjList.Add(newCharacter);
        }

        for (int i = 0; i < battleEnemies.Count; i++)
        {
            GameObject newCharacter = Instantiate(enemyPrefabList[battleEnemies[i].enemyData.ID], characterSlotsList[characterSlotsList.Count - 1 - i].transform);
            newCharacter.transform.localScale = new Vector3(newCharacter.transform.localScale.x * scaleSize, newCharacter.transform.localScale.y * scaleSize, newCharacter.transform.localScale.z);
            newCharacter.transform.localPosition = new Vector3(newCharacter.transform.localPosition.x, newCharacter.transform.localPosition.y - 50.0f, newCharacter.transform.localPosition.z);
            enemyObjList.Add(newCharacter);
        }
    }

    private void SelectSlot(int slotid)
    {

    }

    private void OpenSetting()
    {

    }

    private void ClickSpecialSkill() { }

}
