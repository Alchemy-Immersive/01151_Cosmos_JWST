using UnityEngine;

public class ZoomMaterialAdjuster : MonoBehaviour
{
    [SerializeField]
    float lensRadius = 10;

    public Material[] zoomMats;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (zoomMats.Length > 0)
        {
            foreach (Material mat in zoomMats)
            {
                mat.SetVector("_CirclePos", transform.position);
                mat.SetFloat("_CircleRadius", lensRadius);
            }
        }
    }


    private void OnValidate()
    {
        if (zoomMats.Length > 0)
        {
            foreach (Material mat in zoomMats)
            {
                mat.SetVector("_CirclePos", transform.position);
                mat.SetFloat("_CircleRadius", lensRadius);
            }
        }

    }
}
