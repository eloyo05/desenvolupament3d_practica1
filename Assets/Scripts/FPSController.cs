using UnityEngine;

public class FPSController : MonoBehaviour
{
    bool m_AngleLocked = false;
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
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLogCursorKeyCode = KeyCode.O;    
    public int m_ShootButton = 0;

    [Header("Shoot")]
    public Camera m_Camera;
    public float m_MaxShootDistance = 200.0f;
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitParticlesPrefab;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_ReloadAnimationClip;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = transform.rotation.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        SetIdleWeaponAnimation();
    }
    private void Update()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLogCursorKeyCode))
        {
            /* (Input.GetKeyDown(m_DebugLogCursorKeyCode))
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;*/

            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        float l_mouseX = Input.GetAxis("Mouse X");
        float l_mouseY = Input.GetAxis("Mouse Y");
       
        if (m_InvertedYaw)
            l_mouseX =- l_mouseX;
        if (m_InvertedPitch)
            l_mouseY =- l_mouseY;
        if (m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_mouseX * m_yawSpeed * Time.deltaTime;
            m_Pitch = m_Pitch + l_mouseY * m_PitchSpeed * Time.deltaTime;
        }

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

       
        
        if (CanShoot() && MustShoot())
        {
            Shoot();
        }
        if (CanReload() && MustReload())
        {
            Reload();
        }

        bool CanReload()
        {
            return true;
        }
        bool MustReload()
        {
            return Input.GetKeyDown(m_ReloadKeyCode);
        }
        void Reload()
        {
            SetReloadWeaponAnimation();
        }
        bool CanShoot()
        {
            return true;
        }
        bool MustShoot()
        {
            return Input.GetMouseButtonDown(m_ShootButton);
        }
        void Shoot()
        {
            SetShootWeaponAnimation();
            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))            
                CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);            
        }        
    }
    void CreateShootHitParticles(Vector3 Position, Vector3 p_Normal)
    {
        GameObject l_GameObject = GameObject.Instantiate(m_HitParticlesPrefab, GameController.GetGameController().m_DestroyObjects);
        l_GameObject.transform.position = Position;
        l_GameObject.transform.rotation = Quaternion.LookRotation(p_Normal);
    }
    void SetIdleWeaponAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name, 0.1f);
    }
    void SetShootWeaponAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.3f);
    }
    void SetReloadWeaponAnimation()
    {
        m_Animation.CrossFade(m_ReloadAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.3f);
    }
}