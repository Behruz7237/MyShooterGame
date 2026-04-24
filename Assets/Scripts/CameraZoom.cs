using UnityEngine;

/// <summary>
/// Attach to the Camera GameObject (same object that has your FPSMouseLook or camera component).
/// Controls zoom (FOV) on right mouse button hold.
/// </summary>
public class CameraZoom : MonoBehaviour
{
    // ── Inspector ───────────────────────────────────────────────────────────────
    [Header("FOV Settings")]
    [Tooltip("Normal field of view when not zooming.")]
    [SerializeField] private float normalFOV = 60f;

    [Tooltip("Zoomed-in field of view (lower = more zoom).")]
    [SerializeField] private float zoomedFOV = 30f;

    [Tooltip("Speed of the zoom transition. Higher = snappier.")]
    [SerializeField] private float zoomSpeed = 10f;

    // ── Private ─────────────────────────────────────────────────────────────────
    private Camera _camera;
    private float  _targetFOV;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────────
    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_camera == null)
        {
            Debug.LogError("[CameraZoom] No Camera component found on this GameObject! " +
                           "Attach CameraZoom to the same object as your Camera.", this);
            enabled = false;
            return;
        }

        // Initialise both values from whatever the camera's current FOV is
        normalFOV  = _camera.fieldOfView;
        _targetFOV = normalFOV;
    }

    private void Update()
    {
        // Right mouse button held → zoom in; released → zoom out
        _targetFOV = Input.GetMouseButton(1) ? zoomedFOV : normalFOV;

        // Smoothly interpolate towards the target FOV
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFOV, zoomSpeed * Time.deltaTime);
    }
}
