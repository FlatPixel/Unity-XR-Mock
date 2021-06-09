using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class MockAnchor : MockTrackable
    {
        private void OnEnable()
        {
            trackableId = AnchorApi.Attach(pose);

            tracking = TrackingState.Tracking;
            AnchorApi.SetTrackingState(trackableId, tracking);
        }

        private void OnDisable()
        {
            tracking = TrackingState.None;
            AnchorApi.SetTrackingState(trackableId, tracking);

            AnchorApi.Remove(trackableId);
        }

        private void Update()
        {
            AnchorApi.SetTrackingState(trackableId, tracking);
            if (tracking != TrackingState.Tracking) return;

            AnchorApi.Update(trackableId, pose, TrackingState.Tracking);
        }
    }
}
