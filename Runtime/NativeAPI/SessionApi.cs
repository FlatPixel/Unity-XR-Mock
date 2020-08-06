using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Mock
{
    public static class SessionApi
    {
        public static GameObject _arMockEditorPrefab = null;
        public static GameObject _arMockEditor = null;

        public static TrackingState trackingState
        {
            set
            {
                NativeApi.UnityXRMock_setTrackingState(value);
            }
        }

        public static bool Start()
        {
            Debug.Log("XRMock::Start Session");
            if (_arMockEditor == null)
                _arMockEditorPrefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.flatpixel.xr-mock/Runtime/Simulation/Prefabs/AR Mock Editor.prefab", typeof(GameObject));

            _arMockEditor = UnityEngine.Object.Instantiate(_arMockEditorPrefab);
            _arMockEditor.SetActive(true);

            return true;
        }

        public static bool Stop()
        {
            Debug.Log("XRMock::Stop Session");
            UnityEngine.Object.Destroy(_arMockEditor);
            _arMockEditor = null;

            return false;
        }
    }
}
