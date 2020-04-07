using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HorizonBasedAmbientOcclusion.Universal
{
    [CustomEditor(typeof(HBAORendererSettings))]
    public class HBAORendererSettingsEditor : Editor
    {
        private HBAORendererSettings m_HBAORendererSettings;
        private Texture2D m_HBAOTex;
        private GUIStyle m_SettingsGroupStyle;
        private GUIStyle m_TitleLabelStyle;
        private int m_SelectedPreset;
        // settings group <setting, property reference>
        private Dictionary<FieldInfo, List<SerializedProperty>> m_GroupFields = new Dictionary<FieldInfo, List<SerializedProperty>>();
        private readonly Dictionary<int, HBAORendererFeature.Preset> m_Presets = new Dictionary<int, HBAORendererFeature.Preset>()
        {
            { 0, HBAORendererFeature.Preset.Normal },
            { 1, HBAORendererFeature.Preset.FastPerformance },
            { 2, HBAORendererFeature.Preset.FastestPerformance },
            { 3, HBAORendererFeature.Preset.Custom },
            { 4, HBAORendererFeature.Preset.HighQuality },
            { 5, HBAORendererFeature.Preset.HighestQuality }
        };


        void OnEnable()
        {
            m_HBAORendererSettings = (HBAORendererSettings)target;
            m_HBAOTex = Resources.Load<Texture2D>("hbao_urp");

            var settingsGroups = typeof(HBAORendererSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes(typeof(HBAORendererSettings.SettingsGroup), false).Any());
            foreach (var group in settingsGroups)
            {
                foreach (var setting in group.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!m_GroupFields.ContainsKey(group))
                        m_GroupFields[group] = new List<SerializedProperty>();

                    var property = serializedObject.FindProperty(group.Name + "." + setting.Name);
                    if (property != null)
                        m_GroupFields[group].Add(property);
                }
            }

            m_SelectedPreset = m_Presets.Values.ToList().IndexOf(m_HBAORendererSettings.GetCurrentPreset());
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SetStyles();

            EditorGUILayout.BeginVertical();
            {
                // header
                GUILayout.Space(10.0f);
                GUILayout.Label(m_HBAOTex, m_TitleLabelStyle, GUILayout.ExpandWidth(true));

                Event e = Event.current;

                // settings groups
                foreach (var group in m_GroupFields)
                {
                    var groupProperty = serializedObject.FindProperty(group.Key.Name);
                    if (groupProperty == null)
                        continue;

                    GUILayout.Space(6.0f);
                    Rect rect = GUILayoutUtility.GetRect(16f, 22f, m_SettingsGroupStyle);
                    var suffix = groupProperty.displayName == "Presets" ? "" : " Settings";
                    GUI.Box(rect, ObjectNames.NicifyVariableName(groupProperty.displayName + suffix), m_SettingsGroupStyle);
                    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                    {
                        groupProperty.isExpanded = !groupProperty.isExpanded;
                        e.Use();
                    }

                    if (!groupProperty.isExpanded)
                        continue;

                    // presets is a special case
                    if (group.Key.FieldType == typeof(HBAORendererSettings.Presets))
                    {
                        GUILayout.Space(6.0f);
                        m_SelectedPreset = GUILayout.SelectionGrid(m_SelectedPreset, m_Presets.Values.Select(x => ObjectNames.NicifyVariableName(x.ToString())).ToArray(), 3);
                        GUILayout.Space(6.0f);
                        if (GUILayout.Button("Apply Preset"))
                        {
                            Undo.RecordObject(target, "Apply Preset");
                            m_HBAORendererSettings.ApplyPreset(m_Presets[m_SelectedPreset]);
                            EditorUtility.SetDirty(target);
                            /*if (!EditorApplication.isPlaying)
                            {
                                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                            }*/
                        }
                    }

                    foreach (var field in group.Value)
                    {
                        // hide real presets
                        if (group.Key.FieldType == typeof(HBAORendererSettings.Presets))
                            continue;

                        // hide resolution when deinterleaved HBAO is on
                        if (group.Key.FieldType == typeof(HBAORendererSettings.GeneralSettings) && field.name == "resolution")
                        {
                            if (m_HBAORendererSettings.GetDeinterleaving() != HBAORendererFeature.Deinterleaving.Disabled)
                            {
                                continue;
                            }
                        }
                        // warn about deinterleaving not supported with SPSR
                        //else if (group.Key.FieldType == typeof(HBAORendererSettings.GeneralSettings) && field.name == "debugMode")
                        //{
                        //    #if UNITY_2019_3_OR_NEWER
                        //    if (m_HBAORendererSettings.GetDeinterleaving() != HBAORendererSettings.Deinterleaving.Disabled &&
                        //        IsVrRunning() && PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass)
                        //    {
                        //        GUILayout.Space(6.0f);
                        //        EditorGUILayout.HelpBox("Deinterleaving is not supported with Single Pass Stereo Rendering...", MessageType.Warning);
                        //    }
                        //    #else
                        //    if (m_HBAORendererSettings.GetDeinterleaving() != HBAORendererSettings.Deinterleaving.Disabled &&
                        //        PlayerSettings.virtualRealitySupported && PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass)
                        //    {
                        //        GUILayout.Space(6.0f);
                        //        EditorGUILayout.HelpBox("Deinterleaving is not supported with Single Pass Stereo Rendering...", MessageType.Warning);
                        //    }
                        //    #endif
                        //}
                        // hide noise type when deinterleaved HBAO is on
                        else if (group.Key.FieldType == typeof(HBAORendererSettings.GeneralSettings) && field.name == "noiseType")
                        {
                            if (m_HBAORendererSettings.GetDeinterleaving() != HBAORendererFeature.Deinterleaving.Disabled)
                            {
                                continue;
                            }
                        }
                        // hide multiBounceInfluence setting when not used
                        else if (group.Key.FieldType == typeof(HBAORendererSettings.AOSettings) && field.name == "multiBounceInfluence")
                        {
                            if (!m_HBAORendererSettings.UseMultiBounce())
                            {
                                continue;
                            }
                        }
                        // warn about distance falloff greater than max distance
                        else if (group.Key.FieldType == typeof(HBAORendererSettings.AOSettings) && field.name == "perPixelNormals")
                        {
                            if (m_HBAORendererSettings.GetAoDistanceFalloff() > m_HBAORendererSettings.GetAoMaxDistance())
                            {
                                GUILayout.Space(6.0f);
                                EditorGUILayout.HelpBox("Distance Falloff shoudn't be superior to Max Distance", MessageType.Warning);
                            }
                        }
                        else if (group.Key.FieldType == typeof(HBAORendererSettings.TemporalFilterSettings) && field.name == "enabled")
                        {
                            GUILayout.Space(6.0f);
                            EditorGUILayout.HelpBox("Currently Universal Render Pipeline does not generate motion vectors, awaiting support...", MessageType.Warning);

                            if (m_HBAORendererSettings.IsTemporalFilterEnabled())
                                m_HBAORendererSettings.EnableTemporalFilter(false);
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(12.0f);
                        EditorGUILayout.PropertyField(field);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void SetStyles()
        {
            // set banner label style
            m_TitleLabelStyle = new GUIStyle(GUI.skin.label);
            m_TitleLabelStyle.alignment = TextAnchor.MiddleCenter;
            m_TitleLabelStyle.contentOffset = new Vector2(0f, 0f);

            // get shuriken module title style
            GUIStyle skurikenModuleTitleStyle = "ShurikenModuleTitle";

            // clone it as to not interfere with the original, and adjust it
            m_SettingsGroupStyle = new GUIStyle(skurikenModuleTitleStyle);
            m_SettingsGroupStyle.font = (new GUIStyle("Label")).font;
            m_SettingsGroupStyle.fontStyle = FontStyle.Bold;
            m_SettingsGroupStyle.border = new RectOffset(15, 7, 4, 4);
            m_SettingsGroupStyle.fixedHeight = 22;
            m_SettingsGroupStyle.contentOffset = new Vector2(10f, -2f);
        }

        /*
        #if UNITY_2019_3_OR_NEWER
        List<UnityEngine.XR.XRDisplaySubsystemDescriptor> displaysDescs = new List<UnityEngine.XR.XRDisplaySubsystemDescriptor>();
        List<UnityEngine.XR.XRDisplaySubsystem> displays = new List<UnityEngine.XR.XRDisplaySubsystem>();

        private bool IsVrRunning()
        {
            bool vrIsRunning = false;
            displays.Clear();
            SubsystemManager.GetInstances(displays);
            foreach (var displaySubsystem in displays)
            {
                if (displaySubsystem.running)
                {
                    vrIsRunning = true;
                    break;
                }
            }

            return vrIsRunning;
        }
        #endif
        */

        [CustomPropertyDrawer(typeof(HBAORendererSettings.MinMaxSliderAttribute))]
        public class MinMaxSliderDrawer : PropertyDrawer
        {

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {

                if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    Vector2 range = property.vector2Value;
                    float min = range.x;
                    float max = range.y;
                    var attr = attribute as HBAORendererSettings.MinMaxSliderAttribute;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.MinMaxSlider(position, label, ref min, ref max, attr.min, attr.max);
                    if (EditorGUI.EndChangeCheck())
                    {
                        range.x = min;
                        range.y = max;
                        property.vector2Value = range;
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label, "Use only with Vector2");
                }
            }
        }
    }
}
