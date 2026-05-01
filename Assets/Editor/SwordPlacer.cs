#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Interactions;

/// <summary>
/// Editor tool: Tools → Sword Placer
/// Places the new sword model on the player's Gun Holder in the Scene view
/// so you can visually adjust position, rotation, and scale.
/// When you're happy, click "Save as Weapon Prefab" to create a ready-to-use prefab.
/// </summary>
public class SwordPlacer : EditorWindow
{
    // The new sword mesh/prefab from the Asset Store (drag it in)
    private GameObject _newSwordSource;

    // The live preview instance in the scene
    private GameObject _previewInstance;

    // The parent we attach to (Gun Holder on the player)
    private Transform _gunHolder;

    // Temporary player prefab if we had to spawn one
    private GameObject _dummyPlayer;

    // Saved transform values
    private Vector3 _position = Vector3.zero;
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _scale = Vector3.one;

    // Save path
    private string _savePath = "Assets/Prefabs/Guns/NewSword.prefab";

    [MenuItem("Tools/Sword Placer")]
    public static void ShowWindow()
    {
        var window = GetWindow<SwordPlacer>("Sword Placer");
        window.minSize = new Vector2(350, 500);
    }

    private void OnGUI()
    {
        GUILayout.Label("🗡️ Sword Placer Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This tool places the new sword on your player's Gun Holder " +
            "so you can adjust its size, position, and rotation visually.\n\n" +
            "Steps:\n" +
            "1. Drag the new sword prefab/model into the slot below\n" +
            "2. Click 'Spawn Preview on Player'\n" +
            "3. Use the Scene view gizmos (W/E/R) or the fields below to adjust\n" +
            "4. Click 'Save as Weapon Prefab' when done",
            MessageType.Info);

        EditorGUILayout.Space(10);

        // --- Source sword slot ---
        EditorGUILayout.LabelField("New Sword Source", EditorStyles.boldLabel);
        _newSwordSource = (GameObject)EditorGUILayout.ObjectField(
            "Sword Prefab/Model", _newSwordSource, typeof(GameObject), false);

        EditorGUILayout.Space(10);

        // --- Spawn / Destroy preview ---
        EditorGUILayout.LabelField("Preview Controls", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = _newSwordSource != null && _previewInstance == null;
            if (GUILayout.Button("Spawn Preview on Player", GUILayout.Height(30)))
            {
                SpawnPreview();
            }
            GUI.enabled = _previewInstance != null;
            if (GUILayout.Button("Remove Preview", GUILayout.Height(30)))
            {
                RemovePreview();
            }
            GUI.enabled = true;
        }

        EditorGUILayout.Space(10);

        // --- Transform controls ---
        if (_previewInstance != null)
        {
            EditorGUILayout.LabelField("Transform Adjustment", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "You can also select the sword in the Hierarchy and use " +
                "W (Move), E (Rotate), R (Scale) in the Scene view.",
                MessageType.None);

            // Sync from live object
            _position = _previewInstance.transform.localPosition;
            _rotation = _previewInstance.transform.localEulerAngles;
            _scale = _previewInstance.transform.localScale;

            EditorGUI.BeginChangeCheck();
            _position = EditorGUILayout.Vector3Field("Local Position", _position);
            _rotation = EditorGUILayout.Vector3Field("Local Rotation", _rotation);
            _scale = EditorGUILayout.Vector3Field("Local Scale", _scale);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_previewInstance.transform, "Adjust Sword Transform");
                _previewInstance.transform.localPosition = _position;
                _previewInstance.transform.localEulerAngles = _rotation;
                _previewInstance.transform.localScale = _scale;
            }

            EditorGUILayout.Space(5);

