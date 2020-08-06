using UnityEngine;
using UnityEngine.XR.Management;
using System.IO;

namespace UnityEditor.XR.Mock
{
    /// <summary>
    /// Holds settings that are used to configure the Unity XRMock Plugin.
    /// </summary>
    [System.Serializable]
    [XRConfigurationData("XRMock", "UnityEditor.XR.Mock.XRMockSettings")]
    public class XRMockSettings : ScriptableObject
    {
        /// <summary>
        /// Enum which defines whether XRMock is optional or required.
        /// </summary>
        public enum Requirement
        {
            /// <summary>
            /// XRMock is required, which means the app cannot be installed on devices that do not support XRMock.
            /// </summary>
            Required,

            /// <summary>
            /// XRMock is optional, which means the the app can be installed on devices that do not support XRMock.
            /// </summary>
            Optional
        }

        [SerializeField, Tooltip("Toggles whether XRMock is required for this app. Will make app only downloadable by devices with XRMock support if set to 'Required'.")]
        Requirement m_Requirement;

        /// <summary>
        /// Determines whether XRMock is required for this app: will make app only downloadable by devices with XRMock support if set to <see cref="Requirement.Required"/>.
        /// </summary>
        public Requirement requirement
        {
            get { return m_Requirement; }
            set { m_Requirement = value; }
        }

        /// <summary>
        /// Gets the currently selected settings, or create a default one if no <see cref="XRMockSettings"/> has been set in Player Settings.
        /// </summary>
        /// <returns>The XRMock settings to use for the current Player build.</returns>
        public static XRMockSettings GetOrCreateSettings()
        {
            var settings = currentSettings;
            if (settings != null)
                return settings;

            return CreateInstance<XRMockSettings>();
        }

        /// <summary>
        /// Get or set the <see cref="XRMockSettings"/> that will be used for the player build.
        /// </summary>
        public static XRMockSettings currentSettings
        {
            get => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out XRMockSettings settings) ? settings : null;

            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(k_SettingsKey);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(k_SettingsKey, value, true);
                }
            }
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        internal static bool TrySelect()
        {
            var settings = currentSettings;
            if (settings == null)
                return false;

            Selection.activeObject = settings;
            return true;
        }

        void Awake()
        {
            if (EditorBuildSettings.TryGetConfigObject(k_OldConfigObjectName, out XRMockSettings result))
            {
                EditorBuildSettings.RemoveConfigObject(k_OldConfigObjectName);
            }
        }

        const string k_SettingsKey = "UnityEditor.XR.Mock.XRMockSettings";
        const string k_OldConfigObjectName = "com.unity.xr.mock.PlayerSettings";
    }
}
