using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovingTarget : MonoBehaviour, IInteractable
{
    private enum TargetType
    {
        Static,
        Moving
    }

    [SerializeField] private TargetType m_TargetType;

    // Start is called before the first frame update
    [Header("Movement")]
    public float m_Speed = 2.0f;
    public float m_LeftLimit = -5.0f;
    public float m_RightLimit = 5.0f;
    public int m_Direction = 1;
    public bool m_IsMoving = false;
    public bool m_StartedChallenge = false;

    [Header("Animation")]
    private Animation m_Animation;
    public AnimationClip m_TargetDown;
    public AnimationClip m_TargetUp;

    [Header("Interaction")]
    public float interactMoveSpeed = 3.0f;
    private bool m_IsInteracting = false;
    private Vector3 targetPosition;
    public float m_distance = 30.0f;

    private float m_RL;
    private float m_LL;
    private bool m_IsShot = false;
    private ChallengeStarted m_ChallengeStarted;
    private float m_SecondsDown = 3.0f;



    [Header("Score")]
    private int pointsPerHit = 30;


    private void Start()
    {
        m_ChallengeStarted = FindObjectOfType<ChallengeStarted>();
        m_RL = transform.position.x + m_RightLimit;
        m_LL = transform.position.x + m_LeftLimit;
        m_Animation = GetComponent<Animation>();
        ShootingScoreManager.Instance.UpdateScoreUI();

    }


    private void Update()
    {

        if (!m_StartedChallenge)
        {
            return;
        }
        if (m_IsShot)
        {
            return;
        }
        if (m_TargetType == TargetType.Moving && m_StartedChallenge == true)
        {

            transform.position += Vector3.right * m_Direction * m_Speed * Time.deltaTime;

            if (transform.position.x >= m_RL)
            {
                m_Direction = -1;
            }
            else if (transform.position.x <= m_LL)
            {
                m_Direction = 1;
            }
        }
        else if (m_TargetType == TargetType.Static && m_IsInteracting)
        {

            m_IsMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, interactMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                m_IsInteracting = false;
                m_IsMoving = false;
            }
            /*if (m_IsInteracting)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, interactMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    m_IsInteracting = false;
                    Interact();
                }
            }*/
            // Interact();

        }
        else
        {
            m_IsMoving = false;
        }
        if (m_ChallengeStarted == true)
        {
            pointsPerHit = 30;
        }
        else
        {
            pointsPerHit = 0;
        }


    }
    public void GetShoot()
    {

            if (!m_IsShot && m_ChallengeStarted.m_IsChallengeActive)
            {

                if (!m_IsShot && !(m_TargetType == TargetType.Static && m_IsMoving))
                {
                    Debug.Log("Diana tumbada");
                    m_IsShot = true;

                    if (ShootingScoreManager.Instance != null)
                    {
                        ShootingScoreManager.Instance.AddScore(pointsPerHit);
                        ShootingScoreManager.Instance.UpdateScoreUI();
                    }
                    StartCoroutine(HandleTargetDown());

                }
                else
                {
                    Debug.Log("Diana en moviemiento, ");
                }
            }
            else if (!m_ChallengeStarted.m_IsChallengeActive)
            {
                Debug.Log("No se puede disparar, el desafio no ha comenzado");
            }
    }

    public void Interact()
    {
        m_IsInteracting = true;
        targetPosition = new Vector3(transform.position.x + m_distance, transform.position.y, transform.position.z);
        //Debug.Log("Boton ok");
    }

    IEnumerator HandleTargetDown()
    {
        SetTargetDownAnimation();
        yield return new WaitForSeconds(m_SecondsDown);

        SetTargetUpAnimation();
        m_IsShot = false;
    }


    void SetTargetUpAnimation()
    {
        m_Animation.CrossFade(m_TargetUp.name);
    }
    void SetTargetDownAnimation()
    {
        m_Animation.CrossFade(m_TargetDown.name);
    }

    public void StartChallenge()
    {
        m_StartedChallenge = true;
    }
    public void StopChallenge()
    {
        m_StartedChallenge = false;
        m_IsMoving = false;
    }
}