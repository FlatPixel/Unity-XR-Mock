using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    public static class SessionApi
    {
        public static GameObject _arMockEditorPrefab = null;
        public static GameObject _arMockEditor = null;
        private static Scene _arMockSimulationScene;

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
            //if (_arMockEditor == null)
            //    _arMockEditorPrefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.flatpixel.xr-mock/Runtime/Simulation/Prefabs/AR Mock Editor.prefab", typeof(GameObject));

            //_arMockEditor = Object.Instantiate(_arMockEditorPrefab);
            //_arMockEditor.SetActive(true);

            LoadSceneParameters sceneParams = new LoadSceneParameters(LoadSceneMode.Additive);
            _arMockSimulationScene = EditorSceneManager.LoadSceneInPlayMode("Packages/com.flatpixel.xr-mock/Runtime/Simulation/Scene/DefaultSimulationScene.unity", sceneParams);
            return true;
        }

        public static bool Stop()
        {
            Debug.Log("XRMock::Stop Session");
            //Object.Destroy(_arMockEditor);
            //_arMockEditor = null;
            SceneManager.UnloadSceneAsync("Packages/com.flatpixel.xr-mock/Samples/ScenesSimulation/HorizontalAndVerticalPlanesSimulation.unity");

            return false;
        }
    }
}
