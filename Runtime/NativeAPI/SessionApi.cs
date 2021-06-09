using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARSubsystems;

namespace FlatPixel.XR.Mock
{
    public static class SessionApi
    {
        public static GameObject _arMockEditorPrefab = null;
        public static GameObject _arMockEditor = null;

        [HideInInspector]
        public static string _simulationScenePath = "Packages/com.flatpixel.xr-mock/Runtime/Simulation/Scene/DefaultSimulationScene.unity";
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
            SimulationScenePath scenePathSO = null;

            // Find all assets named SceneSimulationPath
            string[] guids = AssetDatabase.FindAssets("SceneSimulationPath");
            if (guids.Length >= 1)
            {
                string soAssetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                scenePathSO = (SimulationScenePath)AssetDatabase.LoadAssetAtPath(soAssetPath, typeof(SimulationScenePath));
            }

            // scenePathSO = Resources.Load<SimulationScenePath>("SceneSimulationPath");

            if (scenePathSO != null)
                _simulationScenePath = scenePathSO.scenePath;

            Debug.Log("_simulationScenePath: " + _simulationScenePath);
            LoadSceneParameters sceneParams = new LoadSceneParameters(LoadSceneMode.Additive);
            // _arMockSimulationScene = SceneManager.LoadScene(_simulationScenePath, sceneParams);
            _arMockSimulationScene = EditorSceneManager.LoadSceneInPlayMode(_simulationScenePath, sceneParams);

            return true;
        }

        public static bool Stop()
        {
            Debug.Log("XRMock::Stop Session");
            //Object.Destroy(_arMockEditor);
            //_arMockEditor = null;
            SceneManager.UnloadSceneAsync(_arMockSimulationScene);

            return false;
        }

        public static bool Reset()
        {
            if (_arMockSimulationScene.IsValid() == false) return false;

            GameObject[] gameObjects = _arMockSimulationScene.GetRootGameObjects();

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].name == "SessionSimulation")
                {
                    gameObjects[i].GetComponent<Example.SimulateSession>().Reset();
                }
            }

            return true;
        }
    }
}
