using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BackgroundMaskScript : MonoBehaviour
{

    public Material[] affectedMats;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material mat in affectedMats)
        {
            mat.SetVector("_Mask_Position", transform.position);
            
        }
    }


    private void OnValidate()
    {
        foreach (Material mat in affectedMats)
        {
            mat.SetVector("_Mask_Position", transform.position);

        }
    }
}
