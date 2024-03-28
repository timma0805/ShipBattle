using Newtonsoft.Json;
using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    
    public async Task Init()
    {
        await ParseCardJson();
    }

    private async Task ParseCardJson()
    {
        try
        {
            string json = Resources.Load("JSON/CardJson").ToString();
            cardDB = JsonConvert.DeserializeObject<CardsDB>("{\"cards\":" + json + "}");
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    [Serializable]
    public class CardsDB
    {
        public List<CardData> cards;

    }
}


