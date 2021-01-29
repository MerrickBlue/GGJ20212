using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This enemy is supposed to appear near both players, if possible outside of their view
/// </summary>
public class AI_Enemy_Souls : MonoBehaviour
{
    [Header("The speed this character is gonna have both patrolling and attacking")]
    //The speed this character is moving when patroling.
    [SerializeField] protected float patrolSpeed;
    //The speed this character takes when it has seen the FPS player and is moving towards attacking it.
    [SerializeField] protected float maxAttackSpeed;
    //This is the character's acceleration. Meaning the increments that takes him from 0 to the maximum speed depending on which action is taking.
    [SerializeField] protected float acceleration;

    [Space(10)]

    [Header("Here we define the variables to see the FPS player.")]
    [SerializeField] protected float viewRadius;
    [SerializeField] protected LayerMask _FPSPlayerLayer;



    //The private variables needed to control this enemy.
    //The rigidbody to move the enemy's body arround.
    protected Rigidbody rb;

    //A boolean to when the enemy sees the player he can "Kill" and react appropietly.
    [SerializeField] protected bool killablePlayerOnSight;
    protected Transform FPSPlayer;

    //The real movement when attacking and patrolling, that way the character doesn't go to full speed in a frame.
    protected float movingSpeed = 0;

    //This boolena will be checked by the player by the time it's killing this character. That way we can stop moving.
    [HideInInspector] public bool beingKilled;

    #region Inside this region I'm keeping every variable needed for this character to patrol
    //Most of this variables can later be hidden in the inspector. For now I'm serializing them in order to be able to modify them in real time.
    //How short this character can stay away from the actual target before stopping and changing direction.
    [SerializeField] protected float distanceToTargetThreshold;
    [SerializeField] protected float distanceToTarget;

    //Every time this character reaches it's destination it acquires a new one. Eventually this could be hidden from inspector as it'll never be changed manually.
    [SerializeField] protected Vector3 _patrolDestination;
    
    //This is how long it'l take between reaching the patrol point destination and rotate towards.
    [SerializeField] protected float changeDistanceReaction;
    protected float changeDistanceTimer;

    //This is how fast this character can rotate. I'm not sure if rotating will make any sense.
    [SerializeField] protected float rotationSpeed;

    [SerializeField] protected bool obstacleInFront;
    [SerializeField] protected LayerMask _obstaclesLayer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //We get the rigidbody from this same object.
        rb = this.GetComponent<Rigidbody>();

        //It's likely that this caracter doesn't spawn directly in a position where it sees the FPS player, so it must decide where to go first.
        _patrolDestination = DefinePatrolTarget();
        //We already set the timer to the waiting time set in the inspector.
        changeDistanceTimer = changeDistanceReaction;
    }

    // Update is called once per frame
    void Update()
    {
        //If this character is being killed then we 
        if (beingKilled)
        {
            movingSpeed = 0;
            return;
        }
        //If there's no killable player on sight we check for it and we patrol around.
        if (!killablePlayerOnSight)
        {
            CheckForFPSPlayer();
            CalculateDistance();

            //If we are still not close enough to that patrolling point we move towards it.
            if (distanceToTarget > distanceToTargetThreshold && !obstacleInFront)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, _patrolDestination, patrolSpeed * Time.deltaTime);
            }
            //Once this character reached the next point, we should wait a momento and then define a new destination point.
            else
            {
                //We start the waiting period.
                if (changeDistanceTimer > 0)
                {
                    changeDistanceTimer -= Time.deltaTime;
                }
                else
                {
                    //It's likely that this caracter doesn't spawn directly in a position where it sees the FPS player, so it must decide where to go first.
                    _patrolDestination = DefinePatrolTarget();
                    //We already set the timer to the waiting time set in the inspector.
                    changeDistanceTimer = changeDistanceReaction;
                }
            }

            //At this point we check for walls, if one is about to get in the way, the player will find a new point to travel to.
            CheckForWalls();
        }
        //If the player is on sight this character moves towards it.
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(FPSPlayer.position.x, this.transform.position.y, FPSPlayer.position.z), maxAttackSpeed * Time.deltaTime);
        }
    }

    protected void CheckForWalls()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, _patrolDestination - this.transform.position, out hit, 5, _obstaclesLayer))
        {
            Debug.Log("Obstacle in front");
            //changeDistanceTimer = changeDistanceReaction;
            //It's likely that this caracter doesn't spawn directly in a position where it sees the FPS player, so it must decide where to go first.
            obstacleInFront = true;
        }
        else
        {
            obstacleInFront = false;
        }
    }

    //This function calculates and stores the distance between this character and the point to where it's patroling.
    protected void CalculateDistance()
    {
        distanceToTarget = Vector3.Distance(this.transform.position, _patrolDestination);
    }

    //This function should define a Vector towards where this character shall move.
    protected Vector3 DefinePatrolTarget()
    {     
        var v2 = (Random.insideUnitCircle.normalized * Random.Range(5, 10) + new Vector2 (this.transform.position.x, this.transform.position.z));

        Vector3 ret = new Vector3 (v2.x, this.transform.position.y, v2.y);

        return ret;
    }

    protected void CheckForFPSPlayer()
    {
        Collider[] player = Physics.OverlapSphere(new Vector3(this.transform.position.x, 0, this.transform.position.z), viewRadius, _FPSPlayerLayer);

        if (player.Length > 0)
        {
            if (player[0] != null)
            {
                killablePlayerOnSight = true;
                FPSPlayer = player[0].transform;
            }
        }
    }

    #region The region that holds the functions that will tell this character that is being held hostage by the soul player
    public void AttachToSoulPlayer()
    {
        rb.isKinematic = true;
    }

    public void DetachFromSoulPlayer()
    {
        rb.isKinematic = true;
    }
    #endregion

    //Temporal function to know where this character is looking at to check for walls
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(_patrolDestination - this.transform.position);
        Gizmos.DrawRay(transform.position, direction);
    }
}
