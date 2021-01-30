using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Enemy_Body : MonoBehaviour
{
    [Header("Health setup for this enemy")]
    public float enemyHealth = 100f;

    [Header("The speed this character is gonna have both patrolling and attacking")]
    [SerializeField] protected float patrolSpeed = 100f;
    [SerializeField] protected float maxAttackSpeed;
    [SerializeField] protected float acceleration;

    [Header("Player sight setup variables")]
    [SerializeField] protected float viewRadius;
    [SerializeField] protected LayerMask _SoulPlayerLayer;
    [SerializeField] protected bool haveEverDetected = false;

    //This enemy's rigid body variables
    protected Rigidbody rb;

    [SerializeField] protected bool killableSoulPlayerOnSight;

    //This is how far the player must be for this character to give up on pursuing it.
    [SerializeField] protected float giveUpDistance = 10;
    //After giving up on a chase, we use the following variables to give this character a small cooldown. This way it's safer for the player to escape.
    protected bool cooldown;
    protected float cooldownTimer = 0;
    //Player transform reference
    protected Transform SoulPlayer;

    //The real movement when attacking and patrolling, that way the character doesn't go to full speed in a frame.
    protected float movingSpeed = 0;

    #region AI Patrol variables

    [SerializeField] protected float distanceToTargetThreshold;
    [SerializeField] protected float distanceToTarget;

    //Every time this character reaches it's destination it acquires a new one. Eventually this could be hidden from inspector as it'll never be changed manually.
    [SerializeField] protected Vector3 _patrolDestination;

    //This is how long it'll take between reaching the patrol point destination and rotate towards.
    [SerializeField] protected float changeDistanceReaction;
    [SerializeField] protected float backToPatrolTimer = 3f;
    protected float changeDistanceTimer;

    //Probably not needed 
    [SerializeField] protected float rotationSpeed;

    [SerializeField] protected bool obstacleInFront;
    [SerializeField] protected LayerMask _obstaclesLayer;

    #endregion

    private void Start()
    {
        //Take our own rigid body
        rb = this.GetComponent<Rigidbody>();

        //It's likely that this caracter doesn't spawn directly in a position where it sees the FPS player, so it must decide where to go first.
        _patrolDestination = DefinePatrolTarget();

        //We already set the timer to the waiting time set in the inspector.
        changeDistanceTimer = changeDistanceReaction;
    }

    private void Update()
    {
        #region this section holds the cooldown for when this character has been chasing the player it can kill and can now 
        if (cooldown)
        {
            //The cooldown  time was set below at the end of the update function as it's when this character calculates if the player's gone too far away.
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                cooldown = false;
            }

            //Since we're on cooldown we exit this function.
            return;
        }
        #endregion

        BodyAI_Sight();
    }

    void BodyAI_Sight()
    {
        if (!killableSoulPlayerOnSight)
        {
            CheckForSoulPlayer();
            CalculateDistance();
            
            //if the stored distance to our patrol point is greater than the distance threshold and we dont have an obstacel in front, then move to patrol point
            if (distanceToTarget > distanceToTargetThreshold && !obstacleInFront)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, _patrolDestination, patrolSpeed * Time.deltaTime);
            }
            //Once this character reached its patrol point, we should wait a moment and then define a new destination point.
            else
            {
                if (changeDistanceTimer > 0)
                {
                    changeDistanceTimer -= Time.deltaTime;
                }
                else
                {
                    _patrolDestination = DefinePatrolTarget();
                    //Set the disntance timer to the waiting time set in the inspector
                    changeDistanceTimer = changeDistanceReaction;
                }
            }
            //Check for wall before we move to the next nav point
            CheckForWalls();
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(SoulPlayer.position.x, this.transform.position.y, SoulPlayer.position.z), maxAttackSpeed * Time.deltaTime);


            //Since we are chasing the FPS player at this moment, there should always be a player assigned as a variable, but still it doesn't hurt to check again.
            if (SoulPlayer != null)
            {
                //We create a point as if this character were flying so we could meassure the distance between that point and the player.
                Vector3 _aerealPosition = new Vector3(this.transform.position.x, SoulPlayer.position.y, this.transform.position.z);
                var distanceFromTarget = Vector3.Distance(_aerealPosition, SoulPlayer.position);
                //If the player has gone farther away from this character than the give up distance set via the inspector, then this class shoudl give up.
                if (distanceFromTarget >= giveUpDistance)
                {
                    //First we set the player on sight boollean to false.
                    killableSoulPlayerOnSight = false;
                    //We start a cooldown.
                    cooldownTimer = 4;
                    cooldown = true;
                    //Just in case we also tell this character to forget about the player
                    SoulPlayer = null;
                }
            }
        }
    }

    //This function calculates and stores the distance between this character and the point to where it's patroling.
    protected void CalculateDistance()
    {
        distanceToTarget = Vector3.Distance(this.transform.position, _patrolDestination);
    }
    
    //Function to get the soulPlayer transform player if it is valid and set killable target as true
    protected void CheckForSoulPlayer()
    {
        Collider[] soulPlayer = Physics.OverlapSphere(new Vector3(this.transform.position.x, this.transform.position.x, this.transform.position.z), viewRadius, _SoulPlayerLayer);
        if (soulPlayer.Length > 0)
        {
            if (soulPlayer[0] != null)
            {
                Debug.Log("Soul Player Close");
                killableSoulPlayerOnSight = true;
                haveEverDetected = true;
                SoulPlayer = soulPlayer[0].transform;
            }
        }
        else if (soulPlayer.Length <= 0)
        {
            Debug.Log("No Soul player close");
            killableSoulPlayerOnSight = false;
        }
    }

    protected void CheckForWalls()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, _patrolDestination - this.transform.position, out hit, 5, _obstaclesLayer))
        {
            Debug.Log("Ground Obstacle found");
            obstacleInFront = true;
        }
        else
        {
            obstacleInFront = false;
        }
    }

    protected Vector3 DefinePatrolTarget()
    {
        var v2 = (Random.insideUnitCircle.normalized * Random.Range(5, 10) + new Vector2(this.transform.position.x, this.transform.position.z));

        Vector3 ret = new Vector3(v2.x, this.transform.position.y, v2.y);

        return ret;
    }

    //Damage function to call on the FPS enemy from the relevant script
    public void FPS_EnemyDamage(float damageAmmount)
    {
        enemyHealth -= damageAmmount;
    }

    //Temporal function to know where this character is looking at to check for walls
    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, viewRadius);

        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(_patrolDestination - this.transform.position);
        Gizmos.DrawRay(transform.position, direction);
    }
}
