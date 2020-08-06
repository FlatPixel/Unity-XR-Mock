using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.Mock.Example
{
    public class SimulateAnchors : MonoBehaviour
    {
        [SerializeField]
        ARAnchorManager _anchorManager;

        [SerializeField]
        int m_Count = 4;

        [SerializeField]
        float m_Radius = 5f;

        List<ARAnchor> _anchors;

        IEnumerator Start()
        {
            _anchors = new List<ARAnchor>();

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < m_Count; ++i)
            {
                var position = Random.insideUnitSphere * m_Radius + transform.position;
                var rotation = Quaternion.AngleAxis(Random.Range(0, 360), Random.onUnitSphere);

                var referencePoint = _anchorManager.AddAnchor(new Pose(position, rotation));
                if (referencePoint != null)
                    _anchors.Add(referencePoint);

                yield return new WaitForSeconds(.5f);
            }

            var previousPosition = transform.localPosition;

            while (enabled)
            {
                if (transform.hasChanged)
                {
                    var delta = transform.position - previousPosition;
                    previousPosition = transform.position;

                    foreach (var referencePoint in _anchors)
                    {
                        var pose = new Pose(referencePoint.transform.position + delta, referencePoint.transform.rotation);
                        AnchorApi.Update(referencePoint.trackableId, pose, TrackingState.Tracking);

                        yield return new WaitForSeconds(.5f);
                    }

                    transform.hasChanged = false;
                }

                yield return null;
            }
        }
    }
}
