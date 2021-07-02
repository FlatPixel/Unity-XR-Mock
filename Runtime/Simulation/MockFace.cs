﻿using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class MockFace : MockTrackable
    {
        [Header("Face parameters")]
        [SerializeField]
        float m_TrackingLostProbability = 0.01f;

        public CameraController mockCamera;
        public Vector3 offsetCamera;

        public Transform leftEye;
        public Transform rightEye;

        public Pose poseLeftEye { get { return new Pose(leftEye.position, leftEye.rotation); } }
        public Pose poseRightEye { get { return new Pose(rightEye.position, rightEye.rotation); } }

        public Mesh mesh;

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
            transform.rotation = Quaternion.Euler(0, 180, 0);
            trackableId = FaceApi.Add(pose, poseLeftEye, poseRightEye, mesh);
            transform.rotation = Quaternion.Euler(0, 0, 0);

            tracking = TrackingState.Tracking;
            FaceApi.SetTrackingState(trackableId, tracking);
        }

        private void OnDisable()
        {
            tracking = TrackingState.None;
            FaceApi.SetTrackingState(trackableId, tracking);

            FaceApi.Remove(trackableId);
        }

        private void Update()
        {
            FaceApi.SetTrackingState(trackableId, tracking);
            if (tracking != TrackingState.Tracking) return;

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
            transform.rotation = Quaternion.Euler(0, 180, 0);
            FaceApi.Update(trackableId, pose, poseLeftEye, poseRightEye, mesh);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
