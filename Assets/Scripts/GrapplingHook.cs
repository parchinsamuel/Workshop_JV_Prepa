using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public static GrapplingHook Instance;

    [Header("Réglages du rayon")]
    [Range(0f, 100f)] public float DistanceMax = 30f;
    public LayerMask HangingLayers;

    [Header("Physique")]
    [Range(0.1f, 50f)] public float Elasticite = 4.5f;
    [Range(0.1f, 100f)] public float ForceRessort = 15f;
    [Range(0.1f, 50f)] public float MasseEchelle = 4.5f;

    [Header("Game Feel")]
    public float forceImpulsionInitiale = 15f;
    [Range(0.1f, 1f)] public float ratioRaccourcissement = 0.4f;

    [Header("Feedback Visuel")]
    public GameObject MarqueurVisuel;

    [HideInInspector] public bool IsGrappling = false;

    private SpringJoint _joint;
    private Rigidbody _rb;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody>();
        if (MarqueurVisuel != null)
        {
            MarqueurVisuel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LancerRayon();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ArreterRayon();
        }
    }

    void LancerRayon()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, DistanceMax, HangingLayers))
        {
            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = hit.point;

            float distance = Vector3.Distance(transform.position, hit.point);

            _joint.maxDistance = distance * ratioRaccourcissement;
            _joint.minDistance = distance * 0.25f;

            _joint.spring = ForceRessort;
            _joint.damper = Elasticite;
            _joint.massScale = MasseEchelle;

            Vector3 directionBalancier = Camera.main.transform.forward;

            //Vector3 directionTraction = (hit.point - transform.position).normalized;
            _rb.AddForce(directionBalancier * forceImpulsionInitiale, ForceMode.Impulse);

            if (MarqueurVisuel != null)
            {
                MarqueurVisuel.transform.position = hit.point;
                MarqueurVisuel.SetActive(true);
            }

            IsGrappling = true;
        }
    }

    void ArreterRayon()
    {
        if (_joint != null)
        {
            Destroy(_joint);
        }
        IsGrappling = false;

        if (MarqueurVisuel != null)
        {
            MarqueurVisuel.SetActive(false);
        }
    }
}
