using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class SpaceBackground : MonoBehaviour
{
    public static SpaceBackground Instance { get; private set; }

    Material spaceMat;

    public bool deactivateOnStart;

    //public GameObject ControlPanel;
    public SelectableStarScript[] hotspots;
    public ParticleSystem meteors;

    [SerializeField] private Transform pinchHelperParent;
    [SerializeField] private Transform pinchViz;
    [SerializeField] private Transform rotator;
    [SerializeField] private float rotationMultiplier = 10f;
    [SerializeField] private bool useAccelerationMode = false;
    private float currentAccelerationX, currentAccelerationY, currentAccelerationZ = 0f;
    [Range(0.01f, 2f)]
    [SerializeField] private float accelerationMultiplier = .1f;
    [Range(0f, 1f)]
    [SerializeField] private float accelerationReductionLerp = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spaceMat = GetComponent<Renderer>().sharedMaterial;

        spaceMat.SetFloat("_Exposure", 1);

        if (deactivateOnStart)
        {
            foreach (SelectableStarScript hotspot in hotspots)
            {

                hotspot.gameObject.SetActive(false);

            }
            gameObject.SetActive(false);
        }
        
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

    private void FixedUpdate()
    {
        if (useAccelerationMode)
        {
            currentAccelerationX = Mathf.Lerp(currentAccelerationX, 0f, accelerationReductionLerp);
            rotator.transform.Rotate(Vector3.up, currentAccelerationX, Space.Self);

            currentAccelerationY = Mathf.Lerp(currentAccelerationY, 0f, accelerationReductionLerp);
            rotator.transform.Rotate(Vector3.right, -currentAccelerationY, Space.Self);

            //currentAccelerationZ = Mathf.Lerp(currentAccelerationZ, 0f, accelerationReductionLerp);
            //transform.Rotate(Vector3.forward, currentAccelerationZ, Space.Self);

            
        }
    }


    public void Select(bool _newPinch, Vector3 interactorPos)
    {

        if (_newPinch)
        {
            this.transform.SetParent(null);

            pinchHelperParent.position = interactorPos;
            pinchHelperParent.rotation = Camera.main.transform.rotation;
            pinchViz.transform.localPosition = Vector3.zero;

            rotator.rotation = Camera.main.transform.rotation;
            this.transform.SetParent(rotator, true);
        }

        Vector3 lastVizLocalPos = pinchViz.transform.localPosition;
        pinchViz.transform.position = interactorPos;

        float localDeltaX = (pinchViz.transform.localPosition.x - lastVizLocalPos.x) * rotationMultiplier;
        float localDeltaY = (pinchViz.transform.localPosition.y - lastVizLocalPos.y) * rotationMultiplier;
        //float localDeltaZ = (pinchViz.transform.localPosition.z - lastVizLocalPos.z) * rotationMultiplier;

        //Vector3 newRotation = new Vector3(transform.localEulerAngles.x + localDeltaY, transform.localEulerAngles.y + localDeltaX,
            //transform.localEulerAngles.z - localDeltaZ);


        if (useAccelerationMode)
        {
            currentAccelerationX += (localDeltaX * accelerationMultiplier);
            currentAccelerationY += (localDeltaY * accelerationMultiplier);
            //currentAccelerationZ += (localDeltaZ * accelerationMultiplier);

        }



        //transform.localRotation = Quaternion.Euler(newRotation);

        rotator.transform.Rotate(Vector3.up, localDeltaX, Space.Self);
        //transform.Rotate(Vector3.forward, localDeltaZ, Space.Self);
        rotator.transform.Rotate(Vector3.right, -localDeltaY, Space.Self);
    }

    public void Deselect()
    {
        //this.transform.parent = null;
    }



    public void DarkenBG(float duration = 3)
    {
        DeactivateHotspots();

        StartCoroutine(DoMaterialTransition(duration, "_Exposure", 0.08f, spaceMat, 1));
        Debug.Log("darkening bg");
        //GetComponent<Collider>().enabled = false;

        //Invoke("DeactivateHotspots", 1);
        
    }

    public void LightenBG()
    {
        bool allSelected = true;
        foreach(SelectableStarScript star in hotspots)
        {
            if (!star.selected)
            {
                allSelected = false;
            }
        }
        if (!allSelected)
        {
            StartCoroutine(DoMaterialTransition(3, "_Exposure", 1, spaceMat));
            Debug.Log("lightening bg");
            //GetComponent<Collider>().enabled = true;

            ActivateHotspots();
        }
        else
        {
            StartCoroutine(DoMaterialTransition(3, "_Exposure", 1, spaceMat));
            Debug.Log("re-activate hotspots delayed");
            Invoke("ActivateHotspots", 28);

            meteors.Play();

            foreach (SelectableStarScript star in hotspots)
            {
                star.selected = false;
            }
        }
        
    }

    public void ActivateHotspots()
    {
        meteors.Stop();

        foreach (SelectableStarScript hotspot in hotspots)
        {
            hotspot.enabled = true;
            hotspot.gameObject.SetActive(true);
            //hotspot.selected = false;
            hotspot.Appear();
        }

        if (!ControlPanel.Instance.gameObject.activeInHierarchy)
        {
            ControlPanel.Instance.gameObject.SetActive(true);
            ControlPanel.Instance.Appear();
        }


    }


    public void DeactivateHotspots()
    {
        //GetComponent<PlayableDirector>().Stop();

        foreach (SelectableStarScript hotspot in hotspots)
        {

            hotspot.Disappear();

        }

        //ControlPanel.Instance.Disappear();
    }


    IEnumerator DoMaterialTransition(float duration, string valueString, float targetValue, Material mat, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        float t = 0;
        float startValue = mat.GetFloat(valueString);
        float value = 0;

        while (t <= duration)
        {
            t += Time.deltaTime;
            var percent = Mathf.Clamp01(t / duration);

            value = Mathf.Lerp(startValue, targetValue, percent);

            mat.SetFloat(valueString, value);

            yield return null;
        }


    }

}
