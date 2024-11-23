using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    //[SerializeField] private SOEvents raiseEvent;

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
            GameObject.Destroy(m_DeadUI.gameObject);
            GameObject.Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(m_DeadUI.gameObject);
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

    //public SOEvents raiseEvents()
    //{
    //    return raiseEvent;
    //}

    public void InitLevel(CodeCharacterController player)
    {
        m_Player.m_BluePortal.gameObject.SetActive(false);
        m_Player.m_OrangePortal.gameObject.SetActive(false);
        m_Player.m_DummyPortal.gameObject.SetActive(false);
        m_Player.m_CrosshairImage.sprite = m_Player.m_EmptyCrosshair;
        m_Player.transform.position = player.transform.position;
        m_Player.transform.rotation = player.transform.rotation;
    }

    public void OnMainMenu()
    {
        GameObject.Destroy(gameObject);
        GameObject.Destroy(m_Player.gameObject);
        GameObject.Destroy(m_Player.m_BluePortal.gameObject);
        GameObject.Destroy(m_Player.m_OrangePortal.gameObject);
        GameObject.Destroy(m_Player.m_DummyPortal.gameObject);
        GameObject.Destroy(m_Player.m_Canvas.gameObject);
        GameObject.Destroy(m_DeadUI.gameObject);
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
        StartCoroutine(RestartMovment());
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
        GetPlayer().OneTime = true;
    }

    IEnumerator RestartMovment()
    {
        yield return new WaitForSeconds(0.5f);
        GetPlayer().m_CanMove = true;
    }

    public void StartTransitionToScene(string sceneName)
    {
        StartCoroutine(ScreenShakeAndFadeOut(sceneName));
    }

    private IEnumerator ScreenShakeAndFadeOut(string sceneName)
    {
        GetPlayer().m_CanMove = false;
        yield return StartCoroutine(CameraShake(m_ShakeDuration, m_ShakeAmplitud));
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(FadeIn());
        GetPlayer().m_CanMove = true;
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
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1.0f;
        Color tempColor = m_FadeOutImage.color;

        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime / m_fadeSpeed;
            tempColor.a = alpha;
            m_FadeOutImage.color = tempColor;
            yield return null;
        }
    }
}
