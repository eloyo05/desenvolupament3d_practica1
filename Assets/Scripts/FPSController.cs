using UnityEngine;

public class FPSController : MonoBehaviour
{


    public Transform m_PitchController;

    float m_Yaw;
    float m_Pitch;

    public float m_yawSpeed;
    public float m_PitchSpeed;
    public bool m_InvertedPitch;
    public bool m_InvertedYaw;
    public float m_MinPitch;
    public float m_MaxPitch;
    CharacterController m_CharacterController;
    float m_VerticalSpeed = 0.0f;
    public float m_Speed;
    public float m_SprintSpeed;
    public float m_JumpSpeed;

    [Header("Input")]
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = transform.rotation.eulerAngles.x;
    }
    private void Update()
    {
        float l_mouseX = Input.GetAxis("Mouse X");
        float l_mouseY = Input.GetAxis("Mouse Y");
       
        if (m_InvertedYaw)
            l_mouseX =- l_mouseX;
        if (m_InvertedPitch)
            l_mouseY =- l_mouseY;

        m_Yaw += l_mouseX * m_yawSpeed * Time.deltaTime;
        m_Pitch += l_mouseY * m_PitchSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        Vector3 l_Direction = Vector3.zero;
        Vector3 l_Right = transform.right;
        Vector3 l_Forward = transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();

        if (Input.GetKey(m_RightKeyCode))
            l_Direction = l_Right;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Direction -= l_Right;

        if (Input.GetKey(m_UpKeyCode))
            l_Direction = l_Forward;
        else if (Input.GetKey(m_DownKeyCode))
            l_Direction -= l_Forward;

        float l_Speed = m_Speed;
        if (Input.GetKey(m_SprintKeyCode))
            l_Speed = m_SprintSpeed;
        if(Input.GetKeyDown(m_JumpKeyCode) /*&& m_VerticalSpeed == 0.0f*/ && m_CharacterController.isGrounded)
            m_VerticalSpeed = m_JumpSpeed;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;

        l_Direction.Normalize();
        l_Direction = l_Direction * l_Speed *  Time.deltaTime;
        l_Direction.y = m_VerticalSpeed * Time.deltaTime;
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);
        if((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
        m_VerticalSpeed = 0.0f;
        else if((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f) //màscara binària
            m_VerticalSpeed = 0.0f;
    }
}
