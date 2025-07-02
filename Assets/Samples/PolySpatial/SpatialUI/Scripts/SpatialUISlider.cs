using UnityEngine;
using UnityEngine.UI;
using Unity;
using System.Collections;

namespace PolySpatial.Samples
{
    public class SpatialUISlider : SpatialUI
    {
        public AnimationCurve m_AnimationCurve;
        [Header("Config")]
        [SerializeField]
        MeshRenderer m_FillRenderer;
        [SerializeField] private bool resetOnEnable = true;

        public Vector2 MinMax;
        public float centrePoint = 0.5f;

        float m_BoxColliderSizeX;
        float targetPercentage;

        public delegate void OnSliderUpdatedCallback(float _newState);
        public OnSliderUpdatedCallback OnSliderUpdated;

        public GameObject sliderUIButton;


        public float currentPercentage = 0.5f;
        bool selected;
        public AudioSource CTA_Clip;
        public float clipDelay = 5;

        void Start()
        {
            m_BoxColliderSizeX = GetComponent<BoxCollider>().size.x;

            if (sliderUIButton != null)
            {
                sliderUIButton.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (resetOnEnable)
            {
                SetPercentage(0);
                selected = false;
            }
        }

        private void Update()
        {
            
        }


        public override void Press(Vector3 position)
        {
            if (sliderUIButton != null)
            {
                sliderUIButton.SetActive(false);
            }

            selected = true;

            base.Press(position);
            var localPosition = transform.InverseTransformPoint(position);

            StopAllCoroutines();

            var percentage = localPosition.x / m_BoxColliderSizeX + 0.5f;
            percentage = Mathf.Clamp(percentage, 0f, 1f);
            targetPercentage = Remap(percentage, 0, 1, 1, 0);

            var newPercentage = Mathf.Lerp(currentPercentage, targetPercentage, Time.deltaTime * 3);

            currentPercentage = Mathf.Clamp(newPercentage, MinMax.x, MinMax.y);

            SetPercentage(currentPercentage);

        }

        public void Release()
        {
            StartCoroutine(DoRelease());
            Debug.Log("slider released");
        }

        IEnumerator DoRelease()
        {
            while(currentPercentage > targetPercentage + 0.05 || currentPercentage < targetPercentage - 0.05)
            {
                var newPercentage = Mathf.Lerp(currentPercentage, targetPercentage, Time.deltaTime * 2);

                currentPercentage = Mathf.Clamp(newPercentage, MinMax.x, MinMax.y);

                SetPercentage(currentPercentage);
                yield return null;
            }
        }

        public void SetPercentage(float percentage)
        {
            if (m_FillRenderer != null)
            {
                m_FillRenderer.sharedMaterial.SetFloat("_SliderPercentage", percentage);
            }
                
            currentPercentage = percentage;
            if (OnSliderUpdated != null) OnSliderUpdated(currentPercentage);
        }

        public static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }

        private void OnValidate()
        {
            SetPercentage(currentPercentage);
        }


        public void AnimateToCentre()
        {
            StartCoroutine(DoAnimate(3, centrePoint));
            StartCoroutine(Play_CTA_Clip(clipDelay));
            Debug.Log("animating slider");
        }

        public void AnimateToCentreDelayed()
        {
            Invoke("DelayedAnimate", 16.5f);
        }

        public void AnimateToCentreNoDelay()
        {
            Invoke("AnimateNoDelay", 0f);
        }

        void DelayedAnimate()
        {
            StartCoroutine(DoAnimate(3, centrePoint));
            StartCoroutine(Play_CTA_Clip(clipDelay - 15.5f));
            Debug.Log("animating slider");
        }

        void AnimateNoDelay()
        {
            StartCoroutine(AnimateToStartThenCenter());

            //StartCoroutine(Play_CTA_Clip(clipDelay - 15.5f));
            Debug.Log("animating slider");
        }

        IEnumerator AnimateToStartThenCenter()
        {
            StartCoroutine(Play_CTA_Clip(11));
            yield return new WaitForSeconds(11);

            yield return StartCoroutine(DoAnimate(5, 0.55f));

            yield return new WaitForSeconds(1f);


            if (sliderUIButton != null)
            {
                sliderUIButton.SetActive(true);
            }
        }

        public void AnimateToStart()
        {
            StartCoroutine(DoAnimate(1, 0));
            Debug.Log("animating to start");
        }

        IEnumerator DoAnimate(float duration, float targetValue)
        {

            float startValue = currentPercentage;
            var increment = 0f;

            while (increment <= duration)
            {
                increment += Time.deltaTime;
                var percent = Mathf.Clamp01(increment / duration);
                var curvePercent = m_AnimationCurve.Evaluate(percent);

                currentPercentage = Mathf.Lerp(startValue, targetValue, curvePercent);
                SetPercentage(currentPercentage);

                yield return null;
            }

        }

        IEnumerator Play_CTA_Clip(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            if (!selected)
            {
                CTA_Clip.Play();
            }
        }
    }
}
