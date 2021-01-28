using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioRandom 
{
    public static int RandomAudioClip(int clipLength)
    {
        if (clipLength > 0)
        {
            int clip = Random.Range(0, clipLength);
            return clip;
        }
        else
            return 0;
    }

    /// <summary>
    /// Assigns random Pitch to AudioSource. minPitch is a value between 0.01 - 1. maxPitch is a value between 1.01 - 2. 
    /// </summary>
    public static void RandomAudioPitch(AudioSource myAudioSource, float minPitch, float maxPitch)
    {
        float pitch = Random.Range(minPitch, maxPitch);

        myAudioSource.pitch = pitch;
    }

    /// <summary>
    /// Assigns random Volume to AudioSource. Volume Range is between 0-1f 
    /// </summary>
    public static float RandomAudioVolume(float minVolume, float maxVolume)
    {
        float volume = Random.Range(minVolume, maxVolume);

        return volume;
    }

    public static void ResetAudioSourceValues(AudioSource myAudiosource, bool loop, float volume, float pitch, float spatialBlend, float spread)
    {  
       myAudiosource.loop = loop;        myAudiosource.volume = volume;
       myAudiosource.pitch = pitch;
       myAudiosource.spatialBlend = spatialBlend;
       myAudiosource.spread = spread;
    }
}