            // Quick scale buttons
            EditorGUILayout.LabelField("Quick Scale", EditorStyles.miniLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("0.1x")) ApplyScale(0.1f);
                if (GUILayout.Button("0.25x")) ApplyScale(0.25f);
                if (GUILayout.Button("0.5x")) ApplyScale(0.5f);
                if (GUILayout.Button("1x")) ApplyScale(1f);
                if (GUILayout.Button("2x")) ApplyScale(2f);
                if (GUILayout.Button("5x")) ApplyScale(5f);
            }

            EditorGUILayout.Space(5);

            // Quick rotation buttons
            EditorGUILayout.LabelField("Quick Rotate (Y-axis)", EditorStyles.miniLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+90°")) RotateY(90);
                if (GUILayout.Button("-90°")) RotateY(-90);
                if (GUILayout.Button("+180°")) RotateY(180);
                if (GUILayout.Button("Reset")) ResetRotation();
            }

            EditorGUILayout.Space(15);

            // --- Save as prefab ---
            EditorGUILayout.LabelField("Save", EditorStyles.boldLabel);
            _savePath = EditorGUILayout.TextField("Prefab Save Path", _savePath);

            if (GUILayout.Button("💾 Save as Weapon Prefab", GUILayout.Height(35)))
            {
                SaveAsPrefab();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "This will:\n" +
                "• Save the sword model with your transform adjustments\n" +
                "• Add the Weapon script (for swing animation)\n" +
                "• Add the Swordinteraction script (for damage)\n" +
                "• Add a BoxCollider (trigger) for hit detection\n" +
                "• Save it as a prefab ready for your GunContainer",
                MessageType.Info);
        }
    }

    private void SpawnPreview()
    {
        // Find Gun Holder on the player
        _gunHolder = FindGunHolder();
        if (_gunHolder == null)
        {
            // Spawn a temporary player for preview
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            if (playerPrefab != null)
            {
                _dummyPlayer = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                _gunHolder = _dummyPlayer.transform.Find("CameraPosition/Gun Holder");
            }
        }

        if (_gunHolder == null)
        {
            EditorUtility.DisplayDialog("Sword Placer",
                "Could not find the Gun Holder. Make sure you have a Player prefab at Assets/Prefabs/Player.prefab",
                "OK");
            return;
        }

        // Instantiate preview
        _previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(_newSwordSource);
        if (_previewInstance == null)
        {
            // Fallback for FBX files or models (not prefabs)
            _previewInstance = Instantiate(_newSwordSource);
        }

        _previewInstance.name = "⚔️ NEW SWORD PREVIEW (adjust me!)";
        _previewInstance.transform.SetParent(_gunHolder, false);
        _previewInstance.transform.localPosition = _position;
        _previewInstance.transform.localEulerAngles = _rotation;
        _previewInstance.transform.localScale = _scale;

        // Select it so the user can see the gizmos
        Selection.activeGameObject = _previewInstance;
        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log("[SwordPlacer] ✅ Preview spawned! Use W/E/R in Scene view to adjust, " +
                  "or use the fields in this window.");
    }

    private void RemovePreview()
    {
        if (_previewInstance != null)
        {
            DestroyImmediate(_previewInstance);
            _previewInstance = null;
            Debug.Log("[SwordPlacer] Preview removed.");
        }
        if (_dummyPlayer != null)
        {
            DestroyImmediate(_dummyPlayer);
            _dummyPlayer = null;
        }
    }

    private void ApplyScale(float multiplier)
    {
        if (_previewInstance == null) return;
        Undo.RecordObject(_previewInstance.transform, "Scale Sword");
        _previewInstance.transform.localScale = Vector3.one * multiplier;
    }

    private void RotateY(float degrees)
    {
        if (_previewInstance == null) return;
        Undo.RecordObject(_previewInstance.transform, "Rotate Sword");
        _previewInstance.transform.localEulerAngles += new Vector3(0, degrees, 0);
    }

    private void ResetRotation()
    {
        if (_previewInstance == null) return;
        Undo.RecordObject(_previewInstance.transform, "Reset Sword Rotation");
        _previewInstance.transform.localEulerAngles = Vector3.zero;
    }

    private void SaveAsPrefab()
    {
        if (_previewInstance == null) return;

        // Unparent temporarily to create a clean prefab
        _previewInstance.transform.SetParent(null);

        // Rename
        _previewInstance.name = "NewSword";

        // Add components if not already present
        if (_previewInstance.GetComponent<Weapon>() == null)
        {
            _previewInstance.AddComponent<Weapon>();
        }

        if (_previewInstance.GetComponent<Swordinteraction>() == null)
        {
            _previewInstance.AddComponent<Swordinteraction>();
        }

        // Add trigger collider if none exists
        var collider = _previewInstance.GetComponent<Collider>();
        if (collider == null)
        {
            var box = _previewInstance.AddComponent<BoxCollider>();
            box.isTrigger = true;
            // Set a reasonable default size for a sword
            box.size = new Vector3(0.2f, 0.2f, 1f);
            box.center = new Vector3(0, 0, 0.5f);
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
        }

        // Add Animator if not present (needed for Weapon script)
        if (_previewInstance.GetComponent<Animator>() == null)
        {
            var animator = _previewInstance.AddComponent<Animator>();
            // Try to assign the existing sword controller
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                "Assets/Animations/Player/weapons/Sword.controller");
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
            }
        }

        // Wire up the Weapon script's _animator field via SerializedObject
        var weaponComp = _previewInstance.GetComponent<Weapon>();
        var animatorComp = _previewInstance.GetComponent<Animator>();
        if (weaponComp != null && animatorComp != null)
        {
            var so = new SerializedObject(weaponComp);
            var animProp = so.FindProperty("_animator");
            if (animProp != null)
            {
                animProp.objectReferenceValue = animatorComp;
                so.ApplyModifiedProperties();
            }
        }

        // Save as prefab
        bool success;
        var prefab = PrefabUtility.SaveAsPrefabAsset(_previewInstance, _savePath, out success);

        if (success)
        {
            Debug.Log($"[SwordPlacer] ✅ Sword prefab saved to: {_savePath}");
            Debug.Log("[SwordPlacer] Next step: Open your DefaultGuns ScriptableObject " +
                      "(Assets/Scriptable Objects/Resources/DefaultGuns.asset) and drag " +
                      "this new prefab into the sword slot in the Guns list.");

            EditorUtility.DisplayDialog("Sword Placer — Success! ✅",
                $"Prefab saved to:\n{_savePath}\n\n" +
                "Next steps:\n" +
                "1. Open 'Assets/Scriptable Objects/Resources/DefaultGuns.asset'\n" +
                "2. Find the Sword entry in the Guns list\n" +
                "3. Drag the new prefab into the Gun Prefab slot\n" +
                "4. Adjust the BoxCollider size in the prefab if needed",
                "Got it!");

            // Select the new prefab in Project
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }
        else
        {
            Debug.LogError($"[SwordPlacer] ❌ Failed to save prefab to: {_savePath}");
        }

        // Clean up
        DestroyImmediate(_previewInstance);
        _previewInstance = null;
    }

    private Transform FindGunHolder()
    {
        // Try to find by path on the player
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var gunHolder = player.transform.Find("CameraPosition/Gun Holder");
            if (gunHolder != null) return gunHolder;
        }

        // Fallback: search by name
        var allTransforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var t in allTransforms)
        {
            if (t.name == "Gun Holder")
                return t;
        }

        return null;
    }

    private void OnDestroy()
    {
        // Clean up preview if window is closed
        RemovePreview();
    }
}
#endif
