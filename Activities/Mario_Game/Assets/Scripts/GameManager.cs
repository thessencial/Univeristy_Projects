using System.Collections.Generic;
using UnityEngine;

public interface IRestartGameElement
{
    void RestartGame();
}
public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    MarioController m_Player;
    HealthController m_Health;
    ShellControler m_Shell;
    public GameObject m_Pause;
    public GameObject m_End;
    public bool m_Paused = false;
    public int m_TotalStars;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();

    private void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!m_Pause.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_Pause.SetActive(true);
                AudioListener.pause = true;
                m_Paused = true;
                Time.timeScale = 0.0f;
            }
            else
            {
                Reanude();
            }
        }

        if (m_TotalStars >= 5)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_End.SetActive(true);
            m_Paused = true;
        }
    }

    public void Reanude()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_Pause.SetActive(false);
        AudioListener.pause = false;
        m_Paused = false;
        Time.timeScale = 1.0f;
    }

    static public GameManager GetGameManager()
    {
        return m_GameManager;
    }

    public void SetPlayer(MarioController player)
    {
        m_Player = player;
    }

    public MarioController GetPlayer()
    {
        return m_Player;
    }

    public void SetShell(ShellControler shell)
    {
        m_Shell = shell;
    }

    public ShellControler GetShell()
    {
        return m_Shell;
    }

    public void SetHealthContoller(HealthController health)
    {
        m_Health = health;
    }

    public HealthController GetHealth()
    {
        return m_Health;
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponent<AudioSource>().Stop(); 
        GetComponent<AudioSource>().Play(); 
        AudioListener.pause = false;
    }
}
