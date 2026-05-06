using Unity.VisualScripting;
using UnityEngine;

public class OxygenCapsule : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private OxygenManager oxygenManager;

    private void OnTriggerEnter(Collider other)
    {
            oxygenManager.AddOxygen();
            Destroy(gameObject);
    }
}
