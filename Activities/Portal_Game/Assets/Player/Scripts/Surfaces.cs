using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Surfaces : MonoBehaviour
{
    // Start is called before the first frame update

    //public float m_iceSliderFactor = 5.0f;
    private Rigidbody m_Rigidbody;
    //private bool m_IsOnIce = false;
    public float m_ExtraBounceForce = 10.0f;

    private void Start()
    {
        m_Rigidbody =GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestroyWall"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bouncy"))
        {
            Vector3 bounceDirection = Vector3.up;
            m_Rigidbody.AddForce(bounceDirection * m_ExtraBounceForce, ForceMode.Impulse);
        }
    }


    /*private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            m_IsOnIce = true;
        }
    }
    private void FixedUpdate()
    {
        if (m_IsOnIce)
        {
            Vector3 slideDirection = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            m_Rigidbody.velocity = slideDirection * m_iceSliderFactor;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            m_IsOnIce = false;
        }
    }*/
  
}
