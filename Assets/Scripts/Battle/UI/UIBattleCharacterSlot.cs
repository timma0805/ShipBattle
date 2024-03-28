using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBattleCharacterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public Image characterImg;
    [SerializeField]
    public Image groundImg;
    [SerializeField]
    public Button selectBtn;
    [SerializeField]
    public Button confirmBtn;
    [SerializeField]
    public Button cancelBtn;

    private int slotid;
    private Action<int> selectSlotCallback;
    private bool isMouseOver = false;

    #region UnityActivities
    private void Awake()
    {
        selectBtn.onClick.AddListener(ClickSelectSlot);
        confirmBtn.onClick.AddListener(ClickConfirmBtn);
        cancelBtn.onClick.AddListener(ClickCancelBtn);

        characterImg.gameObject.SetActive(false);
        selectBtn.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    #endregion

    public void Init(int _slotid, Action<int> _selectCallback)
    {
        slotid = _slotid;
        selectSlotCallback = _selectCallback;
    }

    private void ClickSelectSlot()
    {
        Debug.Log("ClickSelectSlot");

        if (selectSlotCallback != null)
        {
            selectSlotCallback(slotid);
        }
    }

    private void onMouseOver()
    {
        isMouseOver = true;

        selectBtn.gameObject.SetActive(true);
    }

    private void onMouseExit()
    {
        isMouseOver = false;

        selectBtn.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit();
    }

    private void ClickConfirmBtn()
    {
        Debug.Log("ClickConfirmBtn");
    }
    private void ClickCancelBtn()
    {
        Debug.Log("ClickCancelBtn");
    }

    private void TriggerOptions(bool isActive)
    {
        confirmBtn.gameObject.SetActive(isActive);
        cancelBtn.gameObject.SetActive(isActive);
    }
}
