using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IRestartGameElement
{
    private enum State
    {
        Idel,
        Patrol,
        Alert,
        Chase,
        Death
    }

    private enum Type
    {
        Goomba,
        Koopa
    }

    [SerializeField] private Type m_TypeEnemy;

    [Header("DataAI")]
    private State m_State;
    private NavMeshAgent m_Agent;
    private Vector3[] m_PointRout;
    private int m_CurrentPoint = 0;
    [SerializeField] private int m_MyRout = 0;
    [SerializeField] private float m_DistanceChase;
    [SerializeField] private float m_DistanceAlert;
    [SerializeField] private float m_AngleVision = 60.0f;
    [SerializeField] private LayerMask m_LayerMask;
    private Vector3 m_DirectionSee;
    private Ray m_Ray;
    private float m_Distance;
    private float m_Counter = 2.0f;

    CharacterController m_CharactherController;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    Animator m_Animator;

    [Header("ItemsDrop")]
    public GameObject m_CoinPrefab;
    public GameObject m_ShellPrefab;

    [Header("Audio")]
    AudioSource m_AudioSource;
    public AudioClip m_ClipStep;
    public AudioClip m_ClipDeath;
    public AudioClip m_ClipAlert;


    [Header("Particles")]
    public ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_CharactherController = GetComponent<CharacterController>();
        m_Agent = GetComponentInChildren<NavMeshAgent>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_PointRout = RoutesEnemies.instance.GetRoute(m_MyRout);
        SetIdleState();
    }

    void Update()
    {
        if (!GameManager.GetGameManager().GetPlayer().m_Death)
        {
            switch (m_State)
            {
                case State.Idel:

                    SetPatrolState();

                    break;

                case State.Patrol:

                    UpdatePatrol();

                    break;

                case State.Alert:

                    UpadteAlert();
                    m_Counter -= Time.deltaTime;

                    break;

                case State.Chase:

                    UpdateChase();

                    break;

                case State.Death:
                    break;
            }
        }
    }

    public void SetIdleState()
    {
        m_State = State.Idel;
    }

    public void SetPatrolState()
    {
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Run", false);
        m_CurrentPoint = GetTheClosestPatrolPosition();
        m_State = State.Patrol;
    }

    public void SetAlertState()
    {
        m_Animator.SetBool("Alert", true);
        m_Animator.SetBool("Run", false);
        m_State = State.Alert;
    }

    public void SetChaseState()
    {
        m_Animator.SetBool("Run", true);
        m_Agent.isStopped = false;
        m_State = State.Chase;
    }

    void SetDeathState()
    {
        m_State = State.Death;
    }

    private void UpdatePatrol()
    {
        if (!m_Agent.hasPath || m_Agent.remainingDistance <= m_Agent.stoppingDistance)
        {
            ++m_CurrentPoint;
        }

        if (m_CurrentPoint >= m_PointRout.Length)
        {
            m_CurrentPoint = 0;
        }
        else
        {
            m_Agent.SetDestination(m_PointRout[m_CurrentPoint]);
        }

        if (Hear())
        {
            SetAlertState();
        }
    }

    private void UpadteAlert()
    {
        m_Agent.isStopped = true;
        if (See())
        {
            SetChaseState();
        }

        if (m_Counter <= 0)
        {
            if (m_State == State.Alert)
            {
                SetPatrolState();
                m_Agent.isStopped = false;
            }
            m_Counter = 2.0f;
        }

    }

    private void UpdateChase()
    {
        m_Agent.SetDestination(GameManager.GetGameManager().GetPlayer().transform.position);

        if (!RangeChase())
        {
            SetAlertState();
        }
    }

    private bool RangeChase()
    {
        m_DirectionSee = (transform.position + Vector3.up * 1.2f) - (GameManager.GetGameManager().GetPlayer().transform.position + Vector3.up * 1.2f);
        m_Distance = m_DirectionSee.magnitude;
        return m_Distance < m_DistanceChase;
    }

    bool Hear()
    {
        return Vector3.Distance(transform.position, GameManager.GetGameManager().GetPlayer().transform.position) < m_DistanceAlert;
    }

    bool See()
    {
        m_DirectionSee = (transform.position + Vector3.up * 1.2f) - (GameManager.GetGameManager().GetPlayer().transform.position + Vector3.up * 1.2f);
        m_Distance = m_DirectionSee.magnitude;

        if (RangeChase())
        {
            m_DirectionSee /= m_Distance;
            float Angle = Vector3.Dot(m_DirectionSee, transform.forward);

            if (Angle <= Mathf.Cos(m_AngleVision * Mathf.Deg2Rad / 2.0f))
            {
                m_Ray = new Ray(GameManager.GetGameManager().GetPlayer().transform.position + Vector3.up * 1.2f, m_DirectionSee);

                if (!Physics.Raycast(m_Ray, m_Distance, m_LayerMask.value))
                {
                    return true;
                }
            }
        }
        return false;

    }

    int GetTheClosestPatrolPosition()
    {
        int l_ClosestPatrolPositionId = 0;
        float l_MinDistance = Mathf.Infinity;
        Vector3 l_EnemyPosition = transform.position;

        for (int i = 0; i < m_PointRout.Length; ++i)
        {
            Vector3 l_PatrolPosition = m_PointRout[i];
            float l_DistanceToPatrolPosition = Vector3.Distance(l_EnemyPosition, l_PatrolPosition);

            if (l_DistanceToPatrolPosition < l_MinDistance)
            {
                l_MinDistance = l_DistanceToPatrolPosition;
                l_ClosestPatrolPositionId = i;
            }
        }

        return l_ClosestPatrolPositionId;
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_CharactherController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharactherController.enabled = true;
    }

    public void Kill()
    {
        SetDeathState();
        m_Animator.SetBool("Death", true);
        m_ParticleSystem.Play();
        StartCoroutine(AnimationDeath());
    }

    IEnumerator AnimationDeath()
    {
        yield return new WaitForSeconds(1f);
        DropCoin();

        if (m_TypeEnemy == Type.Koopa)
        {
            DropShell();
        }

        gameObject.SetActive(false);
    }

    private void DropCoin()
    {
        if (m_CoinPrefab != null)
        {
            Instantiate(m_CoinPrefab, transform.position + new Vector3(0f, 1.0f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No se ha asignado una moneda");
        }
    }

    private void DropShell()
    {
        if (m_ShellPrefab != null)
        {
            Instantiate(m_ShellPrefab, transform.position + new Vector3(0f, 1.0f, 0f), Quaternion.identity);
        }
    }

    void SoundStep()
    {
        m_AudioSource.PlayOneShot(m_ClipStep);
    }

    void AlertSound()
    {
        m_AudioSource.PlayOneShot(m_ClipAlert);
    }

    void DeathSound()
    {
        m_AudioSource.PlayOneShot(m_ClipDeath);
    }
}
