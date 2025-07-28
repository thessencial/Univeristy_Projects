using UnityEngine;
using UnityEngine.AI;

public class EnemyControler : MonoBehaviour, IRestartGameElement
{
    private enum State
    {
        Idel,
        Patrol,
        Alert,
        Chase,
        Atack,
        Hit,
        Die
    }

    private State state;

    private enum StateRandom
    {
        Yes,
        No
    }

    [SerializeField] private StateRandom random;

    private NavMeshAgent agent;
    private Ray ray;

    private Vector3[] PointRout;
    private int CurrentPoint = 0;
    [SerializeField] private int MyRout = 0;

    [SerializeField] private float DistanceChase;
    [SerializeField] private float DistanceAttack;
    [SerializeField] private float DistanceAlert;
    [SerializeField] private float AngleVision = 60.0f;
    [SerializeField] private LayerMask layerMask;
    private Vector3 DirectionSee;
    private float Distance;
    [SerializeField] private float V = 30.0f;

    private float Counter = 2.0f;
    [SerializeField] private float SetFireRate;
    private float FireRate;

    public Transform m_LifeBarUIPosition;
    public EnemyLifeBarUI m_EnemyLifeBarUI;
    float m_Life;
    public float m_MaxLife = 50.0f;

    private Vector3 chek;
    [SerializeField] private Transform Dron;
    [SerializeField] private ParticleSystem ParticleSystem;

    void Awake()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Start()
    {
        PointRout = RoutesEnemies.instance.GetRoute(MyRout);
        SetPosition();
        SetIdleState();
        m_Life = m_MaxLife;
        FireRate = SetFireRate;
        chek = transform.position;
        //m_StartPosition = transform.position;
        //m_StartRotation = transform.rotation;
    }

