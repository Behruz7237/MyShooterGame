#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// One-click setup for the FPS Gun System.
/// Run from the Unity menu bar: Tools → FPS Gun System → Run Full Setup
/// </summary>
public static class GunSystemSetup
{
    // ── Menu Items ───────────────────────────────────────────────────────────────

    [MenuItem("Tools/FPS Gun System/Run Full Setup")]
    public static void RunFullSetup()
    {
        int fixes = 0;

        fixes += SetupCameraZoom();
        fixes += VerifyGunHolder();
        fixes += VerifyBulletPrefab();

        if (fixes > 0)
        {
            // Mark the scene dirty so Unity knows to save it
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log($"[GunSystemSetup] ✅ Setup complete — {fixes} item(s) configured. " +
                       "Save the scene with Ctrl+S.");
        }
        else
        {
            Debug.Log("[GunSystemSetup] ✅ Everything is already set up correctly. Nothing to do.");
        }
    }

    [MenuItem("Tools/FPS Gun System/Verify Only (No Changes)")]
    public static void VerifyOnly()
    {
        Debug.Log("[GunSystemSetup] ── Running verification ──────────────────");
        VerifyGunHolder(readOnly: true);
        VerifyBulletPrefab(readOnly: true);
        CheckCameraZoom(readOnly: true);
        Debug.Log("[GunSystemSetup] ── Verification done ────────────────────");
    }

    // ── Step 1: Add CameraZoom to Main Camera ────────────────────────────────────

