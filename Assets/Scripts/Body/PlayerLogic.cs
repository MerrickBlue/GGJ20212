using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Move Settings")]
    public CharacterController controller;
    public float speed = 10f;
    public float jumpHeight = 3f;

    [Space(10)]

    [SerializeField] protected Animator _animator;

    [Space(10)]

    [Header("Gravity Settings")]
    public float gravityForce = -9.18f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float colliderActivatorTimer = 0.1f;

    [Space(10)]

    [Header("Projectile Settings")]
    public GameObject rocks;
    public float shotForce; 
    public Transform shotPoint;

    [Space(10)]

    private float shotForceParabole;
    private Vector3 velocity;
    private bool isGrounded;
    public float timeBeforeDeath;

    [Space(10)]

    //The ammo counter for this player.
    [SerializeField] private int rockCount = 10;

    [Space(10)]

    //Audio
    [Header("Audio")]
    public AudioSource FPSAudioSource;
    public AudioClip[] RockRecollectSFX;
    public AudioClip[] RockThrowSFX;

    // Update is called once per frame
    void Update()
    {

        //Set the parabole shot force to always be half the shot vertical force
        shotForceParabole = shotForce / 2;

        Gravity();
        PlayerMovementLogic();

        RockShooter();
    }

    void PlayerMovementLogic()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        //Get the correct direction to move to
        Vector3 direction = transform.right * xAxis + transform.forward * zAxis;
        
        //Move the player game object * speed and delta time
        controller.Move(direction * speed * Time.deltaTime);

        Jump();

        velocity.y += gravityForce * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        #region this region will hold the command so the Animator knows when the player is moving and when it's not
        if (xAxis != 0 || zAxis != 0)
        {
            _animator.SetBool("Walking", true);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }
        #endregion
    }

    void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }
    }

    //Pickup Rock Trigger
    private void OnTriggerEnter(Collider other)
    {
        //We collide with the rock pickup prefab and collect it if it has the Rock tag
        if (other.gameObject.CompareTag("Rock"))
        {
            //Add a rock to the rock counter
            RockCollector();

            //audio
            AudioManager.AudioManag.PlaySFX(FPSAudioSource, RockRecollectSFX, 0.8f, 1.2f, 0.8f, 1f, false);

            //Destroy parent gameObject when we touch the child trigger
            Destroy(other.transform.parent.gameObject);
        }
    }

    void RockCollector()
    {
        //Add one rock to the rock counter of we have 0 or more rocks
        if (rockCount >= 0)
        {
            rockCount += 1;
        }
    }

    void RockShooter()
    {
        //If we have a at least one rock we can shoot
        //if (rockCount > 0)
        //{
            if (Input.GetButtonDown("Fire1"))
            {
                //Instantiate a bullet rock prefab we define in editor inspector and save the instance in a temp variable
                GameObject tempBullet_Handler;
                tempBullet_Handler = Instantiate(rocks, shotPoint.position, shotPoint.rotation);

                //audio
                AudioManager.AudioManag.PlaySFX(FPSAudioSource, RockThrowSFX, 0.7f, 1.3f, 0.8f, 1f, false);

                //Ignore collisions between our player and the bullet collider at the moment we shoot each bullet
                Physics.IgnoreCollision(tempBullet_Handler.GetComponent<Collider>(), shotPoint.parent.GetComponent<Collider>());

                //I set the velocity of each bullet to 0 at spawn as safe measure to avoid strange behaviour
                tempBullet_Handler.GetComponent<Rigidbody>().velocity = Vector3.zero;

                //Store variable shot X and Z power depending on wheter moving forward or not
                float shotX;
                float shotZ;

                //If we keep the same force on X and Z while moving forward, the projectile lags behing because it has not enough forward force
                //so I added a condition to check if we move forward and if so, I add an extra force multiplier to the bullet

                if (Input.GetAxis("Vertical") > 0)
                {
                    Debug.Log("Boosted Shot");
                    shotX = shotPoint.forward.x * shotForce * 1.5f;
                    shotZ = shotPoint.forward.z * shotForce * 1.5f;
                }
                else
                {
                    Debug.Log("Normal Shot");
                    shotX = shotPoint.forward.x * shotForce;
                    shotZ = shotPoint.forward.z * shotForce;
                }

                //Take the rigid body of the temporal instance var and add a shot force to it in the forward game object axis
                tempBullet_Handler.GetComponent<Rigidbody>().velocity = (new Vector3(shotX, shotForceParabole , shotZ));

                //Destroy te rock bullet after 10 seconds
                Destroy(tempBullet_Handler, 5);
                
                //Substract a rock for each time we shoot
                rockCount -= 1;
            //}
        }
    }
}
