using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace FlatPixel.XR.Mock
{
    /// <summary>
    /// Manages the lifecycle of XRMock subsystems.
    /// </summary>
    public class XRMockLoader : XRLoaderHelper
    {
        GameObject _arMockEditorPrefab;
        GameObject _arMockEditor;

        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
        static List<XRDepthSubsystemDescriptor> s_DepthSubsystemDescriptors = new List<XRDepthSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRFaceSubsystemDescriptor> s_FaceSubsystemDescriptors = new List<XRFaceSubsystemDescriptor>();
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
        static List<XREnvironmentProbeSubsystemDescriptor> s_EnvironmentProbeSubsystemDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
        static List<XROcclusionSubsystemDescriptor> s_OcclusionSubsystemDescriptors = new List<XROcclusionSubsystemDescriptor>();

        /// <summary>
        /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();

        /// <summary>
        /// The `XRCameraSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRCameraSubsystem cameraSubsystem => GetLoadedSubsystem<XRCameraSubsystem>();

        /// <summary>
        /// The `XRDepthSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRDepthSubsystem depthSubsystem => GetLoadedSubsystem<XRDepthSubsystem>();

        /// <summary>
        /// The `XRPlaneSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();

        /// <summary>
        /// The `XRAnchorSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRAnchorSubsystem anchorSubsystem => GetLoadedSubsystem<XRAnchorSubsystem>();

        /// <summary>
        /// The `XRRaycastSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();

        /// <summary>
        /// The `XRImageTrackingSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRImageTrackingSubsystem imageTrackingSubsystem => GetLoadedSubsystem<XRImageTrackingSubsystem>();

        /// <summary>
        /// The `XRInputSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();

        /// <summary>
        /// The `XRFaceSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XRFaceSubsystem faceSubsystem => GetLoadedSubsystem<XRFaceSubsystem>();

        /// <summary>
        /// The `XREnvironmentProbeSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XREnvironmentProbeSubsystem environmentProbeSubsystem => GetLoadedSubsystem<XREnvironmentProbeSubsystem>();

        /// <summary>
        /// The `XROcclusionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        public XROcclusionSubsystem occlusionSubsystem => GetLoadedSubsystem<XROcclusionSubsystem>();

        /// <summary>
        /// Initializes the loader.
        /// </summary>
        /// <returns>`True` if the session subsystem was successfully created, otherwise `false`.</returns>
        public override bool Initialize()
        {
#if UNITY_EDITOR
            Debug.Log("XRMock::Initialize");

            // if (_arMockEditor == null)
            //     _arMockEditorPrefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.flatpixel.xr-mock/Runtime/Simulation/Prefabs/AR Mock Editor.prefab", typeof(GameObject));

            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, "UnityXRMock-Session");
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, "UnityXRMock-Camera");
            CreateSubsystem<XRDepthSubsystemDescriptor, XRDepthSubsystem>(s_DepthSubsystemDescriptors, "UnityXRMock-Depth");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, "UnityXRMock-Plane");
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, "UnityXRMock-Anchor");
            CreateSubsystem<XRFaceSubsystemDescriptor, XRFaceSubsystem>(s_FaceSubsystemDescriptors, "UnityXRMock-Face");
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, "UnityXRMock-Raycast");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "UnityXRMock-Input");

            if (sessionSubsystem == null)
            {
                Debug.LogError("Failed to load session subsystem.");
            }

            return sessionSubsystem != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// Starts all subsystems.
        /// </summary>
        /// <returns>`True` if the subsystems were started, otherwise `false`.</returns>
        public override bool Start()
        {
            return true;
        }

        /// <summary>
        /// Stops all subsystems.
        /// </summary>
        /// <returns>`True` if the subsystems were stopped, otherwise `false`.</returns>
        public override bool Stop()
        {
            return true;
        }

        /// <summary>
        /// Destroys each subsystem.
        /// </summary>
        /// <returns>Always returns `true`.</returns>
        public override bool Deinitialize()
        {
            Debug.Log("XRMock::Deinitialize");
#if UNITY_EDITOR
            DestroySubsystem<XRCameraSubsystem>();
            DestroySubsystem<XRDepthSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XRFaceSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
#endif
            return true;
        }
    }
}
