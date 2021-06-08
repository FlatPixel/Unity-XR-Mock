using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    [Preserve]
    public sealed class UnityXRMockSessionSubsystem : XRSessionSubsystem
    {
        public const string ID = "UnityXRMock-Session";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Register()
        {
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = ID,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MockProvider),
                subsystemTypeOverride = typeof(UnityXRMockSessionSubsystem),
#else
                subsystemImplementationType = typeof(UnityXRMockSessionSubsystem),
#endif
                supportsInstall = false,
                supportsMatchFrameRate = false
            });
        }

#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MockProvider();
#endif

        private class MockProvider : Provider
        {
            private bool isPaused = false;

            public MockProvider() { }

            public override Feature requestedFeatures => NativeApi.GetRequestedFeatures();

            public override Promise<SessionInstallationStatus> InstallAsync() => new SessionInstallationPromise();

            public override Promise<SessionAvailability> GetAvailabilityAsync() => new SessionAvailabilityPromise();

            public override unsafe NativeArray<ConfigurationDescriptor> GetConfigurationDescriptors(Allocator allocator)
            {
                var descriptors = new NativeArray<ConfigurationDescriptor>(2, allocator);

                Feature capabilities1 = Feature.WorldFacingCamera
                                        | Feature.PositionAndRotation
                                        | Feature.PlaneTracking
                                        | Feature.Raycast
                                        | Feature.PointCloud;
                descriptors[0] = new ConfigurationDescriptor(IntPtr.Zero + 1, capabilities1, 0);
                
                Feature capabilities2 = Feature.UserFacingCamera
                                        | Feature.PositionAndRotation
                                        | Feature.FaceTracking
                                        | Feature.Raycast;
                descriptors[1] = new ConfigurationDescriptor(IntPtr.Zero + 2, capabilities2, 2);
                
                return descriptors;
            }

#if UNITY_2020_2_OR_NEWER
            public override void Start() => SessionApi.Start();

            public override void Stop() => SessionApi.Stop();
#else
            public override void Resume() => SessionApi.Start();

            public override void Pause() => SessionApi.Stop();
#endif

            public override void Update(XRSessionUpdateParams updateParams, Configuration configuration)
            {
                if (this.trackingState == TrackingState.Limited && !this.isPaused)
                {
                    NativeApi.UnityXRMock_setTrackingState(TrackingState.Tracking);
                }
            }

            public override void OnApplicationPause()
            {
                this.isPaused = true;
                NativeApi.UnityXRMock_setTrackingState(TrackingState.Limited);
            }

            public override void OnApplicationResume()
            {
                this.isPaused = false;
            }

            public override IntPtr nativePtr => IntPtr.Zero;

            public override TrackingState trackingState => NativeApi.UnityXRMock_getTrackingState();
        }

        private class SessionInstallationPromise : Promise<SessionInstallationStatus>
        {
            public SessionInstallationPromise()
            {
                this.Resolve(SessionInstallationStatus.Success);
            }

            public override bool keepWaiting => false;

            protected override void OnKeepWaiting() { }
        }

        private class SessionAvailabilityPromise : Promise<SessionAvailability>
        {
            public SessionAvailabilityPromise()
            {
                this.Resolve(SessionAvailability.Supported | SessionAvailability.Installed);
            }

            public override bool keepWaiting => false;

            protected override void OnKeepWaiting() { }
        }
    }
}