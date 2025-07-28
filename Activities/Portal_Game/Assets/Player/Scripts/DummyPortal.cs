using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DummyPortal : BasePortal
{
    public GameObject m_OkPortal;
    public GameObject m_WrongPortal;
    public GameObject m_OrangePortal;
    public GameObject m_BluePortal;

    public List<Transform> m_ValidPoints;
    public LayerMask m_ValidityLayerMask;
    public float m_ValidityOffset = 0.02f;
    public float m_ValidityAngleInDegrees = 3.0f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }


    public void SetPortalOk(bool Ok)
    {
        //m_OkPortal.SetActive(Ok);
        m_WrongPortal.SetActive(!Ok);
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
}
