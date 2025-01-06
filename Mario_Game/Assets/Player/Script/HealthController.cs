using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour, IRestartGameElement
{

    public float m_Health = 3;
    public float m_MiniHealth = 8;
    public TMP_Text HealthText;
    public Image m_HealthImage;
    public Image m_BackgroundImage;
    public Image m_GlossImage;
    public Image m_DeathImage;
    public Button m_ButtonRestart;
    public Button m_ButtonMenu;
    public float m_FillSpeed = 0.05f;
    private float m_Time = 3.0f;
    private float m_ColorTimer = 1.0f;
    public Animation m_UIAnimator;
    public AnimationClip m_ShowUI;
    public AnimationClip m_HideUI;
    private bool m_HudAnimationPlayed;
    public bool m_IsHudVisible = false;
    public float m_SecondsToDeathAnimationEnds = 1.5f;
    public float m_SecondsDeathUIAnimation = 1.5f;
    public Image m_DeathUI;
    public AnimationClip m_DeathUiAnimation;
    public AudioSource m_AudioSource;
    public AudioClip m_BowserLaugh;
    public AudioClip m_HealthUp;

    void Start()
    {
        m_HudAnimationPlayed = false;
        m_Health = 3;
        m_MiniHealth = 8;
        GameManager.GetGameManager().SetHealthContoller(this);
        GameManager.GetGameManager().AddRestartGameElement(this);
        UpdateHealthText();
        UpdateColor();
        m_DeathUI.gameObject.SetActive(false);
    }

    void Update()
    {
        m_Time -= Time.deltaTime;
        m_ColorTimer -= Time.deltaTime;
        if (m_Time <= 0.0f && !m_HudAnimationPlayed)
        {
            m_UIAnimator.Play(m_HideUI.name);
            m_HudAnimationPlayed = true;
            m_IsHudVisible = false;
        }
        if (m_ColorTimer <= 0.0f)
        {
            UpdateColor();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cure();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_Health = 1;
        }
#endif

    }

    public void TakeDamage()
    {
        m_Time = 3.0f;
        m_HudAnimationPlayed = false;

        if (!m_IsHudVisible)
        {
            m_UIAnimator.Play(m_ShowUI.name);
            m_IsHudVisible = true;
        }

        if (m_MiniHealth > 0)
        {
            m_MiniHealth--;
            m_HealthImage.fillAmount -= 1.0f / 8.0f;
            //float targetFillAmount = HealthImage.fillAmount - 1f / 8f;
            float targetFillAmount = m_HealthImage.fillAmount;
            StartCoroutine(AnimationHealthBar(targetFillAmount));
            UpdateHealthText();
            UpdateColor();
        }

        if (m_HealthImage.fillAmount <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        m_Health--;
        StartCoroutine(DeathAnimation());
    }

    void Cure()
    {
        m_AudioSource.PlayOneShot(m_HealthUp);
        m_Time = 3.0f;
        m_ColorTimer = 1.0f;
        m_HudAnimationPlayed = false;
        if (!m_IsHudVisible)
        {
            m_UIAnimator.Play(m_ShowUI.name);
            m_IsHudVisible = true;

        }

        m_MiniHealth++;
        m_HealthImage.fillAmount += 1.0f / 8.0f;
        ColorUtility.TryParseHtmlString("#2CFF00", out Color Green);
        m_HealthImage.color = Green;
        m_GlossImage.color = Green;
        ColorUtility.TryParseHtmlString("#217800", out Color DarkGreen);
        m_BackgroundImage.color = DarkGreen;
    }

    void UpdateHealthText()
    {
        HealthText.text = $"<sprite index={m_Health}>";
    }

    void UpdateColor()
    {
        if (m_MiniHealth == 8)
        {
            ColorUtility.TryParseHtmlString("#2196F3", out Color Blue);
            m_HealthImage.color = Blue;
            m_GlossImage.color = Blue;
            ColorUtility.TryParseHtmlString("#0D47A1", out Color DarkBlue);
            m_BackgroundImage.color = DarkBlue;
        }
        else if (m_MiniHealth <= 7 && m_MiniHealth >= 4)
        {
            ColorUtility.TryParseHtmlString("#FAFF00", out Color Yellow);
            m_HealthImage.color = Yellow;
            m_GlossImage.color = Yellow;
            ColorUtility.TryParseHtmlString("#787A13", out Color DarkYellow);
            m_BackgroundImage.color = DarkYellow;
        }
        else if (m_MiniHealth <= 3)
        {
            ColorUtility.TryParseHtmlString("#FF0000", out Color Red);
            m_HealthImage.color = Red;
            m_GlossImage.color = Red;
            ColorUtility.TryParseHtmlString("#660000", out Color DarkRed);
            m_BackgroundImage.color = DarkRed;
        }
    }

    IEnumerator AnimationHealthBar(float targetFillAmount)
    {
        float l_InitialFillAmount = m_HealthImage.fillAmount;
        while (Mathf.Abs(m_HealthImage.fillAmount - targetFillAmount) > 0.01f)
        {
            m_HealthImage.fillAmount = Mathf.Lerp(m_HealthImage.fillAmount, targetFillAmount, m_FillSpeed);
            yield return null;
        }
        m_HealthImage.fillAmount = targetFillAmount;
    }

    IEnumerator DeathAnimation()
    {
        GameManager.GetGameManager().GetPlayer().m_Death = true;
        yield return new WaitForSeconds(m_SecondsToDeathAnimationEnds);
        m_DeathUI.gameObject.SetActive(true);
        m_UIAnimator.Play(m_DeathUiAnimation.name);
        m_AudioSource.PlayOneShot(m_BowserLaugh);
        yield return new WaitForSeconds(m_SecondsDeathUIAnimation);
        m_DeathUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_DeathImage.gameObject.SetActive(true);
        AudioListener.pause = true;
        if (m_Health <= 0)
        {
            m_ButtonRestart.gameObject.SetActive(false);
            m_ButtonMenu.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        m_MiniHealth = 8;
        StartCoroutine(AnimationHealthBar(1));
        UpdateHealthText();
        UpdateColor();
    }
}
