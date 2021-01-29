using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class shall control both when the enemies spawn close to each player, the chances of which enemy spawns and tells the rest of the game when is won, and how close each character is to each other.
/// </summary>
public class GC_SoulsSpawner : MonoBehaviour
{
    [HideInInspector] public static GC_SoulsSpawner instance;

    //The Soul enemy prefab.
    [SerializeField] protected GameObject soulEnemies;
    [SerializeField] protected GameObject bodyEnemies;

    //This is the starting time between spawns for the interval between spawns.
    [SerializeField] protected float timeBetweenSpawns = 5;
    //How much enemies will be spawn every cycle of spawning.
    [SerializeField] protected int spawnsEveryCycle = 2;

    //This is the soulPlayer
    public CTRL_Soul soulPlayer;
    //This should actually be the body/FPS player
    public PlayerLogic bodyPlayer;

    //This variable can be used by the HUD controller in order to display how far away the players are from each other.
    [HideInInspector] public float distanceBetweenPlayers;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //If the soul player hasn't been assigned via the inspector we find it.
        if (soulPlayer == null)
        {
            soulPlayer = GameObject.FindObjectOfType<CTRL_Soul>();
        }

        //If the body hasn't been found we look for it in the scene.
        /*if (bodyPlayer == null)
        {
            bodyPlayer = GameObject.FindObjectOfType<CTRL_Body>();
        }*/

        //This coroutine should be trigger by the start game function. But for testing purposes I'm calling it here.
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistanceBetweenPlayers();
    }

    //Every frame we calculate the distance between players to pass it on to the HUD.
    protected void CalculateDistanceBetweenPlayers()
    {
        distanceBetweenPlayers = Vector3.Distance(soulPlayer.transform.position, bodyPlayer.transform.position);
    }

    #region Inside this reagion I'm containing the logic to spawn enemies close to both players.
    //This coroutines will loop from the beginning of the game, until the player's win or loose the game. What it does is spawning two enemies close to each player
    //The type of enemy is given by a 75% chance that the enemy that spawns near one player, is the one that's actually dangerous.
    protected IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        //Every cycle we spawn two enemies near both players.
        for (int i = 0; i < spawnsEveryCycle; i++)
        {
            SpawnNearBodyPlayer();
            SpawnNearSoulPlayer();

            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(SpawnEnemies());
    }

    //Spawn an enemy near Soul Player
    protected void SpawnNearSoulPlayer()
    {
        var v2 =  new Vector2(soulPlayer.transform.position.x, soulPlayer.transform.position.z) +  Random.insideUnitCircle.normalized * Random.Range(5, 10);
        Vector3 ret = new Vector3(v2.x, this.transform.position.y, v2.y);

        //First we determine the type of enemy that will appear. Most chances should be that the enemy instantiated is the one that might harm this player.
        if (Random.value > 0.75f)
        {
            Instantiate(soulEnemies, new Vector3(v2.x, soulPlayer.transform.position.y, v2.y), Quaternion.identity);
        }
        else
        {
            Instantiate(bodyEnemies, new Vector3(v2.x, soulPlayer.transform.position.y, v2.y), Quaternion.identity);
        }
    }

    //Spawn an enemy near the body Player
    protected void SpawnNearBodyPlayer()
    {
        var v2 = new Vector2(bodyPlayer.transform.position.x, bodyPlayer.transform.position.z) + Random.insideUnitCircle.normalized * Random.Range(5, 10);
        Vector3 ret = new Vector3(v2.x, this.transform.position.y, v2.y);

        //First we determine the type of enemy that will appear. Most chances should be that the enemy instantiated is the one that might harm this player.
        if (Random.value > 0.75f)
        {
            Instantiate(bodyEnemies, new Vector3(v2.x, bodyPlayer.transform.position.y, v2.y), Quaternion.identity);
        }
        else
        {
            Instantiate(soulEnemies, new Vector3(v2.x, bodyPlayer.transform.position.y, v2.y), Quaternion.identity);
        }
    }
    #endregion

    #region The region for the Start, win and lose functions.
    public void StartGame()
    {

        /*For now, I don't think we need to spawn the players, it's easier if they always start at the same position for the time being.
        //First we choose random positions from the list we've provided.
        var randomPosition = Random.Range(0, 4);

        //If for some bizarr reason the first random position happened to be outside the list range, we set it to 3.
        if (randomPosition >= 4)
        {
            randomPosition = 3;
        }
        //Now we set the second position from the list.
        var secondRandomPosition = randomPosition + 1;
        
        //Again if the position is outside the lists range we set it to be the last
        if (secondRandomPosition >= 4)
        {
            secondRandomPosition = 0;
        }

        Instantiate(soulPlayerPrefab, spawnPositions[randomPosition].position, Quaternion.identity);
        Instantiate(bodyPlayerPrefab, spawnPositions[secondRandomPosition].position, Quaternion.identity);*/


        //When the game starts we need to lock and disable the mouse cursor so the FPS player can have more control over his own camera.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Finally we start spawning enemies.
        StartCoroutine(SpawnEnemies());
    }

    //if one of the player is killed then this function is called so the Menu pop ups in order to let the players start again.
    public void LoseGame()
    {
        //Before going into the main menu, there should be a You Lose sign.
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        Debug.Log("Game Lost");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }    
    #endregion
}
