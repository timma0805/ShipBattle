using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class CrewCardUI : MonoBehaviour
{
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Image characterImg;
    [SerializeField]
    private TMP_Text nameTxt;
    [SerializeField]
    private TMP_Text typeTxt;

    private Button clickBtn;
    private CharacterData _characterData;
    private Action<CharacterData> _callback;

    private void Awake()
    {
        clickBtn = GetComponent<Button>();
        clickBtn.onClick.AddListener(onClick);
    }

    public void UpdateCharacter(CharacterData characterData, Action<CharacterData> callback)
    {
        _characterData = characterData;

        nameTxt.text = characterData.Name;
        typeTxt.text = characterData.Type.ToString();

        _callback = callback;
    }

    private void onClick()
    {
        if (_callback != null)
            _callback(_characterData);
    }
}
