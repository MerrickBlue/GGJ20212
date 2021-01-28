using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class takes care of the movements and controls for the soul player.
/// </summary>
public class CTRL_Soul : MonoBehaviour
{    
    [Header("This variables adjust the speed the soul player moves constantly, and the increment it gains while holding the W key. How fast it will be in")]
    [SerializeField] protected float speed = 10.0f;
    [SerializeField] protected float speedIncrementMultiplier = 2f;
    [SerializeField] protected float topSpeedIncrement = 5.0f;
    [SerializeField] protected float rotSpeed = 5.0f;

    [Space(10)]

    [Header("This are the variables that help this character destroy the other souls.")]
    [SerializeField] protected string _enemiesTag;
    [SerializeField] protected float _timeBeforeDestruction;

    //The protected variables needed for the movement implementation for this character.
    protected Rigidbody rb;
    //I'm making this serialized so it's shown in the inspector, after testing this shouldn't be visible to the player.
    [SerializeField] protected float realSpeed;
    //If the player is colliding with another soul we need this boolean to make the attack active.
    [SerializeField] protected bool _collidingWithEnemy;
    //Just a timer to calculate how much time is left needed before destroying the enemy we're touching. After testing we can erase the serialization.
    [SerializeField] protected float _timerEnemy = 2;
    //A reference to keep the last enemy that came in contact with this player.
    protected GameObject _enemy;


    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        realSpeed = speed;
    }

    private void Update()
    {   
        //If the player press down the w key. Then we speed up
        if (Input.GetKey(KeyCode.W))
        {
            //While the player is pressing the W key, this character should go faster and faster until it reaches max speed.
            if (realSpeed < speed + topSpeedIncrement)
            {
                realSpeed += Time.deltaTime * speedIncrementMultiplier;
            }
        }
        else              
        {
            //While the player is not pressing the the W key, this character should go faster and faster until it reaches max speed.
            if (realSpeed > speed)
            {
                realSpeed -= Time.deltaTime * speedIncrementMultiplier;
            }
        }

        //Now we take care of the rotation if the player press A or D
        if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }

        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }    
        
        //Now just in case we clamp the speed
        realSpeed = Mathf.Clamp(realSpeed, speed, speed + topSpeedIncrement);

        //If the player is colliding with an enemy, then the speed should be 0 and then 
        if (_collidingWithEnemy)
        {
            realSpeed = 0;

            if(_timerEnemy > 0)
            {
                _timerEnemy -= Time.deltaTime;
            }
            else
            {
                if (_enemy != null)
                {
                    Destroy(_enemy);
                }

                _collidingWithEnemy = false;            
            }

        }

        //We set the movement.
        Move();
    }

    #region This region holds the function that make the soul move and rotate
    protected void Move()
    {
        rb.velocity = transform.up * realSpeed;
    }

    protected void RotateLeft()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotSpeed);
    }

    protected void RotateRight()
    {
        transform.Rotate(Vector3.forward * -Time.deltaTime * rotSpeed);
    }

    #endregion



    #region This region is the one we'll be using for this character attacks
    //We need to detect collisions with the other souls.
    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == _enemiesTag)
        {
            _collidingWithEnemy = true;
            _enemy = other.gameObject;
            _timerEnemy = _timeBeforeDestruction;
        }
    }

    //Not sure if this is something that is usefull.
    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == _enemiesTag)
        {
            _collidingWithEnemy = false;
        }
    }
    #endregion
}
