using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock.Example
{
    public class MockTrackable : MonoBehaviour
    {
        protected TrackableId trackableId;
        protected Pose pose { get { return new Pose(transform.position, transform.rotation); } }
        
        [Header("Duration Before being tracked")]
        [SerializeField]
        public float untrackedDuration = 0;

        [Header("Tracking state")]
        [SerializeField]
        public TrackingState tracking = TrackingState.None;
    }
}
