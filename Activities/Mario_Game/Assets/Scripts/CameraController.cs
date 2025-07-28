using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private float m_CameraIdleTimer = 0.0f;
    private float m_IdleTime = 5.0f;
    [SerializeField] private float m_JoystickSensivility = 0.5f;

    public Transform m_FollowObject;
    float m_Yaw = 0.0f;
    float m_Pitch = 0.0f;
    public float m_MinPitch = -60.0f;
    public float m_MaxPitch = 60.0f;
    public float m_MinCameraDistance = 5.0f;
    public float m_MaxCameraDistance = 15.0f;
    public float m_YawSpeed = 0.0f;
    public float m_PitchSpeed = 0.0f;
    public LayerMask m_LayerMask;
    public float m_OffsetHit = 0.1f;
    float m_HorizontalAxis;
    float m_VerticalAxis;

    private void LateUpdate()
    {
        if (!GameManager.GetGameManager().m_Paused)
        {
            m_HorizontalAxis = Input.GetAxis("Mouse X");
            m_VerticalAxis = Input.GetAxis("Mouse Y");
        }

        if (Gamepad.current != null)
        {
            Vector2 leftStickInput = Gamepad.current.rightStick.ReadValue();
            m_HorizontalAxis += leftStickInput.x * m_JoystickSensivility;
            m_VerticalAxis += leftStickInput.y * m_JoystickSensivility;
        }

        if (Mathf.Abs(m_HorizontalAxis) > 0.01f || Mathf.Abs(m_VerticalAxis) > 0.01f)
        {
            m_CameraIdleTimer = 0.0f;
        }
        else
        {
            m_CameraIdleTimer += Time.deltaTime;
        }

        if (m_CameraIdleTimer >= m_IdleTime)
        {
            RepositionCameraBehindPlayer();
            m_CameraIdleTimer = 0.0f;
        }
        

        Vector3 l_LookDirection = m_FollowObject.position - transform.position;
        float l_DistanceToPlayer = l_LookDirection.magnitude;
        l_LookDirection.y = 0.0f;
        l_LookDirection.Normalize();

        m_Yaw = Mathf.Atan2(l_LookDirection.x, l_LookDirection.z) * Mathf.Rad2Deg;

        m_Yaw += m_HorizontalAxis * m_YawSpeed * Time.deltaTime;
        m_Pitch += m_VerticalAxis * m_PitchSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_PitchInRadians = m_Pitch * Mathf.Deg2Rad;

        Vector3 l_CameraForward = new Vector3(Mathf.Sin(l_YawInRadians) * Mathf.Cos(l_PitchInRadians), Mathf.Sin(l_PitchInRadians), Mathf.Cos(l_YawInRadians) * Mathf.Cos(l_PitchInRadians));
        l_DistanceToPlayer = Mathf.Clamp(l_DistanceToPlayer, m_MinCameraDistance, m_MaxCameraDistance);
        Vector3 l_DesiredPosition = m_FollowObject.position - l_CameraForward * l_DistanceToPlayer;

        Ray l_Ray = new Ray(m_FollowObject.position, -l_CameraForward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, l_DistanceToPlayer, m_LayerMask.value))
        {
            l_DesiredPosition = l_RaycastHit.point + l_CameraForward * m_OffsetHit;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(m_FollowObject.position);

    }   
    private void RepositionCameraBehindPlayer()
    {
        Vector3 l_BackDirection = -m_FollowObject.forward;
        l_BackDirection.y = 0.0f;
        l_BackDirection.Normalize();
        
        Vector3 l_DesiredPosition = m_FollowObject.position + l_BackDirection * m_MaxCameraDistance;
        l_DesiredPosition.y = transform.position.y;

        Ray l_Ray = new Ray(m_FollowObject.position, -l_BackDirection);

        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxCameraDistance, m_LayerMask.value))
        {
            l_DesiredPosition = l_RaycastHit.point + l_BackDirection * m_OffsetHit;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(m_FollowObject.position);
    }
}    
