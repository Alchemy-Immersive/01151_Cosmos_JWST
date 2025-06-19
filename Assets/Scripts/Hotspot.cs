using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class Hotspot : MonoBehaviour
{
    public PlayableDirector hotspotAppear, hotspotToClose;
    public GameObject innercircle;
    public float appearDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hotspotToClose.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnEnable()
    {
        hotspotAppear.time = 0;
        hotspotAppear.Play();
        //Invoke("Appear", appearDelay);
        StartCoroutine(Appear(appearDelay));
    }

    IEnumerator Appear(float _appearDelay)
    {
        yield return new WaitForSeconds(0.01f);
        hotspotAppear.Pause();
        yield return new WaitForSeconds(_appearDelay);
        innercircle.SetActive(true);
        hotspotAppear.Play();

        StartCoroutine(StopTimeline((float)hotspotAppear.duration, hotspotAppear));
    }


    IEnumerator StopTimeline(float delay, PlayableDirector timeline)
    {
        yield return new WaitForSeconds(delay);

        timeline.Stop();
    }

    public void TransitionHotspot()
    {
        hotspotToClose.Play();
        
    }

    public void ReverseTransition()
    {
        StartCoroutine(ReverseTimeline(hotspotToClose, true));

    }

    public void Disappear()
    {
        hotspotToClose.Stop();
        hotspotToClose.gameObject.SetActive(false);
        hotspotAppear.time = 0;
        hotspotAppear.Evaluate();

        innercircle.SetActive(false);
        StartCoroutine(ReverseTimeline(hotspotAppear, true));
    }



    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear)
    {
        float dt = (float)timeline.time;

        timeline.Stop();

        timeline.time = dt;

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 1f; // (float)timeline.duration;

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
