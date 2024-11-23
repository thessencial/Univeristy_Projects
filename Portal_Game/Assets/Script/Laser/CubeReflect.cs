using UnityEngine;

public class CubeReflect : MonoBehaviour
{
    [SerializeField] private LineRenderer m_laser;
    [SerializeField] private LineRenderer m_laser2;
    [SerializeField] private LayerMask m_layerMask;
    private float m_MaxDistance = 50.0f;
    bool m_ActiveRefelt;

    bool m_Teleportabol = true;
    Rigidbody m_Rigidbody;
    [SerializeField] private float m_TeleportOffset = 1.5f;

    private PortalButton m_PortalButton;
    private Portal m_Portal;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_laser.gameObject.SetActive(m_ActiveRefelt);
        if (m_laser2 != null)
            m_laser2.gameObject.SetActive(m_ActiveRefelt);
        m_ActiveRefelt = false;
    }

    public void CreateRefraction()
    {
        if (m_ActiveRefelt)
            return;

        m_ActiveRefelt = true;
        Ray l_Ray = new Ray(m_laser.transform.position, m_laser.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_layerMask.value))
        {
            m_laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_RaycastHit.distance));
            m_laser.gameObject.SetActive(true);

            if (l_RaycastHit.collider.CompareTag("Reflect"))
                l_RaycastHit.collider.GetComponent<CubeReflect>().CreateRefraction();
            else if (l_RaycastHit.collider.CompareTag("Portal"))
            {
                l_RaycastHit.collider.GetComponent<Portal>().ActivateLaser(l_RaycastHit.point, transform.forward);
                m_Portal = l_RaycastHit.collider.GetComponent<Portal>();
            }
            else if (l_RaycastHit.collider.CompareTag("LaserIn"))
            {
                m_PortalButton = l_RaycastHit.collider.GetComponent<PortalButton>();
                l_RaycastHit.collider.GetComponent<PortalButton>().Interact2();
            }
            else
            {
                if (m_PortalButton != null)
                {
                    m_PortalButton.m_One = true;
                    m_PortalButton.m_Event2.Invoke();
                }

                if (m_Portal != null)
                {
                    m_Portal.m_Mirror.m_laser.gameObject.SetActive(false);
                }

                m_Portal = null;
                m_PortalButton = null;
            }
        }
        else
            m_laser.gameObject.SetActive(false);
        if (m_laser2 != null)
        {
            Ray l_Ray2 = new Ray(m_laser2.transform.position, m_laser2.transform.forward);
            if (Physics.Raycast(l_Ray2, out RaycastHit l_RaycastHit2, m_MaxDistance, m_layerMask.value))
            {
                m_laser2.SetPosition(1, new Vector3(0.0f, 0.0f, l_RaycastHit2.distance));
                m_laser2.gameObject.SetActive(true);

                if (l_RaycastHit2.collider.CompareTag("Reflect"))
                    l_RaycastHit2.collider.GetComponent<CubeReflect>().CreateRefraction();
            }
            else
                m_laser2.gameObject.SetActive(false);

        }
    }

    public bool IsTeleportable()
    {
        return m_Teleportabol;
    }

    public void SetTeleportable(bool Teleportable)
    {
        m_Teleportabol = Teleportable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTeleportable() && other.CompareTag("Portal"))
        {
            Teleport(other.GetComponent<Portal>());
        }
        else if (other.gameObject.CompareTag("DestroyWall"))
        {
            Destroy(gameObject);
        }
    }

    private void Teleport(Portal _Portal)
    {
        if (!GameManager.GetGameManager().GetPlayer().m_Attached)
        {
            Vector3 l_MovementDirection = m_Rigidbody.velocity;
            l_MovementDirection.Normalize();

            Vector3 l_Position = transform.position + l_MovementDirection * m_TeleportOffset;
            Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(l_Position);
            Vector3 l_WorldPosition = _Portal.m_Mirror.transform.TransformPoint(l_LocalPosition);

            Vector3 l_forward = transform.forward;
            Vector3 l_LocalForward = _Portal.m_OtherPortal.InverseTransformDirection(l_forward);
            Vector3 l_WorldForward = _Portal.m_Mirror.transform.TransformDirection(l_LocalForward);

            Vector3 l_LoclaVelocity = _Portal.m_OtherPortal.InverseTransformDirection(m_Rigidbody.velocity);
            Vector3 l_WorldVelocity = _Portal.m_Mirror.transform.TransformDirection(l_LoclaVelocity);

            float l_Sacle = _Portal.m_Mirror.transform.localScale.x / _Portal.transform.localScale.x;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.transform.position = l_WorldPosition;
            m_Rigidbody.transform.rotation = Quaternion.LookRotation(l_WorldForward);
            m_Rigidbody.transform.localScale = Vector3.one * l_Sacle;
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.velocity = l_WorldVelocity;

        }
    }
}
