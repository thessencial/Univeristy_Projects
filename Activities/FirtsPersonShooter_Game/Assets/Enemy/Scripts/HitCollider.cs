using UnityEngine;

public class HitCollider: MonoBehaviour
{
    public enum THitColliderType
    {
        HELIX = 0,
        HEAD
    }

    public THitColliderType m_HitColliderType;
    public EnemyControler m_Enemy;
    
    public void Hit()
    {
        float l_LifePoints = 0.0f;
        switch (m_HitColliderType)
        {
            case THitColliderType.HEAD:
                l_LifePoints = 20.0f;
                    break;
            case THitColliderType.HELIX:
                l_LifePoints = 10.0f;
                break;
        }
        m_Enemy.AddLife(-l_LifePoints);
    }
}
