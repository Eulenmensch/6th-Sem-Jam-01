using System;
using UnityEngine;

namespace HorizonBasedAmbientOcclusion.Universal
{
    [CreateAssetMenu(fileName = "HBAORendererSettings", menuName = "HBAO/HBAO Renderer Settings", order = 100)]
    public class HBAORendererSettings : ScriptableObject
    {
        [Serializable]
        public struct Presets
        {
            public HBAORendererFeature.Preset preset;

            [SerializeField]
            public static Presets defaults
            {
                get
                {
                    return new Presets
                    {
                        preset = HBAORendererFeature.Preset.Normal
                    };
                }
            }
        }

        [Serializable]
        public struct GeneralSettings
        {
            public bool enable;

            [Tooltip("The quality of the AO.")]
            [Space(10)]
            public HBAORendererFeature.Quality quality;

            [Tooltip("The deinterleaving factor.")]
            public HBAORendererFeature.Deinterleaving deinterleaving;

            [Tooltip("The resolution at which the AO is calculated.")]
            public HBAORendererFeature.Resolution resolution;

            [Tooltip("The type of noise to use.")]
            [Space(10)]
            public HBAORendererFeature.NoiseType noiseType;

            [Tooltip("The debug mode actually displayed on screen.")]
            [Space(10)]
            public HBAORendererFeature.DebugMode debugMode;

            [SerializeField]
            public static GeneralSettings defaults
            {
                get
                {
                    return new GeneralSettings
                    {
                        enable = true,
                        quality = HBAORendererFeature.Quality.Medium,
                        deinterleaving = HBAORendererFeature.Deinterleaving.Disabled,
                        resolution = HBAORendererFeature.Resolution.Full,
                        noiseType = HBAORendererFeature.NoiseType.Dither,
                        debugMode = HBAORendererFeature.DebugMode.Disabled
                    };
                }
            }
        }

        [Serializable]
        public struct AOSettings
        {
            [Tooltip("AO radius: this is the distance outside which occluders are ignored.")]
            [Space(6), Range(0.25f, 5f)]
            public float radius;

            [Tooltip("Maximum radius in pixels: this prevents the radius to grow too much with close-up " +
                      "object and impact on performances.")]
            [Range(16, 256)]
            public float maxRadiusPixels;

            [Tooltip("For low-tessellated geometry, occlusion variations tend to appear at creases and " +
                     "ridges, which betray the underlying tessellation. To remove these artifacts, we use " +
                     "an angle bias parameter which restricts the hemisphere.")]
            [Range(0, 0.5f)]
            public float bias;

            [Tooltip("This value allows to scale up the ambient occlusion values.")]
            [Range(0, 4)]
            public float intensity;

            [Tooltip("Enable/disable MultiBounce approximation.")]
            public bool useMultiBounce;

            [Tooltip("MultiBounce approximation influence.")]
            [Range(0, 1)]
            public float multiBounceInfluence;

            [Tooltip("The amount of AO offscreen samples are contributing.")]
            [Range(0, 1)]
            public float offscreenSamplesContribution;

            [Tooltip("The max distance to display AO.")]
            [Space(10)]
            public float maxDistance;

            [Tooltip("The distance before max distance at which AO start to decrease.")]
            public float distanceFalloff;

            [Tooltip("The type of per pixel normals to use.")]
            [Space(10)]
            public HBAORendererFeature.PerPixelNormals perPixelNormals;

            [Tooltip("This setting allow you to set the base color if the AO, the alpha channel value is unused.")]
            [Space(10)]
            public Color baseColor;

            [SerializeField]
            public static AOSettings defaults
            {
                get
                {
                    return new AOSettings
                    {
                        radius = 0.8f,
                        maxRadiusPixels = 128f,
                        bias = 0.05f,
                        intensity = 1f,
                        useMultiBounce = false,
                        multiBounceInfluence = 1f,
                        offscreenSamplesContribution = 0f,
                        maxDistance = 150f,
                        distanceFalloff = 50f,
                        perPixelNormals = HBAORendererFeature.PerPixelNormals.Reconstruct4Samples,
                        baseColor = Color.black
                    };
                }
            }
        }

