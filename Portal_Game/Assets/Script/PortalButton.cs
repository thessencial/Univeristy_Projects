using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour, IInteractable
{
    public UnityEvent m_Event;
    public UnityEvent m_Event2;

    private Animation m_Animation;
    [SerializeField] private AnimationClip m_ButtonDownClip;
    [SerializeField] private AnimationClip m_ButtonUpClip;

    [SerializeField] private bool m_OneTime;
    private GameObject m_Cube;

    public bool m_One = true;

    private void Start()
    {
        m_Animation = GetComponent<Animation>();
    }

    public void Interact()
    {
        if (m_OneTime)
        {
            if (m_Cube != null)
                m_Cube.SetActive(false);
            m_Event.Invoke();
            StartCoroutine(Animation());
            m_Cube = GameObject.FindWithTag("Cube");
        }
        else
        {
            m_Event.Invoke();
            StartCoroutine(Animation());
        }
    }

    public void Interact2()
    {
        if (m_One)
        {
          m_Event.Invoke();
            m_One = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Interact2();
            ButtonDown();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            m_Event2.Invoke();
            m_One = true;
            ButtonUp();
        }
    }

    IEnumerator Animation()
    {
        m_Animation.CrossFade(m_ButtonDownClip.name);
        yield return new WaitForSeconds(m_ButtonDownClip.length);
        m_Animation.CrossFade(m_ButtonUpClip.name);
    }

    private void ButtonDown()
    {
        m_Animation.CrossFade(m_ButtonDownClip.name);
    }

    public void ButtonUp()
    {
        m_Animation.CrossFade(m_ButtonUpClip.name);
    }
}
