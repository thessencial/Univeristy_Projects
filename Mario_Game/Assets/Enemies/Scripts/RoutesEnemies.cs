using System;
using UnityEngine;

[Serializable]
public class Routes
{
    public Vector3[] routes;
}

public class RoutesEnemies : MonoBehaviour
{
    [SerializeField] private Routes[] routesEnemies;

    public static RoutesEnemies instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3[] GetRoute(int i)
    {
        return routesEnemies[i].routes;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < routesEnemies.Length; i++)
        {
            for (int j = 0; j < routesEnemies[i].routes.Length - 1; j++)
            {
                Gizmos.DrawLine(routesEnemies[i].routes[j], routesEnemies[i].routes[j + 1]);
            }
        }
    }
}
