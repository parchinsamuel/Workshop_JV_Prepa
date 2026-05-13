using UnityEngine;

public class Platform_Mobility : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private GameObject _platform;

    [Header("Parameters")]

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float distance = 3;
    [SerializeField] private float rotationY, rotationZ;
    [SerializeField] private bool topDownDirection;
    [SerializeField] private bool changeDirection;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = _platform.transform.position;
    }

    void Update()
    {
        PlatformMovement();
    }

    public void PlatformMovement()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, distance);

        // 1. Direction de base
        Vector3 direction = topDownDirection ? Vector3.up : Vector3.right;

        // 2. Appliquer rotation SI demandé
        if (changeDirection)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationY, rotationZ);
            direction = rotation * direction;
        }

        // 3. Appliquer mouvement
        _platform.transform.position = startPosition + direction * movement;
    }
}

