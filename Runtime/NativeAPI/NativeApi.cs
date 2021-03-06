using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    /// <summary>
    /// Provides functionality to inject data into the XRMock provider.
    /// </summary>
    public static class NativeApi
    {
        static NativeApi()
        {
            UnityXRMock_setTrackableIdGenerator(NewTrackableId);
        }

        public static void UnityXRMock_connectDevice(int id)
        {
            LogNotImplemented();
        }

        public static void UnityXRMock_disconnectDevice(int id)
        {
            LogNotImplemented();
        }

        public static void UnityXRMock_setPose(Pose pose, Matrix4x4 transform)
        {
            if (Camera.main == null) return;

            Camera.main.transform.localPosition = pose.position;
            Camera.main.transform.localRotation = pose.rotation;
            Camera.main.transform.localScale = transform.lossyScale;
        }

        public delegate IntPtr AddAnchorHandler(float px, float py, float pz,
            float rx, float ry, float rz, float rw);

        public delegate bool RequestRemoveAnchorDelegate(UInt64 id1, UInt64 id2);

        public static void UnityARMock_setAddAnchorHandler(
            AddAnchorHandler fp, RequestRemoveAnchorDelegate fp2)
        {
            LogNotImplemented();
        }

        //Test passing all params without need for marshaling
        public static bool UnityXRMock_addReferenceResultData(ulong id0, ulong id1, bool result, int trackingState)
        {
            LogNotImplemented();
            return false;
        }

        public static bool UnityXRMock_processPlaneEvent(IntPtr planeData, int size)
        {
            LogNotImplemented();
            return false;
        }

        public delegate void SetLightEstimationDelegate(bool enabled);

        public delegate void UnityProcessRaycastCallbackDelegate(float x, float y, byte type);

        public static void UnityARMock_setRaycastHandler(UnityProcessRaycastCallbackDelegate fp)
        {
            LogNotImplemented();
        }

        #region Features

        public static Feature requestedFeatures = Feature.None;

        public static Feature GetRequestedFeatures()
        {
            return requestedFeatures;
        }

        public static void SetFeatureRequested(Feature feature, bool enabled)
        {
            if (enabled)
            {
                requestedFeatures |= feature;

                if (feature == Feature.WorldFacingCamera)
                    CameraApi.currentFacingDirection = Feature.WorldFacingCamera;
                if (feature == Feature.UserFacingCamera)
                    CameraApi.currentFacingDirection = Feature.UserFacingCamera;
            }
            else
            {
                requestedFeatures &= ~feature;
            }
        }

        #endregion

        #region CameraProvider
        public static void UnityXRMock_setLightEstimation(SetLightEstimationDelegate fp)
        {
            LogNotImplemented();
        }

        public static void UnityXRMock_setCameraFrameData(IntPtr frameData)
        {
            LogNotImplemented();
        }
        #endregion

        public static TrackableId UnityXRMock_createTrackableId(string trackableId)
        {
            if (!string.IsNullOrWhiteSpace(trackableId))
            {
                string[] bytes = trackableId.Split('-');
                if (bytes.Length == 2)
                {
                    ulong subId1 = Convert.ToUInt64(bytes[0], 16);
                    ulong subId2 = Convert.ToUInt64(bytes[1], 16);
                    return new TrackableId(subId1, subId2);
                }
            }

            return TrackableId.invalidId;
        }

        public static TrackableId UnityXRMock_createTrackableId(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            ulong subId1 = BitConverter.ToUInt64(bytes, 0);
            ulong subId2 = BitConverter.ToUInt64(bytes, 8);
            return new TrackableId(subId1, subId2);
        }

        [MonoPInvokeCallback(typeof(Func<TrackableId>))]
        public static TrackableId NewTrackableId()
        {
            return UnityXRMock_createTrackableId(Guid.NewGuid());
        }

        private static TrackingState s_trackingState = TrackingState.Tracking;

        public static TrackingState UnityXRMock_getTrackingState()
        {
            return s_trackingState;
        }

        public static void UnityXRMock_setTrackingState(
            TrackingState trackingState)
        {
            s_trackingState = trackingState;
        }

        #region Plane APIs

        public class PlaneInfo
        {
            private const float AxisAlignmentEpsilon = 0.25f; // ~15 deg (arccos(0.25) ~= 75.52 deg

            public TrackableId id;
            public TrackableId subsumedById;
            public Pose pose;
            public Vector2 center;
            public Vector2 bounds;
            public Vector2[] boundaryPoints;
            public TrackingState trackingState;
            public int numPoints;

            public BoundedPlane ToBoundedPlane(BoundedPlane defaultPlane)
            {
                return new BoundedPlane(
                    this.id,
                    this.subsumedById,
                    this.pose,
                    this.center,
                    this.bounds,
                    GetAlignment(this.pose),
                    this.trackingState,
                    defaultPlane.nativePtr,
                    defaultPlane.classification);
            }

            public static PlaneAlignment GetAlignment(Pose pose)
            {
                var normal = pose.up;
                if (Mathf.Abs(normal.y) < AxisAlignmentEpsilon)
                {
                    return PlaneAlignment.Vertical;
                }
                else if (Mathf.Abs(normal.y) > (1.0f - AxisAlignmentEpsilon))
                {
                    return PlaneAlignment.HorizontalUp;
                }

                return PlaneAlignment.NotAxisAligned;
            }
        }

        private readonly static Dictionary<TrackableId, PlaneInfo> s_planes = new Dictionary<TrackableId, PlaneInfo>();
        private readonly static Dictionary<TrackableId, PlaneInfo> s_addedPlanes = new Dictionary<TrackableId, PlaneInfo>();
        private readonly static Dictionary<TrackableId, PlaneInfo> s_updatedPlanes = new Dictionary<TrackableId, PlaneInfo>();
        private readonly static Dictionary<TrackableId, PlaneInfo> s_removedPlanes = new Dictionary<TrackableId, PlaneInfo>();

        public static void UnityXRMock_setPlaneData(
            TrackableId planeId, Pose pose, Vector2 center, Vector2 bounds,
            Vector2[] boundaryPoints, int numPoints, TrackingState trackingState)
        {
            if (!s_planes.ContainsKey(planeId) || s_addedPlanes.ContainsKey(planeId))
            {
                if (!s_planes.ContainsKey(planeId))
                {
                    s_planes[planeId] = new PlaneInfo()
                    {
                        id = planeId
                    };
                }

                s_addedPlanes[planeId] = s_planes[planeId];
            }
            else
            {
                s_updatedPlanes[planeId] = s_planes[planeId];
            }

            var planeInfo = s_planes[planeId];
            planeInfo.pose = pose;
            planeInfo.center = center;
            planeInfo.bounds = bounds;
            planeInfo.boundaryPoints = boundaryPoints;
            planeInfo.numPoints = numPoints;
            planeInfo.trackingState = trackingState;
        }

        public static void UnityXRMock_setPlaneTrackingState(TrackableId planeId, TrackingState trackingState)
        {
            if (!s_planes.ContainsKey(planeId) || s_addedPlanes.ContainsKey(planeId))
            {
                if (!s_planes.ContainsKey(planeId))
                {
                    s_planes[planeId] = new PlaneInfo();
                }

                s_addedPlanes[planeId] = s_planes[planeId];
            }
            else
            {
                s_updatedPlanes[planeId] = s_planes[planeId];
            }

            var planeInfo = s_planes[planeId];
            planeInfo.trackingState = trackingState;
        }

        public static void UnityXRMock_removePlane(TrackableId planeId)
        {
            if (s_planes.ContainsKey(planeId))
            {
                if (!s_addedPlanes.Remove(planeId))
                {
                    s_removedPlanes[planeId] = s_planes[planeId];
                }

                s_planes.Remove(planeId);
                s_updatedPlanes.Remove(planeId);
            }
        }

        public static void UnityXRMock_subsumedPlane(TrackableId planeId, TrackableId subsumedById)
        {
            if (!s_planes.ContainsKey(planeId) || s_addedPlanes.ContainsKey(planeId))
            {
                if (!s_planes.ContainsKey(planeId))
                {
                    s_planes[planeId] = new PlaneInfo()
                    {
                        id = planeId
                    };
                }

                s_addedPlanes[planeId] = s_planes[planeId];
            }
            else
            {
                s_updatedPlanes[planeId] = s_planes[planeId];
            }

            var planeInfo = s_planes[planeId];
            planeInfo.subsumedById = subsumedById;
        }

        public static void UnityXRMock_consumedPlaneChanges()
        {
            s_addedPlanes.Clear();
            s_updatedPlanes.Clear();
            s_removedPlanes.Clear();
        }

        public static void UnityXRMock_planesReset()
        {
            UnityXRMock_consumedPlaneChanges();
            s_planes.Clear();
        }

        public static IDictionary<TrackableId, PlaneInfo> planes => s_planes;
        public static IEnumerable<PlaneInfo> addedPlanes => s_addedPlanes.Values;
        public static IEnumerable<PlaneInfo> updatedPlanes => s_updatedPlanes.Values.OrderBy(m => m.subsumedById != TrackableId.invalidId);
        public static IEnumerable<PlaneInfo> removedPlanes => s_removedPlanes.Values;

        #endregion

        public static void UnityXRMock_setDepthData(
            Vector3[] positions, float[] confidences, int count)
        {
            // TODO LogNotImplemented();
        }

        #region CameraApi.projectionMatrix

        private static Matrix4x4? projectionMatrix;

        public static Matrix4x4? UnityXRMock_getProjectionMatrix() => projectionMatrix;

        public static void UnityXRMock_setProjectionMatrix(Matrix4x4 projectionMatrix, Matrix4x4 inverseProjectionMatrix, bool hasValue)
        {
            if (hasValue)
            {
                NativeApi.projectionMatrix = projectionMatrix;
            }
            else
            {
                NativeApi.projectionMatrix = null;
            }
        }

        #endregion

        private static Func<TrackableId> s_trackableIdGenerator;

        public static Func<TrackableId> UnityXRMock_getTrackableIdGenerator()
        {
            return s_trackableIdGenerator;
        }

        public static void UnityXRMock_setTrackableIdGenerator(
            Func<TrackableId> generator)
        {
            s_trackableIdGenerator = generator;
        }

        #region Anchor APIs

        private readonly static Dictionary<TrackableId, AnchorInfo> s_anchors = new Dictionary<TrackableId, AnchorInfo>();
        private readonly static Dictionary<TrackableId, AnchorInfo> s_addedAnchors = new Dictionary<TrackableId, AnchorInfo>();
        private readonly static Dictionary<TrackableId, AnchorInfo> s_updatedAnchors = new Dictionary<TrackableId, AnchorInfo>();
        private readonly static Dictionary<TrackableId, AnchorInfo> s_removedAnchors = new Dictionary<TrackableId, AnchorInfo>();

        public class AnchorInfo
        {
            public TrackableId id;
            public Pose pose;
            public TrackingState trackingState;

            public XRAnchor ToXRAnchor(XRAnchor defaultAnchor)
            {
                return new XRAnchor(this.id, this.pose, this.trackingState, defaultAnchor.nativePtr);
            }
        }

        public static void UnityXRMock_setAnchorTrackingState(TrackableId id, TrackingState trackingState)
        {
            if (!s_anchors.ContainsKey(id) || s_addedAnchors.ContainsKey(id))
            {
                if (!s_anchors.ContainsKey(id))
                {
                    s_anchors[id] = new AnchorInfo();
                }

                s_addedAnchors[id] = s_anchors[id];
            }
            else
            {
                s_updatedAnchors[id] = s_anchors[id];
            }

            var anchorInfo = s_anchors[id];
            anchorInfo.trackingState = trackingState;
        }

        public static TrackableId UnityXRMock_attachAnchor(TrackableId trackableId, Pose pose)
        {
            if (trackableId == TrackableId.invalidId)
            {
                trackableId = s_trackableIdGenerator();
            }

            if (!s_anchors.ContainsKey(trackableId) || s_addedAnchors.ContainsKey(trackableId))
            {
                if (!s_anchors.ContainsKey(trackableId))
                {
                    s_anchors[trackableId] = new AnchorInfo()
                    {
                        id = trackableId
                    };
                }

                s_addedAnchors[trackableId] = s_anchors[trackableId];
            }
            else
            {
                s_updatedAnchors[trackableId] = s_anchors[trackableId];
            }

            var anchorInfo = s_anchors[trackableId];
            anchorInfo.pose = pose;
            anchorInfo.trackingState = TrackingState.Tracking;
            return anchorInfo.id;
        }

        public static void UnityXRMock_updateAnchor(TrackableId trackableId, Pose pose, TrackingState trackingState)
        {
            var anchorInfo = s_anchors[trackableId];
            anchorInfo.pose = pose;
            anchorInfo.trackingState = trackingState;
            s_updatedAnchors[trackableId] = anchorInfo;
        }

        public static void UnityXRMock_removeAnchor(TrackableId trackableId)
        {
            if (s_anchors.ContainsKey(trackableId))
            {
                if (!s_addedAnchors.Remove(trackableId))
                {
                    s_removedAnchors[trackableId] = s_anchors[trackableId];
                }

                s_anchors.Remove(trackableId);
                s_updatedAnchors.Remove(trackableId);
            }
        }

        public static void UnityXRMock_consumedAnchorChanges()
        {
            s_addedAnchors.Clear();
            s_updatedAnchors.Clear();
            s_removedAnchors.Clear();
        }

        public static void UnityXRMock_anchorReset()
        {
            UnityXRMock_consumedAnchorChanges();
            s_anchors.Clear();
        }

        public static IDictionary<TrackableId, AnchorInfo> anchors => s_anchors;
        public static IReadOnlyCollection<AnchorInfo> addedAnchors => s_addedAnchors.Values;
        public static IReadOnlyCollection<AnchorInfo> updatedAnchors => s_updatedAnchors.Values;
        public static IReadOnlyCollection<AnchorInfo> removedAnchors => s_removedAnchors.Values;

        #endregion

        #region Faces APIs

        [StructLayout(LayoutKind.Sequential)]
        public struct Face
        {
            public TrackableId m_TrackableId;
            public Pose m_Pose;
            public TrackingState m_TrackingState;
            public IntPtr m_NativePtr;
            public Pose m_LeftEyePose;
            public Pose m_RightEyePose;
            public Vector3 m_FixationPoint;
        }

        public class FaceInfo
        {
            public TrackableId id;
            public Pose pose;
            public TrackingState trackingState;
            public IntPtr nativePtr;
            public Pose leftEyePose;
            public Pose rightEyePose;
            public Vector3 fixationPoint;
            public Mesh mesh;

            public Face ToFace(XRFace defaultFace)
            {
                return new Face
                {
                    m_FixationPoint = fixationPoint,
                    m_TrackableId = id,
                    m_Pose = pose,
                    m_TrackingState = trackingState,
                    m_LeftEyePose = leftEyePose,
                    m_RightEyePose = rightEyePose,
                    m_NativePtr = defaultFace.nativePtr
                };
            }
        }

        public static void UnityXRMock_setFaceData(
            TrackableId id, Pose pose, Pose leftEye, Pose rightEye, Mesh mesh,
            TrackingState trackingState)
        {
            if (!s_faces.ContainsKey(id) || s_addedFaces.ContainsKey(id))
            {
                if (!s_faces.ContainsKey(id))
                {
                    s_faces[id] = new FaceInfo()
                    {
                        id = id
                    };
                }

                s_addedFaces[id] = s_faces[id];
            }
            else
            {
                s_updatedFaces[id] = s_faces[id];
            }

            var faceInfo = s_faces[id];
            faceInfo.pose = pose;
            faceInfo.leftEyePose = leftEye;
            faceInfo.rightEyePose = rightEye; 
            faceInfo.mesh = mesh;
            faceInfo.trackingState = trackingState;
        }

        public static void UnityXRMock_setFaceTrackingState(TrackableId id, TrackingState trackingState)
        {
            if (!s_faces.ContainsKey(id) || s_addedFaces.ContainsKey(id))
            {
                if (!s_faces.ContainsKey(id))
                {
                    s_faces[id] = new FaceInfo();
                }

                s_addedFaces[id] = s_faces[id];
            }
            else
            {
                s_updatedFaces[id] = s_faces[id];
            }

            var faceInfo = s_faces[id];
            faceInfo.trackingState = trackingState;
        }

        public static void UnityXRMock_removeFace(TrackableId id)
        {
            if (s_faces.ContainsKey(id))
            {
                if (!s_addedFaces.Remove(id))
                {
                    s_removedFaces[id] = s_faces[id];
                }

                s_faces.Remove(id);
                s_updatedFaces.Remove(id);
            }
        }

        public static void UnityXRMock_consumedFacesChanges()
        {
            s_addedFaces.Clear();
            s_updatedFaces.Clear();
            s_removedFaces.Clear();
        }

        public static void UnityXRMock_facesReset()
        {
            UnityXRMock_consumedFacesChanges();
            s_faces.Clear();
        }

        private readonly static Dictionary<TrackableId, FaceInfo> s_faces = new Dictionary<TrackableId, FaceInfo>();
        private readonly static Dictionary<TrackableId, FaceInfo> s_addedFaces = new Dictionary<TrackableId, FaceInfo>();
        private readonly static Dictionary<TrackableId, FaceInfo> s_updatedFaces = new Dictionary<TrackableId, FaceInfo>();
        private readonly static Dictionary<TrackableId, FaceInfo> s_removedFaces = new Dictionary<TrackableId, FaceInfo>();

        public static IDictionary<TrackableId, FaceInfo> faces => s_faces;
        public static IReadOnlyCollection<FaceInfo> addedFaces => s_addedFaces.Values;
        public static IReadOnlyCollection<FaceInfo> updatedFaces => s_updatedFaces.Values;
        public static IReadOnlyCollection<FaceInfo> removedFaces => s_removedFaces.Values;

        #endregion

        public static void UnityXRMock_setRaycastHits(
            XRRaycastHit[] hits, int size)
        {
            LogNotImplemented();
        }

        private static void LogNotImplemented([CallerMemberName] string memberName = "")
        {
            Debug.LogError($"{nameof(NativeApi)}.{memberName} not implemented");
        }
    }
}