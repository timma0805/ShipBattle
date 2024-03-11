using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleMapTile : MonoBehaviour
{
  
    private BattleMapObject attachedObj;
    private int height = 0;
    private Material material;
    //Callback
    private Action<BattleMapTile> selectTileCallback;
    private Action<BattleMapTile> unselectTileCallback;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCallbacks(Action<BattleMapTile> _selectTileCallback,  Action<BattleMapTile> _unselectTileCallback)
    {
        selectTileCallback = _selectTileCallback;
        unselectTileCallback = _unselectTileCallback;
    }

    private void OnMouseEnter()
    {
        selectTileCallback?.Invoke(this);
    }

    private void OnMouseExit()
    {
        unselectTileCallback?.Invoke(this);
    }

    public void ChangeMaterialColor(Color color)
    {
        material.color = color;
    }

}
