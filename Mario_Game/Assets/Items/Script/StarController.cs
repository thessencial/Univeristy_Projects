using System.Collections;
using TMPro;
using UnityEngine;

public class StarController : MonoBehaviour
{
    public TMP_Text m_StarText;
    private static int m_StarAmount = 0;
    AudioSource m_AudioSource;
    public AudioClip m_ClipAppears;
    public AudioClip m_ClipColected;

    private void Start()
    {
        m_StarText = GameObject.Find("StarText").GetComponent<TMP_Text>();
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.PlayOneShot(m_ClipAppears);
        UpdateCoinText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_AudioSource.PlayOneShot(m_ClipColected);
            m_StarAmount++;
            UpdateCoinText();
            StartCoroutine(Destroy());
            GameManager.GetGameManager().m_TotalStars++;
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    private void UpdateCoinText()
    {
        m_StarText.text = "";
        string amountString = m_StarAmount.ToString();

        foreach (char digit in amountString)
        {
            int spriteIndex = digit - '0';
            m_StarText.text += $"<sprite index={spriteIndex}>";
        }
    }

}
