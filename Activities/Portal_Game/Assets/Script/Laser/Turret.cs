using System;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private LineRenderer m_laser;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private float m_MaxDistance = 50.0f;
    [SerializeField] private float m_MaxAngleAlife = 10.0f;

    bool m_Teleportabol = true;
    Rigidbody m_Rigidbody;
    [SerializeField] private float m_TeleportOffset = 1.5f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsAlife())
        {
            Ray l_Ray = new Ray(m_laser.transform.position, m_laser.transform.forward);
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_layerMask.value))
            {
                m_laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_RaycastHit.distance));
                m_laser.gameObject.SetActive(true);

                if (l_RaycastHit.collider.CompareTag("Player"))
                {
                    l_RaycastHit.collider.GetComponent<CodeCharacterController>().Kill();
                }
                else if (l_RaycastHit.collider.CompareTag("Turret"))
                {
                    l_RaycastHit.collider.GetComponent<Turret>().Dead();
                }
            }
        }
        else
            m_laser.gameObject.SetActive(false);
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    bool IsAlife()
    {
        return Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_MaxAngleAlife * Mathf.Deg2Rad);
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
