using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Move Settings")]
    public CharacterController controller;
    public float speed = 10f;
    public float jumpHeight = 3f;

    [Header("Gravity Settings")]
    public float gravityForce = -9.18f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Projectile Settings")]
    public GameObject rocks;
    public Vector3 shotForce;
    public Transform shotPoint;

    private Vector3 velocity;
    private bool isGrounded;
    //The ammo counter for this player.
    [SerializeField] private int rockCount = 10;

    // Update is called once per frame
    void Update()
    {
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
        if (rockCount > 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //Instantiate a bullet rock prefab we define in editor inspector and save the instance in a temp variable
                GameObject tempBullet_Handler;
                tempBullet_Handler = Instantiate(rocks, shotPoint.position, Quaternion.identity);

                //Take the rigid body of the temporal instance var and add a shot force to it in the forward game object axis
                tempBullet_Handler.GetComponent<Rigidbody>().AddForce(shotForce);
                
                //Substract a rock for each time we shoot
                rockCount -= 1;
            }
        }
    }
}
