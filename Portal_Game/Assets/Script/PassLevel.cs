using UnityEngine;

public class PassLevel : MonoBehaviour
{
    [SerializeField] private string m_NameLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeLevel();
        }
    }

    public void ChangeLevel()
    {
        GameManager.GetGameManager().StartTransitionToScene(m_NameLevel);
    }
}
