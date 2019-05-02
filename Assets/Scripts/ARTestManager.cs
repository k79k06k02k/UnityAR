using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARTestManager : MonoBehaviour
{
    public Transform target;


    private ARSessionOrigin m_sessionOrigin;

    private TrackableId m_lastTrackableId;


    private void Awake()
    {
        m_sessionOrigin = GetComponent<ARSessionOrigin>();
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

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 screenCenter = m_sessionOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        m_sessionOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            if (hits[0].trackableId.Equals(m_lastTrackableId))
            {
                return;
            }

            Vector3 position = hits[0].pose.position;
            Vector3 angle = hits[0].pose.rotation.eulerAngles;
            if (Mathf.Abs(position.y) > 20 ||
                Mathf.Abs(angle.x) > 20 ||
                Mathf.Abs(angle.z) > 20)
            {
                return;
            }

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            SetTarget(position, Quaternion.LookRotation(cameraBearing));

            m_lastTrackableId = hits[0].trackableId;
        }
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
            target.SetParent(m_sessionOrigin.trackablesParent);
            target.SetPositionAndRotation(position, rotation);

            target.gameObject.SetActive(true);
        }
    }

}
