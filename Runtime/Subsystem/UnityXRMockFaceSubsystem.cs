using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    [Preserve]
    public sealed class UnityXRMockFaceSubsystem : XRFaceSubsystem
    {
        public const string ID = "UnityXRMock-Face";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Register()
        {
            XRFaceSubsystemDescriptor.Create(new FaceSubsystemParams
            {
                id = ID,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MockProvider),
                subsystemTypeOverride = typeof(UnityXRMockFaceSubsystem),
#else
                subsystemImplementationType = typeof(UnityXRMockFaceSubsystem),
#endif
                supportsEyeTracking = true,
                supportsFaceMeshNormals = true,
                supportsFaceMeshUVs = true,
                supportsFaceMeshVerticesAndIndices = true,
                supportsFacePose = true
            });
        }

#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MockProvider();
#endif

        private class MockProvider : Provider
        {
            public override void Start() { }

            public override void Destroy()
            {
                NativeApi.UnityXRMock_facesReset();
            }

            public override void Stop() { }

            public override TrackableChanges<XRFace> GetChanges(XRFace defaultFace, Allocator allocator)
            {
                try
                {
                    var addedToFace = new NativeArray<NativeApi.Face>(NativeApi.addedFaces.Select(m => m.ToFace(defaultFace)).ToArray(), allocator);
                    var added = new NativeArray<XRFace>(addedToFace.Reinterpret<XRFace>(), allocator);

                    var updatedToFace = new NativeArray<NativeApi.Face>(NativeApi.updatedFaces.Select(m => m.ToFace(defaultFace)).ToArray(), allocator);

                    return TrackableChanges<XRFace>.CopyFrom(
                        added,
                        new NativeArray<XRFace>(
                            updatedToFace.Reinterpret<XRFace>(), allocator),
                        new NativeArray<TrackableId>(
                            NativeApi.removedFaces.Select(m => m.id).ToArray(), allocator),
                        allocator);
                }
                finally
                {
                    NativeApi.UnityXRMock_consumedFacesChanges();
                }
            }


            public unsafe override void GetFaceMesh(
                TrackableId faceId,
                Allocator allocator,
                ref XRFaceMesh faceMesh)
            {
                if (NativeApi.faces.ContainsKey(faceId) == false || NativeApi.faces[faceId].mesh == null)
                { 
                    faceMesh.Dispose();
                    faceMesh = default(XRFaceMesh);
                    return;
                }

                var xrFaceMesh = NativeApi.faces[faceId].mesh;
                var vertexCount = xrFaceMesh.vertexCount;
                var triangleCount = xrFaceMesh.triangles.Count();


                faceMesh.Resize(
                        vertexCount,
                        triangleCount,
                        XRFaceMesh.Attributes.Normals | XRFaceMesh.Attributes.UVs,
                        allocator);

                NativeArray<Vector3>.Copy(xrFaceMesh.vertices, faceMesh.vertices, vertexCount);
                NativeArray<Vector3>.Copy(xrFaceMesh.normals, faceMesh.normals, vertexCount);
                NativeArray<Vector2>.Copy(xrFaceMesh.uv, faceMesh.uvs, vertexCount);
                NativeArray<int>.Copy(xrFaceMesh.triangles, faceMesh.indices, triangleCount);
            }
        }
    }
}
