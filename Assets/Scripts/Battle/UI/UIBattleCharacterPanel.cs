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

    private void SelectSlot(int slotid)
    {

    }

    private void OpenSetting()
    {

    }

    private void ClickSpecialSkill() { }

}
