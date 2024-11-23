using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform m_InteractorSource;
    public float m_InteractRange;
    public LayerMask m_ButtonLayerMask;
    public Camera m_Camera;
    public TextMeshProUGUI m_InteractionText;
    public string m_InteractionMessage = "Press E to Interact";

    //private bool m_IsLookingAtInteractable = false;
    private IInteractable m_CurrentInteractable = null;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Lab")
        {
            gameObject.GetComponent<Interactor>().enabled = false;
        }
    }

    private void Start()
    {
        m_InteractionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(ray, out RaycastHit m_HitInfo, m_InteractRange, m_ButtonLayerMask.value))
        {
            if (m_HitInfo.collider.gameObject.TryGetComponent(out IInteractable m_interactObj))
            {
                if (m_CurrentInteractable == null || m_CurrentInteractable != m_interactObj)
                {
                    m_CurrentInteractable = m_interactObj;
                    m_InteractionText.text = m_InteractionMessage;
                    m_InteractionText.gameObject.SetActive(true);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    m_interactObj.Interact();
                }
            }
            else
            {
                ClearInteractable();
            }
        }
        else
        {
            ClearInteractable();
        }
    }

    private void ClearInteractable()
    {
        if (m_CurrentInteractable != null)
        {
            m_CurrentInteractable = null;
            m_InteractionText.gameObject.SetActive(false);
        }
    }

}