using System.Collections.Generic;
using UnityEngine;

public class Portal : BasePortal
{
    public Transform m_OtherPortal;
    [SerializeField] private Camera m_Camera;
    public Portal m_Mirror;
    [SerializeField] private float m_OffsetCamera = 0.9f;

    public List<Transform> m_ValidPoints;
    public LayerMask m_ValidityLayerMask;
    public float m_ValidityOffset = 0.02f;
    public float m_ValidityAngleInDegrees = 3.0f;

    public LineRenderer m_laser;
    [SerializeField] private LayerMask m_layerMask;
    private float m_MaxDistance = 50.0f;
    [SerializeField] private float m_TeleportOffset;

    public static Portal m_Insstance;

    public enum Type
    {
        Blue,
        Orange
    }

    public Type m_Type;

    private void Start()
    {
        m_laser.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        m_laser.gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 l_position = CodeCharacterController.instance.m_Camera.transform.position;
        Vector3 l_forwadr = CodeCharacterController.instance.m_Camera.transform.forward;
        Vector3 l_localPosition = m_OtherPortal.InverseTransformPoint(l_position);
        Vector3 l_localForwardadr = m_OtherPortal.InverseTransformDirection(l_forwadr);

        Vector3 l_WorldPosition = m_Mirror.transform.TransformPoint(l_localPosition);
        Vector3 l_WorldForward = m_Mirror.transform.TransformDirection(l_localForwardadr);
        m_Mirror.m_Camera.transform.position = l_WorldPosition;
        m_Mirror.m_Camera.transform.forward = l_WorldForward;

        float l_DistanceToPotral = Vector3.Distance(l_WorldPosition, m_Mirror.transform.position);
        float l_DistanceNearClip = m_OffsetCamera + l_DistanceToPotral;
        m_Mirror.m_Camera.nearClipPlane = l_DistanceNearClip;
    }

    public bool IsValidPosition(Vector3 Position, Vector3 Normal)
    {
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);

        Camera l_PlayerCamera = CodeCharacterController.instance.m_Camera;
        float l_Angle = Mathf.Cos(m_ValidityAngleInDegrees * Mathf.Deg2Rad);
        bool l_IsValid = true;

        for (int i = 0; i < m_ValidPoints.Count; ++i)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - l_PlayerCamera.transform.position;
            float l_Distance = l_Direction.magnitude;
            l_Direction /= l_Distance;
            Ray l_Ray = new Ray(l_PlayerCamera.transform.position, l_Direction);
            Color l_Color = Color.green;
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, l_Distance + m_ValidityOffset, m_ValidityLayerMask.value))
            {
                if (l_RaycastHit.collider.CompareTag("Drawable"))
                {
                    if (Vector3.Distance(m_ValidPoints[i].position, l_RaycastHit.point) < m_ValidityOffset)
                    {
                        float l_DotAngle = Vector3.Dot(l_RaycastHit.normal, m_ValidPoints[i].forward);
                        if (l_DotAngle < l_Angle)
                        {
                            l_IsValid = false;
                            l_Color = Color.magenta;
                        }
                    }
                    else
                    {
                        l_IsValid = false;
                        l_Color = Color.cyan;
                    }
                }
                else
                {
                    l_IsValid = false;
                    l_Color = Color.blue;
                }
            }
            else
            {
                l_IsValid = false;
                l_Color = Color.red;
            }
            Debug.DrawLine(l_PlayerCamera.transform.position, m_ValidPoints[i].position, l_Color, 2.0f);
        }

        return l_IsValid;
    }

    public void ActivateLaser(Vector3 Position, Vector3 Forward)
    {
        if (m_Type == Type.Blue)
        {
            Vector3 l_Position = Position + m_Mirror.transform.position * m_TeleportOffset;
            Vector3 l_LocalPosition = m_OtherPortal.InverseTransformPoint(l_Position);
            Vector3 l_WorldPosition = m_Mirror.transform.TransformPoint(l_LocalPosition);

            Vector3 l_forward = Forward;
            Vector3 l_LocalForward = m_OtherPortal.InverseTransformDirection(l_forward);
            Vector3 l_WorldForward = m_Mirror.transform.TransformDirection(l_LocalForward);

            m_Mirror.m_laser.transform.position = l_WorldPosition;
            m_Mirror.m_laser.transform.forward = l_WorldForward;

            Ray l_Ray = new Ray(m_Mirror.m_laser.transform.position, m_Mirror.m_laser.transform.forward);
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_layerMask.value))
            {
                m_Mirror.m_laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_RaycastHit.distance));
                m_Mirror.m_laser.gameObject.SetActive(true);

                if (l_RaycastHit.collider.CompareTag("Reflect"))
                    l_RaycastHit.collider.GetComponent<CubeReflect>().CreateRefraction();
            }
            else
                m_Mirror.m_laser.gameObject.SetActive(false);
        }
        else if (m_Type == Type.Orange)
        {
            Vector3 l_Position = Position + m_Mirror.transform.position * m_TeleportOffset;
            Vector3 l_LocalPosition = m_OtherPortal.InverseTransformPoint(l_Position);
            Vector3 l_WorldPosition = m_Mirror.transform.TransformPoint(l_LocalPosition);

            Vector3 l_forward = Forward;
            Vector3 l_LocalForward = m_OtherPortal.InverseTransformDirection(l_forward);
            Vector3 l_WorldForward = m_Mirror.transform.TransformDirection(l_LocalForward);

            m_Mirror.m_laser.transform.position = l_WorldPosition;
            m_Mirror.m_laser.transform.forward = l_WorldForward;

            Ray l_Ray = new Ray(m_Mirror.m_laser.transform.position, m_Mirror.m_laser.transform.forward);
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_layerMask.value))
            {
                m_Mirror.m_laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_RaycastHit.distance));
                m_Mirror.m_laser.gameObject.SetActive(true);

                if (l_RaycastHit.collider.CompareTag("Reflect"))
                    l_RaycastHit.collider.GetComponent<CubeReflect>().CreateRefraction();
            }
            else
                m_Mirror.m_laser.gameObject.SetActive(false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_Type == Type.Blue)
            m_Mirror.m_laser.gameObject.SetActive(false);
        else if (m_Type == Type.Orange)
            m_Mirror.m_laser.gameObject.SetActive(false);
    }
}
