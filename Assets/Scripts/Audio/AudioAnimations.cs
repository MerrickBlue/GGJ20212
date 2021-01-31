using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnimations : MonoBehaviour
{
    public AudioSource CharASource;
    public AudioSource VocASource;
    public AudioClip[] FootstepsSFX;
    public AudioClip[] EnemyStepSFX;
    public AudioClip[] IddleVOC;
    public bool isEnemy;
    

    public void SFXFootstep()
    {
        if (isEnemy)
        {
            CharASource.PlayOneShot(EnemyStepSFX[AudioRandom.RandomAudioClip(EnemyStepSFX.Length)]);
        }
        else
        {
            CharASource.PlayOneShot(FootstepsSFX[AudioRandom.RandomAudioClip(FootstepsSFX.Length)]);
        }
    }

    public void VOCIddleEnemy()
    {
        if (!VocASource.isPlaying && isEnemy)
            VocASource.PlayOneShot(IddleVOC[AudioRandom.RandomAudioClip(IddleVOC.Length)]);
    }
}
