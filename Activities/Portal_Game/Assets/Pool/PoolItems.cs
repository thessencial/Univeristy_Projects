using System.Collections.Generic;
using UnityEngine;


public class PoolItems : MonoBehaviour
{
    public static PoolItems instance;

    private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private int total_pool = 10;

    [SerializeField] private GameObject[] bullet;
    private int CurrentElement = 0;
    private bool i = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    void Start()
    {
        for (int i = 0; i < total_pool; i++)
        {
            GameObject obj = Instantiate(bullet[Random.Range(0, bullet.Length)], this.transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        i = true;
        while (i)
        {
            GameObject returnGO = pool[Random.Range(0, bullet.Length - 1)];
            ++CurrentElement;
            if (CurrentElement >= pool.Count)
            {
                CurrentElement = 0;
            }

            if (!returnGO.activeInHierarchy)
            {
                i = false;
                return returnGO;
            }
        }
        return null;
    }

}
