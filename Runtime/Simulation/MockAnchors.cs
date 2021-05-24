using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace FlatPixel.XR.Mock.Example
{
    public class MockAnchors : MockTrackable
    {
        [SerializeField]
        int m_Count = 4;

        [SerializeField]
        float m_Radius = 5f;

        List<TrackableId> _anchors;

        IEnumerator Start()
        {
            _anchors = new List<TrackableId>();

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < m_Count; ++i)
            {
                var position = Random.insideUnitSphere * m_Radius + transform.position;
                var rotation = Quaternion.AngleAxis(Random.Range(0, 360), Random.onUnitSphere);

                var trackableId = AnchorApi.Attach(new Pose(position, rotation));
                if (trackableId != null)
                    _anchors.Add(trackableId);

                yield return new WaitForSeconds(.5f);
            }

            var previousPosition = transform.localPosition;

            while (enabled)
            {
               if (transform.hasChanged)
               {
                   var delta = transform.position - previousPosition;
                   previousPosition = transform.position;

                   foreach (var trackableId in _anchors)
                   {
                       NativeApi.AnchorInfo anchor = NativeApi.anchors[trackableId];
                       var pose = new Pose(anchor.pose.position + delta, anchor.pose.rotation);
                       AnchorApi.Update(trackableId, pose, TrackingState.Tracking);

                       yield return new WaitForSeconds(.5f);
                   }

                   transform.hasChanged = false;
               }

               yield return null;
            }
        }
    }
}
