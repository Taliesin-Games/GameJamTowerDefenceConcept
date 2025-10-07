using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.Reflection;

namespace SOG.CVDFilter
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Volume))]
    public class CVDFilter : MonoBehaviour
    {
        Volume postProcessVolume;

        CVDProfilesSO profiles;
        [SerializeField] VisionTypeNames currentType;
        public VisionTypeInfo SelectedVisionType { get; private set; }

        const string soFileName = "CVDProfiles";
        const string soSearchTerm = "t:ScriptableObject " + soFileName;

        void Reset()
        {
            Setup();
            ChangeProfile();
        }
        void Start()
        {
            Setup();
            ChangeProfile();
        }
        void Setup()
        {
#if UNITY_EDITOR
            // Only try loading if profiles isn't assigned yet
            if (profiles == null)
                AssignProfileSO();
#endif
            ConfigureVolume();
        }

        void AssignProfileSO()
        {
#if UNITY_EDITOR
            // Prevent early execution during editor reloads
            if (!UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // Delay until editor is ready
                UnityEditor.EditorApplication.delayCall += AssignProfileSO;
                return;
            }

            string[] guid = AssetDatabase.FindAssets("t:CVDProfilesSO");
            if (guid.Length == 0)
            {
                Debug.LogError($"[{nameof(CVDFilter)}] Unable to locate a CVDProfilesSO asset. Please create one via Assets > Create > CVD Filter > Profiles.");
                return;
            }

            profiles = AssetDatabase.LoadAssetAtPath<CVDProfilesSO>(AssetDatabase.GUIDToAssetPath(guid[0]));
            if (profiles == null)
            {
                Debug.LogError($"[{nameof(CVDFilter)}] Loaded asset path returned null.");
                return;
            }

            SelectedVisionType = profiles.VisionTypes.Count > 0 ? profiles.VisionTypes[0] : default;
#endif
        }

        void ConfigureVolume()
        {
            postProcessVolume = GetComponent<Volume>();
            postProcessVolume.isGlobal = true;
        }

        public void ChangeProfile()
        {
            if (profiles == null || profiles.VisionTypes == null || profiles.VisionTypes.Count == 0)
                return;

            SelectedVisionType = profiles.VisionTypes[(int)currentType];
            postProcessVolume.profile = SelectedVisionType.profile;
            return;
        }
    }

    public enum VisionTypeNames
    {
        Normal,
        Protanopia,
        Protanomaly,
        Deuteranopia,
        Deuteranomaly,
        Tritanopia,
        Tritanomaly,
        Achromatopsia,
        Achromatomaly
    }

    [System.Serializable]
    public struct VisionTypeInfo
    {
        public VisionTypeNames typeName;
        public string description;
        public VolumeProfile profile;
        public Texture2D previewImage;
    }
}