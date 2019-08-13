using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARTestManager : MonoBehaviour 
{
    public Transform target;


    private ARSessionOrigin m_sessionOrigin;
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


    private void Update()
    {
        if (m_arRaycastManager.Raycast(m_sessionOrigin.camera.transform.forward, m_hitResults, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            Pose pose = m_hitResults[0].pose;

            Debug.LogErrorFormat("[Raycast Planes] TrackableId:{0}, Pose position:{1}, Pose rotate:{2}",
                                m_hitResults[0].trackableId.ToString(),
                                pose.position,
                                pose.rotation.eulerAngles);

            Vector3 position = pose.position;
            Vector3 angle = pose.rotation.eulerAngles;
            if (Mathf.Abs(position.y) > 20 ||
                Mathf.Abs(angle.x) > 20 ||
                Mathf.Abs(angle.z) > 20)
            {
                return;
            }


            Vector3 cameraForward = m_sessionOrigin.camera.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            SetTarget(position, Quaternion.LookRotation(cameraBearing));
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
