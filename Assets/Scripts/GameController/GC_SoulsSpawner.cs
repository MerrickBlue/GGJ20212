using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class shall control both when the enemies spawn close to each player, the chances of which enemy spawns and tells the rest of the game when is won, and how close each character is to each other.
/// </summary>
public class GC_SoulsSpawner : MonoBehaviour
{
    public static GC_SoulsSpawner instance;

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
    //public CTRL_Soul bodyPlayer;

    [HideInInspector] public float distanceBetweenPlayers;
    
    [HideInInspector] public float distanceBetweenPlayersToDeactivateRadar;

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
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistanceBetweenPlayers();

    }

    protected IEnumerator SpawnEnemuies()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        //Every cycle we spawn two enemies near both players.
        for (int i = 0; i < spawnsEveryCycle; i++)
        {
            SpawnNearBodyPlayer();
            SpawnNearSoulPlayer();

            yield return new WaitForSeconds(0.5f);
        }
    }

    //Every frame we calculate the distance between players to pass it on to the HUD.
    protected void CalculateDistanceBetweenPlayers()
    {
        //distanceBetweenPlayers = Vector3.Distance(soulPlayer.transform.position, bodyPlayer.Transform.Position);
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
        /*var v2 = new Vector2(bodyPlayer.transform.position.x, bodyPlayer.transform.position.z) + Random.insideUnitCircle.normalized * Random.Range(5, 10);
        Vector3 ret = new Vector3(v2.x, this.transform.position.y, v2.y);

        //First we determine the type of enemy that will appear. Most chances should be that the enemy instantiated is the one that might harm this player.
        if (Random.value > 0.75f)
        {
            Instantiate(bodyEnemies, new Vector3(v2.x, bodyPlayer.transform.position.y, v2.y), Quaternion.identity);
        }
        else
        {
            Instantiate(soulEnemies, new Vector3(v2.x, bodyPlayer.transform.position.y, v2.y), Quaternion.identity);
        }*/
    }
}
