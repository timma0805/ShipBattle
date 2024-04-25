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
    [SerializeField]
    public TMP_Text statusTxt;

    public int slotid { get; private set; }
    private Action<int> selectSlotCallback;
    private bool isMouseOver = false;

    public bool isPlayerCharacter { get; private set; }
    public GameObject characterObj { get; private set; }
    private Animator characterAnimator;
    private bool isSelecting = false;
    private bool isSelectingArea = false;
    private bool isLoopingAnimation = false;

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
        hpTxt.text = string.Empty;
        cdTxt.text = string.Empty;
        statusTxt.text = string.Empty;

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
        statusTxt.text = string.Empty;
    }

    public async Task<bool> MoveCharacterToSlot(GameObject newCharacter, bool isPlayer , CharacterData characterData)
    {
        if(characterObj != null)
        {
            Debug.LogError("Have character in Slot");
            return false;
        }

        var tcs = new TaskCompletionSource<bool>();
        isPlayerCharacter = isPlayer;

        newCharacter.transform.SetParent(transform);
        characterObj = newCharacter;
        characterAnimator = newCharacter.GetComponent<Animator>();

        Vector3 targetVector = new Vector3();
        if(isPlayer)
        {
            targetVector = new Vector3(0, - 100.0f, 0);        
        }
        else
        {
            targetVector = new Vector3(0, - 50.0f, 0);         
        }

        PlayCharacterAnimation(CharacterAnimationEnum.walk, false);
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
        PlayCharacterAnimation(CharacterAnimationEnum.idle, false);
        await Task.Delay(100);

        return true;
    }

    // Callback method triggered by iTween when the movement is complete
    void OnMoveComplete(TaskCompletionSource<bool> tcs)
    {
        Debug.Log("OnMoveComplete");
        tcs.SetResult(true);
    }

    public async Task PlayCharacterAnimation(CharacterAnimationEnum animationEnum, bool isLoop)
    {
        if(characterObj == null) return;

        isLoopingAnimation = false;

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

            if(isLoop)
            {
                isLoopingAnimation = true;
                while(isLoopingAnimation)
                {
                    if(!characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationName"))
                        characterAnimator.Play(animationName);

                    await Task.Delay(100);
                }
            }
            else
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

    public void RemoveCharacter(bool disableCharacter)
    {
        if(disableCharacter && characterObj != null)
        {
            characterObj.SetActive(false);
        }
        characterObj = null;
        characterAnimator = null;
        HideInformation();
    }

    public void UpdateHP(int hp)
    {
        hpTxt.text = "HP: " + hp.ToString();
    }

    public void UpdateCountdown(string skillName, int cd) { 

        if(cd > 0) 
            cdTxt.text = skillName + ": " + cd.ToString();
        else
            cdTxt.text = string.Empty;
    }

    public void UpdateStatus(Dictionary<CharacterStatus,float> statusDic)
    {
        statusTxt.text = string.Empty;

        if (statusDic.Count != 0)
        {
            foreach (var item in statusDic)
            {
                if(item.Key == CharacterStatus.Freeze) 
                    statusTxt.text += $"Freeze {item.Value}\n";
                else if (item.Key == CharacterStatus.Shield)
                    statusTxt.text += $"Shield {item.Value}\n";
                else if (item.Key == CharacterStatus.Blind)
                    statusTxt.text += $"Blind {item.Value}\n";
                else if (item.Key == CharacterStatus.Untargetable)
                    statusTxt.text += $"Untargetable {item.Value} \n";
                else if (item.Key == CharacterStatus.Bleed)
                    statusTxt.text += $"Bleed {item.Value} \n";
                else if (item.Key == CharacterStatus.Posion)
                    statusTxt.text += $"Posion {item.Value} \n";
                else if (item.Key == CharacterStatus.Unmovement)
                    statusTxt.text += $"Unmovement {item.Value} \n";
                else if (item.Key == CharacterStatus.Stun)
                    statusTxt.text += $"Stun {item.Value} \n";
                else if (item.Key == CharacterStatus.Dogde) 
                    statusTxt.text += $"Dodge {item.Value} \n";
                  else if (item.Key == CharacterStatus.Weakness) 
                    statusTxt.text += $"Weakness {item.Value} \n";
                else if (item.Key == CharacterStatus.IncreaseAttack)
                    statusTxt.text += $"IncreaseAttack {item.Value} \n";
                else if (item.Key == CharacterStatus.Fire)
                    statusTxt.text += $"Fire {item.Value} \n";
                else if (item.Key == CharacterStatus.Defense)
                    statusTxt.text += $"Defense {item.Value} \n";
                else if (item.Key == CharacterStatus.Prepare)
                    statusTxt.text += $"Prepare {item.Value} \n";

            }
        }
    }

    public void AddStatus(CardEffectType effectType, int value)
    {

    }

    public void ShowEffectArea(bool needSelect, bool isAreaEffect, bool isPlayerAction)
    {
        isSelecting = needSelect;
        isSelectingArea = isAreaEffect;
        selectBtn.gameObject.SetActive(isAreaEffect);
        groundImg.color = isPlayerAction? Color.green: Color.red;
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
