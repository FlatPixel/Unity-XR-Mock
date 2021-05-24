using UnityEngine;
using System.Collections;

namespace FlatPixel.XR.Mock
{
    [CreateAssetMenu(fileName = "SceneSimulationPath", menuName = "XR Mock/Scene simulation path", order = 1)]
    public class SimulationScenePath : ScriptableObject
    {
        public string scenePath;
        readonly public string defaultSimulationPath = "Packages/com.flatpixel.xr-mock/Runtime/Simulation/Scene/DefaultSimulationScene.unity";
    }
}