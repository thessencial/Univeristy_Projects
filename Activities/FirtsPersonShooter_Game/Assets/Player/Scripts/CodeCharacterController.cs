using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CodeCharacterController : MonoBehaviour, IRestartGameElement
{
    public static CodeCharacterController instance;

    //Rotacion
    public Camera m_Camera;
    public Transform pitchController;
    public float YawSpeed;
    public float PitchSpeed;
    public float MinPicth;
    public float MaxPicth;
    public GameObject m_Player;

    private float _Yaw;
    private float _Pitch;

    //Moviemetno
    private CharacterController characterController;
    float _VerticalSpeed = 0.0f;

    public float CharacterSpeed;
    public float FastSpeed;
    public float JumpSpeed;

    [Header("Shoot")]
    public float m_MaxShootDistance;
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitParticlesPrefab;
    public float m_MaxAmmo = 50;
    public float m_Bullet = 8;
    bool m_LockAngle;
    CpoolElement m_PoolElement;

    [Header("Keys")]
    public KeyCode LeftKeyCode = KeyCode.A;
    public KeyCode RightKeyCode = KeyCode.D;
    public KeyCode UpKeyCode = KeyCode.W;
    public KeyCode DownKeyCode = KeyCode.S;
    public KeyCode RunKeyCode = KeyCode.LeftShift;
    public KeyCode JumpKeyCode = KeyCode.Space;
    public KeyCode ReloadCode = KeyCode.R;
    public KeyCode m_LockAngleKeyCode = KeyCode.J;

    [Header("Animation")]
    public Animation m_Animation;
    public Animation m_BulletsAnimation;
    public Animation m_IconsAnimation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_ReloadAnimationClip;
    public AnimationClip m_7BulletsAnimation;
    public AnimationClip m_5BulletsAnimation;
    public AnimationClip m_3BulletsAnimation;
    public AnimationClip m_1BulletsAnimation;
    public AnimationClip m_RechargeIconAnimation;
    public GameObject m_8Bullets;
    public GameObject m_6Bullets;
    public GameObject m_4Bullets;
    public GameObject m_2Bullets;
    public float m_ShootFadeTime = 0.1f;
    public float m_ShootOutFadetime = 0.1f;

    [Header("UI")]
    public TextMeshProUGUI m_Ammo;
    public Slider m_HealthSlider;
    public Slider m_ShieldSlider;
    public Canvas m_Canvas;
    public Canvas m_GameOver;

    [Header("Health")]
    //public GameObject m_HealthBar;
    public float m_MaxHealth = 100;
    private float m_CurrentHealth;
    private float m_DisplayedHealth;
    public float m_HealthBarSpeed = 1.5f;

    [Header("Shield")]
    public float m_MaxShield = 100;
    private float m_CurrentShield;
    private float m_DisplayedShield;
    public float m_ShieldBarSpeed = 1.5f;

    [Header("Audio")]
    public AudioSource m_ControlAudio;
    public AudioClip m_ShootSound;
    public AudioClip m_StepSound;
    public AudioClip m_ReloadSound;
    public AudioClip m_DamageSound;
    public AudioClip m_AmmoSound;
    public AudioClip m_ShildSound;
    public AudioClip m_HealthSound;

    public Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Others")]
    public int Keys = 0;
    bool OneTime = true;
    [SerializeField] private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        //DontDestroyOnLoad(m_HealthSlider);

        GameManager l_GameManager = GameManager.GetGameManager();

        if (l_GameManager.GetPlayer() != null)
        {
            l_GameManager.InitLevel(this);
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
        }

        m_Player = gameObject;

        DontDestroyOnLoad(m_Canvas);
        DontDestroyOnLoad(m_GameOver);
        DontDestroyOnLoad(m_Player);

        l_GameManager.SetPlayer(this);

        l_GameManager.AddRestartGameElement(this);

        //GameObject.DontDestroyOnLoad(gameObject);
        m_CurrentHealth = m_MaxHealth;
        m_DisplayedHealth = m_MaxHealth;
        m_HealthSlider.maxValue = m_MaxHealth;
        m_HealthSlider.value = m_CurrentHealth;

        m_CurrentShield = m_MaxShield;
        m_DisplayedShield = m_MaxShield;
        m_ShieldSlider.maxValue = m_MaxShield;
        m_ShieldSlider.value = m_CurrentShield;

        _Yaw = transform.eulerAngles.y;
        _Pitch = pitchController.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_LockAngle = false;
        Cursor.visible = false;
        SetIdleAnimation();

        Init();
    }

    private void Init()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        // Decol
        m_HitParticlesPrefab.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        m_PoolElement = new CpoolElement(7, m_HitParticlesPrefab);
        //
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Lab" && OneTime == true)
        {
            Init();
            OneTime = false;
        }

        float l_HorizontalValue = Input.GetAxis("Mouse X");
        float l_VerticalValue = Input.GetAxis("Mouse Y");
        m_Ammo.text = (m_Bullet + "/" + m_MaxAmmo);

        if (!m_LockAngle)
        {
            _Yaw = _Yaw + l_HorizontalValue * YawSpeed * Time.deltaTime;
            _Pitch = _Pitch - l_VerticalValue * PitchSpeed * Time.deltaTime;
            _Pitch = Mathf.Clamp(_Pitch, MinPicth, MaxPicth);
        }

        transform.rotation = Quaternion.Euler(0.0f, _Yaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(_Pitch, 0.0f, 0.0f);

        //----------------------------------------------------------------------------------

        float ForwardRadiantes = _Yaw * Mathf.Deg2Rad;
        float RightRadaitne = (_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 Forward = new Vector3(Mathf.Sin(ForwardRadiantes), 0.0f, Mathf.Cos(ForwardRadiantes));
        Vector3 Rigth = new Vector3(Mathf.Sin(RightRadaitne), 0.0f, Mathf.Cos(RightRadaitne));
        Vector3 Movment = Vector3.zero;

        if (Input.GetKey(RightKeyCode))
        {
            Movment = Rigth;
        }
        else if (Input.GetKey(LeftKeyCode))
        {
            Movment = -Rigth;
        }

        if (Input.GetKey(UpKeyCode))
        {
            Movment += Forward;
        }
        else if (Input.GetKey(DownKeyCode))
        {
            Movment -= Forward;
        }


        if (characterController.isGrounded && Input.GetKey(JumpKeyCode))
        {
            _VerticalSpeed = JumpSpeed;
        }

        Movment.Normalize();
        _VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        float speedMultiplay = 1.0f;

        if (Input.GetKey(RunKeyCode))
        {
            speedMultiplay = FastSpeed;
        }

        Movment *= CharacterSpeed * speedMultiplay * Time.deltaTime;
        Movment.y = _VerticalSpeed * Time.deltaTime;
        CollisionFlags collisionFlags = characterController.Move(Movment);

        if ((collisionFlags & CollisionFlags.Below) != 0)
        {
            _VerticalSpeed = 0.0f;
        }
        else if ((collisionFlags & CollisionFlags.Below) != 0 && _VerticalSpeed > 0.0f)
        {
            _VerticalSpeed = 0.0f;
        }

        if (CanShoot())
            Shoot();

        if (Input.GetKeyDown(m_LockAngleKeyCode))
        {
            m_LockAngle = !m_LockAngle;
        }

        if (CanReload())
        {
            Reload();
        }
        UpdateBulletsAnimation();

        if (m_HealthSlider.value != m_CurrentHealth)
        {
            m_DisplayedHealth = Mathf.Lerp(m_DisplayedHealth, m_CurrentHealth, Time.deltaTime * m_HealthBarSpeed);
            m_HealthSlider.value = m_DisplayedHealth;
        }
        if (m_ShieldSlider.value != m_CurrentShield)
        {
            m_DisplayedShield = Mathf.Lerp(m_DisplayedShield, m_CurrentShield, Time.deltaTime * m_ShieldBarSpeed);
            m_ShieldSlider.value = m_DisplayedShield;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(30.0f);
            //Debug.Log("Vida -10. Vida acutal: " + m_CurrentHealth + m_CurrentShield);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Heal(30.0f);
            AddShield(30.0f);
            //Debug.Log("Vida +10. Vida actual: " + m_CurrentHealth + m_CurrentShield);
        }
#endif

        if (transform.position.y < -1)
        {
            Kill();
        }

    }

    bool CanReload()
    {
        if (m_MaxAmmo > 0 && m_Bullet != 8)
        {
            return Input.GetKeyDown(ReloadCode);
        }
        else
        {
            return false;
        }
    }

    void Reload()
    {

        float m_BulletsNeeded = 8 - m_Bullet;
        float m_BulletsToReload = Mathf.Min(m_BulletsNeeded, m_MaxAmmo);

        m_Bullet += m_BulletsToReload;
        m_MaxAmmo -= m_BulletsToReload;

        m_MaxAmmo = Mathf.Max(m_MaxAmmo, 0);

        //Debug.Log("Balas recargadas " + m_BulletsToReload + ". Municion restante: " + m_MaxAmmo);
        SetReloadAnimation();
        m_ControlAudio.PlayOneShot(m_ReloadSound);
        SetIconAnimation();
        //UpdateBulletsAnimation();
    }

    bool CanShoot()
    {
        if (m_Bullet > 0)
        {
            return Input.GetMouseButtonDown(0);
        }
        else
        {
            return false;
        }
    }

    void Shoot()
    {
        m_ParticleSystem.Play();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.CompareTag("HitCollider"))
            {
                l_RaycastHit.collider.GetComponent<HitCollider>().Hit();
            }
            else
            {
                CreateHitParticules(l_RaycastHit.point, l_RaycastHit.normal, l_RaycastHit.transform);
            }
            MovingTarget target = l_RaycastHit.collider.GetComponent<MovingTarget>();
            if (target != null) { target.GetShoot(); }
        }
        SetShootAnimation();
        m_ControlAudio.PlayOneShot(m_ShootSound);
        m_Bullet -= 1;
        //UpdateBulletsAnimation();
        //Debug.Log(m_Bullet);

    }

    void CreateHitParticules(Vector3 Position, Vector3 Normal, Transform Parent)
    {
        GameObject l_HitParticules = m_PoolElement.GetNextElement();
        l_HitParticules.transform.position = Position;
        l_HitParticules.transform.rotation = Quaternion.LookRotation(Normal);
        l_HitParticules.transform.position += l_HitParticules.transform.forward / 1000;
        l_HitParticules.SetActive(true);

        l_HitParticules.transform.SetParent(Parent);
    }

    void SetIdleAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name);
    }

    void SetShootAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, m_ShootFadeTime);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, m_ShootOutFadetime);
    }

    void SetReloadAnimation()
    {
        m_Animation.CrossFade(m_ReloadAnimationClip.name);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name);
    }
    void Set7BulletsAnimation()
    {
        m_BulletsAnimation.CrossFade(m_7BulletsAnimation.name);
    }
    void Set5BulletsAnimation()
    {
        m_BulletsAnimation.CrossFade(m_5BulletsAnimation.name);
    }
    void Set3BulletsAnimation()
    {
        m_BulletsAnimation.CrossFade(m_3BulletsAnimation.name);
    }
    void Set1BulletsAnimation()
    {
        m_BulletsAnimation.CrossFade(m_1BulletsAnimation.name);
    }
    void SetIconAnimation()
    {
        m_IconsAnimation.CrossFade(m_RechargeIconAnimation.name);
        StartCoroutine(StopIconAnimationAfterDelay(2.0f));

    }
    void UpdateBulletsAnimation()
    {
        if (m_Bullet == 8)
        {
            m_8Bullets.SetActive(true);
            m_6Bullets.SetActive(true);
            m_4Bullets.SetActive(true);
            m_2Bullets.SetActive(true);

        }
        else if (m_Bullet == 7)
        {
            Set7BulletsAnimation();
        }
        else if (m_Bullet == 6)
        {
            m_8Bullets.SetActive(false);
            m_6Bullets.SetActive(true);
            m_4Bullets.SetActive(true);
            m_2Bullets.SetActive(true);
        }
        else if (m_Bullet == 5)
        {
            Set5BulletsAnimation();
        }
        else if (m_Bullet == 4)
        {
            m_8Bullets.SetActive(false);
            m_6Bullets.SetActive(false);
            m_4Bullets.SetActive(true);
            m_2Bullets.SetActive(true);
        }
        else if (m_Bullet == 3)
        {
            Set3BulletsAnimation();
        }
        else if (m_Bullet == 2)
        {
            m_8Bullets.SetActive(false);
            m_6Bullets.SetActive(false);
            m_4Bullets.SetActive(false);
            m_2Bullets.SetActive(true);
        }
        else if (m_Bullet == 1)
        {
            Set1BulletsAnimation();
        }
        else if (m_Bullet == 0)
        {
            m_8Bullets.SetActive(false);
            m_6Bullets.SetActive(false);
            m_4Bullets.SetActive(false);
            m_2Bullets.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        if (m_CurrentShield > 0)
        {
            float m_ShieldDamage = damage * 0.75f;
            float m_HealthDamage = damage * 0.25f;

            m_CurrentShield -= m_ShieldDamage;
            m_CurrentShield = Math.Clamp(m_CurrentShield, 0, m_MaxShield);

            m_CurrentHealth -= m_HealthDamage;
            m_CurrentHealth = Math.Clamp(m_CurrentHealth, 0, m_MaxHealth);
            m_ControlAudio.PlayOneShot(m_DamageSound);
        }
        else
        {
            m_CurrentHealth -= damage;
            m_CurrentHealth = Math.Clamp(m_CurrentHealth, 0, m_MaxHealth);
            m_ControlAudio.PlayOneShot(m_DamageSound);
        }

        if (m_CurrentHealth <= 0)
        {
            Kill();
        }
    }

    void Heal(float amount)
    {
        m_CurrentHealth += amount;
        m_CurrentHealth = Math.Clamp(m_CurrentHealth, 0, m_MaxHealth);
        //m_HealthSlider.value = m_CurrentHealth;
    }

    void AddShield(float amount)
    {
        m_CurrentShield += amount;
        m_CurrentShield = Math.Clamp(m_CurrentShield, 0, m_MaxShield);
    }

    void AddAmmo(float amount)
    {
        m_MaxAmmo += amount;
    }

    IEnumerator StopIconAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_IconsAnimation.Stop(m_RechargeIconAnimation.name);
    }

    //-------------------------------------------------------------------------------------------------------------------------------


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag is "Item")
        {
            if (other.gameObject.TryGetComponent(out Ammo ammo))
            {
                if (m_MaxAmmo <= 50)
                {
                    m_ControlAudio.PlayOneShot(m_AmmoSound);
                    float a = ammo.infoAmmo.ReturnAmount();
                    AddAmmo(a);
                    ammo.Disable();
                }
            }
            if (other.gameObject.TryGetComponent(out Shield shield))
            {
                if (m_CurrentShield < 100)
                {
                    m_ControlAudio.PlayOneShot(m_ShildSound);
                    float a = shield.infoShield.ReturnAmount();
                    AddShield(a);
                    shield.Disable();
                }
            }
            if (other.gameObject.TryGetComponent(out Health health))
            {
                if (m_CurrentHealth < 100)
                {
                    m_ControlAudio.PlayOneShot(m_HealthSound);
                    float a = health.infoHealth.ReturnAmount();
                    Heal(a);
                    health.Disable();
                }
            }
        }
        else if (other.CompareTag("DeadZone"))
        {
            Kill();
        }
        else if (other.CompareTag("Key"))
        {
            other.gameObject.SetActive(false);
            ++Keys;
        }
    }

    public void Kill()
    {
        GameManager.GetGameManager().RestartGame();
    }

    public void RestartGame()
    {
        characterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CurrentHealth = m_MaxHealth;
        m_CurrentShield = m_MaxShield;
        characterController.enabled = true;
    }
}
