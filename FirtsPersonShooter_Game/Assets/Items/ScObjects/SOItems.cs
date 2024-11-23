using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class SOItems : ScriptableObject
{
    public float Amount;
    public AnimatorOverrideController Animation;

    public float ReturnAmount()
    {
        return Amount;
    }

}
