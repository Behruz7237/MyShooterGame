using UnityEngine;

public class SceneUpdater : MonoBehaviour
{
    [SerializeField] private MeshRenderer _cube;
    [SerializeField] private Material _differentMaterial;

    private void Start()
    {
        ChangeCubeColor();
    }

    private void ChangeCubeColor()
    {
        _cube.material = _differentMaterial;
    }
}
