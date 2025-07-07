using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                //pan the camera  based on the pan direction in the inspector
                CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - transform.position).normalized;
            if (customInspectorObjects.swapCamerasLeftRight && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                CameraManager.Instance.swapCamerasLeftRight(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if(customInspectorObjects.swapCamerasUpDown && customInspectorObjects.cameraOnTop != null && customInspectorObjects.cameraOnBottom != null)
            {
                CameraManager.Instance.swapCamerasUpDown(customInspectorObjects.cameraOnTop, customInspectorObjects.cameraOnBottom, exitDirection);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                //pan the camera  back to the starting position
                CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.panDirection, true);

            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCamerasLeftRight = false;
    public bool swapCamerasUpDown = false;
    
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public CinemachineVirtualCamera cameraOnTop;
    [HideInInspector] public CinemachineVirtualCamera cameraOnBottom;


    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControlTrigger))]
public class CameraControlTriggerEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.customInspectorObjects.swapCamerasLeftRight)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left",
                cameraControlTrigger.customInspectorObjects.cameraOnLeft,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera On Right",
                cameraControlTrigger.customInspectorObjects.cameraOnRight,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if(cameraControlTrigger.customInspectorObjects.swapCamerasUpDown)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnTop = EditorGUILayout.ObjectField("Camera On Top",
                cameraControlTrigger.customInspectorObjects.cameraOnTop,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            cameraControlTrigger.customInspectorObjects.cameraOnBottom = EditorGUILayout.ObjectField("Camera On Bottom",
                cameraControlTrigger.customInspectorObjects.cameraOnBottom,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance",
                cameraControlTrigger.customInspectorObjects.panDistance);

            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time",
                cameraControlTrigger.customInspectorObjects.panTime);
        }
        
        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
#endif


