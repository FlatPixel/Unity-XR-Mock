using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class MockFace : MockTrackable
    {
        [SerializeField]
        float m_TrackingLostProbability = 0.01f;

        public Transform leftEye;
        public Transform rightEye;

        public Pose poseLeftEye { get { return new Pose(leftEye.position, leftEye.rotation); } }
        public Pose poseRightEye { get { return new Pose(rightEye.position, rightEye.rotation); } }

        IEnumerator Start()
        {
            var id = FaceApi.Add(pose, poseLeftEye, poseRightEye);

            while (enabled)
            {
                FaceApi.Update(id, pose, poseLeftEye, poseRightEye);

                if (Random.value < m_TrackingLostProbability)
                {
                    FaceApi.SetTrackingState(id, TrackingState.None);
                    yield return new WaitForSeconds(1f);
                    FaceApi.SetTrackingState(id, TrackingState.Tracking);
                }

                yield return null;
            }
        }
    }
}
