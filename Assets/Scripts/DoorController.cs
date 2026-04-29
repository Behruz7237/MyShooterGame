using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public float openHeight = 3f;
    public float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    void Start()
    {
        closedPos = door.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isOpen = true;
    }

    void Update()
    {
        if (isOpen)
        {
            door.position = Vector3.Lerp(door.position, openPos, Time.deltaTime * speed);
        }
    }
}