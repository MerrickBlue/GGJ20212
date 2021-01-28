using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestroy : MonoBehaviour
{
    public float TimeForDestroy;

    void Start()
    {
        StartCoroutine(DestroyingAudio());
    }

    IEnumerator DestroyingAudio()
    {
        yield return new WaitForSeconds(TimeForDestroy);
        Destroy(gameObject);
    }
}
