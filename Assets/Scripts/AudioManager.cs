using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Intro")]
    public AudioSource introSource;
    public AudioSource introSourceLoop;

    [Header("Telescope")]
    public AudioSource telescopeSource;
    public AudioSource telescopeSourceLoop;
    //public AudioClip telescopeIntro, telescopeLoop;

    [Header("Gaia")]
    public AudioSource gaiaBgSource;
    public AudioSource gaiaBgSourceLoop;

    [Header("Pillars of Creation")]
    public AudioSource pocBgSource;
    public AudioSource pocBgSourceLoop;
    //public AudioClip pocIntro, pocLoop;

    [Header("Deep Field")]
    public AudioSource deepfieldBgSource;
    public AudioSource deepfieldBgSourceLoop;
    //public AudioClip deepfieldIntro, deepfieldLoop;

    [Header("Wolf Rayet")]
    public AudioSource wolfRayetBgSource;
    public AudioSource wolfRayetBgSourceLoop;

    [Header("Southern Ring Nebula")]
    public AudioSource srnBgSource;
    public AudioSource srnBgSourceLoop;
    //public AudioClip srnIntro, srnLoop;

    [Header("Penguin Galaxy")]
    public AudioSource penguinGBgSource;
    public AudioSource penguinGBgSourceLoop;

    [Header("Final VO")]
    public AudioSource finalVOsource;
    bool pocPlayed, srnPlayed, deepFieldPlayed, penguinGplayed, wolfRPlayed, galacticNPlayed, protostarPlayed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IntroTitleStart();

        pocPlayed = srnPlayed = deepFieldPlayed = penguinGplayed = wolfRPlayed = galacticNPlayed = protostarPlayed = false;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IntroTitleStart()
    {

        Debug.Log("intro music playing");

        introSourceLoop.volume = 0;
        introSourceLoop.loop = true;
        introSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(introSourceLoop, .4f, 3.0f));
    }

    void IntroQuiet()
    {
        StartCoroutine(AudioFade.FadeAudioSource(introSource, .4f, 3.0f));
        
    }


    public void BeginPressed()
    {
        CancelInvoke();

        StartCoroutine(AudioFade.FadeAudioSource(introSourceLoop, 0f, 2.0f, IntroFinished));
        void IntroFinished()
        {
            introSourceLoop.Stop();
        }

        introSource.volume = 0;
        introSource.loop = false;
        introSource.PlayDelayed(1);

        StartCoroutine(AudioFade.FadeAudioSource(introSource, 1.0f, 2.0f)); //fade in intro sound
        Debug.Log("title music playing");

        Invoke("BeginPart2", introSource.clip.length - 6);
    }

    void BeginPart2()
    {
        StartCoroutine(AudioFade.FadeAudioSource(introSource, 0f, 3.0f, IntroFinished));
        void IntroFinished()
        {
            introSource.Stop();
            Debug.Log("intro source stopped");
        }
        Debug.Log("telescope music playing");

        telescopeSource.volume = 0;
        telescopeSource.loop = false;
        //telescopeSource.clip = telescopeIntro;
        telescopeSource.Play();

        StartCoroutine(AudioFade.FadeAudioSource(telescopeSource, 1f, 3.0f, FadedIn)); //fade in telescope track
        void FadedIn()
        {
            Debug.Log("telescope source faded in");
        }
        Invoke("TelescopeLooping", telescopeSource.clip.length - 0.1f);
    }

    void TelescopeLooping()
    {
        
        Debug.Log("telescope source looping");
        //telescopeSource.clip = telescopeLoop;
        telescopeSourceLoop.loop = true;
        telescopeSourceLoop.volume = 1;
        telescopeSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(telescopeSourceLoop, 0.5f, 2.0f)); //quiet telescope music.
        telescopeSource.Stop();
    }

    public void GaiaAppear()
    {
        CancelInvoke();

        StartCoroutine(AudioFade.FadeAudioSource(telescopeSource, 0f, 2.0f));// fade off intro
        StartCoroutine(AudioFade.FadeAudioSource(telescopeSourceLoop, 0f, 2.0f, TelescopeFinished));// fade off intro
        void TelescopeFinished()
        {
            telescopeSource.Stop();
            telescopeSourceLoop.Stop();
        }
        Debug.Log("gaia music playing");

        gaiaBgSource.volume = 0;
        gaiaBgSource.loop = false;
        gaiaBgSource.Play();
        StartCoroutine(AudioFade.FadeAudioSource(gaiaBgSource, 1f, 2.0f, GaiaFadedIn)); //fade in gaia track
        void GaiaFadedIn()
        {
            Invoke("GaiaQuiet", 4);
        }

        gaiaBgSourceLoop.volume = 1;
        gaiaBgSourceLoop.loop = true;
        gaiaBgSourceLoop.PlayDelayed(gaiaBgSource.clip.length);
    }

    void GaiaQuiet()
    {

        StartCoroutine(AudioFade.FadeAudioSource(gaiaBgSource, 0.5f, 3.0f)); //fade in gaia track
        StartCoroutine(AudioFade.FadeAudioSource(gaiaBgSourceLoop, 0.5f, 3.0f));
    }

    void GaiaSilent()
    {
        StartCoroutine(AudioFade.FadeAudioSource(gaiaBgSource, 0.02f, 2.0f)); //fade in gaia track
        StartCoroutine(AudioFade.FadeAudioSource(gaiaBgSourceLoop, 0.02f, 2.0f));
    }

    public void ReturnToJWST()
    {
        CancelInvoke();
        finalVOsource.Stop();

        GaiaSilent();
        Debug.Log("telescope playing again");
        telescopeSourceLoop.volume = 0;

        telescopeSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(telescopeSourceLoop, 0.5f, 3.0f)); //fade in telescope track

    }


    public void POC_Open()
    {
        pocPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("pillars of creation music playing");
        pocBgSource.volume = 0;
        pocBgSource.loop = false;
        pocBgSource.Play();
        StartCoroutine(AudioFade.FadeAudioSource(pocBgSource, 1f, 2.0f)); //fade in POC track
        Invoke("POC_Loop", pocBgSource.clip.length);
    }

    void POC_Loop()
    {
        pocBgSourceLoop.volume = 1;
        pocBgSourceLoop.loop = true;
        pocBgSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(pocBgSourceLoop, 0.6f, 3.0f)); //quiet POC music.
    }

    public void SRN_Open()
    {
        srnPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("southern ring nebula music playing");
        srnBgSource.volume = 0;
        srnBgSource.loop = false;
        srnBgSource.Play();
        StartCoroutine(AudioFade.FadeAudioSource(srnBgSource, 1f, 2.0f)); //fade in SRN track
        Invoke("SRN_Loop", srnBgSource.clip.length);
    }

    void SRN_Loop()
    {
        srnBgSourceLoop.volume = 1;
        srnBgSourceLoop.loop = true;
        srnBgSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(srnBgSourceLoop, 0.6f, 3.0f)); //quiet POC music.
    }

    public void PG_Open()
    {
        penguinGplayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("penguin galaxy music playing");
        //penguinGBgSource.volume = 0;
        //penguinGBgSource.loop = false;
        //penguinGBgSource.Play();
        //StartCoroutine(AudioFade.FadeAudioSource(penguinGBgSource, 1f, 2.0f)); //fade in PG track
        //Invoke("PG_Loop", penguinGBgSource.clip.length);
    }

    void PG_Loop()
    {
        penguinGBgSourceLoop.volume = 1;
        penguinGBgSourceLoop.loop = true;
        penguinGBgSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(penguinGBgSourceLoop, 0.6f, 3.0f)); //quiet PG music.
    }

    public void DeepField_Open()
    {
        deepFieldPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("deep field music playing");
        deepfieldBgSource.volume = 0;
        deepfieldBgSource.loop = false;
        deepfieldBgSource.Play();
        StartCoroutine(AudioFade.FadeAudioSource(deepfieldBgSource, 1f, 2.0f)); //fade in SRN track
        Invoke("DeepField_Loop", deepfieldBgSource.clip.length);
    }

    void DeepField_Loop()
    {
        deepfieldBgSourceLoop.volume = 1;
        deepfieldBgSourceLoop.loop = true;
        deepfieldBgSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(deepfieldBgSourceLoop, 0.6f, 3.0f)); //quiet POC music.
    }
    
    public void WolfRayet_Open()
    {
        wolfRPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("wolf rayet music playing");
        //wolfRayetBgSource.volume = 0;
        //wolfRayetBgSource.loop = false;
        //wolfRayetBgSource.Play();
       // StartCoroutine(AudioFade.FadeAudioSource(wolfRayetBgSource, 1f, 2.0f)); //fade in WR track
        //Invoke("WolfRayet_Loop", wolfRayetBgSource.clip.length);
    }

    void WolfRayet_Loop()
    {
        wolfRayetBgSourceLoop.volume = 1;
        wolfRayetBgSourceLoop.loop = true;
        wolfRayetBgSourceLoop.Play();
        StartCoroutine(AudioFade.FadeAudioSource(wolfRayetBgSourceLoop, 0.6f, 3.0f)); //quiet WR music.
    }

    public void ActiveGalacticNucleus_Open()
    {
        galacticNPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("active galactic nucleus music playing");
    }

    public void Protostar_Open()
    {
        protostarPlayed = true;

        CancelInvoke();
        GaiaSilent();
        Debug.Log("protostar music playing");
    }


    public void ZoomPanelClosed()
    {
        CancelInvoke();

        Invoke("GaiaQuiet", 1);

        Debug.Log("panel music stopping");
        if (pocBgSource.isPlaying || pocBgSourceLoop.isPlaying)
        {
            StartCoroutine(AudioFade.FadeAudioSource(pocBgSource, 0f, 3.0f, ZoomFadeComplete)); //quiet POC music.
            StartCoroutine(AudioFade.FadeAudioSource(pocBgSourceLoop, 0f, 3.0f, ZoomFadeComplete));
        }
        if (srnBgSource.isPlaying || srnBgSourceLoop.isPlaying)
        {
            StartCoroutine(AudioFade.FadeAudioSource(srnBgSource, 0f, 3.0f, ZoomFadeComplete)); //quiet POC music.
            StartCoroutine(AudioFade.FadeAudioSource(srnBgSourceLoop, 0f, 3.0f, ZoomFadeComplete));
        }
        if (deepfieldBgSource.isPlaying || deepfieldBgSourceLoop.isPlaying)
        {
            StartCoroutine(AudioFade.FadeAudioSource(deepfieldBgSource, 0f, 3.0f, ZoomFadeComplete)); //quiet POC music.
            StartCoroutine(AudioFade.FadeAudioSource(deepfieldBgSourceLoop, 0f, 3.0f, ZoomFadeComplete));
        }

        void ZoomFadeComplete()
        {
            pocBgSource.Stop();
            srnBgSource.Stop();
            deepfieldBgSource.Stop();

            pocBgSourceLoop.Stop();
            srnBgSourceLoop.Stop();
            deepfieldBgSourceLoop.Stop();
        }

        if (pocPlayed && srnPlayed && deepFieldPlayed && wolfRPlayed & penguinGplayed & galacticNPlayed & protostarPlayed)
        {
            Invoke("PlayFinalVO", 3);
        }
    }

    void PlayFinalVO()
    {
        finalVOsource.Play();

        pocPlayed = srnPlayed = deepFieldPlayed = penguinGplayed = wolfRPlayed = galacticNPlayed = protostarPlayed = false;
    }

}
