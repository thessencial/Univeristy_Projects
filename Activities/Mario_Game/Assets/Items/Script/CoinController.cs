using System.Collections;
using TMPro;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public TMP_Text m_CoinText;
    private static int m_CoinAmount = 0;
    AudioSource m_AudioSource;
    public AudioClip m_ClipAppears;
    public AudioClip m_ClipColected;

    private void Start()
    {
        m_CoinText = GameObject.Find("CoinsText").GetComponent<TMP_Text>();
        m_AudioSource = GetComponent<AudioSource>();
        UpdateCoinText();
        m_AudioSource.PlayOneShot(m_ClipAppears);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_AudioSource.PlayOneShot(m_ClipColected);
            m_CoinAmount++;
            UpdateCoinText();
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void UpdateCoinText()
    {
        m_CoinText.text = "";
        string amountString = m_CoinAmount.ToString();

        foreach (char digit in amountString)
        {
            int spriteIndex = digit - '0';
            m_CoinText.text += $"<sprite index={spriteIndex}>";
        }
    }


}

