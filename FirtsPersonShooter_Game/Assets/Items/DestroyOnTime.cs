using System.Collections;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float m_LifeTime;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(m_LifeTime);
        GameObject.Destroy(gameObject);
    }
}


