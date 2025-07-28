using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public AudioSource m_AudioSource;
    public AudioClip m_ClipButton;
    public AudioClip m_ClipPlay;
    public AudioClip m_ClipExit;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
    }

    public void PlaySound()
    {
        m_AudioSource.PlayOneShot(m_ClipButton);
    }

    public void Play()
    {
        StartCoroutine(SoundPlay());
    }

    IEnumerator SoundPlay()
    {
        m_AudioSource.PlayOneShot(m_ClipPlay);
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadSceneAsync("Game");
    }

    public void Exit()
    {
        StartCoroutine(SoundExit());
    }

    IEnumerator SoundExit()
    {
        m_AudioSource.PlayOneShot(m_ClipExit);
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
        Debug.Log("Salir");
    }

    public void ReturnMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public void Reanude()
    {
        GameManager.GetGameManager().Reanude();
    }
}
