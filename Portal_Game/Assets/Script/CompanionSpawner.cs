using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_CompanionPrefab;
    [SerializeField] private Transform m_SpanwerPoint;

    public void Spawn()
    {
        GameObject gameObject = Instantiate(m_CompanionPrefab);
        gameObject.transform.position = m_SpanwerPoint.position;
    }
}
