using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System.Collections;

public class InfoPanel : MonoBehaviour
{
    public TMP_Text title, body;
    public PlayableDirector appearTimeline;
    public AudioSource vOSource;
    public AudioClip mirrorClip, shieldClip, intrumentsClip;

    [Space()]
    public AudioSource sfxSource;
    public AudioClip panelAppear, panelDisappear;

    public Transform modelRotator;
    Vector3 originalRotation = new Vector3(0, -145, 0);

    public GameObject[] panels;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);

        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {

    }




    public void ChangeTextMirror()
    {
        
        ControlPanel.Instance.Disappear();

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }


        /*
        title.text = "magic mirror";

        body.text = "Webb’s primary mirror, in the shape of a honeycomb," +
            " is capable of detecting the furthest galaxies in the universe." +
            " The mirror is made of a thin film of gold: chosen for its ability" +
            " to reflect visible and infrared light better than any other material.";
        */

        vOSource.Stop();
        vOSource.clip = mirrorClip;
        vOSource.PlayDelayed(1.5f);

        sfxSource.clip = panelAppear;
        sfxSource.PlayDelayed(3.5f);

        for(int i = 0; i < JWST_Script.Instance.hotspots.Length; i++)
        {
            var timeline = JWST_Script.Instance.hotspots[i].GetComponent<PlayableDirector>();
            timeline.Stop();

            if (i != 0) // remove all except first
            {
                
                StartCoroutine(ReverseTimeline(timeline, true));
            }
        }
        //JWST_Script.Instance.modelAppear.Stop();
        Vector3 newRotation = new Vector3(0, 160, 0);
        JWST_Script.Instance.RotateModel(newRotation);

        StartCoroutine(SpawnPanel(0, 3.5f, panels[0]));
    }

    public void ChangeTextSunshield()
    {
        ControlPanel.Instance.Disappear();

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }


        /*
        title.text = "perfect temp";

        body.text = "A sunshield the size of a tennis court shades the sensitive" +
            " instruments from the intense heat and light of the Sun. With 5 reflective layers," +
            " each as thin as a human hair, the temperature difference between the" +
            " hot and cold sides of the telescope can be as high as 300 degrees Celsius. ";
        */

        vOSource.Stop();
        vOSource.clip = shieldClip;
        vOSource.PlayDelayed(1.5f);

        sfxSource.clip = panelAppear;
        sfxSource.PlayDelayed(3.5f);

        for (int i = 0; i < JWST_Script.Instance.hotspots.Length; i++)
        {
            var timeline = JWST_Script.Instance.hotspots[i].GetComponent<PlayableDirector>();
            timeline.Stop();

            if (i != 1) // remove all except 2nd
            {
                
                StartCoroutine(ReverseTimeline(timeline, true));
            }
        }
        Vector3 newRotation = new Vector3(0, 85, 0);
        JWST_Script.Instance.RotateModel(newRotation);

        StartCoroutine(SpawnPanel(1, 3.5f, panels[3]));
    }

    public void ChangeTextInstruments()
    {
        ControlPanel.Instance.Disappear();

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        
        /*
        title.text = "super sensors";

        body.text = "Webb’s cutting edge instruments reveal the universe in impeccable detail," +
            " far surpassing the capabilities of its predecessors. Four different sensing" +
            " and imaging instruments onboard can detect the faintest of infrared signals." +
            " Near infrared cameras can see beyond the visible spectrum, through cosmic dust" +
            " that can typically obscure our view of distant objects. Mid infrared sensors" +
            " and Spectroscopes can even detect the composition of distant planet’s atmospheres:" +
            " giving us clues as to which could potentially support life. ";
        */

        vOSource.Stop();
        vOSource.clip = intrumentsClip;
        vOSource.PlayDelayed(1.5f);

        sfxSource.clip = panelAppear;
        sfxSource.PlayDelayed(3.5f);

        for (int i = 0; i < JWST_Script.Instance.hotspots.Length; i++)
        {
            var timeline = JWST_Script.Instance.hotspots[i].GetComponent<PlayableDirector>();
            timeline.Stop();

            if (i != 2) // remove all except 3rd
            {
                
                StartCoroutine(ReverseTimeline(timeline, true));
            }
        }
        Vector3 newRotation = new Vector3(0, 290, 0);
        JWST_Script.Instance.RotateModel(newRotation);

        StartCoroutine(SpawnPanel(2, 3.5f, panels[6]));
    }



    public void Close()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        StartCoroutine(ReverseTimeline(appearTimeline, true));

        vOSource.Stop();
        JWST_Script.Instance.ActivateHotspots(2);
        JWST_Script.Instance.ResetRotation();

        sfxSource.clip = panelDisappear;
        sfxSource.Play();

        Invoke("ActivateControlPanel", 1.5f);
    }

    void ActivateControlPanel()
    {
        ControlPanel.Instance.gameObject.SetActive(true);
        ControlPanel.Instance.Appear();
    }

    public void CloseFinal()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        StartCoroutine(ReverseTimeline(appearTimeline, true));

        vOSource.Stop();

        sfxSource.clip = panelDisappear;
        sfxSource.Play();
    }

    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear)
    {

        float dt = (float)timeline.time;

        timeline.Stop();

        timeline.time = dt;

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 2.5f; // (float)timeline.duration;

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


    private IEnumerator SpawnPanel(int hotspotNum, float delay, GameObject _panel)
    {
        yield return new WaitForSeconds(delay);

        _panel.SetActive(true);
        _panel.GetComponent<PlayableDirector>().Play();

        Vector3 position = JWST_Script.Instance.hotspots[hotspotNum].transform.position;
        position.y += 0.05f;

        transform.position = position;


        Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                transform.position.y, Camera.main.transform.position.z);

        transform.LookAt(targetPostition);
    }

}
