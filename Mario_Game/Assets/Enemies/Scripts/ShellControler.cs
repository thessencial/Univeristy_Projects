using UnityEngine;

public class ShellControler : MonoBehaviour
{
    Rigidbody m_ObjectRB;
    bool m_Moving = false;
    public int m_BouncingTime = 3;
    public Transform m_PointSphere;
    public float m_Radius = 10.0f;
    public LayerMask m_LayerMask;

    private void Start()
    {
        m_ObjectRB = GetComponent<Rigidbody>();
        GameManager.GetGameManager().SetShell(this);
    }

    public void SetForce(float Force, Vector3 Forward)
    {
        m_ObjectRB.isKinematic = false;
        m_Moving = true;
        m_ObjectRB.AddForce(Forward * Force, ForceMode.Impulse);
        //m_ObjectRB.velocity = Forward * Force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Moving)
        {
            if (Physics.CheckSphere(m_PointSphere.position, m_Radius, m_LayerMask))
            {
                if (m_BouncingTime > 0)
                {
                    m_BouncingTime--;
                }
                else
                {
                    m_BouncingTime = 3;
                    m_Moving = false;
                    m_ObjectRB.isKinematic = true;
                }

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<EnemyController>().Kill();
                }
                else if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<HealthController>().TakeDamage();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_PointSphere.position, m_Radius);
    }
}
