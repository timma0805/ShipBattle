using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class CoreManager : MonoBehaviour
{
    private InputManager inputManager;

    // Start is called before the first frame update

    private void Awake()
    {
        Init();

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        SetupPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        TestDisplay();
    }

    private async Task Init()
    {
        inputManager = new InputManager();
        inputManager.Init();
    }

    private void SetupPlayer()
    {

    }

    #region Test
    private void TestDisplay()
    {
    }

    #endregion
}
