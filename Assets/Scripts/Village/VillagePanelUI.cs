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
    private Sprite[] openBGSprites;
    [SerializeField]
    private Sprite[] closeBGSprites;

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
    private const float moveSpeed = 5f;
    private const float moveThreshold = 25f; // Threshold distance from screen edge to start moving


    // Start is called before the first frame update
    private void Awake()
    {
        bgRectTransform = bgImg.GetComponent<RectTransform>();
        mapMaskEndPadding = bgImg.GetComponent<RectTransform>().rect.width/2;
        mapMaskPaddingGap = (mapMaskEndPadding - mapMaskStartPadding) / (openBGSprites.Length - dummyMapSpriteCount);
        leaveBtn.onClick.AddListener(LeaveVillage);
    }

    void Update()
    {
        // Get the mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Get the screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Check if mouse is near the screen edge
        if (mousePosition.x <= moveThreshold && bgRectTransform.localPosition.x < 500.0f) // Right edge
        {
            MoveBGImage(Vector2.right);
        }
        else if (mousePosition.x >= screenWidth - moveThreshold && bgRectTransform.localPosition.x > -500.0f) // Left edge
        {
            MoveBGImage(Vector2.left);
        }
        else if (mousePosition.y <= moveThreshold && bgRectTransform.localPosition.y < 550.0f) // Top edge
        {
            MoveBGImage(Vector2.up);
        }
        else if (mousePosition.y >= screenHeight - moveThreshold && bgRectTransform.localPosition.y > -370) // Bottom edge
        {
            MoveBGImage(Vector2.down);
        }
    }

    // Function to move the BG image in a specified direction
    private void MoveBGImage(Vector2 direction)
    {
        // Move the image using its RectTransform
        bgRectTransform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }


    public void Init(Action closeCallback)
    {
        _closeCallback = closeCallback;
    }

    private async Task OpenMapAnimation()
    {
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
    }

    private async Task CloseMapAnimation()
    {
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
    }

    public async void ShowVillageWithAccessPoints(Vector2[] posList, string[] strings, Action[] callbacks)
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
                    _callbacks[temp]();
                });
               accessPoints[i].gameObject.SetActive(true);
            }
        }

        gameObject.SetActive(true);
    }

    private async void LeaveVillage()
    {
        await CloseMapAnimation();

        if (_closeCallback != null)
            _closeCallback();

        gameObject.SetActive(false);

    }
}
