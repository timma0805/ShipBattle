using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillagePanelUI : MonoBehaviour
{
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Button[] accessPoints;
    [SerializeField]
    private Button leaveBtn;

    private Action[] _callbacks;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(Action endCallback)
    {
        leaveBtn.onClick.RemoveAllListeners();

        if(endCallback != null ) 
            leaveBtn.onClick.AddListener(() => endCallback());
    }

    public void ShowVillageWithAccessPoints(Vector2[] posList, string[] strings, Action[] callbacks)
    {
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

    private void LeaveVillage()
    {
        gameObject.SetActive(false);
    }
}
