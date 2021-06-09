using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class SimulateSession : MonoBehaviour
    {
        [Header("Tracking")]
        [SerializeField]
        TrackingState m_TrackingState = TrackingState.None;

        [Space()]
        private List<MockTrackable> m_worldTrackables;
        private List<MockTrackable> m_faceTrackables;

        [Space()]
        [SerializeField]
        private float m_InitializationDuration = 2;
        private float m_LastTime = 0;

        private void Start()
        {
            m_worldTrackables = new List<MockTrackable>();
            m_faceTrackables = new List<MockTrackable>();

            foreach (var trackable in GetComponentsInChildren<MockTrackable>(true))
            {
                trackable.gameObject.SetActive(false);

                if (trackable is MockFace)
                    m_faceTrackables.Add(trackable);
                else
                    m_worldTrackables.Add(trackable);
            }
        }

        public void Reset()
        {
            foreach (var trackable in GetComponentsInChildren<MockTrackable>(true))
                trackable.gameObject.SetActive(false);

            m_TrackingState = TrackingState.None;
        }

        void Update()
        {
            switch (m_TrackingState)
            {
                case TrackingState.None:
                    m_TrackingState = TrackingState.Limited;
                    m_LastTime = Time.time;
                    break;
                case TrackingState.Limited:
                    if (Time.time - m_LastTime > m_InitializationDuration)
                    {
                        m_TrackingState = TrackingState.Tracking;
                        m_LastTime = Time.time;
                    }
                    break;
                case TrackingState.Tracking:
                    if (CameraApi.currentFacingDirection == Feature.WorldFacingCamera)
                        foreach (var trackable in m_worldTrackables)
                            if (trackable.gameObject.activeSelf == false
                            && Time.time - m_LastTime > trackable.untrackedDuration)
                                trackable.gameObject.SetActive(true);
                    if (CameraApi.currentFacingDirection == Feature.UserFacingCamera)
                        foreach (var trackable in m_faceTrackables)
                            if (trackable.gameObject.activeSelf == false
                            && Time.time - m_LastTime > trackable.untrackedDuration)
                                trackable.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            SessionApi.trackingState = m_TrackingState;
        }
    }
}
