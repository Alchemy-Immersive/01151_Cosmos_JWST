using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolySpatial.Samples;

public class BeforeAfterSliderMover : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private SpatialUISlider uiSliderRef;
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Vector2 sfxRange = new Vector2(0f, 1f);
    [SerializeField] private float sfxVolumeReduction = 0.01f;
    [SerializeField] private float sfxVolumeIncrease = 0.1f;

    void Start()
    {
        uiSliderRef.OnSliderUpdated += SliderUpdated;
    }

    private void OnEnable()
    {
        sfxSource.time = Random.Range(0f, sfxSource.clip.length - 1f);
        sfxSource.volume = 0f;
        sfxSource.Play();
    }

    private void FixedUpdate()
    {
        if (sfxSource.volume > sfxRange.x)
            sfxSource.volume -= sfxVolumeReduction;
    }

    private void SliderUpdated(float _percentage)
    {
        //float percentage = Remap(_percentage, uiSliderRef.MinMax.x, uiSliderRef.MinMax.y, 0, 1);

        transform.localPosition = Vector3.Lerp(rightEdge.transform.localPosition, leftEdge.transform.localPosition, _percentage);
        if (sfxSource.volume < sfxRange.y)
            sfxSource.volume += sfxVolumeIncrease;
    }

    public static float Remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }
}
