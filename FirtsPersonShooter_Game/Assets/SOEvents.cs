using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Events", menuName = "ScriptableObject/Events")]
public class SOEvents : ScriptableObject
{
    public UnityAction EnemyDead;

    public void RaiseEvent()
    {
        EnemyDead?.Invoke();
    }
}
