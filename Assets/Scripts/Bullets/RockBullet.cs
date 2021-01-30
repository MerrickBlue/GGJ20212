using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBullet : MonoBehaviour
{
    [Header("This defines which enemies can be damaged with the rock bullet")]
    [SerializeField] protected string flyingEnemiesTag;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == flyingEnemiesTag)
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
