using System.Collections.Generic;
using UnityEngine;

public class StarField : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private float starSizeMin = 0f;
    [Range(0, 100)]
    [SerializeField] private float starSizeMax = 5f;
    private List<StarDataLoader.Star> stars;
    private List<GameObject> starObjects;

    public GameObject starPrefab;

    [SerializeField] private int starFieldScale = 200;

    void Start()
    {
        // Read in the star data.
        StarDataLoader sdl = new();
        stars = sdl.LoadData();
        starObjects = new();
        foreach (StarDataLoader.Star star in stars)
        {
            // Create star game objects.
            //GameObject stargo = GameObject.CreatePrimitive(PrimitiveType.Quad);

            GameObject stargo = Instantiate(starPrefab);
            stargo.transform.parent = transform;
            stargo.name = $"HR {star.catalog_number}";
            stargo.transform.localPosition = star.position * starFieldScale;
            stargo.transform.localScale = Vector3.one * Mathf.Lerp(starSizeMin, starSizeMax, star.size);
            stargo.transform.LookAt(transform.position);
            stargo.transform.Rotate(0, 180, 0);
            Material material = stargo.GetComponent<MeshRenderer>().material;
            //material.shader = Shader.Find("Unlit/StarShader");
            //material.shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            //material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, star.size));
            material.color = star.colour;
            starObjects.Add(stargo);
        }
    }

    // Could also do in Update with Time.deltatime scaling.
    private void FixedUpdate()
    {

    }

    private void OnValidate()
    {
        if (starObjects != null)
        {
            for (int i = 0; i < starObjects.Count; i++)
            {
                // Update the size set in the shader.
                Material material = starObjects[i].GetComponent<MeshRenderer>().material;
                material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
            }
        }
        else
        {
            // Read in the star data.
            StarDataLoader sdl = new();
            stars = sdl.LoadData();
            starObjects = new();
            foreach (StarDataLoader.Star star in stars)
            {
                // Create star game objects.
                //GameObject stargo = GameObject.CreatePrimitive(PrimitiveType.Quad);

                GameObject stargo = Instantiate(starPrefab);
                stargo.transform.parent = transform;
                stargo.name = $"HR {star.catalog_number}";
                stargo.transform.localPosition = star.position * starFieldScale;
                stargo.transform.localScale = Vector3.one * Mathf.Lerp(starSizeMin, starSizeMax, star.size);
                stargo.transform.LookAt(transform.position);
                stargo.transform.Rotate(0, 180, 0);
                Material material = stargo.GetComponent<MeshRenderer>().material;
                //material.shader = Shader.Find("Unlit/StarShader");
                //material.shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                //material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, star.size));
                material.color = star.colour;
                starObjects.Add(stargo);
            }
        }
    }
}