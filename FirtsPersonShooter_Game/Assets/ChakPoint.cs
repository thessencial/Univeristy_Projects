using System;
using UnityEngine;

public class ChakPoint : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.GetGameManager().raiseEvents().EnemyDead += Save;
    }

    private void OnDisable()
    {
        GameManager.GetGameManager().raiseEvents().EnemyDead -= Save;
    }

    private void Save()
    {
        throw new NotImplementedException();
    }

}
