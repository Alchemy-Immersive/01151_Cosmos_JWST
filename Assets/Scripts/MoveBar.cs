using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBar : MonoBehaviour
{

    public Transform parentObject;
    GameObject mainCamera;
    public bool useLookAt = true;
    //public Vector3 offset;

    Vector3 startPos;
    Vector3 handStartPos, handCurrentPos;

    [Header("SlowFollow")]
    public bool slowFollow;
    public float slowTime = 1;
    Vector3 m_Velocity = Vector3.zero;
    Vector3 newPos;

    /*
    [Header("Physics")]
    public bool usePhysics;
    public Rigidbody m_Rigidbody;
    [SerializeField]
    float k_ToVel = 10f;
    [SerializeField]
    float k_MaxVel = 20f;
    [SerializeField]
    float k_MaxForce = 40.0f;
    [SerializeField]
    float k_Gain = 5f;
    Vector3 m_StartingOffset;*/

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");

        if(parentObject == null)
        {
            parentObject = transform.parent;
        }
    }


    public void Select(Vector3 position)
    {

        handStartPos = position;

        startPos = parentObject.transform.position;

        //m_StartingOffset = position - startPos;
    }

    public void Release()
    {
        if(slowFollow)
            StartCoroutine(SlowDown(newPos));
    }

    IEnumerator SlowDown(Vector3 targetPosition)
    {
        float dist = Vector3.Distance(parentObject.position, targetPosition);

        while (dist >= 0.05f)
        {
            parentObject.position = Vector3.SmoothDamp(parentObject.position, targetPosition, ref m_Velocity, slowTime + .5f);


            dist = Vector3.Distance(parentObject.position, targetPosition);

            yield return null;
        }
    }

    public void Move(Vector3 position)
    {
        handCurrentPos = position;

        var deltaPosX = handCurrentPos.x - handStartPos.x;
        var deltaPosY = handCurrentPos.y - handStartPos.y;
        var deltaPosZ = handCurrentPos.z - handStartPos.z;

        newPos = new Vector3(startPos.x + deltaPosX, startPos.y + deltaPosY, startPos.z + deltaPosZ);

        /*
        if (usePhysics)
        {
            var targetPos = position - m_StartingOffset;

            var distance = targetPos - parentObject.position;
            var targetVelocity = Vector3.ClampMagnitude(k_ToVel * distance, k_MaxVel);
            var error = targetVelocity - m_Rigidbody.velocity;
            var force = Vector3.ClampMagnitude(k_Gain * error, k_MaxForce);
            m_Rigidbody.AddForce(force);

        }
        else
        {

        }*/

        if (slowFollow)
        {
            parentObject.position = Vector3.SmoothDamp(parentObject.position, newPos, ref m_Velocity, slowTime);
        }
        else
            parentObject.position = newPos;


        //parentObject.position = position + offset;

        if (useLookAt)
        {
            Vector3 targetPostition = new Vector3(mainCamera.transform.position.x,
                     parentObject.position.y,
                     mainCamera.transform.position.z);
            parentObject.LookAt(targetPostition);


        }

    }
}