        [Serializable]
        public struct TemporalFilterSettings
        {
            [Space(6)]
            public bool enabled;

            [Tooltip("The type of variance clipping to use.")]
            public HBAORendererFeature.VarianceClipping varianceClipping;

            [SerializeField]
            public static TemporalFilterSettings defaultSettings
            {
                get
                {
                    return new TemporalFilterSettings
                    {
                        enabled = false,
                        varianceClipping = HBAORendererFeature.VarianceClipping._4Tap
                    };
                }
            }
        }

        [Serializable]
        public struct BlurSettings
        {
            [Tooltip("The type of blur to use.")]
            [Space(6)]
            public HBAORendererFeature.BlurType type;

            [Tooltip("This parameter controls the depth-dependent weight of the bilateral filter, to " +
                     "avoid bleeding across edges. A zero sharpness is a pure Gaussian blur. Increasing " +
                     "the blur sharpness removes bleeding by using lower weights for samples with large " +
                     "depth delta from the current pixel.")]
            [Space(10), Range(0, 16)]
            public float sharpness;

            [SerializeField]
            public static BlurSettings defaults
            {
                get
                {
                    return new BlurSettings
                    {
                        type = HBAORendererFeature.BlurType.Medium,
                        sharpness = 8f
                    };
                }
            }
        }

        [Serializable]
        public struct ColorBleedingSettings
        {
            [Space(6)]
            public bool enabled;

            [Tooltip("This value allows to control the saturation of the color bleeding.")]
            [Space(10), Range(0, 4)]
            public float saturation;

            [Tooltip("Use masking on emissive pixels")]
            [Range(0, 1)]
            public float brightnessMask;

            [Tooltip("Brightness level where masking starts/ends")]
            [MinMaxSlider(0, 2)]
            public Vector2 brightnessMaskRange;

