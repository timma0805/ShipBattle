using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleMapController : MonoBehaviour
{
    public struct BattleMapInitInitParameters
    {
        public int mapIndex;
    }
    public struct BattleMapCharactersParameters
    {
        public int playerWidth;
        public int playerLength;
        public GameObject playerPrefab;
        public int[] enemyLengthList;
        public int[] enemyWidthList;
        public GameObject[] enemyPrefabsList;
    }

    [Header("Testing")]
    public bool enableTest = true;
    public Vector2 testTargetPos;
    public FaceDirection testTargetDirection;
    public bool testTargetIsPlayer;

    [SerializeField]
    private GameObject mapPanelObj;
    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject[] enemyPrefabsList;

    //Setting
    private int mapX;
    private  int mapY;
    private const float tileLength = 10.0f;

    private Vector2 playerStartPos;
    private Vector2[] enemyStartPosList;

    //Data
    private BattleMapData mapData;
    private BattleMapTile[] mapTiles;
    private GameObject playerObj;
    private GameObject[] enemyObjs;
    private BattleMapTile currentTile;
    private int playerLength;
    private int playerWidth;
    private Vector2 curPlayerPos;
    private int[] enemyLengthList;
    private int[] enemyWidthList;
    private FaceDirection playerDirection;
    private FaceDirection[] enemyDirectionList;
    private int[] enemyIndexList;
    private Vector2[] curEnemyPosList;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if(enableTest)
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                MoveToTargetTile(testTargetPos,testTargetDirection, testTargetIsPlayer);
            }
        }
    }

    public async Task<int[]> Init(BattleMapInitInitParameters parameters)
    {
        ReadMapData(parameters.mapIndex);
        await GenerateMapTiles();
        return enemyIndexList;
    }

    public async Task SpawnCharacters(BattleMapCharactersParameters parameters)
    {
        playerLength = parameters.playerWidth;
        playerWidth = parameters.playerWidth;
        playerPrefab = parameters.playerPrefab;
        enemyLengthList = parameters.enemyLengthList;
        enemyWidthList = parameters.enemyWidthList;
        enemyPrefabsList = parameters.enemyPrefabsList;

        SpawnPlayer();
        SpawnEnemy();
    }

    private void ReadMapData(int mapIndex)
    {
        //Read Data From BattleMapDataListSO
        mapData = new BattleMapData();

        mapX = mapData.Width;
        mapY = mapData.Length;
        playerStartPos = mapData.playerSpawnPos;
        playerDirection = mapData.playerSpawnDirection;
        enemyStartPosList = mapData.enemySpawnPosList;
        enemyDirectionList = mapData.enemySpawnDirectionList;
        enemyIndexList = mapData.enemyIndexList;
    }

    private async Task GenerateMapTiles()
    {
        mapTiles = new BattleMapTile[mapX * mapY];

        if (tilePrefab == null)
        {
            Debug.LogError("tilePrefab null");
            return;
        }

        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                BattleMapTile newTile = GameObject.Instantiate(tilePrefab, mapPanelObj.transform).GetComponent< BattleMapTile>();
                newTile.GetComponent<Renderer>().material = new Material(newTile.GetComponent<Renderer>().material);
                newTile.transform.localPosition = new Vector3(i * tileLength, 0, j * tileLength);
                newTile.name = "mapTile" + (i* mapX + j);
                newTile.AddCallbacks(SelectTile, UnSelectTile);
                mapTiles[i* mapX + j] = newTile;
            }
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("SpawnPlayer playerShipPrefab null");
            return;
        }
        Debug.Log("SpawnPlayer");
        playerObj = GameObject.Instantiate(playerPrefab, mapPanelObj.transform);
        MoveToTargetTile(playerStartPos, playerDirection, true);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabsList == null)
        {
            Debug.LogError("SpawnEnemy enemyPrefabsList null");
            return;
        }

        enemyObjs = new GameObject[enemyPrefabsList.Length];
        curEnemyPosList = new Vector2[enemyPrefabsList.Length];

        Debug.Log("SpawnEnemy" + enemyObjs.Length);

        for (int i = 0; i < enemyPrefabsList.Length; i++)
        {
            GameObject enemyObj = GameObject.Instantiate(enemyPrefabsList[i], mapPanelObj.transform);
            enemyObjs[i] = enemyObj;
            MoveToTargetTile(enemyStartPosList[i], enemyDirectionList[i], false);
        }
    }

    private void MoveToTargetTile(Vector2 pos, FaceDirection direction,  bool isPlayer, int enemyIndex = 0)
    {
        int X = (int)pos.x;
        int Y = (int)pos.y;
        int targetLength = isPlayer ? playerLength : enemyLengthList[enemyIndex];
        int targetWidth = isPlayer ? playerWidth : enemyWidthList[enemyIndex];
        GameObject targetObj = isPlayer ? playerObj : enemyObjs[enemyIndex];

        (X,Y) = MakeMovePossible(X, Y, direction, targetWidth, targetLength);
        Debug.Log($"MoveToTargetTile X{X} Y{Y}");

        int targetIndex = X * mapX + Y;
        if(targetIndex > mapTiles.Length)
        {
            Debug.LogError($"TargetIndex Out Range X{X} Y{Y}");
            return;
        }

        var targetTile = mapTiles[targetIndex];
        var targetTilePosition = new Vector3(targetTile.transform.localPosition.x, targetObj.transform.localPosition.y, targetTile.transform.localPosition.z);

        Debug.Log("targetTilePosition" + targetTilePosition);

        if (direction == FaceDirection.Front)
        {
            if (targetWidth > 1 && targetWidth % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x + tileLength / 2.0f, targetTilePosition.y, targetTilePosition.z);
            }
            if (targetLength > 1 && targetLength % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x , targetTilePosition.y, targetTilePosition.z + tileLength / 2.0f);
            }
        }
        else if (direction == FaceDirection.Back)
        {
            if (targetWidth > 1 && targetWidth % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x - tileLength / 2.0f, targetTilePosition.y, targetTilePosition.z );
            }
            if (targetLength > 1 && targetLength % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x, targetTilePosition.y, targetTilePosition.z - tileLength / 2.0f);
            }
        }
        else if (direction == FaceDirection.Right)
        {
            if (targetWidth > 1 && targetWidth % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x, targetTilePosition.y, targetTilePosition.z - tileLength / 2.0f);
            }
            if (targetLength > 1 && targetLength % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x - tileLength / 2.0f, targetTilePosition.y, targetTilePosition.z );
            }
        }
        else if (direction == FaceDirection.Left)
        {
            if (targetWidth > 1 && targetWidth % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x, targetTilePosition.y, targetTilePosition.z + tileLength / 2.0f);
            }
            if (targetLength > 1 && targetLength % 2 == 0)
            {
                targetTilePosition = new Vector3(targetTilePosition.x + tileLength / 2.0f, targetTilePosition.y, targetTilePosition.z );
            }
        }

        //Move to target position
        targetObj.transform.localPosition = targetTilePosition;

        // Rotate the targetObj by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(0, 90.0f * (int)direction, 0);
        targetObj.transform.localRotation = target;

        //Save pos
        if (isPlayer)
            curPlayerPos = pos;
        else
            curEnemyPosList[enemyIndex] = pos;


    }

    private (int, int) MakeMovePossible(int X, int Y, FaceDirection direction, int targetWidth, int targetLength)
    {
        Debug.Log($"MakeMovePossible: X{X}, Y{Y}, direction{direction}, width{targetWidth}, length{targetLength}");
        if (direction == FaceDirection.Front )
        {
            if (X + targetWidth / 2 > mapX-1)
                X = mapX - targetWidth / 2-1;
            else if(X - targetWidth / 2 < 0)
                X = 0;

            if (Y + targetLength / 2 > mapY - 1)
                Y = mapY - targetLength / 2 - 1;
            else if (Y - targetLength / 2 < 0)
                Y = 0;
        }
        else if (direction == FaceDirection.Back)
        {
            if (X + targetWidth / 2 > mapX - 1)
                X = mapX - targetWidth / 2 - 1;
            else if (X - targetWidth / 2 < 0)
                X = targetWidth / 2;

            if (Y + targetLength / 2 > mapY - 1)
                Y = mapY - targetLength / 2 - 1;
            else if (Y - targetLength / 2 < 0)
                Y = targetLength/2;
        }
        else if (direction == FaceDirection.Right || direction == FaceDirection.Left)
        {
            if (X + targetLength / 2 > mapX-1)
                X = mapX - targetLength / 2-1;
            else if (X - targetLength / 2 < 0)
                X = targetLength / 2;

            if (Y + targetWidth / 2 > mapY-1)
                Y = mapY - targetWidth / 2-1;
            else if (Y - targetWidth / 2 < 0)
                Y = targetWidth / 2;
        }
        return (X,Y);
    }

    private void SelectTile(BattleMapTile tile)
    {
        if (currentTile == tile)
            return;

         currentTile = tile;
    }
    private void UnSelectTile(BattleMapTile tile)
    {
        if (currentTile != tile)
            return;

        currentTile = null;
    }

    public BattleMapTile GetCurrentSelectedTitle()
    {
        return currentTile;
    }

    public GameObject GetPlayerObj()
    {
        return playerObj;
    }

    public GameObject GetEnemyObj(int index)
    {
        return enemyObjs[index];
    }

    public GameObject[] GetEnemyObjs()
    {
        return enemyObjs;
    }

    public Vector2 GetPlayerCurrentPos()
    {
        return curPlayerPos;
    }

    public Vector2[] GetAllEnemyCurrentPos()
    {
        return curEnemyPosList;
    }

    public Vector2 GetEnemyCurrentPos(int index)
    {
        return curEnemyPosList[index];
    }

    public FaceDirection GetPlayerFaceDiretion()
    {
        return playerDirection;
    }

    public FaceDirection[] GetAllEnemyFaceDirection()
    {
        return enemyDirectionList;
    }

    public FaceDirection GetEnemyFaceDirection(int index)
    {
        return enemyDirectionList[index];
    }

    public void ShowCardEffectWithTitlesColor(Vector2 startPos, int rangeX, int rangeY, FaceDirection direction, Color color)
    {
        if(playerDirection == FaceDirection.Front)
        {
            ChangeTitlesColor(startPos, rangeX, rangeY, direction, color);
        }
        else if (playerDirection == FaceDirection.Back)
        {
            if(direction == FaceDirection.Front)
                ChangeTitlesColor(startPos, rangeX, rangeY, FaceDirection.Back, color);
            else if (direction == FaceDirection.Back)
                ChangeTitlesColor(startPos, rangeX, rangeY, FaceDirection.Front, color);
            else if (direction == FaceDirection.Right)
                ChangeTitlesColor(startPos, rangeX, rangeY, FaceDirection.Left, color);
            else
                ChangeTitlesColor(startPos, rangeX, rangeY, FaceDirection.Right, color);
        }
        else if (playerDirection == FaceDirection.Right)
        {
            if (direction == FaceDirection.Front)
                ChangeTitlesColor(startPos, rangeX, rangeY, direction, color);
            else if (direction == FaceDirection.Back)
                ChangeTitlesColor(startPos, rangeX, rangeY, direction, color);
            else if (direction == FaceDirection.Right)
                ChangeTitlesColor(startPos, rangeX, rangeY, direction, color);
            else
                ChangeTitlesColor(startPos, rangeX, rangeY, direction, color);
        }

    }

    public void ChangeTitlesColor(Vector2 startPos, int rangeX, int rangeY, FaceDirection direction, Color color)
    {
        int startX = -1;
        int endX = -1;
        int startY = -1;
        int endY = -1;

        if (direction == FaceDirection.Front)
        {
            startX = Mathf.RoundToInt(startPos.x - rangeX / 2);
            endX = startX + rangeX;

            startY = (int)startPos.y;
            endY = startY + rangeY;

        }
        else if(direction == FaceDirection.Back)
        {
            startX = Mathf.RoundToInt(startPos.x - rangeX / 2);
            endX = startX + rangeX;

            startY = (int)startPos.y - rangeY;
            endY = (int)startPos.y;
        }
        else if (direction == FaceDirection.Right)
        {
            startX = (int)startPos.x;
            endX = startX + rangeX;

            startY = Mathf.RoundToInt(startPos.y - rangeY / 2);
            endY = startY + rangeY;
        }
        else if (direction == FaceDirection.Left)
        {
            startX = (int)startPos.x - rangeX;
            endX = (int)startPos.x;

            startY = Mathf.RoundToInt(startPos.y - rangeY / 2);
            endY = startY + rangeY;
        }

        startX = startX < 0 ? 0 : startX;
        endX = endX > mapX ? mapX : endX;
        startY = startY < 0 ? 0 : startY;
        endY = endY > mapY ? mapY : endY;

        for ( int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                mapTiles[x * mapX + y].ChangeMaterialColor(color);
            }
        }
    }

    public void ResetMapTitlesColor()
    {
        for(int i = 0; i < mapTiles.Length; i++)
        {
            mapTiles[i].ChangeMaterialColor(Color.white);
        }
    }
}
