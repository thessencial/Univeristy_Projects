using UnityEngine;

public abstract class Items : MonoBehaviour
{
    protected virtual float Pick()
    {
        return 0;
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }
}
