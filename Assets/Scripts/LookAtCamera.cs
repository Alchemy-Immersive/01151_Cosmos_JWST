using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool lockYaxis = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lockYaxis)
        {
            Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                transform.position.y, Camera.main.transform.position.z);

            transform.LookAt(targetPostition);
        }
        else
        {
            transform.LookAt(Camera.main.transform.position);
        }

        
    }
}
