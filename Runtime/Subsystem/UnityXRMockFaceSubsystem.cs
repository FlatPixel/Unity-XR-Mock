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

            struct TransformVerticesJob : IJobParallelFor
            {
                [ReadOnly]
                public NativeArray<Vector3> verticesIn;

                [WriteOnly]
                public NativeArray<Vector3> verticesOut;

                public void Execute(int i)
                {
                    verticesOut[i] = new Vector3(
                         verticesIn[i].x,
                         verticesIn[i].y,
                        -verticesIn[i].z);
                }
            }

            struct TransformUVsJob : IJobParallelFor
            {
                [ReadOnly]
                public NativeArray<Vector2> uvsIn;

                [WriteOnly]
                public NativeArray<Vector2> uvsOut;

                public void Execute(int i)
                {
                    uvsOut[i] = new Vector2(
                        uvsIn[i].x,
                        uvsIn[i].y);
                }
            }

            struct Triangle<T> where T : struct
            {
                public T a;
                public T b;
                public T c;

                public Triangle(T a, T b, T c)
                {
                    this.a = a;
                    this.b = c;
                    this.c = b;
                }
            }

            struct TransformIndicesJob : IJobParallelFor
            {
                [ReadOnly]
                public NativeArray<int> triangleIndicesIn;

                [WriteOnly]
                public NativeArray<Triangle<int>> triangleIndicesOut;

                public void Execute(int i)
                {
                    triangleIndicesOut[i] = new Triangle<int>(
                        triangleIndicesIn[i],
                        triangleIndicesIn[i] + 1,
                        triangleIndicesIn[i] + 2);
                }
            }

            public unsafe override void GetFaceMesh(
                TrackableId faceId,
                Allocator allocator,
                ref XRFaceMesh faceMesh)
            {
                if (!NativeApi.faces.ContainsKey(faceId))
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

                /*for(int i = 0; i < vertexCount; i++)
                {
                    Debug.Log("\tfaceMesh.vertices[" + i + "] = " + faceMesh.vertices[i].ToString());
                    Debug.Log("\tfaceMesh.normals[" + i + "] = " + faceMesh.normals[i].ToString());
                    Debug.Log("\tfaceMesh.uvs[" + i + "] = " + faceMesh.uvs[i].ToString());
                }

                for (int i = 0; i < triangleCount; i++)
                    Debug.Log("\tfaceMesh.indices[" + i + "] = " + faceMesh.indices[i].ToString());*/

                /*var vertexJobHandle = new TransformVerticesJob
                {
                    verticesIn = new NativeArray<Vector3>(xrFaceMesh.vertices, Allocator.Persistent),
                    verticesOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(faceMesh.vertices.GetUnsafePtr(), vertexCount, Allocator.Persistent)
                };

                var uvJobHandle = new TransformUVsJob
                {
                    uvsIn = new NativeArray<Vector2>(xrFaceMesh.uv, Allocator.Persistent),
                    uvsOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector2>(faceMesh.uvs.GetUnsafePtr(), vertexCount, Allocator.Persistent)
                };

                var normalJobHandle = new TransformVerticesJob
                {
                    verticesIn = new NativeArray<Vector3>(xrFaceMesh.normals, Allocator.Persistent),
                    verticesOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(faceMesh.normals.GetUnsafePtr(), vertexCount, Allocator.Persistent)
                };

                var indexJobHandle = new TransformIndicesJob
                {
                    triangleIndicesIn = new NativeArray<Int32>(xrFaceMesh.triangles, Allocator.Persistent),
                    // "cast" it to an array of Triangles
                    triangleIndicesOut = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Triangle<int>>(faceMesh.indices.GetUnsafePtr(), triangleCount, Allocator.Persistent)
                };

                // Wait on all four
                JobHandle.CombineDependencies(
                    JobHandle.CombineDependencies(vertexJobHandle.Schedule(vertexCount, 32), normalJobHandle.Schedule(vertexCount, 32)),
                    indexJobHandle.Schedule(triangleCount, 32), uvJobHandle.Schedule(vertexCount, 32)).Complete();

                vertexJobHandle.verticesIn.Dispose();
                vertexJobHandle.verticesOut.Dispose();
                normalJobHandle.verticesIn.Dispose();
                normalJobHandle.verticesOut.Dispose();
                uvJobHandle.uvsIn.Dispose();
                uvJobHandle.uvsOut.Dispose();
                indexJobHandle.triangleIndicesIn.Dispose();
                indexJobHandle.triangleIndicesOut.Dispose();*/
            }
        }
    }
}
