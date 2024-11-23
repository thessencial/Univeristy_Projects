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
    //public GameObject m_HitParticlesPrefab;
    //public float m_MaxAmmo = 50;
    //public float m_Bullet = 8;
    bool m_LockAngle;
    //CpoolElement m_PoolElement;
    public bool m_OrangePortalCounter = false;
    public bool m_BluePortalCounter = false;

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
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public float m_ShootFadeTime = 0.1f;
    public float m_ShootOutFadetime = 0.1f;


    [Header("CrossHair")]
    public Image m_CrosshairImage;
    public Sprite m_EmptyCrosshair;
    public Sprite m_OrangeCrosshair;
    public Sprite m_BlueCrosshair;
    public Sprite m_FullCrosshair;

    [Header("Portal Scaling")]
    //public float m_MinPortalSize = 0.5f;
    //public float m_MaxPortalSize = 2.0f;
    private readonly float[] m_PortalSizes = { 0.5f, 1, 2.0f };
    public int m_CurrentPortalSize = 1;



    [Header("UI")]
    //public TextMeshProUGUI m_Ammo;
    //public Slider m_HealthSlider;
    //public Slider m_ShieldSlider;
    public Canvas m_Canvas;
    public Canvas m_GameOver;

    //[Header("Health")]
    ////public GameObject m_HealthBar;
    //public float m_MaxHealth = 100;
    //private float m_CurrentHealth;
    //private float m_DisplayedHealth;
    //public float m_HealthBarSpeed = 1.5f;

    //[Header("Shield")]
    //public float m_MaxShield = 100;
    //private float m_CurrentShield;
    //private float m_DisplayedShield;
    //public float m_ShieldBarSpeed = 1.5f;

    [Header("Audio")]
    public AudioSource m_ControlAudio;
    public AudioClip m_DeadSound;
    public AudioClip m_PortalEnter;
    public AudioClip m_PortalExit;
    public AudioClip m_ShootBluePortalSound;
    public AudioClip m_ShootOrangePortalSound;
    //public AudioClip m_ReloadSound;
    //public AudioClip m_DamageSound;
    //public AudioClip m_AmmoSound;
    //public AudioClip m_ShildSound;
    //public AudioClip m_HealthSound;

    public Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Teleport")]
    [SerializeField] private float m_TeleportOffset = 1.5f;
    private Vector3 m_MovementDirection;
    [SerializeField] private float m_MaxAngleToTeleport = 60.0f;

    [Header("Portal")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public DummyPortal m_DummyPortal;


    [Header("Portal preview")]
    [SerializeField] Material m_BluePortalPreviewMaterial;
    [SerializeField] Material m_OrangePortalPreviewMaterial;

    float m_Timer = 4.0f;



    [Header("Attach")]
    public Transform m_AttachTranformeCube;
    public Transform m_AttachTranformeTurretAndReflect;
    bool m_Attaching;
    public bool m_Attached;
    Rigidbody m_ObjectRB;
    public float m_AttachSpeed = 8.0f;
    public float m_StartDistanceToRotate = 2.5f;
    public float m_Force = 20.0f;
    [SerializeField] Transform m_PreviuseParent;
    public float m_MiniDistanceAttach = 1.0f;
    public float m_MaxDistanceAttach = 20.0f;

    [Header("Others")]
    //public int Keys = 0;
    public bool OneTime = true;

    public bool m_CanMove = true;

    //[SerializeField] private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        m_Timer = 1.0f;

        GameManager l_GameManager = GameManager.GetGameManager();
        m_CrosshairImage.sprite = m_EmptyCrosshair;

        if (l_GameManager.GetPlayer() != null)
        {
            l_GameManager.InitLevel(this);
            GameObject.Destroy(m_BluePortal.gameObject);
            GameObject.Destroy(m_OrangePortal.gameObject);
            GameObject.Destroy(m_DummyPortal.gameObject);
            GameObject.Destroy(m_Canvas.gameObject);
            GameObject.Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
        }

        l_GameManager.SetPlayer(this);

        l_GameManager.AddRestartGameElement(this);

        GameObject.DontDestroyOnLoad(gameObject);
        GameObject.DontDestroyOnLoad(m_Canvas);

        //m_CurrentHealth = m_MaxHealth;
        //m_DisplayedHealth = m_MaxHealth;

        //m_CurrentShield = m_MaxShield;
        //m_DisplayedShield = m_MaxShield;
        //m_ShieldSlider.maxValue = m_MaxShield;
        //m_ShieldSlider.value = m_CurrentShield;

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
    }

    void Update()
    {
        float l_HorizontalValue = Input.GetAxis("Mouse X");
        float l_VerticalValue = Input.GetAxis("Mouse Y");
        //m_Ammo.text = (m_Bullet + "/" + m_MaxAmmo);

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
        m_MovementDirection = Vector3.zero;

        if (m_CanMove)
        {
            if (Input.GetKey(RightKeyCode))
            {
                m_MovementDirection = Rigth;
            }
            else if (Input.GetKey(LeftKeyCode))
            {
                m_MovementDirection = -Rigth;
            }

            if (Input.GetKey(UpKeyCode))
            {
                m_MovementDirection += Forward;
            }
            else if (Input.GetKey(DownKeyCode))
            {
                m_MovementDirection -= Forward;
            }
        }


        if (characterController.isGrounded && Input.GetKey(JumpKeyCode))
        {
            _VerticalSpeed = JumpSpeed;
        }

        m_MovementDirection.Normalize();
        _VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        float speedMultiplay = 1.0f;

        if (Input.GetKey(RunKeyCode))
        {
            speedMultiplay = FastSpeed;
        }

        Vector3 Movment = m_MovementDirection * CharacterSpeed * speedMultiplay * Time.deltaTime;
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
        //-------------------------------------------------------------------------------------------

        if (m_Attached || m_Attaching)
        {
            if (m_ObjectRB != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DetachObject(m_Force);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    DetachObject(0.0f);
                }
            }
            else
            {
                m_Attached = false;
                m_Attaching = false;
            }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryAttachObject();
                m_Timer = 1.0f;
            }

            m_Timer -= Time.deltaTime;

            if (m_Timer <= 0.0f)
            {

                float l_ScrollWheel = Input.GetAxis("Mouse ScrollWheel");
                if (l_ScrollWheel != 0.0f)
                {
                    if (l_ScrollWheel > 0)
                    {
                        m_CurrentPortalSize = Mathf.Min(m_CurrentPortalSize + 1, m_PortalSizes.Length - 1);
                    }
                    else if (l_ScrollWheel < 0)
                    {
                        m_CurrentPortalSize = Mathf.Max(m_CurrentPortalSize - 1, 0);
                    }

                    if (Input.GetMouseButton(0) || (Input.GetMouseButton(1)))
                    {
                        m_DummyPortal.transform.localScale = Vector3.one * m_PortalSizes[m_CurrentPortalSize];
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    PreviewPortal(m_BluePortal, m_BluePortalPreviewMaterial);
                    m_DummyPortal.transform.localScale = Vector3.one * m_PortalSizes[m_CurrentPortalSize];
                    // m_DumyPortal.gameObject.SetActive(true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Shoot(m_BluePortal);
                    m_ControlAudio.PlayOneShot(m_ShootBluePortalSound);
                    m_BluePortal.transform.localScale = Vector3.one * m_PortalSizes[m_CurrentPortalSize];
                    UpdateCrosHair(m_BluePortal);
                    m_DummyPortal.gameObject.SetActive(false);
                }

                if (Input.GetMouseButton(1))
                {
                    PreviewPortal(m_OrangePortal, m_OrangePortalPreviewMaterial);
                    m_DummyPortal.transform.localScale = Vector3.one * m_PortalSizes[m_CurrentPortalSize];
                    //m_DumyPortal.gameObject.SetActive(true);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    Shoot(m_OrangePortal);
                    m_ControlAudio.PlayOneShot(m_ShootOrangePortalSound);
                    m_OrangePortal.transform.localScale = Vector3.one * m_PortalSizes[m_CurrentPortalSize];
                    UpdateCrosHair(m_OrangePortal);
                    m_DummyPortal.gameObject.SetActive(false);

                    //if (m_HealthSlider.value != m_CurrentHealth)
                    //{
                    //    m_DisplayedHealth = Mathf.Lerp(m_DisplayedHealth, m_CurrentHealth, Time.deltaTime * m_HealthBarSpeed);
                    //    m_HealthSlider.value = m_DisplayedHealth;
                    //}
                    //if (m_ShieldSlider.value != m_CurrentShield)
                    //{
                    //    m_DisplayedShield = Mathf.Lerp(m_DisplayedShield, m_CurrentShield, Time.deltaTime * m_ShieldBarSpeed);
                    //    m_ShieldSlider.value = m_DisplayedShield;
                    //}
                }
            }



        }

        if (Input.GetKeyDown(m_LockAngleKeyCode))
        {
            m_LockAngle = !m_LockAngle;
        }

        if (m_Attaching && m_ObjectRB != null)
        {
            UpdateAttaching();
        }
        //---------------------------------------------------


    }

    bool CanShoot()
    {
        return false;
    }

    void Shoot(Portal _Portal)
    {
        //m_ParticleSystem.Play();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (m_DummyPortal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal))
            {
                _Portal.gameObject.SetActive(true);
                _Portal.transform.position = l_RaycastHit.point;
                _Portal.transform.rotation = Quaternion.LookRotation(l_RaycastHit.normal);

                if (_Portal == m_BluePortal)
                {
                    m_BluePortalCounter = true;
                }
                else
                {
                    m_OrangePortalCounter = true;
                }
            }
            else
            {
                _Portal.gameObject.SetActive(false);

            }
        }
        m_DummyPortal.gameObject.SetActive(false);
        //SetShootAnimation();
        //m_ControlAudio.PlayOneShot(m_ShootSound);
        //m_Bullet -= 1;
        //UpdateBulletsAnimation();
        //Debug.Log(m_Bullet);

    }

    //void CreateHitParticules(Vector3 Position, Vector3 Normal, Transform Parent)
    //{
    //    GameObject l_HitParticules = m_PoolElement.GetNextElement();
    //    l_HitParticules.transform.position = Position;
    //    l_HitParticules.transform.rotation = Quaternion.LookRotation(Normal);
    //    l_HitParticules.transform.position += l_HitParticules.transform.forward / 1000;
    //    l_HitParticules.SetActive(true);

    //    l_HitParticules.transform.SetParent(Parent);
    //}

    void SetIdleAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name);
    }

    void SetShootAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, m_ShootFadeTime);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, m_ShootOutFadetime);
    }

    void UpdateCrosHair(Portal _Portal)
    {

        /*if ((int)_Portal.m_Type == 0 && _Portal.gameObject.activeSelf)
        {
            m_CrosshairImage.sprite = m_BlueCrosshair;
            m_BluePortalCounter = true;
        }
        else if ((int)_Portal.m_Type == 0 && !_Portal.gameObject.activeSelf)
        {
            m_CrosshairImage.sprite = m_EmptyCrosshair;
            m_BluePortalCounter = false;
        }

        if ((int)_Portal.m_Type == 1 && _Portal.gameObject.activeSelf)
        {
            m_CrosshairImage.sprite = m_OrangeCrosshair;
            m_OrangePortalCounter = true;
        }
        else if ((int)_Portal.m_Type == 1 && !_Portal.gameObject.activeSelf)
        {
            m_CrosshairImage.sprite = m_EmptyCrosshair;
            m_OrangePortalCounter = false;
        }

        if (m_OrangePortalCounter && m_BluePortalCounter)
        {
            m_CrosshairImage.sprite = m_FullCrosshair;
        }*/

        if ((int)_Portal.m_Type == 0)
        {
            m_BluePortalCounter = _Portal.gameObject.activeSelf;
        }
        else if ((int)_Portal.m_Type == 1)
        {
            m_OrangePortalCounter = _Portal.gameObject.activeSelf;
        }

        if (m_OrangePortalCounter && m_BluePortalCounter)
        {
            m_CrosshairImage.sprite = m_FullCrosshair;
        }
        else if (m_BluePortalCounter)
        {
            m_CrosshairImage.sprite = m_BlueCrosshair;
        }
        else if (m_OrangePortalCounter)
        {
            m_CrosshairImage.sprite = m_OrangeCrosshair;
        }
        else
        {
            m_CrosshairImage.sprite = m_EmptyCrosshair;
        }

    }
    void PreviewPortal(Portal _Portal, Material previewMaterial)
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            m_DummyPortal.gameObject.SetActive(true);
            m_DummyPortal.transform.position = l_RaycastHit.point;
            m_DummyPortal.transform.rotation = Quaternion.LookRotation(l_RaycastHit.normal);
            if (m_DummyPortal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal))
            {
                m_DummyPortal.SetPortalOk(true);

                /*MeshRenderer renderer = m_DummyPortal.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    renderer.material = previewMaterial;
                }
                else
                {
                    m_DummyPortal.gameObject.SetActive(false);
                }*/
            }
            else
                m_DummyPortal.SetPortalOk(false);
        }
    }

    //IEnumerator StopIconAnimationAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    m_IconsAnimation.Stop(m_RechargeIconAnimation.name);
    //}

    //-------------------------------------------------------------------------------------------------------------------------------


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("DeadZone"))
            Kill();
        else if (other.CompareTag("Portal"))
        {
            m_ControlAudio.PlayOneShot(m_PortalEnter);
            Teleport(other.GetComponent<Portal>());
        }
        else if (other.CompareTag("CheckPoint"))
        {
            m_StartPosition = other.transform.position;
            m_StartRotation = other.transform.rotation;
        }
    }

    private void Teleport(Portal _Portal)
    {
        float l_DotAngle = Vector3.Dot(m_MovementDirection, _Portal.m_OtherPortal.forward);

        if (l_DotAngle >= Mathf.Cos(m_MaxAngleToTeleport * Mathf.Deg2Rad))
        {
            Vector3 l_Position = transform.position + m_MovementDirection * m_TeleportOffset;
            Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(l_Position);
            Vector3 l_WorldPosition = _Portal.m_Mirror.transform.TransformPoint(l_LocalPosition);

            Vector3 l_forward = m_MovementDirection;
            Vector3 l_LocalForward = _Portal.m_OtherPortal.InverseTransformDirection(l_forward);
            Vector3 l_WorldForward = _Portal.m_Mirror.transform.TransformDirection(l_LocalForward);

            characterController.enabled = false;
            transform.position = l_WorldPosition;
            transform.forward = l_WorldForward;
            _Yaw = transform.eulerAngles.y;
            characterController.enabled = true;
        }
        m_ControlAudio.PlayOneShot(m_PortalExit);
    }

    public void Kill()
    {
        if (OneTime)
        {
            m_CanMove = false;
            m_ControlAudio.PlayOneShot(m_DeadSound);
            GameManager.GetGameManager().RestartGame();
            OneTime = false;
        }
    }

    public void RestartGame()
    {
        characterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
        m_CrosshairImage.sprite = m_EmptyCrosshair;
        //m_CurrentHealth = m_MaxHealth;
        //m_CurrentShield = m_MaxShield;
        characterController.enabled = true;
    }

    void TryAttachObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit hit, m_MaxDistanceAttach))
        {
            if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Turret") || hit.collider.CompareTag("Reflect"))
            {
                m_ObjectRB = hit.collider.GetComponent<Rigidbody>();
                if (m_ObjectRB != null)
                {
                    AttachObject();
                }
            }
        }
        else
        {
            Debug.Log("No hay objetos interactuables dentro del rango.");
        }
    }
    /*void AttachObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.CompareTag("Cube"))
            {
                AttachingObject(l_RaycastHit.rigidbody);
            }
            else if (l_RaycastHit.collider.CompareTag("Turret"))
            {
                AttachingObject(l_RaycastHit.rigidbody);
            }
            else if (l_RaycastHit.collider.CompareTag("Reflect"))
            {
                AttachingObject(l_RaycastHit.rigidbody);
            }
        }
    }*/
    void AttachObject()
    {
        if (m_ObjectRB == null) return;

        m_Attaching = true;
        m_ObjectRB.isKinematic = true;
        m_ObjectRB.transform.SetParent(m_AttachTranformeCube);
    }

    void AttachingObject(Rigidbody Object)
    {
        m_ObjectRB = Object;
        m_ObjectRB.isKinematic = true;
        m_Attaching = true;
        m_Attached = false;

        if (m_ObjectRB.TryGetComponent(out CompanionCube CubTeleport))
        {
            CubTeleport.SetTeleportable(false);
        }
        else if (m_ObjectRB.TryGetComponent(out Turret TurretTeleport))
        {
            TurretTeleport.SetTeleportable(false);
        }
        else if (m_ObjectRB.TryGetComponent(out CubeReflect CubeReflectTeleport))
        {
            CubeReflectTeleport.SetTeleportable(false);
        }
    }

    void DetachObject(float Force)
    {
        m_PreviuseParent = GameObject.FindWithTag("Parent").transform;
        m_ObjectRB.transform.SetParent(m_PreviuseParent);
        m_ObjectRB.isKinematic = false;
        m_Attaching = false;
        m_Attached = false;
        m_ObjectRB.velocity = m_AttachTranformeCube.forward * Force;

        if (m_ObjectRB.TryGetComponent(out CompanionCube CubTeleport))
        {
            CubTeleport.SetTeleportable(true);
        }
        else if (m_ObjectRB.TryGetComponent(out Turret TurretTeleport))
        {
            TurretTeleport.SetTeleportable(true);
        }
        else if (m_ObjectRB.TryGetComponent(out CubeReflect CubeReflectTeleport))
        {
            CubeReflectTeleport.SetTeleportable(true);
        }
    }

    void UpdateAttaching()
    {
        if (m_Attaching)
        {
            Vector3 l_Direction = m_AttachTranformeCube.position - m_ObjectRB.position;
            float l_Distance = l_Direction.magnitude;
            l_Direction /= l_Distance;
            float l_Movment = m_AttachSpeed * Time.deltaTime;

            if (l_Movment >= l_Distance || l_Distance < m_MiniDistanceAttach)
            {
                m_Attached = true;
                m_Attaching = false;

                if (m_ObjectRB.CompareTag("Cube"))
                {
                    m_ObjectRB.transform.SetParent(m_AttachTranformeCube);
                    m_ObjectRB.transform.localPosition = Vector3.zero;
                    m_ObjectRB.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    m_ObjectRB.transform.SetParent(m_AttachTranformeTurretAndReflect);
                    m_ObjectRB.transform.localPosition = Vector3.zero;
                    m_ObjectRB.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                m_ObjectRB.transform.position += l_Movment * l_Direction;
                float l_Pct = Mathf.Min(1.0f, l_Distance / m_StartDistanceToRotate);
                m_ObjectRB.transform.rotation = Quaternion.Lerp(m_AttachTranformeCube.rotation, m_ObjectRB.transform.rotation, l_Pct);
            }
        }
    }
}
