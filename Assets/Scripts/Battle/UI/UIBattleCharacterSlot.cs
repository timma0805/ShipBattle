using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    public TMP_Text hpTxt;
    [SerializeField]
    public TMP_Text cdTxt;

    public int slotid { get; private set; }
    private Action<int> selectSlotCallback;
    private bool isMouseOver = false;

    public bool isPlayerCharacter { get; private set; }
    public GameObject characterObj { get; private set; }
    private Animator characterAnimator;
    private bool isSelecting = false;
    private bool isSelectingArea = false;

    //Setting
    const float scaleSize = 100.0f;
    const float moveTime = 1.0f;

    #region UnityActivities
    private void Awake()
    {
        selectBtn.onClick.AddListener(ClickSelectSlot);
        selectBtn.gameObject.SetActive(false);
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
        hpTxt.text = "";
        cdTxt.text = "";

        slotid = _slotid;
        selectSlotCallback = _selectCallback;
    }

    public GameObject ApplyCharacter(GameObject characterPrefab, bool isPlayer, int hp)
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

        UpdateHP(hp);

        return newCharacter;
    }

    public void HideInformation()
    {
        hpTxt.text = string.Empty;
        cdTxt.text = string.Empty;
    }

    public async Task<bool> MoveCharacterToSlot(GameObject newCharacter, bool isPlayer, CharacterData characterData)
    {
        if(characterObj != null)
        {
            Debug.LogError("Have character in Slot");
            return false;
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

        UpdateHP(characterData.CurHP);
        PlayCharacterAnimation(CharacterAnimationEnum.idle);
        await Task.Delay(100);

        return true;
    }

    // Callback method triggered by iTween when the movement is complete
    void OnMoveComplete(TaskCompletionSource<bool> tcs)
    {
        Debug.Log("OnMoveComplete");
        tcs.SetResult(true);
    }

    public async Task PlayCharacterAnimation(CharacterAnimationEnum animationEnum)
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
            switch (animationEnum)
            {
                case CharacterAnimationEnum.walk:
                    characterAnimator.SetBool("isMoving", true);
                    break;

                case CharacterAnimationEnum.attack:
                    characterAnimator.Play("attack");
                    break;

                case CharacterAnimationEnum.idle:
                    characterAnimator.SetBool("isMoving", false);
                    characterAnimator.SetBool("attack", false);
                    break;
                case CharacterAnimationEnum.hurt:
                    float scaleTime = 0.25f;
                    // Scale down to 0.5
                    iTween.ScaleTo(characterObj, iTween.Hash(
                        "scale", new Vector3(characterObj.transform.localScale.x*0.75f, characterObj.transform.localScale.y, characterObj.transform.localScale.z),
                        "time", scaleTime / 2,
                        "easetype", iTween.EaseType.easeInOutSine
                    ));

                    // Wait for half the duration of the scale animation
                    await Task.Delay((int)(scaleTime / 2 * 1000 + 100));

                    // Scale up to 1
                    iTween.ScaleTo(characterObj, iTween.Hash(
                        "scale", new Vector3(scaleSize, characterObj.transform.localScale.y, characterObj.transform.localScale.z),
                        "time", scaleTime / 2,
                        "easetype", iTween.EaseType.easeInOutSine
                    ));

                    break;
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

        if (isSelecting)
        {
            selectBtn.gameObject.SetActive(true);
        }
    }

    private void onMouseExit()
    {
        isMouseOver = false;

        if (isSelecting)
        {
            selectBtn.gameObject.SetActive(isSelectingArea);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit();
    }

    public void RemoveCharacter()
    {
        characterObj = null;
        characterAnimator = null;
        HideInformation();
    }

    public void UpdateHP(int hp)
    {
        hpTxt.text = "HP: " + hp.ToString();
    }

    public void UpdateCountdown(int cd) { 
        cdTxt.text = "CD: " + cd.ToString();
    }

    public void ShowEffectArea(bool needSelect, bool isAreaEffect)
    {
        isSelecting = needSelect;
        isSelectingArea = isAreaEffect;
        selectBtn.gameObject.SetActive(isAreaEffect);
        groundImg.color = Color.red;
    }

    public void Reset()
    {
        isSelecting = false;
        isSelectingArea = false;
        groundImg.color = Color.white;
        selectBtn.gameObject.SetActive(false);

        if (characterObj == null)
            HideInformation();
        
    }
}
