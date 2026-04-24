using UnityEditor;
using UnityEngine;

public class ArenaSetup : EditorWindow
{
    private GameObject enemyPrefab;

    [MenuItem("Tools/Generate FPS Arena")]
    public static void ShowWindow()
    {
        GetWindow<ArenaSetup>("Generate Arena");
    }

    private void OnGUI()
    {
        GUILayout.Label("FPS Arena Generator", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab (Optional)", enemyPrefab, typeof(GameObject), false);

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Arena"))
        {
            GenerateArena();
        }

        GUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "This will create a complete arena with:\n" +
            "- 50x50 ground\n" +
            "- 4 boundary walls\n" +
            "- 5 platforms with walkable stairs\n" +
            "- 2 cover blocks\n" +
            "- 5 enemies",
            MessageType.Info);
    }

    private void GenerateArena()
    {
        // Delete old arena if it exists
        GameObject oldArena = GameObject.Find("FPS_Arena");
        if (oldArena != null) DestroyImmediate(oldArena);

        GameObject arenaRoot = new GameObject("FPS_Arena");

        // ══════════════ GROUND ══════════════
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Arena_Ground";
        ground.transform.position = new Vector3(0, -0.5f, 0);
        ground.transform.localScale = new Vector3(50, 1, 50);
        ground.transform.SetParent(arenaRoot.transform);

        // ══════════════ WALLS ══════════════
        CreateCube(new Vector3(0, 5, 25), new Vector3(50, 10, 1), "North_Wall", arenaRoot.transform);
        CreateCube(new Vector3(0, 5, -25), new Vector3(50, 10, 1), "South_Wall", arenaRoot.transform);
        CreateCube(new Vector3(25, 5, 0), new Vector3(1, 10, 50), "East_Wall", arenaRoot.transform);
        CreateCube(new Vector3(-25, 5, 0), new Vector3(1, 10, 50), "West_Wall", arenaRoot.transform);

        // ══════════════ PLATFORMS + STAIRS ══════════════
        // Each platform gets stairs that start from ground, extend OUTWARD,
        // and connect flush to the platform edge.
        //
        // stairDirection = which way the stairs extend FROM the platform.
        //   Vector3.back  = stairs go toward -Z (south side of platform)
        //   Vector3.forward = stairs go toward +Z (north side of platform)
        //   Vector3.left  = stairs go toward -X (west side)
        //   Vector3.right = stairs go toward +X (east side)

        // Platform 1: Low platform in NW area, stairs come down toward south
        CreatePlatformWithStairs(
            platformPos: new Vector3(-10, 1.5f, 10),
            platformScale: new Vector3(4, 1, 4),
            name: "Platform_1",
            stairDirection: Vector3.back,
            parent: arenaRoot.transform
        );

        // Platform 2: Medium platform in NE area, stairs come down toward south
        CreatePlatformWithStairs(
            platformPos: new Vector3(10, 2f, 10),
            platformScale: new Vector3(4, 1, 4),
            name: "Platform_2",
            stairDirection: Vector3.back,
            parent: arenaRoot.transform
        );

        // Platform 3: High platform in far north, stairs come down toward south
        CreatePlatformWithStairs(
            platformPos: new Vector3(0, 3f, 15),
            platformScale: new Vector3(5, 1, 5),
            name: "Platform_3",
            stairDirection: Vector3.back,
            parent: arenaRoot.transform
        );

        // Platform 4: Medium platform in SW area, stairs come down toward north
        CreatePlatformWithStairs(
            platformPos: new Vector3(-10, 2f, -10),
            platformScale: new Vector3(6, 1, 6),
            name: "Platform_4",
            stairDirection: Vector3.forward,
            parent: arenaRoot.transform
        );

        // Platform 5: Tall platform in SE area, stairs come down toward north
        CreatePlatformWithStairs(
            platformPos: new Vector3(10, 3f, -10),
            platformScale: new Vector3(4, 1, 4),
            name: "Platform_5",
            stairDirection: Vector3.forward,
            parent: arenaRoot.transform
        );

        // ══════════════ COVER BLOCKS ══════════════
        CreateCube(new Vector3(-5, 1f, 0), new Vector3(2, 2, 2), "Cover_1", arenaRoot.transform);
        CreateCube(new Vector3(5, 1f, 0), new Vector3(2, 2, 2), "Cover_2", arenaRoot.transform);

        // ══════════════ ENEMIES ══════════════
        Vector3[] enemyPositions = new Vector3[]
        {
            new Vector3(-18, 1, 18),
            new Vector3(18, 1, 18),
            new Vector3(-18, 1, -18),
            new Vector3(18, 1, -18),
            new Vector3(0, 1, 20)
        };

        GameObject enemiesRoot = new GameObject("Enemies_Root");
        enemiesRoot.transform.SetParent(arenaRoot.transform);

        for (int i = 0; i < enemyPositions.Length; i++)
        {
            if (enemyPrefab != null)
            {
                GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
                enemy.transform.position = enemyPositions[i];
                enemy.name = "Enemy_" + (i + 1);
                enemy.transform.SetParent(enemiesRoot.transform);
            }
            else
            {
                // Create a tagged placeholder capsule
                GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                placeholder.transform.position = enemyPositions[i];
                placeholder.name = "Enemy_" + (i + 1);
                placeholder.tag = "Enemy";
                placeholder.transform.SetParent(enemiesRoot.transform);
            }
        }

        Debug.Log("✅ Arena generated successfully! Stairs are walkable, enemies are placed.");
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  HELPER: Create a simple Cube
    // ═══════════════════════════════════════════════════════════════════════
    private void CreateCube(Vector3 position, Vector3 scale, string name, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        cube.transform.SetParent(parent);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  HELPER: Create a Platform + perfectly aligned walkable stairs
    // ═══════════════════════════════════════════════════════════════════════
    private void CreatePlatformWithStairs(
        Vector3 platformPos, Vector3 platformScale, string name,
        Vector3 stairDirection, Transform parent)
    {
        // ── Create the platform itself ──
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = name;
        platform.transform.position = platformPos;
        platform.transform.localScale = platformScale;
        platform.transform.SetParent(parent);

        // ── Stair dimensions ──
        // Step height 0.3 allows the player to walk up smoothly
        // without jumping, matching CharacterController's stepOffset.
        float stepHeight = 0.3f;
        float stepDepth = 0.8f;
        float stepWidth = 2.0f;

        // The top of the platform surface (where stairs must reach)
        float platformTopY = platformPos.y + (platformScale.y / 2f);

        // Number of steps needed to reach the platform top from ground (Y=0)
        int stepCount = Mathf.CeilToInt(platformTopY / stepHeight);

        // ── Calculate where the stairs connect to the platform edge ──
        // The top step should be flush with the platform edge.
        // stairDirection points AWAY from the platform (where stairs go down).
        //
        // For Z-directed stairs: edge is at platformPos.z ± platformScale.z/2
        // For X-directed stairs: edge is at platformPos.x ± platformScale.x/2

        Vector3 platformEdge = platformPos;
        if (Mathf.Abs(stairDirection.z) > 0.5f)
        {
            // Stairs extend along Z axis
            platformEdge.z += stairDirection.z * (platformScale.z / 2f);
        }
        else
        {
            // Stairs extend along X axis
            platformEdge.x += stairDirection.x * (platformScale.x / 2f);
        }

        // ── Build steps from TOP to BOTTOM ──
        // The top step (i=0) is at the platform edge, at platformTopY.
        // Each subsequent step is one stepHeight lower and one stepDepth further out.

        GameObject stairsRoot = new GameObject(name + "_Stairs");
        stairsRoot.transform.SetParent(parent);

        for (int i = 0; i < stepCount; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.name = "Step_" + (i + 1);

            // Scale: wide enough to walk on, shallow height, appropriate depth
            if (Mathf.Abs(stairDirection.z) > 0.5f)
                step.transform.localScale = new Vector3(stepWidth, stepHeight, stepDepth);
            else
                step.transform.localScale = new Vector3(stepDepth, stepHeight, stepWidth);

            // Position: start at the platform edge and step outward + downward
            Vector3 stepPos = platformEdge;

            // Move outward from platform edge (i=0 is right at edge, i=1 is one step out, etc.)
            if (Mathf.Abs(stairDirection.z) > 0.5f)
                stepPos.z += stairDirection.z * (stepDepth * 0.5f + i * stepDepth);
            else
                stepPos.x += stairDirection.x * (stepDepth * 0.5f + i * stepDepth);

            // Move downward: top step is at platform height, each lower step drops by stepHeight
            stepPos.y = platformTopY - (i * stepHeight) - (stepHeight / 2f);

            step.transform.position = stepPos;
            step.transform.SetParent(stairsRoot.transform);
        }
    }
}
