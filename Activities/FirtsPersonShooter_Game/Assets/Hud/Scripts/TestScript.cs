using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestScript : MonoBehaviour, IInteractable
{

    public GameObject m_Target;
    private float m_distance;
    public float m_Distance1 = -5.0f;
    public float m_Distance2 = 5.0f;
    public float m_Speed = 1.0f;


    public float m_LimitMin = -10.0f;
    public float m_LimitMax = 10.0f;

    public enum Mode
    {
        Backward,
        Forward
    }

    public Mode m_Mode;

    private bool m_Running = false;
    private Vector3 m_TargetPosition;


    void Start()
    {
        Vector3 m_InitialPosition = m_Target.transform.position;
        m_LimitMax += m_InitialPosition.z;
        m_LimitMin += m_InitialPosition.z;
    }

    public void Interact()
    {
        if (m_Mode == Mode.Backward)
        {
            Debug.Log("Backward");
            m_distance = m_Distance1;
        }
        if (m_Mode == Mode.Forward)
        {
            Debug.Log("Forward");
            m_distance = m_Distance2;
        }

        m_TargetPosition = new Vector3(m_Target.transform.position.x, m_Target.transform.position.y, Mathf.Clamp(m_Target.transform.position.z + m_distance, m_LimitMin, m_LimitMax));
        m_Running = true;
        Debug.Log("Boton OK");

    }

    private void Update()
    {
        if (m_Running)
        {
            m_Target.transform.position = Vector3.MoveTowards(m_Target.transform.position, m_TargetPosition, m_Speed * Time.deltaTime);

            if (Vector3.Distance(m_Target.transform.position, m_TargetPosition) < 0.01)
            {
                m_Running = false;

            }
        }
    }
}
