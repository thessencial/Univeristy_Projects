using UnityEngine;

public class Shield : Items
{
    public SOItems infoShield;

    protected override float Pick()
    {
        Debug.Log(infoShield.Amount);
        return infoShield.Amount;
    }
}