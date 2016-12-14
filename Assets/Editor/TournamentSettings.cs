/*
    _____  _____  _____  _____  ______
        |  _____ |      |      |  ___|
        |  _____ |      |      |     |
    
     U       N       I       T      Y
                                         
    
    TerraUnity Co. - Earth Simulation Tools - 2016
    
    http://terraunity.com
    info@terraunity.com
    
    This script is written for Unity 3D Engine.
    Unity 3D Version: Unity 5.x
    
    
    INFO: Tweaks Project Settings to match up with the original setup so that everything operates properly. The customized settings are only compatible to
    TerraLand Tournament project so there is no guarantee that same settings work as expected in other projects.

    Written by: Amir Badamchi
    
*/


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class TournamentSettings : EditorWindow
{
	const bool forceShow = false;

    private static float initial_InputGravity;
    private static float initial_InputSensitivity;
    private static bool initial_InputSnap;
    private static string[] initial_layerNames = new string[32];
    static RenderingPath initial_RenderPath;
    static ColorSpace initial_ColorSpace;
    static bool initial_DefaultIsFullScreen;
    static ApiCompatibilityLevel initial_APICompatibilityLevel;
    static string initial_CompanyName;
    static string initial_ProductName;

    const float recommended_InputGravity = 2;
    const float recommended_InputSensitivity = 1;
    const bool recommended_InputSnap = false;
    const RenderingPath recommended_RenderPath = RenderingPath.DeferredShading;
	const ColorSpace recommended_ColorSpace = ColorSpace.Linear;
    const bool recommended_DefaultIsFullScreen = true;
    const ApiCompatibilityLevel recommended_APICompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
    const string recommended_CompanyName = "TerraUnity";
    const string recommended_ProductName = "TerraLand - Tournament";
    private static Texture2D dialogBanner;
    private static Texture2D[] icon;
    private static string existingLayerName;
    private static List<string> layerNames = new List<string>();
    private static List<string> levelNames;
    private static List<int> pixelLightCount;
    private static List<int> textureQuality;
    private static List<int> anisotropicTextures;
    private static List<int> antiAliasing;
    private static List<bool> softParticles;
    private static List<bool> realtimeReflectionProbes;
    private static List<bool> billboardsFaceCameraPosition;
    private static List<int> shadows;
    private static List<int> shadowResolution;
    private static List<int> shadowProjection;
    private static List<float> shadowDistance;
    private static List<float> shadowNearPlaneOffset;
    private static List<int> shadowCascades;
    private static List<Vector3> shadowCascade4Split;
    private static List<int> blendWeights;
    private static List<int> vSyncCount;
    private static List<float> lodBias;
    private static List<int> maximumLODLevel;
    private static List<int> particleRaycastBudget;
    private static List<int> asyncUploadTimeSlice;
    private static List<int> asyncUploadBufferSize;
    private static int currentQualityIndex;
    private static int sceneCount;
    private static List<string> scenePath;
    private static List<bool> sceneIsEnabled;

    private static MovieTexture videoSlide;
    private static bool isPlayed = false;
    private static Vector2 windowSize = new Vector2(540, 680);
    private static Texture2D logo;
    private static UnityEngine.Color statusColor = UnityEngine.Color.red;
    private static string statusStr = "Project Is Not Setup";
    private static Rect statusRect = new Rect(170, 470, 200, 25);

    static TournamentSettings window;

    static TournamentSettings()
	{
		EditorApplication.update += Update;
	}

    static void Initialize ()
    {
        videoSlide = Resources.Load("Video/TournamentSlide") as MovieTexture;
        videoSlide.loop = true;
        videoSlide.Play();

        logo = Resources.Load("Graphics/Icon_Small") as Texture2D;
        dialogBanner = Resources.Load("Graphics/Banner") as Texture2D;
        icon = new Texture2D[]{Resources.Load("Graphics/Icon") as Texture2D};

        PopulateLayers();
        BackupSettings();

        isPlayed = true;
    }

	static void Update ()
	{
		bool show =
			PlayerSettings.renderingPath != recommended_RenderPath ||
			PlayerSettings.colorSpace != recommended_ColorSpace ||
            PlayerSettings.defaultIsFullScreen != recommended_DefaultIsFullScreen ||
            PlayerSettings.apiCompatibilityLevel != recommended_APICompatibilityLevel ||
            PlayerSettings.companyName != recommended_CompanyName ||
            PlayerSettings.productName != recommended_ProductName ||
            PlayerSettings.resolutionDialogBanner == null ||
            PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0] == null ||
			forceShow;
        
		if (show)
		{
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);

            window = GetWindow<TournamentSettings>(true, "Tournament Settings", true);
            window.position = new Rect
                (
                    (Screen.currentResolution.width / 2) - (windowSize.x / 2),
                    (Screen.currentResolution.height / 2) - (windowSize.y / 2),
                    windowSize.x,
                    windowSize.y
                );
            
            window.minSize = new Vector2(windowSize.x, windowSize.y);
            window.maxSize = new Vector2(windowSize.x, windowSize.y);
		}

		EditorApplication.update -= Update;
	}

	public void OnGUI ()
	{
        Repaint();

        if(!isPlayed)
            Initialize();
		
        if (videoSlide)
            GUI.DrawTexture(new Rect(0, 0, 540, 304), videoSlide, ScaleMode.ScaleToFit);

        GUILayout.Space(330);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Welcome to TerraLand Tournament", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("It is recommended to import the package in a new empty project.");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Press ACCEPT to setup project settings.");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(30);

		GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
		if (GUILayout.Button("ACCEPT"))
		{
            SetSettings();
		}

		if (GUILayout.Button("IGNORE"))
		{
            RevertSettings();
		}
        GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

        GUI.color = statusColor;

        GUIStyle myStyle = new GUIStyle(GUI.skin.box);
        myStyle.fontSize = 15;
        myStyle.normal.textColor = UnityEngine.Color.black;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.Box(statusRect, new GUIContent(statusStr), myStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUI.color = UnityEngine.Color.white;

        GUILayout.Space(70);

        GUI.backgroundColor = new UnityEngine.Color(1,1,1,0.25f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(logo))
            UnityEditor.Help.BrowseURL("http://terraunity.com/terraland-tournament/");
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.backgroundColor = UnityEngine.Color.white;
	}

    private static void GetInput (string inputName)
    {
        SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = inputManager.FindProperty("m_Axes");

        axesProperty.Next(true);
        axesProperty.Next(true);

        bool detected = false;

        while (axesProperty.Next(false))
        {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);

            if (!detected && axis.stringValue == inputName)
            {
                initial_InputGravity = GetChildProperty(axesProperty, "gravity").floatValue;
                initial_InputSensitivity = GetChildProperty(axesProperty, "sensitivity").floatValue;
                initial_InputSnap = GetChildProperty(axesProperty, "snap").boolValue;

                detected = true;
            }
        }
    }

    private static void SetInput (string inputName, float gravity, float sensitivity, bool snap)
    {
        SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = inputManager.FindProperty("m_Axes");

        axesProperty.Next(true);
        axesProperty.Next(true);

        bool detected = false;

        while (!detected && axesProperty.Next(false))
        {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);

            if (axis.stringValue == inputName)
            {
                GetChildProperty(axesProperty, "gravity").floatValue = gravity;
                GetChildProperty(axesProperty, "sensitivity").floatValue = sensitivity;
                GetChildProperty(axesProperty, "snap").boolValue = snap;

                inputManager.ApplyModifiedProperties();

                detected = true;
            }
        }
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        }
        while (child.Next(false));
        return null;
    }

    private static void GetLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        for (int i = 0; i < 32; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            initial_layerNames[i] = layerSP.stringValue;
        }

        tagManager.ApplyModifiedProperties();
    }

    private static void SetLayers(bool recursive)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        if(!recursive)
        {
            PopulateLayers();
            int addedIndex = 0;
            int length = layerNames.Count;

            for (int i = 8; i < 32; i++)
            {
                if(addedIndex < length)
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                    string layerName = layerSP.stringValue;

                    if(LayerExisting(layerName, out existingLayerName))
                    {
                        layerSP.stringValue = existingLayerName;
                        layerNames.Remove(existingLayerName);
                        addedIndex++;
                    }
                }
            }

            addedIndex = 0;
            length = layerNames.Count;

            for (int i = 8; i < 32; i++)
            {
                if(addedIndex < length)
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                    string layerName = layerSP.stringValue;

                    if (layerName == "")
                    {
                        layerSP.stringValue = layerNames[addedIndex];
                        addedIndex++;
                    }
                }
            }  
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                layerSP.stringValue = initial_layerNames[i];
            }
        }

        tagManager.ApplyModifiedProperties();
    }

    private static void PopulateLayers ()
    {
        layerNames = new List<string>();

        layerNames.Add("Terrain");
        layerNames.Add("Road");
        layerNames.Add("Car");
        layerNames.Add("Particles");
        layerNames.Add("CarBody");
        layerNames.Add("Bounds");
        layerNames.Add("CloudsToy");
    }

    private static bool LayerExisting (string layerName, out string existingLayerName)
    {
        if
        (
            layerName == "Terrain" ||
            layerName == "Road" ||
            layerName == "Car" ||
            layerName == "Particles" ||
            layerName == "CarBody" ||
            layerName == "Bounds" ||
            layerName == "CloudsToy"
        )
        {
            existingLayerName = layerName;
            return true;
        }

        existingLayerName = layerName;
        return false;
    }

    private static void GetQuality ()
    {
        SerializedObject qualitySettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
        SerializedProperty levels = qualitySettings.FindProperty("m_QualitySettings");

        SerializedProperty level1 = levels.GetArrayElementAtIndex(0);
        SerializedProperty level2 = levels.GetArrayElementAtIndex(1);
        SerializedProperty level3 = levels.GetArrayElementAtIndex(2);
        SerializedProperty level4 = levels.GetArrayElementAtIndex(3);
        SerializedProperty level5 = levels.GetArrayElementAtIndex(4);
        SerializedProperty level6 = levels.GetArrayElementAtIndex(5);

        QualityVarsInit();

        levelNames.Add(GetChildProperty(level1, "name").stringValue);
        levelNames.Add(GetChildProperty(level2, "name").stringValue);
        levelNames.Add(GetChildProperty(level3, "name").stringValue);
        levelNames.Add(GetChildProperty(level4, "name").stringValue);
        levelNames.Add(GetChildProperty(level5, "name").stringValue);
        levelNames.Add(GetChildProperty(level6, "name").stringValue);


        // Get High Quality Settings
        pixelLightCount.Add(GetChildProperty(level6, "pixelLightCount").intValue);
        textureQuality.Add(GetChildProperty(level6, "textureQuality").enumValueIndex);
        anisotropicTextures.Add(GetChildProperty(level6, "anisotropicTextures").enumValueIndex);
        antiAliasing.Add(GetChildProperty(level6, "antiAliasing").enumValueIndex);
        softParticles.Add(GetChildProperty(level6, "softParticles").boolValue);
        realtimeReflectionProbes.Add(GetChildProperty(level6, "realtimeReflectionProbes").boolValue);
        billboardsFaceCameraPosition.Add(GetChildProperty(level6, "billboardsFaceCameraPosition").boolValue);
        shadows.Add(GetChildProperty(level6, "shadows").enumValueIndex);
        shadowResolution.Add(GetChildProperty(level6, "shadowResolution").enumValueIndex);
        shadowProjection.Add(GetChildProperty(level6, "shadowProjection").enumValueIndex);
        shadowDistance.Add(GetChildProperty(level6, "shadowDistance").floatValue);
        shadowNearPlaneOffset.Add(GetChildProperty(level6, "shadowNearPlaneOffset").floatValue);
        shadowCascades.Add(GetChildProperty(level6, "shadowCascades").enumValueIndex);
        shadowCascade4Split.Add(GetChildProperty(level6, "shadowCascade4Split").vector3Value);
        blendWeights.Add(GetChildProperty(level6, "blendWeights").enumValueIndex);
        vSyncCount.Add(GetChildProperty(level6, "vSyncCount").enumValueIndex);
        lodBias.Add(GetChildProperty(level6, "lodBias").floatValue);
        maximumLODLevel.Add(GetChildProperty(level6, "maximumLODLevel").intValue);
        particleRaycastBudget.Add(GetChildProperty(level6, "particleRaycastBudget").intValue);
        asyncUploadTimeSlice.Add(GetChildProperty(level6, "asyncUploadTimeSlice").intValue);
        asyncUploadBufferSize.Add(GetChildProperty(level6, "asyncUploadBufferSize").intValue);

        // Get Medium Quality Settings
        pixelLightCount.Add(GetChildProperty(level4, "pixelLightCount").intValue);
        textureQuality.Add(GetChildProperty(level4, "textureQuality").enumValueIndex);
        anisotropicTextures.Add(GetChildProperty(level4, "anisotropicTextures").enumValueIndex);
        antiAliasing.Add(GetChildProperty(level4, "antiAliasing").enumValueIndex);
        softParticles.Add(GetChildProperty(level4, "softParticles").boolValue);
        realtimeReflectionProbes.Add(GetChildProperty(level4, "realtimeReflectionProbes").boolValue);
        billboardsFaceCameraPosition.Add(GetChildProperty(level4, "billboardsFaceCameraPosition").boolValue);
        shadows.Add(GetChildProperty(level4, "shadows").enumValueIndex);
        shadowResolution.Add(GetChildProperty(level4, "shadowResolution").enumValueIndex);
        shadowProjection.Add(GetChildProperty(level4, "shadowProjection").enumValueIndex);
        shadowDistance.Add(GetChildProperty(level4, "shadowDistance").floatValue);
        shadowNearPlaneOffset.Add(GetChildProperty(level4, "shadowNearPlaneOffset").floatValue);
        shadowCascades.Add(GetChildProperty(level4, "shadowCascades").enumValueIndex);
        shadowCascade4Split.Add(GetChildProperty(level4, "shadowCascade4Split").vector3Value);
        blendWeights.Add(GetChildProperty(level4, "blendWeights").enumValueIndex);
        vSyncCount.Add(GetChildProperty(level4, "vSyncCount").enumValueIndex);
        lodBias.Add(GetChildProperty(level4, "lodBias").floatValue);
        maximumLODLevel.Add(GetChildProperty(level4, "maximumLODLevel").intValue);
        particleRaycastBudget.Add(GetChildProperty(level4, "particleRaycastBudget").intValue);
        asyncUploadTimeSlice.Add(GetChildProperty(level4, "asyncUploadTimeSlice").intValue);
        asyncUploadBufferSize.Add(GetChildProperty(level4, "asyncUploadBufferSize").intValue);

        // Get Low Quality Settings
        pixelLightCount.Add(GetChildProperty(level2, "pixelLightCount").intValue);
        textureQuality.Add(GetChildProperty(level2, "textureQuality").enumValueIndex);
        anisotropicTextures.Add(GetChildProperty(level2, "anisotropicTextures").enumValueIndex);
        antiAliasing.Add(GetChildProperty(level2, "antiAliasing").enumValueIndex);
        softParticles.Add(GetChildProperty(level2, "softParticles").boolValue);
        realtimeReflectionProbes.Add(GetChildProperty(level2, "realtimeReflectionProbes").boolValue);
        billboardsFaceCameraPosition.Add(GetChildProperty(level2, "billboardsFaceCameraPosition").boolValue);
        shadows.Add(GetChildProperty(level2, "shadows").enumValueIndex);
        shadowResolution.Add(GetChildProperty(level2, "shadowResolution").enumValueIndex);
        shadowProjection.Add(GetChildProperty(level2, "shadowProjection").enumValueIndex);
        shadowDistance.Add(GetChildProperty(level2, "shadowDistance").floatValue);
        shadowNearPlaneOffset.Add(GetChildProperty(level2, "shadowNearPlaneOffset").floatValue);
        shadowCascades.Add(GetChildProperty(level2, "shadowCascades").enumValueIndex);
        blendWeights.Add(GetChildProperty(level2, "blendWeights").enumValueIndex);
        vSyncCount.Add(GetChildProperty(level2, "vSyncCount").enumValueIndex);
        lodBias.Add(GetChildProperty(level2, "lodBias").floatValue);
        maximumLODLevel.Add(GetChildProperty(level2, "maximumLODLevel").intValue);
        particleRaycastBudget.Add(GetChildProperty(level2, "particleRaycastBudget").intValue);
        asyncUploadTimeSlice.Add(GetChildProperty(level2, "asyncUploadTimeSlice").intValue);
        asyncUploadBufferSize.Add(GetChildProperty(level2, "asyncUploadBufferSize").intValue);

        // Get Lowest Quality Settings
        pixelLightCount.Add(GetChildProperty(level1, "pixelLightCount").intValue);
        textureQuality.Add(GetChildProperty(level1, "textureQuality").enumValueIndex);
        anisotropicTextures.Add(GetChildProperty(level1, "anisotropicTextures").enumValueIndex);
        antiAliasing.Add(GetChildProperty(level1, "antiAliasing").enumValueIndex);
        softParticles.Add(GetChildProperty(level1, "softParticles").boolValue);
        realtimeReflectionProbes.Add(GetChildProperty(level1, "realtimeReflectionProbes").boolValue);
        billboardsFaceCameraPosition.Add(GetChildProperty(level1, "billboardsFaceCameraPosition").boolValue);
        shadows.Add(GetChildProperty(level1, "shadows").enumValueIndex);
        shadowResolution.Add(GetChildProperty(level1, "shadowResolution").enumValueIndex);
        shadowProjection.Add(GetChildProperty(level1, "shadowProjection").enumValueIndex);
        shadowDistance.Add(GetChildProperty(level1, "shadowDistance").floatValue);
        shadowNearPlaneOffset.Add(GetChildProperty(level1, "shadowNearPlaneOffset").floatValue);
        shadowCascades.Add(GetChildProperty(level1, "shadowCascades").enumValueIndex);
        blendWeights.Add(GetChildProperty(level1, "blendWeights").enumValueIndex);
        vSyncCount.Add(GetChildProperty(level1, "vSyncCount").enumValueIndex);
        lodBias.Add(GetChildProperty(level1, "lodBias").floatValue);
        maximumLODLevel.Add(GetChildProperty(level1, "maximumLODLevel").intValue);
        particleRaycastBudget.Add(GetChildProperty(level1, "particleRaycastBudget").intValue);
        asyncUploadTimeSlice.Add(GetChildProperty(level1, "asyncUploadTimeSlice").intValue);
        asyncUploadBufferSize.Add(GetChildProperty(level1, "asyncUploadBufferSize").intValue);

        currentQualityIndex = QualitySettings.GetQualityLevel();
    }

    private static void SetQuality (bool recursive)
    {
        SerializedObject qualitySettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
        SerializedProperty levels = qualitySettings.FindProperty("m_QualitySettings");

        SerializedProperty level1 = levels.GetArrayElementAtIndex(0);
        SerializedProperty level2 = levels.GetArrayElementAtIndex(1);
        SerializedProperty level3 = levels.GetArrayElementAtIndex(2);
        SerializedProperty level4 = levels.GetArrayElementAtIndex(3);
        SerializedProperty level5 = levels.GetArrayElementAtIndex(4);
        SerializedProperty level6 = levels.GetArrayElementAtIndex(5);

        if(!recursive)
        {
            GetChildProperty(level1, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level2, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level3, "excludedTargetPlatforms").InsertArrayElementAtIndex(0);
            GetChildProperty(level3, "excludedTargetPlatforms").GetArrayElementAtIndex(0).stringValue = "Standalone";
            GetChildProperty(level4, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level5, "excludedTargetPlatforms").InsertArrayElementAtIndex(0);
            GetChildProperty(level5, "excludedTargetPlatforms").GetArrayElementAtIndex(0).stringValue = "Standalone";
            GetChildProperty(level6, "excludedTargetPlatforms").ClearArray();

            GetChildProperty(level1, "name").stringValue = "Lowest";
            GetChildProperty(level2, "name").stringValue = "Low";
            GetChildProperty(level4, "name").stringValue = "Medium";
            GetChildProperty(level6, "name").stringValue = "High";


            // Set High Quality Settings
            GetChildProperty(level6, "pixelLightCount").intValue = 4;
            GetChildProperty(level6, "textureQuality").enumValueIndex = 0;
            GetChildProperty(level6, "anisotropicTextures").enumValueIndex = 1;
            GetChildProperty(level6, "antiAliasing").enumValueIndex = 0;
            GetChildProperty(level6, "softParticles").boolValue = true;
            GetChildProperty(level6, "realtimeReflectionProbes").boolValue = true;
            GetChildProperty(level6, "billboardsFaceCameraPosition").boolValue = true;
            GetChildProperty(level6, "shadows").enumValueIndex = 2;
            GetChildProperty(level6, "shadowResolution").enumValueIndex = 2;
            GetChildProperty(level6, "shadowProjection").enumValueIndex = 1;
            GetChildProperty(level6, "shadowDistance").floatValue = 100;
            GetChildProperty(level6, "shadowNearPlaneOffset").floatValue = 2;
            GetChildProperty(level6, "shadowCascades").enumValueIndex = 2;
            GetChildProperty(level6, "shadowCascade4Split").vector3Value = new Vector3(0.15f, 0.3f, 0.5f);
            GetChildProperty(level6, "blendWeights").enumValueIndex = 2;
            GetChildProperty(level6, "vSyncCount").enumValueIndex = 0;
            GetChildProperty(level6, "lodBias").floatValue = 1f;
            GetChildProperty(level6, "maximumLODLevel").intValue = 0;
            GetChildProperty(level6, "particleRaycastBudget").intValue = 2048;
            GetChildProperty(level6, "asyncUploadTimeSlice").intValue = 2;
            GetChildProperty(level6, "asyncUploadBufferSize").intValue = 4;

            // Set Medium Quality Settings
            GetChildProperty(level4, "pixelLightCount").intValue = 2;
            GetChildProperty(level4, "textureQuality").enumValueIndex = 0;
            GetChildProperty(level4, "anisotropicTextures").enumValueIndex = 1;
            GetChildProperty(level4, "antiAliasing").enumValueIndex = 0;
            GetChildProperty(level4, "softParticles").boolValue = false;
            GetChildProperty(level4, "realtimeReflectionProbes").boolValue = true;
            GetChildProperty(level4, "billboardsFaceCameraPosition").boolValue = true;
            GetChildProperty(level4, "shadows").enumValueIndex = 2;
            GetChildProperty(level4, "shadowResolution").enumValueIndex = 1;
            GetChildProperty(level4, "shadowProjection").enumValueIndex = 1;
            GetChildProperty(level4, "shadowDistance").floatValue = 40;
            GetChildProperty(level4, "shadowNearPlaneOffset").floatValue = 2;
            GetChildProperty(level4, "shadowCascades").enumValueIndex = 1;
            GetChildProperty(level4, "shadowCascade2Split").floatValue = 0.333f;
            GetChildProperty(level4, "blendWeights").enumValueIndex = 1;
            GetChildProperty(level4, "vSyncCount").enumValueIndex = 0;
            GetChildProperty(level4, "lodBias").floatValue = 1f;
            GetChildProperty(level4, "maximumLODLevel").intValue = 0;
            GetChildProperty(level4, "particleRaycastBudget").intValue = 256;
            GetChildProperty(level4, "asyncUploadTimeSlice").intValue = 2;
            GetChildProperty(level4, "asyncUploadBufferSize").intValue = 4;

            // Set Low Quality Settings
            GetChildProperty(level2, "pixelLightCount").intValue = 0;
            GetChildProperty(level2, "textureQuality").enumValueIndex = 0;
            GetChildProperty(level2, "anisotropicTextures").enumValueIndex = 0;
            GetChildProperty(level2, "antiAliasing").enumValueIndex = 0;
            GetChildProperty(level2, "softParticles").boolValue = false;
            GetChildProperty(level2, "realtimeReflectionProbes").boolValue = false;
            GetChildProperty(level2, "billboardsFaceCameraPosition").boolValue = false;
            GetChildProperty(level2, "shadows").enumValueIndex = 0;
            GetChildProperty(level2, "shadowResolution").enumValueIndex = 0;
            GetChildProperty(level2, "shadowProjection").enumValueIndex = 1;
            GetChildProperty(level2, "shadowDistance").floatValue = 20;
            GetChildProperty(level2, "shadowNearPlaneOffset").floatValue = 2;
            GetChildProperty(level2, "shadowCascades").enumValueIndex = 0;
            GetChildProperty(level2, "blendWeights").enumValueIndex = 1;
            GetChildProperty(level2, "vSyncCount").enumValueIndex = 0;
            GetChildProperty(level2, "lodBias").floatValue = 0.4f;
            GetChildProperty(level2, "maximumLODLevel").intValue = 0;
            GetChildProperty(level2, "particleRaycastBudget").intValue = 16;
            GetChildProperty(level2, "asyncUploadTimeSlice").intValue = 2;
            GetChildProperty(level2, "asyncUploadBufferSize").intValue = 4;

            // Set Lowest Quality Settings
            GetChildProperty(level1, "pixelLightCount").intValue = 0;
            GetChildProperty(level1, "textureQuality").enumValueIndex = 1;
            GetChildProperty(level1, "anisotropicTextures").enumValueIndex = 0;
            GetChildProperty(level1, "antiAliasing").enumValueIndex = 0;
            GetChildProperty(level1, "softParticles").boolValue = false;
            GetChildProperty(level1, "realtimeReflectionProbes").boolValue = false;
            GetChildProperty(level1, "billboardsFaceCameraPosition").boolValue = false;
            GetChildProperty(level1, "shadows").enumValueIndex = 0;
            GetChildProperty(level1, "shadowResolution").enumValueIndex = 0;
            GetChildProperty(level1, "shadowProjection").enumValueIndex = 1;
            GetChildProperty(level1, "shadowDistance").floatValue = 15;
            GetChildProperty(level1, "shadowNearPlaneOffset").floatValue = 2;
            GetChildProperty(level1, "shadowCascades").enumValueIndex = 0;
            GetChildProperty(level1, "blendWeights").enumValueIndex = 0;
            GetChildProperty(level1, "vSyncCount").enumValueIndex = 0;
            GetChildProperty(level1, "lodBias").floatValue = 0.3f;
            GetChildProperty(level1, "maximumLODLevel").intValue = 0;
            GetChildProperty(level1, "particleRaycastBudget").intValue = 4;
            GetChildProperty(level1, "asyncUploadTimeSlice").intValue = 2;
            GetChildProperty(level1, "asyncUploadBufferSize").intValue = 4;

            QualitySettings.SetQualityLevel(5);
        }
        else
        {
            GetChildProperty(level1, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level2, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level3, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level4, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level5, "excludedTargetPlatforms").ClearArray();
            GetChildProperty(level6, "excludedTargetPlatforms").ClearArray();

            GetChildProperty(level1, "name").stringValue = levelNames[0];
            GetChildProperty(level2, "name").stringValue = levelNames[1];
            GetChildProperty(level3, "name").stringValue = levelNames[2];
            GetChildProperty(level4, "name").stringValue = levelNames[3];
            GetChildProperty(level5, "name").stringValue = levelNames[4];
            GetChildProperty(level6, "name").stringValue = levelNames[5];


            // Set High Quality Settings
            GetChildProperty(level6, "pixelLightCount").intValue = pixelLightCount[0];
            GetChildProperty(level6, "textureQuality").enumValueIndex = textureQuality[0];
            GetChildProperty(level6, "anisotropicTextures").enumValueIndex = anisotropicTextures[0];
            GetChildProperty(level6, "antiAliasing").enumValueIndex = antiAliasing[0];
            GetChildProperty(level6, "softParticles").boolValue = softParticles[0];
            GetChildProperty(level6, "realtimeReflectionProbes").boolValue = realtimeReflectionProbes[0];
            GetChildProperty(level6, "billboardsFaceCameraPosition").boolValue = billboardsFaceCameraPosition[0];
            GetChildProperty(level6, "shadows").enumValueIndex = shadows[0];
            GetChildProperty(level6, "shadowResolution").enumValueIndex = shadowResolution[0];
            GetChildProperty(level6, "shadowProjection").enumValueIndex = shadowProjection[0];
            GetChildProperty(level6, "shadowDistance").floatValue = shadowDistance[0];
            GetChildProperty(level6, "shadowNearPlaneOffset").floatValue = shadowNearPlaneOffset[0];
            GetChildProperty(level6, "shadowCascades").enumValueIndex = shadowCascades[0];
            GetChildProperty(level6, "shadowCascade4Split").vector3Value = shadowCascade4Split[0];
            GetChildProperty(level6, "blendWeights").enumValueIndex = blendWeights[0];
            GetChildProperty(level6, "vSyncCount").enumValueIndex = vSyncCount[0];
            GetChildProperty(level6, "lodBias").floatValue = lodBias[0];
            GetChildProperty(level6, "maximumLODLevel").intValue = maximumLODLevel[0];
            GetChildProperty(level6, "particleRaycastBudget").intValue = particleRaycastBudget[0];
            GetChildProperty(level6, "asyncUploadTimeSlice").intValue = asyncUploadTimeSlice[0];
            GetChildProperty(level6, "asyncUploadBufferSize").intValue = asyncUploadBufferSize[0];

            // Set Medium Quality Settings
            GetChildProperty(level4, "pixelLightCount").intValue = pixelLightCount[1];
            GetChildProperty(level4, "textureQuality").enumValueIndex = textureQuality[1];
            GetChildProperty(level4, "anisotropicTextures").enumValueIndex = anisotropicTextures[1];
            GetChildProperty(level4, "antiAliasing").enumValueIndex = antiAliasing[1];
            GetChildProperty(level4, "softParticles").boolValue = softParticles[1];
            GetChildProperty(level4, "realtimeReflectionProbes").boolValue = realtimeReflectionProbes[1];
            GetChildProperty(level4, "billboardsFaceCameraPosition").boolValue = billboardsFaceCameraPosition[1];
            GetChildProperty(level4, "shadows").enumValueIndex = shadows[1];
            GetChildProperty(level4, "shadowResolution").enumValueIndex = shadowResolution[1];
            GetChildProperty(level4, "shadowProjection").enumValueIndex = shadowProjection[1];
            GetChildProperty(level4, "shadowDistance").floatValue = shadowDistance[1];
            GetChildProperty(level4, "shadowNearPlaneOffset").floatValue = shadowNearPlaneOffset[1];
            GetChildProperty(level4, "shadowCascades").enumValueIndex = shadowCascades[1];
            GetChildProperty(level4, "shadowCascade4Split").vector3Value = shadowCascade4Split[1];
            GetChildProperty(level4, "blendWeights").enumValueIndex = blendWeights[1];
            GetChildProperty(level4, "vSyncCount").enumValueIndex = vSyncCount[1];
            GetChildProperty(level4, "lodBias").floatValue = lodBias[1];
            GetChildProperty(level4, "maximumLODLevel").intValue = maximumLODLevel[1];
            GetChildProperty(level4, "particleRaycastBudget").intValue = particleRaycastBudget[1];
            GetChildProperty(level4, "asyncUploadTimeSlice").intValue = asyncUploadTimeSlice[1];
            GetChildProperty(level4, "asyncUploadBufferSize").intValue = asyncUploadBufferSize[1];

            // Set Low Quality Settings
            GetChildProperty(level2, "pixelLightCount").intValue = pixelLightCount[2];
            GetChildProperty(level2, "textureQuality").enumValueIndex = textureQuality[2];
            GetChildProperty(level2, "anisotropicTextures").enumValueIndex = anisotropicTextures[2];
            GetChildProperty(level2, "antiAliasing").enumValueIndex = antiAliasing[2];
            GetChildProperty(level2, "softParticles").boolValue = softParticles[2];
            GetChildProperty(level2, "realtimeReflectionProbes").boolValue = realtimeReflectionProbes[2];
            GetChildProperty(level2, "billboardsFaceCameraPosition").boolValue = billboardsFaceCameraPosition[2];
            GetChildProperty(level2, "shadows").enumValueIndex = shadows[2];
            GetChildProperty(level2, "shadowResolution").enumValueIndex = shadowResolution[2];
            GetChildProperty(level2, "shadowProjection").enumValueIndex = shadowProjection[2];
            GetChildProperty(level2, "shadowDistance").floatValue = shadowDistance[2];
            GetChildProperty(level2, "shadowNearPlaneOffset").floatValue = shadowNearPlaneOffset[2];
            GetChildProperty(level2, "shadowCascades").enumValueIndex = shadowCascades[2];
            GetChildProperty(level2, "blendWeights").enumValueIndex = blendWeights[2];
            GetChildProperty(level2, "vSyncCount").enumValueIndex = vSyncCount[2];
            GetChildProperty(level2, "lodBias").floatValue = lodBias[2];
            GetChildProperty(level2, "maximumLODLevel").intValue = maximumLODLevel[2];
            GetChildProperty(level2, "particleRaycastBudget").intValue = particleRaycastBudget[2];
            GetChildProperty(level2, "asyncUploadTimeSlice").intValue = asyncUploadTimeSlice[2];
            GetChildProperty(level2, "asyncUploadBufferSize").intValue = asyncUploadBufferSize[2];

            // Set Lowest Quality Settings
            GetChildProperty(level1, "pixelLightCount").intValue = pixelLightCount[3];
            GetChildProperty(level1, "textureQuality").enumValueIndex = textureQuality[3];
            GetChildProperty(level1, "anisotropicTextures").enumValueIndex = anisotropicTextures[3];
            GetChildProperty(level1, "antiAliasing").enumValueIndex = antiAliasing[3];
            GetChildProperty(level1, "softParticles").boolValue = softParticles[3];
            GetChildProperty(level1, "realtimeReflectionProbes").boolValue = realtimeReflectionProbes[3];
            GetChildProperty(level1, "billboardsFaceCameraPosition").boolValue = billboardsFaceCameraPosition[3];
            GetChildProperty(level1, "shadows").enumValueIndex = shadows[3];
            GetChildProperty(level1, "shadowResolution").enumValueIndex = shadowResolution[3];
            GetChildProperty(level1, "shadowProjection").enumValueIndex = shadowProjection[3];
            GetChildProperty(level1, "shadowDistance").floatValue = shadowDistance[3];
            GetChildProperty(level1, "shadowNearPlaneOffset").floatValue = shadowNearPlaneOffset[3];
            GetChildProperty(level1, "shadowCascades").enumValueIndex = shadowCascades[3];
            GetChildProperty(level1, "blendWeights").enumValueIndex = blendWeights[3];
            GetChildProperty(level1, "vSyncCount").enumValueIndex = vSyncCount[3];
            GetChildProperty(level1, "lodBias").floatValue = lodBias[3];
            GetChildProperty(level1, "maximumLODLevel").intValue = maximumLODLevel[3];
            GetChildProperty(level1, "particleRaycastBudget").intValue = particleRaycastBudget[3];
            GetChildProperty(level1, "asyncUploadTimeSlice").intValue = asyncUploadTimeSlice[3];
            GetChildProperty(level1, "asyncUploadBufferSize").intValue = asyncUploadBufferSize[3];

            QualitySettings.SetQualityLevel(currentQualityIndex);
        }

        qualitySettings.ApplyModifiedProperties();
    }

    private static void QualityVarsInit ()
    {
        levelNames = new List<string>();
        pixelLightCount =new List<int>();
        textureQuality = new List<int>();
        anisotropicTextures = new List<int>();
        antiAliasing = new List<int>();
        softParticles = new List<bool>();
        realtimeReflectionProbes = new List<bool>();
        billboardsFaceCameraPosition = new List<bool>();
        shadows = new List<int>();
        shadowResolution = new List<int>();
        shadowProjection = new List<int>();
        shadowDistance = new List<float>();
        shadowNearPlaneOffset = new List<float>();
        shadowCascades = new List<int>();
        shadowCascade4Split = new List<Vector3>();
        blendWeights = new List<int>();
        vSyncCount = new List<int>();
        lodBias = new List<float>();
        maximumLODLevel = new List<int>();
        particleRaycastBudget = new List<int>();
        asyncUploadTimeSlice = new List<int>();
        asyncUploadBufferSize = new List<int>();
    }

    private static void GetBuildSettings ()
    {
        SerializedObject buildSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/EditorBuildSettings.asset")[0]);
        SerializedProperty scenes = buildSettings.FindProperty("m_Scenes");

        sceneCount = scenes.arraySize;
        scenePath = new List<string>();
        sceneIsEnabled = new List<bool>();

        for(int i = 0; i < sceneCount; i++)
        {
            SerializedProperty scene = scenes.GetArrayElementAtIndex(i);
            scenePath.Add(GetChildProperty(scene, "path").stringValue);
            sceneIsEnabled.Add(GetChildProperty(scene, "enabled").boolValue);
        }
    }

    private static void SetBuildSettings (bool recursive)
    {
        SerializedObject buildSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/EditorBuildSettings.asset")[0]);
        SerializedProperty scenes = buildSettings.FindProperty("m_Scenes");

        if(!recursive)
        {
            scenes.ClearArray();
            scenes.InsertArrayElementAtIndex(0);
            scenes.InsertArrayElementAtIndex(1);
            SerializedProperty scene1 = scenes.GetArrayElementAtIndex(0);
            SerializedProperty scene2 = scenes.GetArrayElementAtIndex(1);

            GetChildProperty(scene1, "path").stringValue = "Assets/TerraLand Tournament/Scene/Main Menu.unity";
            GetChildProperty(scene2, "path").stringValue = "Assets/TerraLand Tournament/Scene/White Rim Trail, Utah.unity";
            GetChildProperty(scene1, "enabled").boolValue = true;
            GetChildProperty(scene2, "enabled").boolValue = true;
        }
        else
        {
            scenes.ClearArray();

            for(int i = 0; i < sceneCount; i++)
            {
                scenes.InsertArrayElementAtIndex(i);
                SerializedProperty scene = scenes.GetArrayElementAtIndex(i);
                GetChildProperty(scene, "path").stringValue = scenePath[i];
                GetChildProperty(scene, "enabled").boolValue = sceneIsEnabled[i];
            }
        }

        buildSettings.ApplyModifiedProperties();
    }

    static void BackupSettings ()
    {
        GetInput("Horizontal");
        GetLayers();
        GetQuality();
        GetBuildSettings();
        initial_RenderPath = PlayerSettings.renderingPath;
        initial_ColorSpace = PlayerSettings.colorSpace;
        initial_DefaultIsFullScreen = PlayerSettings.defaultIsFullScreen;
        initial_APICompatibilityLevel = PlayerSettings.apiCompatibilityLevel;
        initial_CompanyName = PlayerSettings.companyName;
        initial_ProductName = PlayerSettings.productName;
    }

    static void SetSettings ()
    {
        EditorUtility.DisplayDialog("ACCEPT", "If you are in an existing project, you will lose current project settings.\n\nPressing IGNORE will revert back project settings until this window is open.", "OK");

        SetInput("Horizontal", recommended_InputGravity, recommended_InputSensitivity, recommended_InputSnap);
        SetLayers(false);
        SetQuality(false);
        SetBuildSettings(false);

        PlayerSettings.renderingPath = recommended_RenderPath;
        PlayerSettings.colorSpace = recommended_ColorSpace;
        PlayerSettings.defaultIsFullScreen = recommended_DefaultIsFullScreen;
        PlayerSettings.apiCompatibilityLevel = recommended_APICompatibilityLevel;
        PlayerSettings.companyName = recommended_CompanyName;
        PlayerSettings.productName = recommended_ProductName;
        PlayerSettings.resolutionDialogBanner = dialogBanner;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icon);

        statusColor = UnityEngine.Color.green;
        statusStr = "Everything Is Ok";
    }

    static void RevertSettings ()
    {
        EditorUtility.DisplayDialog("IGNORE", "Are you sure?\n\nProject will not function properly.", "OK");

        SetInput("Horizontal", initial_InputGravity, initial_InputSensitivity, initial_InputSnap);
        SetLayers(true);
        SetQuality(true);
        SetBuildSettings(true);

        PlayerSettings.renderingPath = initial_RenderPath;
        PlayerSettings.colorSpace = initial_ColorSpace;
        PlayerSettings.defaultIsFullScreen = initial_DefaultIsFullScreen;
        PlayerSettings.apiCompatibilityLevel = initial_APICompatibilityLevel;
        PlayerSettings.companyName = initial_CompanyName;
        PlayerSettings.productName = initial_ProductName;

        statusColor = UnityEngine.Color.red;
        statusStr = "Project Is Not Setup";
    }
}

