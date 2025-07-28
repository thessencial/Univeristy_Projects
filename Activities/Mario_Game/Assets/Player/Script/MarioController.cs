using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioController : MonoBehaviour, IRestartGameElement
{

    public enum TPunchType
    {
        LEFT_HAND,
        RIGHT_HAND,
        KICK
    }

    public enum ShellState
    {
        Take,
        Throw
    }

    CharacterController m_CharactherController;
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    public Camera m_Camera;

    public float m_WalkSpeed = 2.0f;
    public float m_RunSpeed = 8.0f;
    public float m_LerpRotationPct = 0.8f;
    public float m_VerticalSpeed = 0.0f;
    int m_CurrentPunchId = 0;
    float m_LastPunchTime;
    float m_PunchComboAvaiableTime = 0.6f;
    public float m_ResetPunchComboStartTimer = 1.5f;
    float m_ResetPunchComboTimer;
    bool m_IsMoving;
    bool m_IsRuning;
    float m_BridgeForce = 6.0f;
    public bool m_Death = false;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    Checkpoint m_CurrentCheckpoint;
    Vector3 m_Movement = Vector3.zero;
    //float m_SpeedJumpMovement = 1.0f;
    float m_TimerIdelSpecial = 10.0f;

    [Header("Jump")]
    public float m_JumpVerticalSpeed = 6.0f;
    public float m_JumpDoubleVerticalSpeed = 8.0f;
    public float m_JumpTripleVerticalSpeed = 10.0f;
    public float m_JumpLongVerticalSpeed = 9.0f;
    public float m_KillJumpVerticalSpeed = 6.0f;
    public float m_WallJumpVerticalSpeed = 6.0f;
    public float m_WallKnockBackForce = 6.0f;
    float m_CurrentJumpVerticalSpeed;
    public float m_WaitSatarJumpTime = 0.12f;
    public float m_MaxAngleToKillGoomba = 15.0f;
    public float M_MinVerticalSpeedToKillGoomba = -1.0f;
    int m_CurrentJumpId = 0;
    public float m_ResetJumpStartTimer = 2.5f;
    float m_ResetJumpTimer;
    bool m_IsWallJump = false;
    bool m_IsJumping;

    [Header("Punch Colliders")]
    public GameObject m_LeftHandPunchHitCollider;
    public GameObject m_RightHandPunchHitCollider;
    public GameObject m_RightFoodPunchHitCollider;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_InteractKeyCode = KeyCode.E;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public int m_PunchKeyCode = 0;

    [Header("Elevator")]
    public float m_MaxAngleToAtached = 8.0f;
    Collider m_CurrentElevator = null;

    [Header("ShellInfo")]
    bool m_ShellDetected = false;
    public ShellState m_ShellState;
    GameObject m_ActualShell;
    Rigidbody m_ObjectRB;
    public float m_ShellForce = 1.0f;
    public Transform m_PointShell;

    [Header("Audio")]
    AudioSource m_AudioSource;
    public AudioClip m_ClipJump;
    public AudioClip m_ClipJumpDouble;
    public AudioClip m_ClipJumpTriple;
    public AudioClip m_ClipJumpLong;
    public AudioClip m_ClipWallJump;
    public AudioClip m_ClipHit;
    public AudioClip m_ClipDeath;
    public AudioClip m_ClipKick;
    public AudioClip m_ClipPunch1;
    public AudioClip m_ClipPunch2;
    public AudioClip m_ClipStart;
    public AudioClip m_ClipStep;
    public AudioClip m_ClipLand;

    [Header("Particles")]
    public ParticleSystem m_ParticleSystem;

    [Header("KnockBack")]
    public float m_ForceKnockBack = -10.0f;
    public float KnockBackTimer = 2.0f;
    float KnockBackCounter;

    private void Awake()
    {
        m_CharactherController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        m_LeftHandPunchHitCollider.SetActive(false);
        m_RightHandPunchHitCollider.SetActive(false);
        m_RightFoodPunchHitCollider.SetActive(false);
        GameManager.GetGameManager().SetPlayer(this);
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        //m_Animator.fireEvents = false;
        m_ResetPunchComboTimer = m_ResetPunchComboStartTimer;
        m_ResetJumpTimer = m_ResetJumpStartTimer;
    }

    void Update()
    {
        if (!GameManager.GetGameManager().m_Paused)
        {

            Vector3 l_Forward = m_Camera.transform.forward;
            Vector3 l_Right = m_Camera.transform.right;
            l_Forward.y = 0.0f;
            l_Right.y = 0.0f;
            l_Forward.Normalize();
            l_Right.Normalize();
            bool l_HasMovement = false;

            if (!m_Death)
            {

                if (KnockBackCounter <= 0 && !m_IsWallJump)
                {

                    if (Input.GetKey(m_RightKeyCode) || (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().x > 0.5f))
                    {
                        m_Movement = l_Right;
                        m_IsMoving = true;
                        l_HasMovement = true;
                    }
                    else if (Input.GetKey(m_LeftKeyCode) || (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().x < -0.5f))
                    {
                        m_Movement = -l_Right;
                        m_IsMoving = true;
                        l_HasMovement = true;
                    }

                    if (Input.GetKey(m_UpKeyCode) || (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().y > 0.5f))
                    {
                        m_Movement += l_Forward;
                        l_HasMovement = true;
                        m_IsMoving = true;
                    }
                    else if (Input.GetKey(m_DownKeyCode) || (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().y < -0.5f))
                    {
                        m_Movement -= l_Forward;
                        l_HasMovement = true;
                        m_IsMoving = true;
                    }
                    else
                    {
                        m_IsMoving = false;
                    }

                    m_Movement.Normalize();
                    float l_Speed = 0.0f;
                    if (l_HasMovement)
                    {
                        if (Input.GetKey(m_RunKeyCode) || (Gamepad.current != null && Gamepad.current.rightShoulder.isPressed))
                        {
                            l_Speed = m_RunSpeed;
                            m_IsRuning = true;
                            m_Animator.SetBool("Runing", true);
                            m_Animator.SetFloat("Speed", 1.0f);
                        }
                        else
                        {
                            l_Speed = m_WalkSpeed;
                            m_IsRuning = false;
                            m_Animator.SetBool("Runing", false);
                            m_Animator.SetFloat("Speed", 0.2f);
                        }
                        Quaternion l_DesiredRotation = Quaternion.LookRotation(m_Movement);
                        transform.rotation = Quaternion.Lerp(transform.rotation, l_DesiredRotation, m_LerpRotationPct * Time.deltaTime);

                    }
                    else
                    {
                        m_Animator.SetFloat("Speed", 0.0f);
                    }

                    if (m_ShellDetected)
                    {
                        if (Input.GetKeyDown(m_InteractKeyCode) || (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
                        {
                            if (m_ShellState == ShellState.Take)
                            {
                                m_ShellState = ShellState.Throw;
                                m_ActualShell.transform.SetParent(null);
                                GameManager.GetGameManager().GetShell().SetForce(m_ShellForce, transform.forward);
                            }
                            else if (m_ShellState == ShellState.Throw)
                            {
                                m_ShellState = ShellState.Take;
                                m_ObjectRB.isKinematic = true;
                                GameManager.GetGameManager().GetShell().m_BouncingTime = 3;
                                m_ActualShell.transform.SetParent(m_PointShell);
                                m_ObjectRB.transform.position = m_PointShell.position;
                            }
                        }
                    }


                    UpdateJump();

                    m_Movement *= l_Speed * Time.deltaTime;

                    m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;

                    m_Movement.y = m_VerticalSpeed * Time.deltaTime;

                    UpdatePunch();

                    m_Animator.SetBool("Hit", false);
                }
                else if (KnockBackCounter > 0)
                {
                    KnockBackCounter -= 0.5f;
                    m_Animator.SetBool("Hit", true);
                }
            }
            else
            {
                m_Animator.SetBool("Death", true);
                m_Movement = Vector3.zero;
            }

            if (!l_HasMovement && !m_IsJumping)
            {
                if (m_TimerIdelSpecial > 0)
                {
                    m_TimerIdelSpecial -= Time.deltaTime;
                }
                else
                    m_Animator.SetBool("IdelSpecial", true);
            }
            else
            {
                m_Animator.SetBool("IdelSpecial", false);
                m_TimerIdelSpecial = 10.0f;
            }


            CollisionFlags l_CollisionFlags = m_CharactherController.Move(m_Movement);
            if ((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f)
            {
                m_Animator.SetBool("Falling", false);
            }

            if (((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f) || (l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            {
                m_VerticalSpeed = 0.0f;
                m_IsJumping = false;
            }

            UpdateElevator();
        }
    }

    void LateUpdate()
    {
        Vector3 l_Angle = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0.0f, l_Angle.y, 0.0f);
    }

    public void UpdateJump()
    {
        if (m_ResetJumpTimer <= 0)
        {
            m_CurrentJumpId = 0;
        }

        if (CanJump() && m_ResetJumpTimer > 0)
        {
            m_ResetJumpTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(m_JumpKeyCode) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            if (CanJump())
            {
                m_IsJumping = true;
                m_ResetJumpTimer = m_ResetJumpStartTimer;
                Jump();
            }
        }
    }

    private bool CanJump()
    {
        if (m_CharactherController.isGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Jump()
    {
        m_Animator.SetTrigger("Jump");

        if (m_CurrentJumpId >= 3)
        {
            m_CurrentJumpId = 0;
        }

        m_CurrentJumpVerticalSpeed = CurrentJumpVerticalSpeed();
        m_AudioSource.clip = CurrentJumpSound();
        m_AudioSource.Play();

        if (!m_IsRuning && !m_IsWallJump)
        {
            m_Animator.SetInteger("JumpType", m_CurrentJumpId);
            m_CurrentJumpId++;
        }

        StartCoroutine(ExecutrJump());
    }

    IEnumerator ExecutrJump()
    {
        yield return new WaitForSeconds(m_WaitSatarJumpTime);
        m_Animator.SetBool("Jump", false);
        m_VerticalSpeed = m_CurrentJumpVerticalSpeed;
        m_Animator.SetBool("Falling", true);
    }

    float CurrentJumpVerticalSpeed()
    {
        if (!m_IsRuning && !m_IsWallJump)
        {
            switch (m_CurrentJumpId)
            {
                case 0:
                    return m_JumpVerticalSpeed;
                case 1:
                    return m_JumpDoubleVerticalSpeed;
                case 2:
                    return m_JumpTripleVerticalSpeed;
            }
        }
        else if (m_IsRuning)
        {
            return m_JumpLongVerticalSpeed;
        }
        else if (m_IsWallJump)
        {
            return m_WallJumpVerticalSpeed;
        }
        return 0.0f;
    }

    AudioClip CurrentJumpSound()
    {
        if (!m_IsRuning && !m_IsWallJump)
        {
            switch (m_CurrentJumpId)
            {
                case 0:
                    return m_ClipJump;
                case 1:
                    return m_ClipJumpDouble;
                case 2:
                    return m_ClipJumpTriple;
            }
        }
        else if (m_IsRuning)
        {
            return m_ClipJumpLong;
        }
        else if (m_IsWallJump)
        {
            return m_ClipWallJump;
        }
        return null;
    }

    void UpdatePunch()
    {
        if ((Input.GetMouseButtonDown(m_PunchKeyCode) && CanPunch()) || (Gamepad.current != null && (Gamepad.current.buttonWest.wasPressedThisFrame && CanPunch())))
        {
            PunchCombo();
            m_ResetPunchComboTimer = m_ResetPunchComboStartTimer;
            m_TimerIdelSpecial = 10.0f;
        }

        if (m_ResetPunchComboTimer <= 0)
        {
            m_CurrentPunchId = 0;
        }
        else
        {
            m_ResetPunchComboTimer -= Time.deltaTime;
        }
    }

    bool CanPunch()
    {
        if (m_IsMoving)
        {
            return false;
        }
        else if (m_IsJumping)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void PunchCombo()
    {
        m_Animator.SetTrigger("Punch");
        float l_DiffTime = Time.time - m_LastPunchTime;
        if (l_DiffTime <= m_PunchComboAvaiableTime)
        {
            m_CurrentPunchId++;
            if (m_CurrentPunchId >= 3)
            {
                m_CurrentPunchId = 0;
            }
        }
        m_LastPunchTime = Time.time;
        m_Animator.SetInteger("PunchCombo", m_CurrentPunchId);
    }

    public void EnableHitCollider(TPunchType PunchType, bool Active)
    {
        switch (PunchType)
        {
            case TPunchType.LEFT_HAND:
                m_LeftHandPunchHitCollider.SetActive(Active);
                break;
            case TPunchType.RIGHT_HAND:
                m_RightHandPunchHitCollider.SetActive(Active);
                break;
            case TPunchType.KICK:
                m_RightFoodPunchHitCollider.SetActive(Active);
                break;
        }
    }

    public void RestartGame()
    {
        m_CharactherController.enabled = false;
        if (m_CurrentCheckpoint == null)
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckpoint.m_RespawnPoint.position;
            transform.rotation = m_CurrentCheckpoint.m_RespawnPoint.rotation;
        }
        m_Death = false;
        m_Animator.SetBool("Death", false);
        m_CharactherController.enabled = true;
        m_AudioSource.PlayOneShot(m_ClipStart);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            m_CurrentCheckpoint = other.GetComponent<Checkpoint>();
        }
        else if (other.CompareTag("Elevator"))
        {
            if (CanAttachedElevator(other))
            {
                AtachedElevator(other);
            }
        }
        else if (other.CompareTag("DeadZone"))
        {
            GameManager.GetGameManager().GetHealth().Death();
        }
        else if (other.CompareTag("Shell"))
        {
            m_ShellDetected = true;
            m_ActualShell = other.gameObject;
            m_ObjectRB = other.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator") && other == m_CurrentElevator)
        {
            DetachElevator();
        }
        else if (other.CompareTag("Shell"))
        {
            m_ShellDetected = false;
        }
    }

    private bool CanAttachedElevator(Collider Elevator)
    {
        if (m_CurrentElevator != null)
            return false;

        return IsAtacheableElevator(Elevator);
    }

    private bool IsAtacheableElevator(Collider Elevator)
    {
        float l_DotAngel = Vector3.Dot(Elevator.transform.forward, Vector3.up);
        if (l_DotAngel >= Mathf.Cos(m_MaxAngleToAtached * Mathf.Deg2Rad))
        {
            return true;
        }
        return false;
    }

    private void AtachedElevator(Collider Elevator)
    {
        transform.SetParent(Elevator.transform.parent);
        m_CurrentElevator = Elevator;
    }

    private void DetachElevator()
    {
        m_CurrentElevator = null;
        transform.SetParent(null);
    }

    void UpdateElevator()
    {
        if (m_CurrentElevator == null)
        {
            return;
        }
        if (!IsAtacheableElevator(m_CurrentElevator))
        {
            DetachElevator();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (IsUpperHit(hit.transform.position))
            {
                hit.gameObject.GetComponent<EnemyController>().Kill();
                m_VerticalSpeed = m_KillJumpVerticalSpeed;
            }
            else
            {
                GameManager.GetGameManager().GetHealth().TakeDamage();
                m_AudioSource.PlayOneShot(m_ClipHit);
                Vector3 l_HitVec = transform.position - hit.gameObject.transform.position;
                KnockBack(l_HitVec);
            }
        }
        else if (hit.gameObject.CompareTag("Bridge"))
        {
            hit.rigidbody.AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }
        else if (hit.gameObject.CompareTag("Shell"))
        {
            if (IsUpperHit(hit.transform.position))
            {
                GameManager.GetGameManager().GetShell().m_BouncingTime = 3;
                GameManager.GetGameManager().GetShell().SetForce(m_ShellForce, transform.forward);
            }
        }

        if (!m_CharactherController.isGrounded && hit.normal.y < 0.1f)
        {
            m_IsWallJump = false;
            if (Input.GetKeyDown(m_JumpKeyCode) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                m_IsJumping = true;
                m_IsWallJump = true;
                m_Animator.SetBool("Wall", true);
                m_CurrentJumpId = 0;
                m_ResetJumpTimer = m_ResetJumpStartTimer;
                Jump();
                m_Movement = hit.normal * m_WallKnockBackForce;
                StartCoroutine(ResetCharacter());
            }
        }
    }

    IEnumerator ResetCharacter()
    {
        yield return new WaitForSeconds(0.05f);
        m_IsWallJump = false;
        m_Animator.SetBool("Wall", false);
    }


    private void KnockBack(Vector3 hit)
    {
        if (!m_Death)
        {
            KnockBackCounter = KnockBackTimer;
            m_Movement = hit.normalized * m_ForceKnockBack;
        }
    }

    private bool IsUpperHit(Vector3 GoombaTransform)
    {
        Vector3 l_GoombaDirectio = transform.position - GoombaTransform;
        l_GoombaDirectio.Normalize();
        float l_DotAngle = Vector3.Dot(l_GoombaDirectio, Vector3.up);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleToKillGoomba * Mathf.Deg2Rad) && m_VerticalSpeed <= M_MinVerticalSpeedToKillGoomba)
        {
            return true;
        }
        return false;
    }

    void DeadSound()
    {
        m_AudioSource.PlayOneShot(m_ClipDeath);
    }

    void PunchSound1()
    {
        m_AudioSource.PlayOneShot(m_ClipPunch1);
    }

    void PunchSound2()
    {
        m_AudioSource.PlayOneShot(m_ClipPunch2);
    }

    void PunchSound3()
    {
        m_AudioSource.PlayOneShot(m_ClipKick);
    }

    void Step()
    {
        m_AudioSource.PlayOneShot(m_ClipStep);
        m_ParticleSystem.Play();
    }

    void Land()
    {
        m_AudioSource.PlayOneShot(m_ClipLand);
    }
}
