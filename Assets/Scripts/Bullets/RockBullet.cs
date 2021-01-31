using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBullet : MonoBehaviour
{
    [Header("This defines which enemies can be damaged with the rock bullet")]
    [SerializeField] protected string flyingEnemiesTag;
    [Header("Audio")]
    public AudioClip[] EBodyDestSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == flyingEnemiesTag)
        {
            Destroy(collision.gameObject);
            //audio
            AudioManager.AudioManag.PlaySFX(AudioManager.AudioManag.UISfx, EBodyDestSFX, 0.8f, 1.1f, 1f, 1f, false);
        }
        if (collision.gameObject.tag != "Player")
        {
            Invoke("DestroyMe", 3);
        }
    }

    protected void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