    private void SetPosition()
    {
        if (random == StateRandom.No)
        {
            agent.enabled = false;
            transform.position = PointRout[CurrentPoint];
            agent.enabled = true;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Idel:

                SetPatrolState();

                break;
            case State.Patrol:

                if (random == StateRandom.No)
                {
                    UpdatePatrol();
                }
                else
                {
                    UpdatePatrolRandom();
                }

                break;
            case State.Alert:

                UpadteAlert();
                Counter -= Time.deltaTime;

                break;
            case State.Chase:

                UpdateChase();

                break;
            case State.Atack:

                UpadteAtack();
                FireRate -= Time.deltaTime;

                break;
            case State.Hit:
                break;
            case State.Die:
                break;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        Vector3 l_ViewportPosition = GameManager.GetGameManager().GetPlayer().m_Camera.WorldToViewportPoint(m_LifeBarUIPosition.position);
        m_EnemyLifeBarUI.SetLifeBarUI(l_ViewportPosition.x, l_ViewportPosition.y, GetLife() / m_MaxLife, l_ViewportPosition.z >= 0);
    }

    float GetLife()
    {
        return m_Life;
    }

    public void AddLife(float LifePoints)
    {
        m_Life = Mathf.Clamp(m_Life + LifePoints, 0.0f, m_MaxLife);
        if (m_Life == 0.0f)
        {
            SetDieState();
        }
    }

    private void UpdatePatrolRandom()
    {

        if ((Vector3.Distance(transform.position, chek)) < 1)
        {
            Vector3 point;

            if (RandomPoint(transform.position, V, out point))
            {
                agent.SetDestination(point);
                chek = point;
            }

        }


        if (Hear())
        {
            SetAlertState();
        }
    }

    private void UpdatePatrol()
    {
        if (Vector3.Distance(transform.position, PointRout[CurrentPoint]) < 1)
        {
            ++CurrentPoint;
        }

        if (CurrentPoint >= PointRout.Length)
        {
            CurrentPoint = 0;
        }
        else
        {
            agent.SetDestination(PointRout[CurrentPoint]);
        }

        if (Hear())
        {
            SetAlertState();
        }
    }

    private void UpadteAlert()
    {
        agent.isStopped = true;
        if (See())
        {
            SetChaseState();
        }

        if (Counter <= 0)
        {
            if (state == State.Alert)
            {
                SetPatrolState();
                agent.isStopped = false;
            }
            Counter = 2.0f;
        }

    }

    private void UpdateChase()
    {
        agent.SetDestination(CodeCharacterController.instance.transform.position);

        if (!RangeChase())
        {
            SetAlertState();
        }

        if (RangeAttack())
        {
            SetAtackState();
        }

    }

    private void UpadteAtack()
    {
        agent.isStopped = true;
        transform.LookAt(CodeCharacterController.instance.transform.position + Vector3.down * 1.4f);

        if (!RangeAttack())
        {
            SetChaseState();
        }

        if (FireRate <= 0)
        {
            DirectionSee = (transform.position + Vector3.up * 1.2f) - (CodeCharacterController.instance.transform.position + Vector3.up * 1.2f);
            Distance = DirectionSee.magnitude;
            ray = new Ray(CodeCharacterController.instance.transform.position + Vector3.up * 1.2f, DirectionSee);
            if (Physics.Raycast(ray, Distance, CodeCharacterController.instance.gameObject.layer))
            {
                CodeCharacterController.instance.TakeDamage(20.0f);
                ParticleSystem.Play();
            }

            FireRate = SetFireRate;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private bool RandomPoint(Vector3 position, float v, out Vector3 point)
    {
        Vector3 RandomPoint = position + UnityEngine.Random.insideUnitSphere * v;
        NavMeshHit Hit;

        if (NavMesh.SamplePosition(RandomPoint, out Hit, 1.0f, NavMesh.AllAreas))
        {
            point = Hit.position;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private bool RangeAttack()
    {
        DirectionSee = (transform.position + Vector3.up * 1.2f) - (CodeCharacterController.instance.transform.position + Vector3.up * 1.2f);
        Distance = DirectionSee.magnitude;
        if (Distance < DistanceAttack)
        {
            return true;
        }
        return false;
    }

    bool See()
    {
        DirectionSee = (transform.position + Vector3.up * 1.2f) - (CodeCharacterController.instance.transform.position + Vector3.up * 1.2f);
        Distance = DirectionSee.magnitude;

        if (RangeChase())
        {
            DirectionSee /= Distance;
            float Angle = Vector3.Dot(DirectionSee, transform.forward);

            if (Angle <= Mathf.Cos(AngleVision * Mathf.Deg2Rad / 2.0f))
            {
                ray = new Ray(CodeCharacterController.instance.transform.position + Vector3.up * 1.2f, DirectionSee);

                if (!Physics.Raycast(ray, Distance, layerMask.value))
                {
                    return true;
                }
            }
        }
        return false;

    }

    private bool RangeChase()
    {
        DirectionSee = (transform.position + Vector3.up * 1.2f) - (CodeCharacterController.instance.transform.position + Vector3.up * 1.2f);
        Distance = DirectionSee.magnitude;
        return Distance < DistanceChase;
    }

    bool Hear()
    {
        return Vector3.Distance(transform.position, CodeCharacterController.instance.transform.position) < DistanceAlert;
    }

    public void RestartGame()
    {
        this.gameObject.SetActive(true);
        m_EnemyLifeBarUI.gameObject.SetActive(true);
        SetPosition();
        //agent.isStopped = true;
        //transform.position = m_StartPosition;
        //transform.rotation = m_StartRotation;
        m_Life = m_MaxLife;
        SetIdleState();
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetIdleState()
    {
        state = State.Idel;
    }

    public void SetPatrolState()
    {
        state = State.Patrol;
    }

    public void SetAlertState()
    {
        state = State.Alert;
    }

    public void SetChaseState()
    {
        agent.isStopped = false;
        state = State.Chase;
    }

    public void SetAtackState()
    {
        state = State.Atack;
    }

    public void SetDieState()
    {
        this.state = State.Die;
        m_EnemyLifeBarUI.gameObject.SetActive(false);
        GameObject item = PoolItems.instance.GetPooledObject();
        item.SetActive(true);
        item.transform.position = Dron.position;
        GameManager.GetGameManager().raiseEvents().RaiseEvent();
        gameObject.SetActive(false);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawRay(CodeCharacterController.instance.transform.position + Vector3.up * 1.2f, DirectionSee);
    //}
}
