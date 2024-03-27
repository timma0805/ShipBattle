using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public CardsDB cardDB { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public async Task Int()
    {
        await ParseCardJson();
    }

    private async Task ParseCardJson()
    {
        string json = Resources.Load("JSON/CardJson").ToString();
        cardDB = JsonUtility.FromJson<CardsDB>("{\"cards\":" + json + "}");
    }

    [Serializable]
    public class CardsDB
    {
        public List<CardData> cards;

    }
}


