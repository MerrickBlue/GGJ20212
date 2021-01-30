using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioMixer MusicMixer;

    public AudioSource MusicIntro; //these are childed gameobjects with audiosources and clips set. audiosources on loop (except Win and Death)
    public AudioSource MusicGame;
    public AudioSource MusicWin;
    public AudioSource MusicDeath;


    [Header("SFX")]
    public AudioMixer SFXMixer;

    public AudioSource GenAmbience; //same as music
    public AudioClip[] UISfxClips;
    public AudioSource UISfx;

    //Singleton
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

    #region SFX Control
    public void PlaySFX(AudioSource sfxSource, AudioClip[] sfxClips, float minPitch, float maxPitch, float minVol, float maxVol, bool oneAtTime)
    {
        var clip = AudioRandom.RandomAudioClip(sfxClips.Length);

        AudioRandom.RandomAudioPitch(sfxSource, minPitch, maxPitch);
        sfxSource.volume = AudioRandom.RandomAudioVolume(minVol, maxVol);

        if (oneAtTime)
        {
            if (!sfxSource.isPlaying)
                sfxSource.PlayOneShot(sfxClips[clip]);
        }
        else
        {
            sfxSource.PlayOneShot(sfxClips[clip]);
        }
    }

    public void PlayUISFX(int index)
    {
        switch(index)
        {
            case 0: 
                UISfx.PlayOneShot(UISfxClips[0]);
                break;
            case 1:
                UISfx.PlayOneShot(UISfxClips[1]);
                break;
            case 2:
                UISfx.PlayOneShot(UISfxClips[2]);
                break;
        }
    }

    public void StopSFX(AudioSource sfxSource)
    {
        sfxSource.Stop();
    }
    #endregion

    #region Music Control

    public void StartMusic()
    {
        if (!MusicIntro.isPlaying && !MusicGame.isPlaying)
        {
            MusicIntro.Play();
            MusicGame.Play();
        }
    }

    public void FadeInIntro(float duration)
    {
        StartCoroutine(AudioFade.MixerFadeIn(MusicMixer, "IV", duration));
        StartCoroutine(AudioFade.MixerFadeOut(MusicMixer, "GV", duration));
    }

    public void FadeInGame(float duration)
    {
        StartCoroutine(AudioFade.MixerFadeIn(MusicMixer, "GV", duration));
    }

    public void PlayWinMusic(float duration) //Should have good ducking on mixer
    {
        if (!MusicWin.isPlaying)
        {
            MusicWin.Play();
            StartCoroutine(AudioFade.MixerFadeIn(MusicMixer, "WV", duration));
            Invoke("FadeOutWinMus", MusicWin.clip.length + 0.2f);
        }
    }

    void FadeOutWinMus()
    {
        StartCoroutine(AudioFade.MixerFadeOut(MusicMixer, "WV", 0.2f));
    }

    public void PlayDeathMusic(float duration) //Should have good ducking on mixer
    {
        if (!MusicDeath.isPlaying)
        {
            MusicDeath.Play();
            StartCoroutine(AudioFade.MixerFadeIn(MusicMixer, "DV", duration));
            Invoke("FadeOutDeathMus", MusicDeath.clip.length + 0.2f);
        }
    }

    void FadeOutDeathMus()
    {
        StartCoroutine(AudioFade.MixerFadeOut(MusicMixer, "DV", 0.2f));
    }

    public void PauseMus()
    {
        MusicIntro.Pause();
        MusicGame.Pause();
        MusicWin.Pause();
        MusicDeath.Pause();
    }

    public void UnPauseMus()
    {
        MusicIntro.UnPause();
        MusicGame.UnPause();
        MusicWin.UnPause();
        MusicDeath.UnPause();
    }

    public void StopMus()
    {
        MusicIntro.Stop();
        MusicGame.Stop();
        MusicWin.Stop();
        MusicDeath.Stop();
    }

    #endregion

    #region Ambience Control

    public void PlayAmbience(float duration)
    {
        if (!GenAmbience.isPlaying)
        {
            GenAmbience.Play();
            StartCoroutine(AudioFade.MixerFadeIn(SFXMixer, "AV", duration));
        }
    }

    public void StopAmbience(float duration)
    {
        if (GenAmbience.isPlaying)
        {
            StartCoroutine(AudioFade.MixerFadeOut(SFXMixer, "AV", duration));
            Invoke("StoppingAmbience", duration + 0.1f);
        }
    }

    void StoppingAmbience()
    {
        GenAmbience.Stop();
    }
    #endregion

    
    #region Testing
    /*
    [Header("Test Tools")]
    public bool PlayMusic;
    public bool CueIntro;
    public bool CueGame;
    public bool CueWin;
    public bool CueDeath;
    public bool PauseM;
    public bool UnPauseM;
    public bool StopM;
    public bool SFXtest;
    public AudioClip[] TestSFX;
    public AudioSource TestASource;
    public bool OneTime;
    public bool StopSound;
    public bool CueAmb;
    public bool StopAmb;

    void Update()
    {
        if (PlayMusic)
        {
            StartMusic();
            PlayMusic = false;
        }
        if (CueIntro)
        {
            FadeInIntro(1f);
            CueIntro = false;
        }
        if (CueGame)
        {
            FadeInGame(1f);
            CueGame = false;
        }
        if (CueWin)
        {
            PlayWinMusic(1f);
            CueWin = false;
        }
        if (CueDeath)
        {
            PlayDeathMusic(1f);
            CueDeath = false;
        }
        if (PauseM)
        {
            PauseMus();
            PauseM = false;
        }
        if (UnPauseM)
        {
            UnPauseMus();
            UnPauseM = false;
        }
        if (StopM)
        {
            StopMus();
            StopM = false;
        }
        if (SFXtest)
        {
            if (OneTime)
            {
                PlaySFX(TestASource, TestSFX, 0.7f, 1.3f, 0.8f, 1f, true);
            }
            else
            {
                PlaySFX(TestASource, TestSFX, 1f, 1f, 1f, 1f, false);
            }

            SFXtest = false;
        }
        if (StopSound)
        {
            StopSFX(TestASource);
        }
        if (CueAmb)
        {
            PlayAmbience(1f);
            CueAmb = false;
        }
        if (StopAmb)
        {
            StopAmbience(1f);
            StopAmb = false;
        }

    }
    */
    #endregion
    
}
