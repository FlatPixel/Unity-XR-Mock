using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    public static class AnchorApi
    {
        public static TrackableId Attach(Pose pose)
        {
            var trackableId = NativeApi.NewTrackableId();
            return NativeApi.UnityXRMock_attachAnchor(trackableId, pose);
        }

        public static void Attach(TrackableId trackableId, Pose pose)
        {
            NativeApi.UnityXRMock_attachAnchor(trackableId, pose);
        }

        public static void Update(TrackableId trackableId, Pose pose, TrackingState trackingState = TrackingState.Tracking)
        {
            NativeApi.UnityXRMock_updateAnchor(trackableId, pose, trackingState);
        }

        public static void Remove(TrackableId trackableId)
        {
            NativeApi.UnityXRMock_removeAnchor(trackableId);
        }

        public static void SetTrackingState(TrackableId id, TrackingState trackingState)
        {
            if (!s_TrackingStates.ContainsKey(id))
                return;

            s_TrackingStates[id] = trackingState;
            NativeApi.UnityXRMock_setAnchorTrackingState(id, s_TrackingStates[id]);
        }

        static Dictionary<TrackableId, TrackingState> s_TrackingStates = new Dictionary<TrackableId, TrackingState>();
    }
}