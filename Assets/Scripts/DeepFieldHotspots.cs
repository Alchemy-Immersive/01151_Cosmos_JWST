using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using PolySpatial.Samples;



public class DeepFieldHotspots : MonoBehaviour
{
    public Hotspot[] hotspots;
    public PlayableDirector diffractionTimeline, redshiftTimeline, lensingTimeline;

    public AudioSource source;
    public AudioClip diffraction, redshift, lensing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diffractionTimeline.gameObject.SetActive(false);
        redshiftTimeline.gameObject.SetActive(false);
        lensingTimeline.gameObject.SetActive(false);

        foreach (Hotspot hotspot in hotspots)
        {
            hotspot.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateHotspots(float delay = 0)
    {
        StartCoroutine(DoActivateHotspots(delay));

    }

    IEnumerator DoActivateHotspots(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        foreach (Hotspot hotspot in hotspots)
        {
            if (!hotspot.gameObject.activeInHierarchy)
            {
                hotspot.gameObject.SetActive(true);
            }
        }
    }

    public void PlayDiffraction()
    {
        source.Stop();
        source.PlayOneShot(diffraction);

        diffractionTimeline.gameObject.SetActive(true);
        diffractionTimeline.Play();

        diffractionTimeline.GetComponent<AudioSource>().enabled = true;
        diffractionTimeline.GetComponent<AudioSource>().mute = false;
        
        //Invoke("StopTimelines", (float)diffractionTimeline.duration);
    }

    public void PlayRedshift()
    {
        source.Stop();
        source.PlayOneShot(redshift);

        redshiftTimeline.gameObject.SetActive(true);
        redshiftTimeline.Play();

        redshiftTimeline.GetComponent<AudioSource>().enabled = true;
        redshiftTimeline.GetComponent<AudioSource>().mute = false;
        
        //Invoke("StopTimelines", (float)redshiftTimeline.duration);
    }

    public void PlayLensing()
    {
        source.Stop();
        source.PlayOneShot(lensing);

        lensingTimeline.gameObject.SetActive(true);
        lensingTimeline.Play();

        lensingTimeline.GetComponent<AudioSource>().enabled = true;
        lensingTimeline.GetComponent<AudioSource>().mute = false;
        
        //Invoke("StopTimelines", (float)lensingTimeline.duration);
    }

    public void CloseDiffraction()
    {
        source.Stop();
        diffractionTimeline.GetComponent<AudioSource>().mute = true;
        diffractionTimeline.GetComponent<AudioSource>().enabled = false;

        StartCoroutine(ReverseTimeline(diffractionTimeline, true));
    }

    public void CloseRedshift()
    {
        source.Stop();
        redshiftTimeline.GetComponent<AudioSource>().mute = true;
        redshiftTimeline.GetComponent<AudioSource>().enabled = false;

        StartCoroutine(ReverseTimeline(redshiftTimeline, true));
    }

    public void CloseLensing()
    {
        source.Stop();
        lensingTimeline.GetComponent<AudioSource>().mute = true;
        lensingTimeline.GetComponent<AudioSource>().enabled = false;

        StartCoroutine(ReverseTimeline(lensingTimeline, true));
    }

    void StopTimelines()
    {
        diffractionTimeline.Stop();
        redshiftTimeline.Stop();
        lensingTimeline.Stop();
    }

    public void Deactivate()
    {
        Debug.Log("deacivating deep field hotspots");

        foreach(Hotspot hotspot in hotspots)
        {
            hotspot.Disappear();
        }
        source.Stop();
        diffractionTimeline.Stop();
        redshiftTimeline.Stop();
        lensingTimeline.Stop();
        diffractionTimeline.gameObject.SetActive(false);
        redshiftTimeline.gameObject.SetActive(false);
        lensingTimeline.gameObject.SetActive(false);
    }


    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        float dt = (float)timeline.time;

        timeline.Stop();

        timeline.time = dt;
        

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 5f; // (float)timeline.duration;

            timeline.time = Mathf.Max(dt, 0);
            timeline.Evaluate();
            yield return null;
        }
        timeline.time = 0;
        timeline.Evaluate();
        timeline.Stop();

        if (disappear)
        {
            timeline.gameObject.SetActive(false);
        }
    }
}
