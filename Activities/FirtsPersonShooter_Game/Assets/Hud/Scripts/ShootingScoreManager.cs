using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShootingScoreManager : MonoBehaviour
{
    //public MovingTarget[] m_Targets;
    public static ShootingScoreManager Instance;
    public int m_Score = 0;
    public TMP_Text m_ScoreText;
    public int m_GlobalScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void AddScore(int points)
    {
        m_GlobalScore += points;
    }

    public void ResetScore()
    {
        m_GlobalScore = 0;
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (m_ScoreText != null)
        {
            m_ScoreText.text = "Score: " + m_GlobalScore;
        }
    }

    public int GetCurrentScore()
    {
        return m_GlobalScore;
    }
}