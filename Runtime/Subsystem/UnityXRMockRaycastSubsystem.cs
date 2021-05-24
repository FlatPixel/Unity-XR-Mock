using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace FlatPixel.XR.Mock
{
    public sealed class UnityXRMockRaycastSubsystem : XRRaycastSubsystem
    {
        public const string ID = "UnityXRMock-Raycast";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Register()
        {
            XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
            {
                id = "UnityXRMock-Raycast",
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MockProvider),
                subsystemTypeOverride = typeof(UnityXRMockRaycastSubsystem),
#else
                subsystemImplementationType = typeof(UnityXRMockRaycastSubsystem),
#endif
                supportedTrackableTypes = TrackableType.PlaneWithinPolygon,
                supportsViewportBasedRaycast = true,
                supportsWorldBasedRaycast = true,
                supportsTrackedRaycasts = true
            });
        }

#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MockProvider();
#endif

        private class MockProvider : Provider
        {
            ARSessionOrigin m_ARSessionOrigin = null;
            Camera m_mainCam = null;

            public override void Start()
            {
                m_ARSessionOrigin = Object.FindObjectOfType<ARSessionOrigin>();
                //Assertions.Assert.IsNotNull(m_ARSessionOrigin);

                m_mainCam = Camera.main;
                //Assertions.Assert.IsNotNull(m_mainCam);
            }

            public override NativeArray<XRRaycastHit> Raycast(XRRaycastHit defaultRaycastHit, Ray ray, TrackableType trackableTypeMask, Allocator allocator)
            {
                if (trackableTypeMask.HasFlag(TrackableType.PlaneWithinPolygon) == false)
                    return new NativeArray<XRRaycastHit>(0, allocator);

                ray = m_ARSessionOrigin.trackablesParent.TransformRay(ray);

                RaycastHit[] internalHits = Physics.RaycastAll(ray, 100.0f);

                int nbARPlane = 0;
                for (int i = 0; i < internalHits.Length; ++i)
                    if (internalHits[i].transform.GetComponent<ARPlane>() != null)
                        nbARPlane++;

                var originTransform = m_ARSessionOrigin.camera != null ? m_ARSessionOrigin.camera.transform : m_ARSessionOrigin.trackablesParent;

                NativeArray<XRRaycastHit> hits = new NativeArray<XRRaycastHit>(nbARPlane, allocator);

                RaycastHit hit;
                int index = 0;
                for (int i = 0; i < internalHits.Length; ++i)
                {
                    hit = internalHits[i];
                    if (internalHits[i].transform.GetComponent<ARPlane>() == null) continue;
                    
                    hits[index] = new XRRaycastHit
                    {
                        trackableId = hit.transform.GetComponent<ARPlane>().trackableId,
                        pose = new Pose(originTransform.parent.InverseTransformPoint(hit.point), Quaternion.FromToRotation(Vector3.up, hit.normal)),
                        distance = hit.distance / originTransform.parent.lossyScale.x,
                        hitType = TrackableType.PlaneWithinPolygon
                    };
                    index++;
                }

                return hits;
            }

            public override NativeArray<XRRaycastHit> Raycast(XRRaycastHit defaultRaycastHit, Vector2 screenPoint, TrackableType trackableTypeMask, Allocator allocator)
            {
                if (trackableTypeMask.HasFlag(TrackableType.PlaneWithinPolygon) == false)
                    return new NativeArray<XRRaycastHit>(0, allocator);

                Ray ray = m_mainCam.ViewportPointToRay(screenPoint);
                RaycastHit[] internalHits = Physics.RaycastAll(ray, 100.0f);

                int nbARPlane = 0;
                for (int i = 0; i < internalHits.Length; ++i)
                    if (internalHits[i].transform.GetComponent<ARPlane>() != null)
                        nbARPlane++;

                var originTransform = m_ARSessionOrigin.camera != null ? m_ARSessionOrigin.camera.transform : m_ARSessionOrigin.trackablesParent;

                NativeArray<XRRaycastHit> hits = new NativeArray<XRRaycastHit>(nbARPlane, allocator);

                RaycastHit hit;
                int index = 0;
                for (int i = 0; i < internalHits.Length; ++i)
                {
                    hit = internalHits[i];
                    if (internalHits[i].transform.GetComponent<ARPlane>() == null) continue;

                    hits[index] = new XRRaycastHit
                    {
                        trackableId = hit.transform.GetComponent<ARPlane>().trackableId,
                        pose = new Pose(originTransform.parent.InverseTransformPoint(hit.point), Quaternion.FromToRotation(Vector3.up, hit.normal)),
                        distance = hit.distance / originTransform.parent.lossyScale.x,
                        hitType = TrackableType.PlaneWithinPolygon
                    };

                    index++;
                }

                return hits;
            }
        }
    }
}