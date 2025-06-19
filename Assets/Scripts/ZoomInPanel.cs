using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using PolySpatial.Samples;

[RequireComponent(typeof(PlayableDirector))]
public class ZoomInPanel : MonoBehaviour
{

    PlayableDirector appearTimeline;
    public PlayableDirector zoomIn;
    public GameObject controlPanel;
    public Transform controlPanelSpawn;
    public SpatialUISlider slider;
    public PlayableDirector disappearTimeline;
    public AudioClip audioClip;
    AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        appearTimeline = GetComponent<PlayableDirector>();
        source = GetComponent<AudioSource>();
        controlPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


    }



    private void Awake()
    {
        
            
    }

    public void PlayAudioClip()
    {
        source.Stop();
        source.PlayOneShot(audioClip);
    }


    public void ClosePanel()
    {
        CancelInvoke();
        StopTimelines();


        StartCoroutine(DoClosePanel());

        if (GetComponentInChildren<DeepFieldHotspots>())
        {
            GetComponentInChildren<DeepFieldHotspots>().Deactivate();
        }
    }

    IEnumerator DoClosePanel()
    {
        controlPanel.SetActive(false);

        SpaceBackground.Instance.LightenBG();
        AudioManager.Instance.ZoomPanelClosed();

        //StartCoroutine(ReverseTimeline(appearTimeline, true));
        disappearTimeline.Play();

        yield return new WaitForSeconds((float)disappearTimeline.duration);

        disappearTimeline.Stop();
        gameObject.SetActive(false);
    }

    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        float dt = (float)timeline.time;

        timeline.Stop();

        timeline.time = dt;

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 3f; // (float)timeline.duration;

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

    public void OpenImage1()
    {
        StopTimelines();
        zoomIn.gameObject.SetActive(true);
        zoomIn.Play();
        Debug.Log("opening img 1");

    }



    public void CloseImage1()
    {
        StopTimelines();

        if (slider != null)
        {
            slider.AnimateToStart();
            Debug.Log("animating slider to start");

            StartCoroutine(ReverseTimeline(zoomIn, true, .5f));
        }
        else
        {
            StartCoroutine(ReverseTimeline(zoomIn, true));
        }

        
    }



    void StopTimelines()
    {
        appearTimeline.Stop();

        if(zoomIn != null)
            zoomIn.Stop();


    }

    public void Appear()
    {
        Invoke("ControlPanelOpen", 3);
        Invoke("StopTimelines", (float)appearTimeline.duration);
    }

    void ControlPanelOpen()
    {
        //Vector3 position = Camera.main.transform.forward * 1f;
        //position.y = Camera.main.transform.position.y - 0.5f;


        //controlPanel.transform.position = position;
        controlPanel.transform.position = controlPanelSpawn.position;
        controlPanel.transform.rotation = controlPanelSpawn.rotation;


        Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                     controlPanel.transform.position.y,
                     Camera.main.transform.position.z);

        //controlPanel.transform.LookAt(targetPostition);

        controlPanel.SetActive(true);

    }

}
