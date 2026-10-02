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
    public float m_Speed;
    [Header("Input")]
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;

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

        if (Input.GetKey(m_RightKeyCode))
            l_Direction += transform.right;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Direction -= transform.right;

        if (Input.GetKey(m_UpKeyCode))
            l_Direction += transform.forward;
        else if (Input.GetKey(m_DownKeyCode))
            l_Direction -= transform.forward;

        l_Direction.Normalize();
        transform.position = transform.position + l_Direction * m_Speed *  Time.deltaTime;
    }
}
