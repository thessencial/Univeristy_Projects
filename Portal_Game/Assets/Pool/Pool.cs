using System.Collections.Generic;
using UnityEngine;


public class Pool : MonoBehaviour
{
    public static Pool instance;

    private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private int total_pool = 10;

    [SerializeField] private GameObject bullet;
    private int CurrentElement = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    void Start()
    {
        for (int i = 0; i < total_pool; i++)
        {
            GameObject obj = Instantiate(bullet, this.transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        GameObject returnGO = pool[CurrentElement];
        ++CurrentElement;
        if (CurrentElement >= pool.Count)
        {
            CurrentElement = 0;
        }

        return returnGO;
    }

}
