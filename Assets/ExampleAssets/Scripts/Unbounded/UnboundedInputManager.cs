using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace PolySpatial.Template
{
    public class UnboundedInputManager : MonoBehaviour
    {
        TouchPhase m_LastTouchPhase;
        UnboundedObjectBehavior m_SelectedObject;
        GameObject selectedObject;

        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        void Update()
        {
            var activeTouches = Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                var primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
                var touchPhase = activeTouches[0].phase;

                if (touchPhase == TouchPhase.Began)
                {
                    if (primaryTouchData.targetObject != null)
                    {
                        selectedObject = primaryTouchData.targetObject;

                        if(primaryTouchData.targetObject.TryGetComponent(out UnboundedObjectBehavior unboundedObject))
                        {
                            m_SelectedObject = unboundedObject;
                            m_SelectedObject.Select(true);
                        }

                        if (selectedObject.TryGetComponent(out SpaceBackground spaceBG))
                        {
                            spaceBG.Select(true, primaryTouchData.inputDevicePosition);
                            Debug.Log("space bg selected");
                        }

                        if (selectedObject.TryGetComponent(out MoveBar moveBar))
                        {
                            moveBar.Select(primaryTouchData.interactionPosition);
                        }
                    }
                }

                if(touchPhase == TouchPhase.Moved)
                {
                    if (selectedObject.TryGetComponent(out SpaceBackground spaceBG))
                    {
                        spaceBG.Select(false, primaryTouchData.inputDevicePosition);
                    }

                    if (selectedObject.TryGetComponent(out MoveBar moveBar))
                    {
                        moveBar.Move(primaryTouchData.interactionPosition);
                    }
                }

                if (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled)
                {
                    if (selectedObject.TryGetComponent(out SpaceBackground spaceBG))
                    {
                        spaceBG.Deselect();
                        Debug.Log("space bg selected");
                    }

                    if (selectedObject.TryGetComponent(out MoveBar moveBar))
                    {
                        moveBar.Release();
                    }

                    if (m_SelectedObject != null)
                    {
                        m_SelectedObject.Select(false);
                        m_SelectedObject = null;
                    }
                }
            }
        }
    }
}
