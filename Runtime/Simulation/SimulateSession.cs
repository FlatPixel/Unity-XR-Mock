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
        [SerializeField]
        private List<MockTrackable> m_trackables;

        float m_LastTime;
        enum State
        {
            Uninitialized,
            Initialized,
            Paused,
            WaitingForFirstTrackable,
            WaitingToAddRemainingTrackables,
            Finished
        }

        [Header("Simulation States (handle trackable discovery)")]
        [SerializeField]
        State m_InnerState;

        [Space()]
        [SerializeField]
        [Tooltip("Array of times to wait in each inner state. Index of the duration array correspond to the index of the inner state. (1 sec by default)")]
        float[] m_DurationPerInnerState;

        private void Start()
        {
            m_trackables = new List<MockTrackable>();

            foreach (var trackable in GetComponentsInChildren<MockTrackable>(true))
            {
                trackable.gameObject.SetActive(false);
                m_trackables.Add(trackable);
            }
        }

        void Update()
        {
            SessionApi.trackingState = m_TrackingState;

            float stateDuration = 1;
            int indexEnum = (int)m_InnerState;
            if (m_DurationPerInnerState != null && indexEnum < m_DurationPerInnerState.Length)
                stateDuration = m_DurationPerInnerState[indexEnum];

            switch (m_InnerState)
            {
                case State.Uninitialized:
                    m_InnerState = State.Initialized;
                    m_LastTime = Time.time;
                    break;
                case State.Initialized:
                    m_InnerState = State.WaitingForFirstTrackable;
                    m_TrackingState = TrackingState.Limited;
                    m_LastTime = Time.time;
                    break;
                case State.WaitingForFirstTrackable:
                    if (Time.time - m_LastTime > stateDuration)
                    {
                        if (m_trackables.Count > 0)
                            m_trackables[0].gameObject.SetActive(true);

                        m_InnerState = State.WaitingToAddRemainingTrackables;
                        m_LastTime = Time.time;
                    }
                    break;
                case State.WaitingToAddRemainingTrackables:
                    if (Time.time - m_LastTime > stateDuration)
                    {
                        for (int i = 1; i < m_trackables.Count; i++)
                            m_trackables[i].gameObject.SetActive(true);

                        m_InnerState = State.Finished;
                        m_TrackingState = TrackingState.Tracking;
                        m_LastTime = Time.time;
                    }
                    break;
            }
        }
    }
}