            [SerializeField]
            public static ColorBleedingSettings defaults
            {
                get
                {
                    return new ColorBleedingSettings
                    {
                        enabled = false,
                        saturation = 1f,
                        brightnessMask = 1f,
                        brightnessMaskRange = new Vector2(0.0f, 0.5f)
                    };
                }
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class SettingsGroup : Attribute { }

        [SerializeField, SettingsGroup]
        private Presets m_Presets = Presets.defaults;
        public Presets presets
        {
            get { return m_Presets; }
            set { m_Presets = value; }
        }

        [SerializeField, SettingsGroup]
        private GeneralSettings m_General = GeneralSettings.defaults;
        public GeneralSettings general
        {
            get { return m_General; }
            set { m_General = value; }
        }

        [SerializeField, SettingsGroup]
        private AOSettings m_AO = AOSettings.defaults;
        public AOSettings ao
        {
            get { return m_AO; }
            set { m_AO = value; }
        }

        [SerializeField, SettingsGroup]
        private TemporalFilterSettings m_TemporalFilter = TemporalFilterSettings.defaultSettings;
        public TemporalFilterSettings temporalFilter
        {
            get { return m_TemporalFilter; }
            set { m_TemporalFilter = value; }
        }

        [SerializeField, SettingsGroup]
        private BlurSettings m_Blur = BlurSettings.defaults;
        public BlurSettings blur
        {
            get { return m_Blur; }
            set { m_Blur = value; }
        }

        [SerializeField, SettingsGroup]
        private ColorBleedingSettings m_ColorBleeding = ColorBleedingSettings.defaults;
        public ColorBleedingSettings colorBleeding
        {
            get { return m_ColorBleeding; }
            set { m_ColorBleeding = value; }
        }

        public class MinMaxSliderAttribute : PropertyAttribute
        {
            public readonly float max;
            public readonly float min;

            public MinMaxSliderAttribute(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public bool IsHBAOEnabled()
        {
            return m_General.enable;
        }

        public void EnableHBAO(bool enable)
        {
            m_General.enable = enable;
        }

        public void ToggleHBAO()
        {
            m_General.enable = !m_General.enable;
        }

        public HBAORendererFeature.Preset GetCurrentPreset()
        {
            return m_Presets.preset;
        }

        public void ApplyPreset(HBAORendererFeature.Preset preset)
        {
            if (preset == HBAORendererFeature.Preset.Custom)
            {
                m_Presets.preset = preset;
                return;
            }

            var debugMode = general.debugMode;

            m_General = GeneralSettings.defaults;
            m_AO = AOSettings.defaults;
            m_ColorBleeding = ColorBleedingSettings.defaults;
            m_Blur = BlurSettings.defaults;

            SetDebugMode(debugMode);

            switch (preset)
            {
                case HBAORendererFeature.Preset.FastestPerformance:
                    SetQuality(HBAORendererFeature.Quality.Lowest);
                    SetAoRadius(0.5f);
                    SetAoMaxRadiusPixels(64.0f);
                    SetBlurType(HBAORendererFeature.BlurType.ExtraWide);
                    break;
                case HBAORendererFeature.Preset.FastPerformance:
                    SetQuality(HBAORendererFeature.Quality.Low);
                    SetAoRadius(0.5f);
                    SetAoMaxRadiusPixels(64.0f);
                    SetBlurType(HBAORendererFeature.BlurType.Wide);
                    break;
                case HBAORendererFeature.Preset.HighQuality:
                    SetQuality(HBAORendererFeature.Quality.High);
                    SetAoRadius(1.0f);
                    break;
                case HBAORendererFeature.Preset.HighestQuality:
                    SetQuality(HBAORendererFeature.Quality.Highest);
                    SetAoRadius(1.2f);
                    SetAoMaxRadiusPixels(256.0f);
                    SetBlurType(HBAORendererFeature.BlurType.Narrow);
                    break;
                case HBAORendererFeature.Preset.Normal:
                default:
                    break;
            }

            m_Presets.preset = preset;
        }

        public HBAORendererFeature.Quality GetQuality()
        {
            return m_General.quality;
        }

        public void SetQuality(HBAORendererFeature.Quality quality)
        {
            m_General.quality = quality;
        }

        public HBAORendererFeature.Deinterleaving GetDeinterleaving()
        {
            return m_General.deinterleaving;
        }

        public void SetDeinterleaving(HBAORendererFeature.Deinterleaving deinterleaving)
        {
            m_General.deinterleaving = deinterleaving;
        }

        public HBAORendererFeature.Resolution GetResolution()
        {
            return m_General.resolution;
        }

        public void SetResolution(HBAORendererFeature.Resolution resolution)
        {
            m_General.resolution = resolution;
        }

        public HBAORendererFeature.NoiseType GetNoiseType()
        {
            return m_General.noiseType;
        }

        public void SetNoiseType(HBAORendererFeature.NoiseType noiseType)
        {
            m_General.noiseType = noiseType;
        }

        public HBAORendererFeature.DebugMode GetDebugMode()
        {
            return m_General.debugMode;
        }

        public void SetDebugMode(HBAORendererFeature.DebugMode debugMode)
        {
            m_General.debugMode = debugMode;
        }

        public float GetAoRadius()
        {
            return m_AO.radius;
        }

        public void SetAoRadius(float radius)
        {
            m_AO.radius = Mathf.Clamp(radius, 0.25f, 5);
        }

        public float GetAoMaxRadiusPixels()
        {
            return m_AO.maxRadiusPixels;
        }

        public void SetAoMaxRadiusPixels(float maxRadiusPixels)
        {
            m_AO.maxRadiusPixels = Mathf.Clamp(maxRadiusPixels, 16, 256);
        }

        public float GetAoBias()
        {
            return m_AO.bias;
        }

        public void SetAoBias(float bias)
        {
            m_AO.bias = Mathf.Clamp(bias, 0, 0.5f);
        }

        public float GetAoOffscreenSamplesContribution()
        {
            return m_AO.offscreenSamplesContribution;
        }

        public void SetAoOffscreenSamplesContribution(float contribution)
        {
            m_AO.offscreenSamplesContribution = Mathf.Clamp01(contribution);
        }

        public float GetAoMaxDistance()
        {
            return m_AO.maxDistance;
        }

        public void SetAoMaxDistance(float maxDistance)
        {
            m_AO.maxDistance = maxDistance;
        }

        public float GetAoDistanceFalloff()
        {
            return m_AO.distanceFalloff;
        }

        public void SetAoDistanceFalloff(float distanceFalloff)
        {
            m_AO.distanceFalloff = distanceFalloff;
        }

        public HBAORendererFeature.PerPixelNormals GetAoPerPixelNormals()
        {
            return m_AO.perPixelNormals;
        }

        public void SetAoPerPixelNormals(HBAORendererFeature.PerPixelNormals perPixelNormals)
        {
            m_AO.perPixelNormals = perPixelNormals;
        }

        public Color GetAoColor()
        {
            return m_AO.baseColor;
        }

        public void SetAoColor(Color color)
        {
            m_AO.baseColor = color;
        }

        public float GetAoIntensity()
        {
            return m_AO.intensity;
        }

        public void SetAoIntensity(float intensity)
        {
            m_AO.intensity = Mathf.Clamp(intensity, 0, 4);
        }

        public bool UseMultiBounce()
        {
            return m_AO.useMultiBounce;
        }

        public void EnableMultiBounce(bool enabled = true)
        {
            m_AO.useMultiBounce = enabled;
        }

        public float GetAoMultiBounceInfluence()
        {
            return m_AO.multiBounceInfluence;
        }

        public void SetAoMultiBounceInfluence(float multiBounceInfluence)
        {
            m_AO.multiBounceInfluence = Mathf.Clamp01(multiBounceInfluence);
        }

        public bool IsTemporalFilterEnabled()
        {
            return m_TemporalFilter.enabled;
        }

        public void EnableTemporalFilter(bool enabled = true)
        {
            m_TemporalFilter.enabled = enabled;
        }

        public HBAORendererFeature.VarianceClipping GetTemporalFilterVarianceClipping()
        {
            return m_TemporalFilter.varianceClipping;
        }

        public void SetTemporalFilterVarianceClipping(HBAORendererFeature.VarianceClipping varianceClipping)
        {
            m_TemporalFilter.varianceClipping = varianceClipping;
        }

        public HBAORendererFeature.BlurType GetBlurType()
        {
            return m_Blur.type;
        }

        public void SetBlurType(HBAORendererFeature.BlurType blurType)
        {
            m_Blur.type = blurType;
        }

        public float GetBlurSharpness()
        {
            return m_Blur.sharpness;
        }

        public void SetBlurSharpness(float sharpness)
        {
            m_Blur.sharpness = Mathf.Clamp(sharpness, 0, 16);
        }

        public bool IsColorBleedingEnabled()
        {
            return m_ColorBleeding.enabled;
        }

        public void EnableColorBleeding(bool enabled = true)
        {
            m_ColorBleeding.enabled = enabled;
        }

        public float GetColorBleedingSaturation()
        {
            return m_ColorBleeding.saturation;
        }

        public void SetColorBleedingSaturation(float saturation)
        {
            m_ColorBleeding.saturation = Mathf.Clamp(saturation, 0, 4);
        }

        public float GetColorBleedingBrightnessMask()
        {
            return m_ColorBleeding.brightnessMask;
        }

        public void SetColorBleedingBrightnessMask(float brightnessMask)
        {
            m_ColorBleeding.brightnessMask = Mathf.Clamp01(brightnessMask);
        }

        public Vector2 GetColorBleedingBrightnessMaskRange()
        {
            return m_ColorBleeding.brightnessMaskRange;
        }

        public void SetColorBleedingBrightnessMaskRange(Vector2 brightnessMaskRange)
        {
            brightnessMaskRange.x = Mathf.Clamp(brightnessMaskRange.x, 0, 2);
            brightnessMaskRange.y = Mathf.Clamp(brightnessMaskRange.y, 0, 2);
            brightnessMaskRange.x = Mathf.Min(brightnessMaskRange.x, brightnessMaskRange.y);
            m_ColorBleeding.brightnessMaskRange = brightnessMaskRange;
        }
    }
}
