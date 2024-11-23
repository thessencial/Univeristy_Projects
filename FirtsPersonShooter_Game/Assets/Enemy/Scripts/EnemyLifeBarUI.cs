using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeBarUI : MonoBehaviour
{
    public Image m_LifeImage;
    RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void SetLifeBarUI(float NormalizedX, float NormalizedY, float LifeNormalized, bool Visible)
    {
        gameObject.SetActive(Visible);
        if (Visible)
        {
            m_RectTransform.anchoredPosition = new Vector3(NormalizedX * 1920.0f, -(1.0f - NormalizedY) * 1080.0f);
            m_LifeImage.fillAmount = LifeNormalized;
        }
    }
}
