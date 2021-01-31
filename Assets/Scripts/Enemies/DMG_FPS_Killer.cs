using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMG_FPS_Killer : MonoBehaviour
{
    [SerializeField] protected float timeBeforeKill = 4;
    [SerializeField] protected float killTimer;
    [SerializeField] protected int layerKill = 8;
    [SerializeField] protected bool killing;

    //Audio
    public AudioSource NotifASource;
    public AudioClip[] DamagingPlayerSFX;
    public AudioClip[] PlayerDeathSFX;


    // Update is called once per frame
    void Update()
    {
        if (!killing)
        {
            return;
        }

        if (killTimer > 0)
        {
            killTimer -= Time.deltaTime;
        }
        else
        {
            GC_SoulsSpawner.instance.LoseGame();
            //Audio
            AudioManager.AudioManag.PlaySFX(AudioManager.AudioManag.UISfx, DamagingPlayerSFX, -0.8f, 1.2f, 1f, 1f, true);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerKill)
        {
            killing = true;
            killTimer = timeBeforeKill;
            //Audio
            AudioManager.AudioManag.PlaySFX(NotifASource, DamagingPlayerSFX, -0.8f, 1.2f, 1f, 1f, false);
            Debug.Log("Killing");
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == layerKill)
        {
            if (!killing)
            {
                return;
            }
            killing = false;
            Debug.Log("Not killing");
        }
    }
}
