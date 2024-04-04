using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBattleCharacterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum CharacterAnimationEnum
    {
        idle,
        walk,
        casting,
        attack,
        hurt,
        die,
        victory
    }

    [SerializeField]
    public Image groundImg;
    [SerializeField]
    public Button selectBtn;
    [SerializeField]
    public Button confirmBtn;
    [SerializeField]
    public Button cancelBtn;

    public int slotid { get; private set; }
    private Action<int> selectSlotCallback;
    private bool isMouseOver = false;

    public bool isPlayerCharacter { get; private set; }
    public GameObject characterObj { get; private set; }
    private Animator characterAnimator;

    //Setting
    const float scaleSize = 100.0f;
    const float moveTime = 1.0f;

    #region UnityActivities
    private void Awake()
    {
        selectBtn.onClick.AddListener(ClickSelectSlot);
        confirmBtn.onClick.AddListener(ClickConfirmBtn);
        cancelBtn.onClick.AddListener(ClickCancelBtn);

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

    public GameObject ApplyCharacter(GameObject characterPrefab, bool isPlayer)
    {
        isPlayerCharacter = isPlayer;
        GameObject newCharacter = Instantiate(characterPrefab, transform);

        if (isPlayer)
        {
            newCharacter.transform.localScale = new Vector3(-newCharacter.transform.localScale.x * scaleSize, newCharacter.transform.localScale.y * scaleSize, newCharacter.transform.localScale.z);
            newCharacter.transform.localPosition = new Vector3(0, newCharacter.transform.localPosition.y - 100.0f, 0);
        }
        else
        {
            newCharacter.transform.localScale = new Vector3(newCharacter.transform.localScale.x * scaleSize, newCharacter.transform.localScale.y * scaleSize, newCharacter.transform.localScale.z);
            newCharacter.transform.localPosition = new Vector3(0, newCharacter.transform.localPosition.y - 50.0f, 0);
        }

        characterObj = newCharacter;
        characterAnimator = newCharacter.GetComponent<Animator>();

        return newCharacter;
    }

    public async Task MoveCharacterToSlot(GameObject newCharacter, bool isPlayer)
    {
        if(characterObj != null)
        {
            Debug.LogError("Have character in Slot");
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        isPlayerCharacter = isPlayer;

        newCharacter.transform.SetParent(transform);
        Vector3 targetVector = new Vector3();

        if(isPlayer)
        {
            targetVector = new Vector3(0, - 100.0f, 0);
        }
        else
        {
            targetVector = new Vector3(0, - 50.0f, 0);
        }

        characterObj = newCharacter;
        characterAnimator = newCharacter.GetComponent<Animator>();
        PlayCharacterAnimation(CharacterAnimationEnum.walk);
        iTween.MoveTo(characterObj, iTween.Hash(
            "position", targetVector,
            "islocal", true,
            "time", moveTime,
            "easetype", iTween.EaseType.easeInOutSine,
            "oncomplete", "OnMoveComplete",
            "oncompletetarget", gameObject, // Callback target,
            "oncompleteparams", tcs // Pass TaskCompletionSource as parameter
        ));
        await tcs.Task;

        PlayCharacterAnimation(CharacterAnimationEnum.idle);
        await Task.Delay(100);
    }

    // Callback method triggered by iTween when the movement is complete
    void OnMoveComplete(TaskCompletionSource<bool> tcs)
    {
        Debug.Log("OnMoveComplete");
        tcs.SetResult(true);
    }

    public void PlayCharacterAnimation(CharacterAnimationEnum animationEnum)
    {
        if(characterObj == null) return;

        if (isPlayerCharacter)
        {
            string animationName = "";
            switch (animationEnum)
            {
                case CharacterAnimationEnum.walk:
                    animationName = "walk";
                    break;
                case CharacterAnimationEnum.idle:
                    animationName = "idle";
                    break;
                case CharacterAnimationEnum.attack:
                    animationName = "attack";
                    break;
                case CharacterAnimationEnum.casting:
                    animationName = "casting";
                    break;
                case CharacterAnimationEnum.die:
                    animationName = "die";
                    break;
                case CharacterAnimationEnum.hurt:
                    animationName = "hurt";
                    break;
                case CharacterAnimationEnum.victory:
                    animationName = "victory";
                    break;
                default:
                    animationName = "idle";
                    break;
            }

            characterAnimator.Play(animationName);
        }
        else
        {
            if(animationEnum == CharacterAnimationEnum.walk)
            {
                characterAnimator.SetBool("isMoving", true);
            }
            else if (animationEnum == CharacterAnimationEnum.attack)
            {
                characterAnimator.Play("attack");
            }
            else if(animationEnum == CharacterAnimationEnum.idle)
            {
                characterAnimator.SetBool("isMoving", false);
                characterAnimator.SetBool("attack", false);
            }
        }

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

    public void RemoveCharacter()
    {
        characterObj = null;
        characterAnimator = null;
    }
}
