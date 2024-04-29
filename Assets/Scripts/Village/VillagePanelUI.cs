using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillagePanelUI : MonoBehaviour
{
    [SerializeField]
    private Camera uiCamera;
    [SerializeField]
    private Sprite[] openBGSprites;
    [SerializeField]
    private Sprite[] closeBGSprites;
    [SerializeField]
    private Image maskImg;
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Image villageImg;
    [SerializeField]
    private Image[] decorationImgList;
    [SerializeField]
    private RectMask2D mapMask;
    [SerializeField]
    private float mapMaskStartPadding;
    [SerializeField]
    private float mapMaskEndPadding;
    [SerializeField]
    private float mapMaskPaddingGap;

    [SerializeField]
    private Button[] accessPoints;
    [SerializeField]
    private Button leaveBtn;

    private Action _closeCallback;
    private Action[] _callbacks;
    private const int dummyMapSpriteCount = 3;

    private RectTransform bgRectTransform;
    private RectTransform villageRectTransform;
    private const float moveSpeed = 5f;
    private const float moveThreshold = 25f; // Threshold distance from screen edge to start moving
    private float screenWidth;
    private float screenHeight;
    private bool isAnimation = false;

    // Start is called before the first frame update
    private void Awake()
    {
        // Get the screen dimensions
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        bgRectTransform = bgImg.GetComponent<RectTransform>();
        villageRectTransform = villageImg.GetComponent<RectTransform>();
        mapMaskEndPadding = bgImg.GetComponent<RectTransform>().rect.width/2;
        mapMaskPaddingGap = (mapMaskEndPadding - mapMaskStartPadding) / (openBGSprites.Length - dummyMapSpriteCount);
        leaveBtn.onClick.AddListener(LeaveVillage);

        maskImg.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isAnimation)
            return;

        // Get the mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Check if mouse is near the screen edge
        if (mousePosition.x <= moveThreshold) // Right edge
        {
            MoveBGImage(Vector2.right);
        }
        else if (mousePosition.x >= screenWidth - moveThreshold ) // Left edge
        {
            MoveBGImage(Vector2.left);
        }
        else if (mousePosition.y <= moveThreshold ) // Top edge
        {
            MoveBGImage(Vector2.up);
        }
        else if (mousePosition.y >= screenHeight - moveThreshold) // Bottom edge
        {
            MoveBGImage(Vector2.down);
        }
    }

    // Function to move the BG image in a specified direction
    private void MoveBGImage(Vector2 direction)
    {
        if (!IsImageInScreen(direction))
            return;
        // Move the image using its RectTransform
        bgRectTransform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    public bool IsImageInScreen(Vector2 direction)
    {
        // Get the image's position in canvas space
        Vector3[] corners = new Vector3[4];
        villageRectTransform.GetWorldCorners(corners);
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]);
        }

        // Check if any corner is outside the screen boundaries
        for(int i = 0;i < corners.Length;i++)
        {
            if(direction== Vector2.left && (i == 0 || i == 1) && corners[i].x < -(villageRectTransform.rect.width / 2.0f) * bgImg.transform.localScale.x)
            {
                return false; 
            }
            else if(direction == Vector2.right && (i == 2 || i == 3) && corners[i].x > (Screen.width + villageRectTransform.rect.width / 2.0f) * bgImg.transform.localScale.x)
            {
                return false;
            }
            else if (direction == Vector2.down && (i == 0 || i == 3) && corners[i].y < -(villageRectTransform.rect.height/2.0f)* bgImg.transform.localScale.x)
            {
                return false;
            }
            else if (direction == Vector2.up && (i == 1 || i == 2) && corners[i].y > (Screen.height + villageRectTransform.rect.height/2.0f)*bgImg.transform.localScale.x)
            {
                return false;
            }      
        }

        return true; // Image is fully within the screen
    }


    public void Init(Action closeCallback, Camera camera)
    {
        uiCamera = camera;
        _closeCallback = closeCallback;
    }

    private async Task OpenMapAnimation()
    {
        isAnimation = true;
        bgRectTransform.localPosition = Vector3.zero;

        for (int i = 0; i < accessPoints.Length; i++)
        {
            accessPoints[i].gameObject.SetActive(false);
        }

        mapMask.padding = new Vector4(mapMaskEndPadding, 0, mapMaskEndPadding, 0);

        for (int i = 0; i < openBGSprites.Length; i++) { if (openBGSprites[i] != null)
            {
                bgImg.sprite = openBGSprites[i];
                if(i > dummyMapSpriteCount)
                    mapMask.padding = new Vector4(mapMaskEndPadding - mapMaskPaddingGap * (i- dummyMapSpriteCount), 0, mapMaskEndPadding - mapMaskPaddingGap * (i-dummyMapSpriteCount), 0);
                await Task.Delay(100);
            }
        }
        isAnimation = false;
    }

    public async Task CloseMapAnimation()
    {
        isAnimation = true;
        bgRectTransform.localPosition = Vector3.zero;

        for (int i = 0; i < accessPoints.Length; i++)
        {
            accessPoints[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < closeBGSprites.Length; i++)
        {
            if (closeBGSprites[i] != null)
            {
                bgImg.sprite = closeBGSprites[i];

                if (i < closeBGSprites.Length- dummyMapSpriteCount)
                    mapMask.padding = new Vector4(mapMaskStartPadding + mapMaskPaddingGap*(i+1), 0,mapMaskStartPadding + mapMaskPaddingGap * (i+1),0);
                await Task.Delay(100);
            }
        }
        isAnimation = false;
    }

    public async void ShowVillageWithAccessPoints(Vector2[] posList, string[] strings, Action[] callbacks, bool needCloseBtn)
    {
        await OpenMapAnimation();

        _callbacks = callbacks;
        for (int i = 0; i < accessPoints.Length; i++)
        {
            accessPoints[i].gameObject.SetActive(false);
            accessPoints[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < accessPoints.Length; i++)
        {
            if(i < posList.Length)
            {
                accessPoints[i].transform.localPosition = posList[i];
                accessPoints[i].GetComponentInChildren<TMP_Text>().text = strings[i];
                int temp = i;
                accessPoints[i].onClick.AddListener(() => {
                    OpenSubPanel();
                    _callbacks[temp]();
                });
               accessPoints[i].gameObject.SetActive(true);
            }
        }

        leaveBtn.gameObject.SetActive(needCloseBtn);
        gameObject.SetActive(true);
    }

    private void OpenSubPanel()
    {
        maskImg.gameObject.SetActive(true);
    }

    public void CloseSubPanel()
    {
        maskImg.gameObject.SetActive(false);
    }

    private async void LeaveVillage()
    {
        await CloseMapAnimation();

        if (_closeCallback != null)
            _closeCallback();

        gameObject.SetActive(false);

    }
}
