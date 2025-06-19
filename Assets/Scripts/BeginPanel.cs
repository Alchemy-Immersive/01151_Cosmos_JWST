using UnityEngine;
using System.Collections;
using UnityEngine.Playables;


public class BeginPanel : MonoBehaviour
{
    public GameObject telescopeParent, spaceParent;
    public Transform spawnPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        telescopeParent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginExperience()
    {
        telescopeParent.transform.position = spawnPos.position;
        telescopeParent.transform.rotation = spawnPos.rotation;

        spaceParent.transform.position = spawnPos.position;
        spaceParent.transform.rotation = spawnPos.rotation;

        telescopeParent.gameObject.SetActive(true);

        JWST_Script.Instance.Begin();

        StartCoroutine(ReverseTimeline(GetComponent<PlayableDirector>(), true));
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
}
