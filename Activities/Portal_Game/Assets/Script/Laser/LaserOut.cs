using UnityEngine;

public class LaserOut : MonoBehaviour
{
    [SerializeField] private LineRenderer m_laser;
    [SerializeField] private LayerMask m_layerMask;
    private float m_MaxDistance = 50.0f;

    private void Update()
    {
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
            }
            else if (l_RaycastHit.collider.CompareTag("LaserIn"))
            {
                l_RaycastHit.collider.GetComponent<PortalButton>().Interact2();
            }
        }
        else
            m_laser.gameObject.SetActive(false);
    }
}
