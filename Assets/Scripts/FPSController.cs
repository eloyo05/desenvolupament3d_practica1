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

    }
}
