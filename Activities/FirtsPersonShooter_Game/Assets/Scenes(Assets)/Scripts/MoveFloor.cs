using UnityEngine;

public class MoveFloor : MonoBehaviour, IRestartGameElement
{

    private Vector3[] Route;
    private int CurrentPoint = 0;

    void Start()
    {
        Route = RoutesEnemies.instance.GetRoute(0);
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Route[CurrentPoint]) < 0.5)
        {
            ++CurrentPoint;
        }

        if (CurrentPoint >= Route.Length)
        {
            CurrentPoint = 0;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Route[CurrentPoint], 2 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }

    public void RestartGame()
    {
        CodeCharacterController.instance.transform.SetParent(null);
    }
}
