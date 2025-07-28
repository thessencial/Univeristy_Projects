using UnityEngine;

public class Ammo : Items
{
    public SOItems infoAmmo;

    protected override float Pick()
    {
        Debug.Log(infoAmmo.Amount);
        return infoAmmo.Amount;
    }
}