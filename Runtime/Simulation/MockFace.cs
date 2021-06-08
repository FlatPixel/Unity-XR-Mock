using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class MockFace : MockTrackable
    {
        [SerializeField]
        float m_TrackingLostProbability = 0.01f;

        public CameraController mockCamera;
        public Vector3 offsetCamera;

        public Transform leftEye;
        public Transform rightEye;

        public Pose poseLeftEye { get { return new Pose(leftEye.position, leftEye.rotation); } }
        public Pose poseRightEye { get { return new Pose(rightEye.position, rightEye.rotation); } }

        private TrackableId id;

        private void OnEnable()
        {
            switch (CameraApi.currentFacingDirection)
            {
                case Feature.WorldFacingCamera:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * -offsetCamera.z;
                    break;
                case Feature.UserFacingCamera:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * offsetCamera.z;
                    break;
                default:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * -offsetCamera.z;
                    break;
            }

            transform.LookAt(mockCamera.transform, mockCamera.transform.up);

            id = FaceApi.Add(pose, poseLeftEye, poseRightEye);
            FaceApi.SetTrackingState(id, TrackingState.Tracking);
        }

        private void OnDisable()
        {
            FaceApi.SetTrackingState(id, TrackingState.None);
            FaceApi.Remove(id);
        }

        private void Update()
        {
            if (enabled == false) return;

            switch (CameraApi.currentFacingDirection)
            {
                case Feature.WorldFacingCamera:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * -offsetCamera.z;
                    break;
                case Feature.UserFacingCamera:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * offsetCamera.z;
                    break;
                default:
                    transform.position = mockCamera.transform.position
                                + mockCamera.transform.right * offsetCamera.x
                                + mockCamera.transform.up * offsetCamera.y
                                + mockCamera.transform.forward * -offsetCamera.z;
                    break;
            }

            transform.LookAt(mockCamera.transform, mockCamera.transform.up);

            FaceApi.Update(id, pose, poseLeftEye, poseRightEye);
        }
    }
}
