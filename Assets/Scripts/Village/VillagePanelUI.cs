using System;
using System.Collections;
using System.Collections.Generic;
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

    public void ShowVillageWithAccessPoints(Vector2[] posList, Action[] callbacks)
    {
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
                accessPoints[i].onClick.AddListener(() => callbacks[i]());
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
