using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    public static class FaceApi
    {
        public static TrackableId Add(Pose pose, Pose leftEye, Pose rightEye, Mesh mesh,  TrackingState trackingState = TrackingState.Tracking)
        {
            var id = NativeApi.UnityXRMock_createTrackableId(Guid.NewGuid());
            s_TrackingStates[id] = trackingState;
            NativeApi.UnityXRMock_setFaceData(id, pose, leftEye, rightEye, mesh, trackingState);
            return id;
        }

        public static void Update(TrackableId id, Pose pose, Pose leftEye, Pose rightEye, Mesh mesh)
        {
            NativeApi.UnityXRMock_setFaceData(id, pose, leftEye, rightEye, mesh, s_TrackingStates[id]);
        }

        public static void SetTrackingState(TrackableId id, TrackingState trackingState)
        {
            if (!s_TrackingStates.ContainsKey(id))
                return;

            s_TrackingStates[id] = trackingState;
            NativeApi.UnityXRMock_setFaceTrackingState(id, s_TrackingStates[id]);
        }

        public static bool TryGetTrackingState(TrackableId id, out TrackingState trackingState)
        {
            return s_TrackingStates.TryGetValue(id, out trackingState);
        }

        public static void Remove(TrackableId id)
        {
            NativeApi.UnityXRMock_removeFace(id);
            s_TrackingStates.Remove(id);
        }

        static void SetFaceData(TrackableId id, Pose pose, Pose leftEye, Pose rightEye, Mesh mesh)
        {
            NativeApi.UnityXRMock_setFaceData(id, pose, leftEye, rightEye, mesh,
                s_TrackingStates[id]);
        }

        static Dictionary<TrackableId, TrackingState> s_TrackingStates = new Dictionary<TrackableId, TrackingState>();
    }
}
