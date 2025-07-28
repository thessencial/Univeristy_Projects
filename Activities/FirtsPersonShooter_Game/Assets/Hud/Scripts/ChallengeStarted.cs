using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class ChallengeStarted : MonoBehaviour
{
    public MovingTarget[] m_Targets;
    private bool m_IsPlayerInZone = false;
    public bool m_IsChallengeActive = false;
    // Start is called before the first frame update

    [Header("UI")]
    public TMP_Text m_StartMessage;
    public TMP_Text m_TimerText;
    public TMP_Text m_RestartText;

    [Header("Sound")]
    public AudioSource m_Clock;

    private float m_Timer;
    private bool m_IsTiming = false;
    public float m_ChallengeDuration = 30.0f;

    void Start()
    {
        m_Clock.Stop();
        if (m_StartMessage != null)
        {
            m_StartMessage.gameObject.SetActive(false);
        }

        if (m_TimerText != null)
        {
            m_TimerText.gameObject.SetActive(false);
            m_TimerText.text = "Tiempo: 0" + m_ChallengeDuration;
        }

        if (m_RestartText != null)
        {
            m_RestartText.gameObject.SetActive(false);
        }

    }
    private void Update()
    {
        if(m_IsPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (!m_IsTiming)
            {
                StartChallenge();
                //m_RestartText.gameObject.SetActive(false);
            }
            else
            {
                StopChallenge();
            }
        }
        if (m_IsTiming)
        {
            m_Timer -= Time.deltaTime;
            m_TimerText.text = "Tiempo: " + Mathf.FloorToInt(m_Timer).ToString();
            if (m_Timer <= 0)
            {
                m_Timer = 0;
                StopChallenge();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_IsPlayerInZone = true;

            if (m_StartMessage != null)
            {
                m_StartMessage.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_IsPlayerInZone = false;
            if (m_StartMessage != null)
            {
                m_StartMessage.gameObject.SetActive(false);
            }
            if (m_RestartText != null)
            {
                m_RestartText.gameObject.SetActive(false);
            }
            m_RestartText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void StartChallenge()
    {
        m_Clock.Play();
        ShootingScoreManager.Instance.ResetScore();

        m_Timer = m_ChallengeDuration;
        m_IsTiming = true;
        m_IsChallengeActive = true;
        m_TimerText.gameObject.SetActive(true);

        if (m_StartMessage != null)
        {
            m_StartMessage.gameObject.SetActive(false);
            m_RestartText.gameObject.SetActive(false);
        }

        foreach (MovingTarget target in m_Targets)
        {
            target.StartChallenge();
        }
        Debug.Log("Desafio Inicidado");

    }
    void StopChallenge()
    {
        m_Clock.Stop();
        m_IsTiming = false;
        m_IsChallengeActive = false;
        m_TimerText.gameObject.SetActive(false);
        //m_RestartText.gameObject.SetActive(true);

        int currentScore = ShootingScoreManager.Instance.GetCurrentScore();
        
        if (m_RestartText != null)
        {
            m_RestartText.gameObject.SetActive(true);
            m_RestartText.text = "Presiona E para reiniciar, has hecho " + currentScore + " puntos.";

        }

        foreach (MovingTarget target in m_Targets)
        {
            target.StopChallenge();
        }
    }

    void RestartChallenge()
    {
        if (m_RestartText != null)
        {
            m_RestartText.gameObject.SetActive(false);
        }
        StartChallenge();
    }
}
