using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolySpatial.Samples;
using TMPro;

public class MaterialAdjustmentSlider : MonoBehaviour
{


    public Material[] mats;
    public string valueToChange;
    public float minValue, maxValue;
    public float defaultValue;
    public TMP_Text valueText;

    public GameObject sliderHandle;
    [SerializeField] private SpatialUISlider1 uiSliderRef;
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Vector2 sfxRange = new Vector2(0f, 1f);
    [SerializeField] private float sfxVolumeReduction = 0.01f;
    [SerializeField] private float sfxVolumeIncrease = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        uiSliderRef.OnSliderUpdated += SliderUpdated;

        var percentage = defaultValue / maxValue;
        uiSliderRef.SetPercentage(percentage);

        
    }

    private void OnDestroy()
    {
        var percentage = defaultValue / maxValue;
        uiSliderRef.SetPercentage(percentage);

        uiSliderRef.OnSliderUpdated -= SliderUpdated;


    }

    // Update is called once per frame
    void Update()
    {

        if (sfxSource.volume > sfxRange.x)
            sfxSource.volume -= sfxVolumeReduction;
    }

    public void Reset()
    {
        var percentage = defaultValue / maxValue;
        uiSliderRef.SetPercentage(percentage);
    }


    private void SliderUpdated(float _percentage)
    {
        sliderHandle.transform.localPosition = Vector3.Lerp(leftEdge.transform.localPosition, rightEdge.transform.localPosition, _percentage);

        float value = Mathf.Lerp(minValue, maxValue, _percentage);

        valueText.text = value.ToString();

        foreach (Material mat in mats)
        {
            mat.SetFloat(valueToChange, value);
        }

        if (sfxSource.volume < sfxRange.y)
            sfxSource.volume += sfxVolumeIncrease;
    }
}
