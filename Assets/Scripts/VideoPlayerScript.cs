using UnityEngine;
using Unity.PolySpatial;

public class VideoPlayerScript : MonoBehaviour
{
    public RenderTexture videoTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PolySpatialObjectUtils.MarkDirty(videoTexture);
    }
}
