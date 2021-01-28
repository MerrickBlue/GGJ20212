using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInstantiate : MonoBehaviour
{
    public AudioClip[] AudioClips;
    public bool RandomPitch;
    public float Volume = 1f;
    public GameObject InstanceASource;

    private GameObject AudioObject;

    public void OnInstatiateFX()
    {
        if (AudioClips != null && AudioClips.Length > 0 && InstanceASource != null)
        {
            AudioObject = Instantiate(InstanceASource);
            AudioObject.transform.position = gameObject.transform.position;
            var audioSource = AudioObject.GetComponent<AudioSource>();

            if (RandomPitch)
                AudioRandom.RandomAudioPitch(audioSource, 0.9f, 1.1f);

            audioSource.clip = AudioClips[AudioRandom.RandomAudioClip(AudioClips.Length)];
            audioSource.volume = Volume;
            audioSource.Play();
        }
    }
}
