using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform m_RespawnPoint;
    public Animator m_CheckPointAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_CheckPointAnimator != null)
                m_CheckPointAnimator.SetTrigger("CheckPoint");
        }
    }
}
