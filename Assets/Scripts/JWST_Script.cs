using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.Playables;
using PolySpatial.Template;



public class JWST_Script : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    public static JWST_Script Instance { get; private set; }

    public InfoPanel infoPanel;
    public GameObject[] hotspots;
    public PlayableDirector titleAppear, modelAppear;
    public PlayableDirector jwstDisappear;
    public Transform model;
    public AnimationCurve m_AnimationCurve;
    AudioSource source;
    public AudioClip introCTA;
    Vector3 startingEuler, targetEuler;
    
    bool rotatedToStart = true;
    bool rotating;
    public float returnRotateTimer = 3;
    public Vector3 launchRotation;
    float rotateTimer;


    [Header("Rotation")]
    [SerializeField] private bool useAccelerationMode = false;
    private float currentAcceleration = 0f;
    [Range(0.01f, 2f)]
    [SerializeField] private float accelerationMultiplier = .1f;
    [Range(0f, 1f)]
    [SerializeField] private float accelerationReductionLerp = 0.1f;
    [SerializeField] private float rotationMultiplier = 0.1f;
    [SerializeField] private Transform pinchHelperParent;
    [SerializeField] private Transform pinchViz;

    [Header("Audio")]
    [SerializeField] private AudioSource tableRotateAudioSource;
    private float rotationSpeed;
    private float lastYaw;
    [SerializeField] private float maxRotationSpeed = 360f; // Adjust based on expected max speed
    [SerializeField] private float volumeSmoothingSpeed = 5f; // Controls how quickly volume changes
    [SerializeField] private float rotationSpeedDecayRate = 5f; // Controls how quickly rotation speed decays when not rotating
    [SerializeField] private Vector2 volumeRange;
    float currentvolume;
    public AudioSource rotateWhooshSource;

    XRHandSubsystem m_HandSubsystem;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();

        GetHandSubsystem();

        startingEuler = model.transform.localEulerAngles;
        targetEuler = startingEuler;

        foreach (GameObject hotspot in hotspots)
        {
            hotspot.SetActive(false);
        }

        //jwstDisappear = GetComponent<PlayableDirector>();

        // Audio Setup
        tableRotateAudioSource.volume = 0;
        tableRotateAudioSource.loop = true;
        tableRotateAudioSource.Play(); // Play muted since we are controlling the volume based on rotation acceleration
    }



    // Update is called once per frame
    void Update()
    {
        if (!isSelected && !rotatedToStart)
        {

            rotateTimer -= Time.deltaTime;
            if(rotateTimer <= 0)
            {
                rotatedToStart = true;
                rotating = true;
                StartCoroutine( RotateToPosition(model, targetEuler, 3));
            }

        }

        currentvolume = Mathf.Lerp(currentvolume, 0, 0.1f);
        //sfxAudioSource.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, Mathf.Abs(currentvolume));

        // Smooth the volume based on rotation speed
        float targetVolume = Mathf.Clamp(rotationSpeed / maxRotationSpeed, 0f, 1f);
        tableRotateAudioSource.volume = Mathf.Lerp(tableRotateAudioSource.volume, targetVolume, Time.deltaTime * volumeSmoothingSpeed);
        // Decay rotation speed when not being manipulated
        if (!isSelected)
        {
            rotationSpeed = Mathf.Lerp(rotationSpeed, 0f, Time.deltaTime * rotationSpeedDecayRate);
        }
    }

    

    private void FixedUpdate()
    {
        if (useAccelerationMode)
        {
            currentAcceleration = Mathf.Lerp(currentAcceleration, 0f, accelerationReductionLerp);
            model.transform.Rotate(Vector3.up, -currentAcceleration);
        }


    }

    public void ActivateHotspots(float delay = 0)
    {
        StartCoroutine(DoActivateHotspots(delay));

    }

    IEnumerator DoActivateHotspots(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        foreach (GameObject hotspot in hotspots)
        {
            if (!hotspot.activeInHierarchy)
            {
                hotspot.SetActive(true);
            }
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        GetHandSubsystem();
        selectEntered.AddListener(StartGrab);
        selectExited.AddListener(EndGrab);

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

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        selectExited.RemoveListener(EndGrab);
        base.OnDisable();
    }

    void StartGrab(SelectEnterEventArgs args)
    {
        //StopAllCoroutines();
        StopCoroutine("RotateToPosition");

        if (interactorsSelecting.Count == 1)
        {
            var interactor = args.interactorObject;
            var interactorPosition = interactor.GetAttachTransform(this).position;

            pinchHelperParent.transform.position = Camera.main.transform.position;// _pos; //setup helper
            pinchViz.transform.localPosition = Vector3.zero;
            pinchHelperParent.LookAt(transform.position);
            pinchHelperParent.transform.position = interactorPosition;

            lastYaw = transform.eulerAngles.y;
        }

        
        Debug.Log("model selected");
    }

    void EndGrab(SelectExitEventArgs args)
    {
        //Debug.Log("end grab");
        if (interactorsSelecting.Count > 0)
        {

            return;
        }
        if (interactorsSelecting.Count == 0)
        {
            rotatedToStart = false;
            rotateTimer = returnRotateTimer;
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;
        if (!isSelected) return;

        currentvolume += 0.2f;

        if (HasMultipleInteractors())
        {

        }
        else
        {
            var interactor = interactorsSelecting[0];
            var interactorTransform = interactor.GetAttachTransform(this);
            var interactorTransformPosition = interactorTransform.position;

            Vector3 lastVizLocalPos = pinchViz.transform.localPosition;
            pinchViz.transform.position = interactorTransformPosition;
            pinchViz.transform.localPosition = new Vector3(pinchViz.transform.localPosition.x, 0f, 0f);

            float pinchVizDelta = pinchViz.transform.localPosition.x - lastVizLocalPos.x;

            float rotationAmmount = pinchVizDelta * rotationMultiplier;

            if (useAccelerationMode)
                currentAcceleration += (rotationAmmount * accelerationMultiplier);

            model.transform.Rotate(Vector3.up, -rotationAmmount, Space.Self);

            float currentYaw = transform.eulerAngles.y;
            rotationSpeed = Mathf.Abs(Mathf.DeltaAngle(currentYaw, lastYaw)) / Time.deltaTime;
            lastYaw = currentYaw;
        }
    }

    bool HasMultipleInteractors()
    {
        return interactorsSelecting.Count > 1;
    }

    public void RotateModel(Vector3 _targetRotation)
    {
        rotatedToStart = false;
        rotateTimer = 0.5f;

        targetEuler = _targetRotation;
    }

    public void ResetRotation()
    {
        rotatedToStart = false;
        rotateTimer = 0.1f;

        targetEuler = startingEuler;
    }

    IEnumerator RotateToPosition(Transform target, Vector3 targetRotation, float duration)
    {
        rotateWhooshSource.Play();

        var increment = 0f;
        Vector3 startingRotation = target.localEulerAngles;

        while (increment <= duration)
        {
            increment += Time.deltaTime;
            var percent = Mathf.Clamp01(increment / duration);
            var curvePercent = m_AnimationCurve.Evaluate(percent);
            var newRotation = Vector3.Lerp(startingRotation, targetRotation, curvePercent);

            target.localEulerAngles = newRotation;
            yield return null;
        }
        rotating = false;

    }

    void GetHandSubsystem()
    {
        var xrGeneralSettings = XRGeneralSettings.Instance;
        if (xrGeneralSettings == null)
            Debug.LogError("XR general settings not set");

        var manager = xrGeneralSettings.Manager;
        if (manager != null)
        {
            var loader = manager.activeLoader;
            if (loader != null)
            {
                m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                if (!CheckHandSubsystem())
                    return;

                m_HandSubsystem.Start();
            }
        }
    }

    bool CheckHandSubsystem()
    {
        if (m_HandSubsystem == null)
        {
            Debug.LogError("Could not find Hand Subsystem");
            //enabled = false;
            return false;
        }

        return true;
    }


    public void Disappear()
    {
        StopAllCoroutines();
        foreach (GameObject hotspot in hotspots)
        {
            hotspot.GetComponent<Hotspot>().Disappear();
        }

        if (infoPanel.isActiveAndEnabled)
        {
            infoPanel.CloseFinal();
        }

        if (!rotatedToStart || rotating)
        {
            StopAllCoroutines();
            rotatedToStart = true;
            StartCoroutine(RotateToPosition(model, startingEuler, 3));
            StartCoroutine(DoDisappear(3));
        }
        else
        {
            StartCoroutine(DoDisappear(0.5f));
        }

        //StartCoroutine(RotateToPosition(launchRotation, 3));
        //Invoke("DoDisappear", 4);
        
    }


    
    IEnumerator DoDisappear(float delay)
    {

        yield return new WaitForSeconds(delay);

        AudioManager.Instance.GaiaAppear();

        ControlPanel.Instance.SwitchToSpace();

        titleAppear.Stop();
        modelAppear.Stop();

        Debug.Log("space activated");
        jwstDisappear.gameObject.SetActive(true);
        jwstDisappear.GetComponent<AudioSource>().enabled = true;
        jwstDisappear.GetComponent<AudioSource>().mute = false;
        
        jwstDisappear.Play();


        yield return new WaitForSeconds((float)jwstDisappear.duration);

        //jwstDisappear.Stop();
    }



    public void Reappear()
    {
        AudioManager.Instance.ReturnToJWST();
        SpaceBackground.Instance.DeactivateHotspots();
        ControlPanel.Instance.ReturnToJWST();

        jwstDisappear.GetComponent<AudioSource>().mute = true;
        jwstDisappear.GetComponent<AudioSource>().enabled = false;

        StartCoroutine(ReverseTimeline(jwstDisappear, false));

        ActivateHotspots((float)jwstDisappear.duration / 3);
    }


    public void Begin()
    {
        titleAppear.Stop();
        modelAppear.Play();
        StartCoroutine(DelayedAudio(39f, introCTA));

        AudioManager.Instance.BeginPressed();

        Invoke("ActivateControlPanel", (float)modelAppear.duration);
    }

    void ActivateControlPanel()
    {
        ControlPanel.Instance.gameObject.SetActive(true);
        ControlPanel.Instance.Appear();
    }

    IEnumerator DelayedAudio(float _delay, AudioClip clip)
    {
        yield return new WaitForSeconds(_delay);
        source.Stop();
        source.PlayOneShot(clip);
    }

    private IEnumerator ReverseTimeline(PlayableDirector timeline, bool disappear)
    {

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
}