    private static int SetupCameraZoom()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            // Try finding by name as fallback
            GameObject camGO = GameObject.Find("Main Camera");
            if (camGO != null) cam = camGO.GetComponent<Camera>();
        }

        if (cam == null)
        {
            Debug.LogWarning("[GunSystemSetup] ⚠️  Could not find Main Camera in the scene. " +
                             "Please add CameraZoom manually to your camera.");
            return 0;
        }

        CameraZoom existing = cam.GetComponent<CameraZoom>();
        if (existing != null)
        {
            Debug.Log($"[GunSystemSetup] ✅ CameraZoom already exists on '{cam.gameObject.name}'.");
            return 0;
        }

        CameraZoom zoom = cam.gameObject.AddComponent<CameraZoom>();
        Debug.Log($"[GunSystemSetup] ✅ Added CameraZoom to '{cam.gameObject.name}'. " +
                   "Normal FOV=60, Zoomed FOV=30, Speed=10.");
        return 1;
    }

    private static void CheckCameraZoom(bool readOnly)
    {
        Camera cam = Camera.main;
        if (cam == null) { Debug.LogWarning("[GunSystemSetup] ⚠️  No Main Camera found."); return; }

        bool has = cam.GetComponent<CameraZoom>() != null;
        string status = has ? "✅" : "❌ MISSING";
        Debug.Log($"[GunSystemSetup] CameraZoom on '{cam.gameObject.name}': {status}");
    }

    // ── Step 2: Verify GunHolder scene object ────────────────────────────────────

    private static int VerifyGunHolder(bool readOnly = false)
    {
        // Find GunHolder by component type
        var gunHolder = Object.FindFirstObjectByType<Assets.Scripts.Player.GunHolder>();

        if (gunHolder == null)
        {
            Debug.LogWarning("[GunSystemSetup] ⚠️  No GunHolder component found in the scene. " +
                             "Make sure your Gun Holder GameObject has the GunHolder script attached.");
            return 0;
        }

        // Use SerializedObject to read private [SerializeField] fields
        SerializedObject so = new SerializedObject(gunHolder);
        int fixes = 0;

        if (!readOnly && so.FindProperty("_defaultGun").objectReferenceValue == null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Guns/Gun Default.prefab");
            if (prefab != null)
            {
                var gunComp = prefab.GetComponent<Assets.Scripts.Interactions.Gun>();
                if (gunComp != null)
                {
                    so.FindProperty("_defaultGun").objectReferenceValue = gunComp;
                    fixes++;
                }
                else Debug.LogWarning("[GunSystemSetup] Gun Default prefab is missing the Gun script.");
            }
            else Debug.LogWarning("[GunSystemSetup] Could not load Assets/Prefabs/Guns/Gun Default.prefab");
        }

        if (!readOnly && so.FindProperty("_pistol").objectReferenceValue == null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Guns/Correct Pistol.prefab");
            if (prefab != null)
            {
                var gunComp = prefab.GetComponent<Assets.Scripts.Interactions.Gun>();
                if (gunComp != null)
                {
                    so.FindProperty("_pistol").objectReferenceValue = gunComp;
                    fixes++;
                }
                else Debug.LogWarning("[GunSystemSetup] Correct Pistol prefab is missing the Gun script.");
            }
            else Debug.LogWarning("[GunSystemSetup] Could not load Assets/Prefabs/Guns/Correct Pistol.prefab");
        }

        if (fixes > 0)
        {
            so.ApplyModifiedProperties();
            Debug.Log($"[GunSystemSetup] ✅ Auto-assigned {fixes} missing prefab(s) to GunHolder.");
        }

        LogField(so, "_defaultGun",      "Default Gun");
        LogField(so, "_pistol",          "Pistol");
        LogField(so, "_gunHoldingPoint", "Gun Holding Point");
        LogField(so, "_cameraTransform", "Camera Transform");

        Debug.Log($"[GunSystemSetup] GunHolder '{gunHolder.gameObject.name}': checked ✅");
        return fixes;
    }

    // ── Step 3: Verify Bullet Prefab ─────────────────────────────────────────────

    private static int VerifyBulletPrefab(bool readOnly = false)
    {
        // Load the bullet prefab from Assets/Prefabs/Bullets/
        string[] guids = AssetDatabase.FindAssets("t:Prefab Bullet", new[] { "Assets/Prefabs/Bullets" });

        if (guids.Length == 0)
        {
            Debug.LogWarning("[GunSystemSetup] ⚠️  Could not find any prefab named 'Bullet' " +
                             "in Assets/Prefabs/Bullets/.");
            return 0;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            // Check BulletController
            var bc = prefab.GetComponent<Assets.Scripts.Interactions.BulletController>();
            string bcStatus = bc != null ? "✅" : "❌ MISSING — add BulletController script";
            Debug.Log($"[GunSystemSetup] '{prefab.name}' BulletController: {bcStatus}");

            // Check Rigidbody
            var rb = prefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                string gravStatus = !rb.useGravity  ? "✅" : "⚠️  UseGravity is ON (should be OFF)";
                string kinStatus  = !rb.isKinematic ? "✅" : "⚠️  IsKinematic is ON (should be OFF)";
                Debug.Log($"[GunSystemSetup] '{prefab.name}' Rigidbody.UseGravity=false: {gravStatus}");
                Debug.Log($"[GunSystemSetup] '{prefab.name}' Rigidbody.IsKinematic=false: {kinStatus}");

                // Fix Rigidbody settings on the prefab if needed
                if (!readOnly && (rb.useGravity || rb.isKinematic))
                {
                    using (var editScope = new EditPrefabScope(path))
                    {
                        var prefabRb = editScope.Root.GetComponent<Rigidbody>();
                        if (prefabRb != null)
                        {
                            prefabRb.useGravity  = false;
                            prefabRb.isKinematic = false;
                            Debug.Log($"[GunSystemSetup] ✅ Fixed Rigidbody settings on '{prefab.name}' prefab.");
                        }
                    }
                    return 1;
                }
            }
            else
            {
                Debug.LogWarning($"[GunSystemSetup] ⚠️  '{prefab.name}' has no Rigidbody. " +
                                 "Add one with UseGravity=false, IsKinematic=false.");
            }

            // Check Collider (IsTrigger)
            var col = prefab.GetComponent<Collider>();
            if (col != null)
            {
                string trigStatus = col.isTrigger ? "✅" : "⚠️  IsTrigger is OFF (should be ON)";
                Debug.Log($"[GunSystemSetup] '{prefab.name}' Collider.IsTrigger: {trigStatus}");
            }
            else
            {
                Debug.LogWarning($"[GunSystemSetup] ⚠️  '{prefab.name}' has no Collider. " +
                                 "Add a BoxCollider/CapsuleCollider with IsTrigger=true.");
            }

            // Check TrailRenderer
            var trail = prefab.GetComponent<TrailRenderer>();
            string trailStatus = trail != null ? "✅" : "ℹ️  No TrailRenderer (optional)";
            Debug.Log($"[GunSystemSetup] '{prefab.name}' TrailRenderer: {trailStatus}");
        }

        return 0;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    private static void LogField(SerializedObject so, string fieldName, string friendlyName)
    {
        var prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogWarning($"[GunSystemSetup]   Field '{fieldName}' not found on component.");
            return;
        }

        bool assigned = prop.objectReferenceValue != null;
        string status = assigned
            ? $"✅ = '{prop.objectReferenceValue.name}'"
            : "❌ NOT ASSIGNED — drag the reference in the Inspector";

        Debug.Log($"[GunSystemSetup]   {friendlyName}: {status}");
    }

    /// <summary>
    /// Helper scope for safely editing and saving a prefab asset.
    /// </summary>
    private class EditPrefabScope : System.IDisposable
    {
        public readonly GameObject Root;
        private readonly string _path;

        public EditPrefabScope(string assetPath)
        {
            _path = assetPath;
            Root  = PrefabUtility.LoadPrefabContents(assetPath);
        }

        public void Dispose()
        {
            PrefabUtility.SaveAsPrefabAsset(Root, _path);
            PrefabUtility.UnloadPrefabContents(Root);
        }
    }
}
#endif
