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
        Debug.LogErrorFormat("[OnPlaneAdded] TrackableId:{0}, Pose position:{1}, Pose rotate:{2}",
                                eventArgs.Plane.Id.ToString(), 
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);
    }

    private void OnPlaneUpdated(PlaneUpdatedEventArgs eventArgs)
    {
        Debug.LogErrorFormat("[OnPlaneUpdated] TrackableId:{0}, Pose position:{1}, Pose rotate:{2}",
                                eventArgs.Plane.Id.ToString(),
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);

        Vector3 angle = eventArgs.Plane.Pose.rotation.eulerAngles;
        if (Mathf.Abs(angle.x) > 20 ||
            Mathf.Abs(angle.y) > 20 ||
            Mathf.Abs(angle.z) > 20)
        {
            return;
        }

        SetTarget(eventArgs.Plane.Center);
    }

    private void OnPlaneRemoved(PlaneRemovedEventArgs eventArgs)
    {
        Debug.LogErrorFormat("[OnPlaneRemoved] TrackableId:{0}, Pose position:{1}, Pose rotate:{2}",
                                eventArgs.Plane.Id.ToString(),
                                eventArgs.Plane.Pose.position,
                                eventArgs.Plane.Pose.rotation.eulerAngles);
    }

    private void OnSessionDestroyed()
    {
        Debug.LogError("OnSessionDestroyed");
    }



    private void SetTarget(Vector3 position)
    {
        if (target != null)
        {
            target.SetParent(m_SessionOrigin.trackablesParent);
            target.position = position;

            target.gameObject.SetActive(true);
        }
    }

}
