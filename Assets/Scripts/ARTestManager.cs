using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARTestManager : MonoBehaviour 
{
    public Transform target;


    ARSessionOrigin m_SessionOrigin;

    private void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void OnEnable()
    {
        ARSubsystemManager.planeAdded += OnPlaneAdded;
        ARSubsystemManager.planeUpdated += OnPlaneUpdated;
        ARSubsystemManager.planeRemoved += OnPlaneRemoved;
        ARSubsystemManager.sessionDestroyed += OnSessionDestroyed;
    }

    private void OnDisable()
    {
        ARSubsystemManager.planeAdded -= OnPlaneAdded;
        ARSubsystemManager.planeUpdated -= OnPlaneUpdated;
        ARSubsystemManager.planeRemoved -= OnPlaneRemoved;
        ARSubsystemManager.sessionDestroyed -= OnSessionDestroyed;
    }



    private void OnPlaneAdded(PlaneAddedEventArgs eventArgs)
    {
        Debug.LogErrorFormat("[OnPlaneAdded] TrackableId:{0}, Center:{1},  Pose position:{2}, Pose rotate:{3}",
                                eventArgs.Plane.Id.ToString(),
                                eventArgs.Plane.Center,
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);
    }

    private void OnPlaneUpdated(PlaneUpdatedEventArgs eventArgs)
    {
        Debug.LogErrorFormat("[OnPlaneUpdated] TrackableId:{0}, Center:{1},  Pose position:{2}, Pose rotate:{3}",
                                eventArgs.Plane.Id.ToString(),
                                eventArgs.Plane.Center,
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);

        Vector3 position = eventArgs.Plane.Pose.position;
        Vector3 angle = eventArgs.Plane.Pose.rotation.eulerAngles;
        if (Mathf.Abs(position.y) > 20 || 
            Mathf.Abs(angle.x) > 20 ||
            Mathf.Abs(angle.z) > 20)
        {
            return;
        }


        Vector3 cameraForward = ARSubsystemManager.cameraSubsystem.Camera.transform.forward;
        Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        SetTarget(position, Quaternion.LookRotation(cameraBearing));
    }

    private void OnPlaneRemoved(PlaneRemovedEventArgs eventArgs)
    {
        Debug.LogErrorFormat("[OnPlaneRemoved] TrackableId:{0}, Center:{1},  Pose position:{2}, Pose rotate:{3}",
                                eventArgs.Plane.Id.ToString(),
                                eventArgs.Plane.Center,
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);
    }

    private void OnSessionDestroyed()
    {
        Debug.LogError("OnSessionDestroyed");
    }



    private void SetTarget(Vector3 position, Quaternion rotation)
    {
        if (target != null)
        {
            target.SetParent(m_SessionOrigin.trackablesParent);
            target.SetPositionAndRotation(position, rotation);

            target.gameObject.SetActive(true);
        }
    }

}
