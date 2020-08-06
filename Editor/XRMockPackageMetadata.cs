using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.Mock;

using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;

namespace UnityEditor.XR.Mock
{
    class XRPackage : IXRPackage
    {
        class XRMockLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        class XRMockPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

        static IXRPackageMetadata s_Metadata = new XRMockPackageMetadata()
        {
            packageName = "XR Mock Plugin",
            packageId = "com.flatpixel.xr-mock",
            settingsType = typeof(XRMockSettings).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new XRMockLoaderMetadata()
                {
                    loaderName = "XR Mock",
                    loaderType = typeof(XRMockLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.Standalone,
                        BuildTargetGroup.Unknown
                    }
                },
            }
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            if (obj is XRMockSettings settings)
            {
                settings.requirement = XRMockSettings.Requirement.Required;
                XRMockSettings.currentSettings = settings;
                return true;
            }

            return false;
        }
    }
}