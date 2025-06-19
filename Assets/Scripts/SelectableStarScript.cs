using UnityEngine;
using System.Collections;
using PolySpatial.Template;
using Unity.PolySpatial;
using UnityEngine.Playables;

public class SelectableStarScript : MonoBehaviour
{
    public Transform starParent;
    public ZoomInPanel zoomInPanel;
    public VisionOSHoverEffect hoverEffect;
    public SpatialUIButton button;

    [HideInInspector]
    public bool selected;

    public PlayableDirector starAppear;
    [HideInInspector]
    public static bool panelOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected = false;
        starAppear = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.enabled = true;
        }

        if (hoverEffect != null)
        {
            hoverEffect.enabled = true;
        }
    }


    public void OpenPanel()
    {
        if (panelOpen)
        {
            return;
        }
        panelOpen = true;

        selected = true;

        SpaceBackground.Instance.DarkenBG(3);
        ControlPanel.Instance.Disappear();

        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.Normalize();

        zoomInPanel.transform.position = Camera.main.transform.position + direction * 5;




        zoomInPanel.gameObject.SetActive(true);
        zoomInPanel.Appear();
        //gameObject.SetActive(false);

        if(button != null)
        {
            button.enabled = false;
        }

        if(hoverEffect != null)
        {
            hoverEffect.enabled = false;
        }
        Debug.Log("panel activated");


        
    }


    public void Disappear()
    {
        StartCoroutine(ReverseTimeline(starAppear, true));
    }

    public void Appear()
    {
        starAppear.Play();

        panelOpen = false;
    }


    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear)
    {

        float dt = (float)timeline.time;

        timeline.Stop();

        timeline.time = dt;

        while (dt > 0.1f)
        {
            dt -= Time.deltaTime * 2f; // (float)timeline.duration;

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
