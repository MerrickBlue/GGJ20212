using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _audioManag;

    public static AudioManager AudioManag { get { return _audioManag; } }

    private void Awake()
    {
        if (_audioManag != null && _audioManag != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _audioManag = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
       
    }

    
    void Update()
    {
        
    }
}
