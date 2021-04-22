using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class SimulateSession : MonoBehaviour
    {
        [SerializeField]
        TrackingState m_TrackingState = TrackingState.None;

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

        State m_State;

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

            switch (m_State)
            {
                case State.Uninitialized:
                    m_State = State.Initialized;
                    m_LastTime = Time.time;
                    break;
                case State.Initialized:
                    m_State = State.WaitingForFirstTrackable;
                    m_TrackingState = TrackingState.Limited;
                    m_LastTime = Time.time;
                    break;
                case State.WaitingForFirstTrackable:
                    if (Time.time - m_LastTime > 2f)
                    {
                        if (m_trackables.Count > 0)
                            m_trackables[0].gameObject.SetActive(true);

                        m_State = State.WaitingToAddRemainingTrackables;
                        m_LastTime = Time.time;
                    }
                    break;
                case State.WaitingToAddRemainingTrackables:
                    if (Time.time - m_LastTime > 2f)
                    {
                        for (int i = 1; i < m_trackables.Count; i++)
                            m_trackables[i].gameObject.SetActive(true);

                        m_State = State.Finished;
                        m_TrackingState = TrackingState.Tracking;
                        m_LastTime = Time.time;
                    }
                    break;
            }
        }
    }
}
