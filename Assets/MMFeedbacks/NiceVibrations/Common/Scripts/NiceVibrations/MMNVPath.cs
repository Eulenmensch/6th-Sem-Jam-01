using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.NiceVibrations
{
    [CreateAssetMenu(fileName = "MMNVPathDefinition", menuName = "MoreMountains/NiceVibrations/MMNVPathDefinition")]
    public class MMNVPath : ScriptableObject
    {
        /// the path to the plugin in XCode (usually Libraries/NiceVibrations/Common/Plugins/iOS/Swift/)
        public string PluginPath;
        /// the name of the module (module.modulemap by default)
        public string ModuleFileName;
        /// the path in Unity (without Assets/, so usually NiceVibrations/Common/Plugins/Swift/)
        public string PluginRelativePath;
    }
}
