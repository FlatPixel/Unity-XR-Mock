using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Mock.Example
{
    public class SimulateSession : MonoBehaviour
    {
        [SerializeField]
        TrackingState m_TrackingState = TrackingState.Tracking;

        [SerializeField]
        GameObject[] planes;

        float m_LastTime;
        enum State
        {
            Uninitialized,
            Initialized,
            Paused,
            WaitingToAddPlane1,
            WaitingToAddRemainingPlanes,
            Finished
        }

        State m_State;

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
                    m_State = State.WaitingToAddPlane1;
                    m_LastTime = Time.time;
                    break;
                case State.WaitingToAddPlane1:
                    if (Time.time - m_LastTime > 2f)
                    {
                        planes[0].SetActive(true);

                        m_State = State.WaitingToAddRemainingPlanes;
                        m_LastTime = Time.time;
                    }
                    break;
                case State.WaitingToAddRemainingPlanes:
                    if (Time.time - m_LastTime > 2f)
                    {
                        for (int i = 1; i < planes.Length; i++)
                        {
                            planes[i].SetActive(true);
                        }

                        m_State = State.Finished;
                        m_LastTime = Time.time;
                    }
                    break;
            }
        }
    }
}
