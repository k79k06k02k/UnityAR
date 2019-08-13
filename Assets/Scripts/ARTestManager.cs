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

    private ARPlaneManager m_arPlaneManager;
    private ARRaycastManager m_arRaycastManager;
    private List<ARRaycastHit> m_hitResults;


    private void Awake()
    {
        m_sessionOrigin = this.GetComponent<ARSessionOrigin>();
        m_arPlaneManager = this.GetComponent<ARPlaneManager>();
        m_arRaycastManager = this.GetComponent<ARRaycastManager>();
    }

    private void OnEnable()
    {
        m_arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        m_arPlaneManager.planesChanged -= OnPlanesChanged;
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
        m_arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

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


    private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        ShowARPlanesLog("ARPlanes added", eventArgs.added);
        ShowARPlanesLog("ARPlanes removed", eventArgs.removed);
        ShowARPlanesLog("ARPlanes updated", eventArgs.updated);
    }
    private void ShowARPlanesLog(string title, List<ARPlane> planes)
    {
        for (int i = 0; i < planes.Count; i++)
        {
            Debug.LogErrorFormat("[{0}] TrackableId:{1}, Center:{2}, position:{3}, rotate:{4}",
                                title,
                                planes[i].trackableId.ToString(),
                                planes[i].center,
                                planes[i].transform.position,
                                planes[i].transform.eulerAngles);
        }
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
