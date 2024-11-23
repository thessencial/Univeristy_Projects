using UnityEngine;

public class MoveDoor : MonoBehaviour
{
    private enum State
    {
        Key,
        Point,
        Nothing,
        Kill
    }

    [SerializeField] private State state;

    [SerializeField] private Animation AnimationDoor;
    [SerializeField] private AnimationClip Nothing_PointOpen;
    [SerializeField] private AnimationClip Nothing_PointClose;
    [SerializeField] private AnimationClip Key;

    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource m_ControlAudio;

    private bool OneTime = true;
    [SerializeField] private Transform m_Target;

    private void Start()
    {
        if (state == State.Kill)
        {
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (state)
            {
                case State.Key:
                    if (CodeCharacterController.instance.Keys >= 1 && OneTime == true)
                    {
                        m_ControlAudio.PlayOneShot(sound);
                        AnimationDoor.CrossFade(Key.name);
                        CodeCharacterController.instance.m_StartPosition = m_Target.position;
                        OneTime = false;
                    }
                    break;
                case State.Point:
                    if (ShootingScoreManager.Instance.m_GlobalScore >= 300)
                    {
                        m_ControlAudio.PlayOneShot(sound);
                        AnimationDoor.CrossFade(Nothing_PointOpen.name);
                    }
                    break;
                case State.Nothing:
                    m_ControlAudio.PlayOneShot(sound);
                    AnimationDoor.CrossFade(Nothing_PointOpen.name);
                    break;
                case State.Kill:
                    CodeCharacterController.instance.Kill();
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (state)
            {
                case State.Key:

                    break;
                case State.Point:
                    if (ShootingScoreManager.Instance.m_GlobalScore >= 300)
                    {
                        m_ControlAudio.PlayOneShot(sound);
                        AnimationDoor.CrossFade(Nothing_PointClose.name);
                    }
                    break;
                case State.Nothing:
                    m_ControlAudio.PlayOneShot(sound);
                    AnimationDoor.CrossFade(Nothing_PointClose.name);
                    break;
            }
        }
    }
}
