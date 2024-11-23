using UnityEngine;

public class Door : MonoBehaviour
{
    private Animation m_Animation;
    [SerializeField] private AnimationClip m_DoorOpenClip;
    [SerializeField] private AnimationClip m_DoorCloseClip;

    public AudioSource m_ControlAudio;
    public AudioClip m_DoorSound;

    private void Start()
    {
        m_Animation = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDoor();
        }
    }

    public void CloseDoor()
    {
        m_Animation.CrossFade(m_DoorCloseClip.name);
        if (m_ControlAudio != null)
            m_ControlAudio.PlayOneShot(m_DoorSound);
    }

    public void OpenDoor()
    {
        m_Animation.CrossFade(m_DoorOpenClip.name);
        if (m_ControlAudio != null)
            m_ControlAudio.PlayOneShot(m_DoorSound);
    }
}
