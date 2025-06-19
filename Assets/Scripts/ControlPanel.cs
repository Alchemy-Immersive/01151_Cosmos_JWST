using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance { get; private set; }

    public PlayableDirector animateToSpaceMode;
    PlayableDirector panelAppear;
    public GameObject telescopeMode, spaceMode;
    public GameObject creditsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelAppear = GetComponent<PlayableDirector>();

        telescopeMode.SetActive(true);
        spaceMode.SetActive(false);
        creditsPanel.SetActive(false);

        gameObject.SetActive(false);
    }

    void Awake()
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

    public void Appear()
    {
        panelAppear.Play();
    }

    public void Disappear()
    {
        animateToSpaceMode.Stop();

        if (creditsPanel.activeInHierarchy)
        {
            StartCoroutine(ReverseTimeline(creditsPanel.GetComponent<PlayableDirector>(), true));

        }

        StartCoroutine(ReverseTimeline(panelAppear, true));
    }


    public void SwitchToSpace()
    {
        panelAppear.Stop();

        animateToSpaceMode.Play();


        if (creditsPanel.activeInHierarchy)
        {
            StartCoroutine(ReverseTimeline(creditsPanel.GetComponent<PlayableDirector>(), true));

        }
    }

    public void ReturnToJWST()
    {
        panelAppear.Stop();

        StartCoroutine(ReverseTimeline(animateToSpaceMode, false));


        if (creditsPanel.activeInHierarchy)
        {
            StartCoroutine(ReverseTimeline(creditsPanel.GetComponent<PlayableDirector>(), true));

        }

        Debug.Log("reversing control panel transition");
    }

    public void CloseCredits()
    {

        if (creditsPanel.activeInHierarchy)
        {
            StartCoroutine(ReverseTimeline(creditsPanel.GetComponent<PlayableDirector>(), true));

        }
    }


    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear)
    {

        timeline.time = timeline.duration;
        float dt = (float)timeline.time;

        timeline.Stop();

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 2f; // (float)timeline.duration;

            timeline.time = Mathf.Max(dt, 0);
            timeline.Evaluate();
            yield return null;
        }
        timeline.time = 0;
        timeline.Evaluate();
        //timeline.Stop();

        if (disappear)
        {
            timeline.gameObject.SetActive(false);
        }
    }
}
