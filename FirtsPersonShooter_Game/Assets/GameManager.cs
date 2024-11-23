using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IRestartGameElement
{
    void RestartGame();
}

public class GameManager : MonoBehaviour
{
    //public GameObject m_DeadUI;
    public static GameManager m_GameManager;
    CodeCharacterController m_Player;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();
    public Animation m_DeadUI;

    [SerializeField] private SOEvents raiseEvent;

    public Image m_FadeOutImage;
    public float m_fadeSpeed = 2.0f;
    public float m_ShakeDuration = 10.0f;
    public float m_ShakeAmplitud = 1.0f;

    private void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    static public GameManager GetGameManager()
    {
        return m_GameManager;
    }

    public CodeCharacterController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(CodeCharacterController player)
    {
        m_Player = player;
    }

    public SOEvents raiseEvents()
    {
        return raiseEvent;
    }

    public void InitLevel(CodeCharacterController player)
    {
        m_Player.transform.position = player.transform.position;
        m_Player.transform.rotation = player.transform.rotation;
    }

    public void OnMainMenu()
    {
        GameObject.Destroy(gameObject);
        GameObject.Destroy(m_Player.gameObject);
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    delegate void OnDeadUIFinishedFn();

    /*public void RestartGame()
    {
        StartCoroutine(DeadUICoroutine(RestartGameAfterDeadUI));
    }
    void RestartGameAfterDeadUI()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
        m_DeadUI.gameObject.SetActive(false);
    }
    IEnumerator DeadUICoroutine(OnDeadUIFinishedFn OnDeadUIFinished)
    {
        m_DeadUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_DeadUI.clip.length);
        OnDeadUIFinished();
    }*/

    public void RestartGame()
    {
        StartCoroutine(DeadUICoroutine(() =>
        {
            foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            {
                l_RestartGameElement.RestartGame();
            }
            m_DeadUI.gameObject.SetActive(false);
        }));
    }

    void RestartGameAfterDeadUI()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
        m_DeadUI.gameObject.SetActive(false);
    }

    IEnumerator DeadUICoroutine(OnDeadUIFinishedFn OnDeadUIFinished)
    {
        m_DeadUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_DeadUI.clip.length);
        OnDeadUIFinished();
    }

    public void StartTransitionToScene(string sceneName)
    {
        StartCoroutine(ScreenShakeAndFadeOut(sceneName));
    }
    private IEnumerator ScreenShakeAndFadeOut(string sceneName)
    {
        yield return StartCoroutine(CameraShake(m_ShakeDuration, m_ShakeAmplitud));
        yield return StartCoroutine(FadeOut());

        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator CameraShake(float duration, float amplitud)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            //float offsetY = Mathf.Sin(elapsed * Mathf.PI * 2) * amplitud;
            float offsetX = Random.Range(-1f, 1f) * amplitud;
            float offsetY = Random.Range(-1f, 1f) * amplitud;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }
    private IEnumerator FadeOut()
    {
        float alpha = 0.0f;
        Color tempColor = m_FadeOutImage.color;

        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime / m_fadeSpeed;
            tempColor.a = alpha;
            m_FadeOutImage.color = tempColor;
            yield return null;
        }
        m_FadeOutImage.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartTransitionToScene("Lab");
        }
    }
}
