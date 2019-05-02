using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARTestManager : MonoBehaviour 
{
    public Transform target;
    public GameObject placementIndicator;


    private ARSessionOrigin m_sessionOrigin;
    private Pose m_placementPose;
    private bool m_placementPoseIsValid = false;


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
        UpdatePlacementIndicator();
        UpdatePlacementPose();

        if (m_placementPoseIsValid &&
            Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SetTarget(m_placementPose.position, m_placementPose.rotation);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (m_placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(m_placementPose.position, m_placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = m_sessionOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        m_sessionOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        m_placementPoseIsValid = hits.Count > 0;
        if (m_placementPoseIsValid)
        {
            m_placementPose = hits[0].pose;

            Debug.LogErrorFormat("[PlaneHits] TrackableId:{0}, Pose position:{1}, Pose rotate:{2}, Distance:{3}, Relative Pose position:{4}, Relative Pose rotate:{5}, Relative Distance:{6}",
                                hits[0].trackableId.ToString(),
                                hits[0].pose.position,
                                hits[0].pose.rotation.eulerAngles,
                                hits[0].distance,
                                hits[0].sessionRelativePose.position,
                                hits[0].sessionRelativePose.rotation.eulerAngles,
                                hits[0].sessionRelativeDistance);

            Vector3 angle = m_placementPose.rotation.eulerAngles;
            if (Mathf.Abs(angle.x) > 20 ||
                Mathf.Abs(angle.z) > 20)
            {
                m_placementPoseIsValid = false;
                return;
            }


            var cameraForward = m_sessionOrigin.camera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            m_placementPose.rotation = Quaternion.LookRotation(cameraBearing);
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
