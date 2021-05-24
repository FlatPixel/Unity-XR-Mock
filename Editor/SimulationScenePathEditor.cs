using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FlatPixel.XR.Mock
{
    [CustomEditor(typeof(SimulationScenePath))]
    public class SimulationScenePathEditor : Editor
    {
        SerializedProperty scenePathProp;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            scenePathProp = serializedObject.FindProperty ("scenePath");
        }

        public override void OnInspectorGUI()
        {
            SimulationScenePath simulationScenePath = (SimulationScenePath)target;

            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Here you can change the default simulation by another simulation provided in the samples or by your own. Simply change the path that point to the desired simiulation scene.", MessageType.Info, true);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
                GUILayout.Label("Simulation Scene Path:", GUILayout.Width(140));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
                var areaStyle = new GUIStyle(GUI.skin.textArea);
                areaStyle.wordWrap = true;
                var width = EditorGUIUtility.currentViewWidth;
                areaStyle.fixedHeight = 0; // reset height, else CalcHeight gives wrong numbers
                areaStyle.fixedHeight = areaStyle.CalcHeight(new GUIContent(scenePathProp.stringValue), width);
                scenePathProp.stringValue = EditorGUILayout.TextField(scenePathProp.stringValue, areaStyle, GUILayout.Height(areaStyle.fixedHeight));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset with default smulation scene"))
                // scenePathProp.stringValue = defaultSimulationPathProp.stringValue;
                scenePathProp.stringValue = simulationScenePath.defaultSimulationPath;
            GUILayout.EndHorizontal();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
    }
}