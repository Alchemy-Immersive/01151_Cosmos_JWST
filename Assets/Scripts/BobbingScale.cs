using UnityEngine;

public class BobbingScale : MonoBehaviour
{

    [Header("Bobbing Scale")]
    public float amplitude = 0.01f;
    public float scaleSpeed = 3;
    public bool doScale = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (doScale)
        {
            float scaleVal = amplitude * Mathf.Sin(Time.time * scaleSpeed) + 1;
            transform.localScale = Vector3.one * scaleVal;
        }
        else
        {
            
        }

    }

    public void Stop()
    {
        doScale = false;
        transform.localScale = Vector3.one;
    }
}
