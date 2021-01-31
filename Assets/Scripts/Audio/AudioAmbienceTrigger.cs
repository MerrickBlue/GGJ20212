using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbienceTrigger : MonoBehaviour
{
    void Start()
    {
        AudioManager.AudioManag.PlayAmbience(1f);
    }

}
