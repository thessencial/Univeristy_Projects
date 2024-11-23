using UnityEngine;

public class Health : Items
{
    public SOItems infoHealth;

    protected override float Pick()
    {
        Debug.Log(infoHealth.Amount);
        return infoHealth.Amount;
    }
}