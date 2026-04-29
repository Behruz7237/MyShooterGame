using System.Reflection;
using UnityEngine;
using Assets.Scripts.Player; // Your custom namespace for GunHolder

public class SceneBootstrapper : MonoBehaviour
{
    [Header("Project Prefabs")]
    [Tooltip("Assign your Player Prefab from Assets/Prefabs")]
    public GameObject playerPrefab;

    private GameObject _playerInstance;
    private Camera _mainCamera;

    private void Awake()
    {
        Debug.Log("[Bootstrapper] Initializing Scene...");
        SetupPlayer();
        SetupCamera();
        RepairPlayerReferences();
        RepairGunHolderReferences();
        Debug.Log("[Bootstrapper] Scene Ready!");
    }

    private void SetupPlayer()
    {
        // 1. Locate or Spawn Player
        _playerInstance = GameObject.FindWithTag("Player");

        if (_playerInstance == null && playerPrefab != null)
        {
            // Spawn player slightly above 0,0,0 to prevent falling through the floor
            _playerInstance = Instantiate(playerPrefab, new Vector3(0, 5f, 0), Quaternion.identity);
            _playerInstance.tag = "Player";
        }
    }

    private void SetupCamera()
    {
        if (_playerInstance == null) return;

        // 1. Ensure only one Main Camera exists
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            _mainCamera = Object.FindFirstObjectByType<Camera>();
            if (_mainCamera != null) _mainCamera.tag = "MainCamera";
        }

        if (_mainCamera == null) return;

        // 2. Find CameraPosition on Player and parent the camera
        Transform cameraPoint = _playerInstance.transform.Find("CameraPosition");
        if (cameraPoint != null)
        {
            _mainCamera.transform.SetParent(cameraPoint);
            _mainCamera.transform.localPosition = Vector3.zero;
            _mainCamera.transform.localRotation = Quaternion.identity;
        }
    }

    private void RepairPlayerReferences()
    {
        if (_playerInstance == null) return;

        // Get PlayerController (from your global namespace)
        var playerController = _playerInstance.GetComponent<PlayerController>();
        if (playerController == null) return;

        // Use Reflection to assign private [SerializeField] fields just in case they broke
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

        // Assign CharacterController
        var ccField = typeof(PlayerController).GetField("controller", flags);
        if (ccField != null && ccField.GetValue(playerController) == null)
            ccField.SetValue(playerController, _playerInstance.GetComponent<CharacterController>());

        // Assign CameraPoint
        var cpField = typeof(PlayerController).GetField("cameraPoint", flags);
        Transform cameraPoint = _playerInstance.transform.Find("CameraPosition");
        if (cpField != null && cpField.GetValue(playerController) == null)
            cpField.SetValue(playerController, cameraPoint);

        // Assign GroundCheck
        var gcField = typeof(PlayerController).GetField("groundCheck", flags);
        Transform groundChecker = _playerInstance.transform.Find("Ground Checker");
        if (gcField != null && gcField.GetValue(playerController) == null)
            gcField.SetValue(playerController, groundChecker);

        // Assign GunHolder
        var ghField = typeof(PlayerController).GetField("_gunHolder", flags);
        GunHolder gunHolder = _playerInstance.GetComponentInChildren<GunHolder>();
        if (ghField != null && ghField.GetValue(playerController) == null)
            ghField.SetValue(playerController, gunHolder);
    }

    private void RepairGunHolderReferences()
    {
        if (_playerInstance == null || _mainCamera == null) return;

        GunHolder gunHolder = _playerInstance.GetComponentInChildren<GunHolder>();
        if (gunHolder == null) return;

        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

        // Assign _cameraTransform
        var camField = typeof(GunHolder).GetField("_cameraTransform", flags);
        if (camField != null && camField.GetValue(gunHolder) == null)
            camField.SetValue(gunHolder, _mainCamera.transform);

        // Assign _gunHoldingPoint
        var ghpField = typeof(GunHolder).GetField("_gunHoldingPoint", flags);
        Transform gunHolderTransform = _playerInstance.transform.Find("CameraPosition/Gun Holder");
        if (ghpField != null && ghpField.GetValue(gunHolder) == null)
            ghpField.SetValue(gunHolder, gunHolderTransform);

        // Assign Animator (assuming it's on the Gun Holder or Player)
        var animField = typeof(GunHolder).GetField("_animator", flags);
        Animator animator = gunHolder.GetComponent<Animator>() ?? _playerInstance.GetComponent<Animator>();
        if (animField != null && animField.GetValue(gunHolder) == null)
            animField.SetValue(gunHolder, animator);
    }
}