using NUnit.Framework;
using System.Collections.Generic;
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

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

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
    public int m_MaxMagazine = 25;
    public int m_Magazine;
    public int m_TotalAmmo = 100;
    public CPoolElements m_HitParticlesPool;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_ReloadAnimationClip;

    [Header("Stats")]
    public double m_Health = 100;
    public double m_MaxHealth = 100;
    public double m_Shield = 100;
    public double m_MaxShield = 100;

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
        m_Health = m_MaxHealth;
        m_Shield = m_MaxShield;
        m_Magazine = m_MaxMagazine;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_HitParticlesPool = new CPoolElements(10, m_HitParticlesPrefab);

        GameController.GetGameController().SetPlayer(this);
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

        if (!m_AngleLocked)
        {
            m_Yaw += l_mouseX * m_yawSpeed * Time.deltaTime;
            m_Pitch += l_mouseY * m_PitchSpeed * Time.deltaTime;

            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        }

        Vector3 l_Direction = Vector3.zero;
        Vector3 l_Right = transform.right;
        Vector3 l_Forward = transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();

        if (Input.GetKey(m_RightKeyCode))
            l_Direction += l_Right;
        if (Input.GetKey(m_LeftKeyCode))
            l_Direction -= l_Right;

        if (Input.GetKey(m_UpKeyCode))
            l_Direction += l_Forward;
        if (Input.GetKey(m_DownKeyCode))
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

            if (Input.GetKeyDown(m_ReloadKeyCode) && m_Magazine < m_MaxMagazine && m_TotalAmmo >= m_MaxMagazine)
            {
                int l_AmmoNeeded = m_MaxMagazine - m_Magazine;
                m_Magazine = m_MaxMagazine;
                m_TotalAmmo -= l_AmmoNeeded;
                return true;
            }
            else if (Input.GetKeyDown(m_ReloadKeyCode) && m_Magazine < m_MaxMagazine && m_TotalAmmo < m_MaxMagazine)
            {
                m_Magazine += m_TotalAmmo;
                m_TotalAmmo = 0;
                return true;
            }
            else
            { 
                return false; 
            }
        }
        void Reload()
        {
            SetReloadWeaponAnimation();
        }
        bool CanShoot()
        {
            if (m_Magazine <= 0)
                return false;
            else
                return true;
        }
        bool MustShoot()
        {
            return Input.GetMouseButtonDown(m_ShootButton);
        }
        void Shoot()
        {
            SetShootWeaponAnimation();
            m_Magazine -= 1;

            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            /*if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))            
                CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);     */ //ABANS
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
            {
                if (l_RaycastHit.collider.CompareTag("HitCollider"))
                {
                    l_RaycastHit.collider.GetComponentInParent<HitCollider>().Hit();
                }
                else                
                {
                    CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);                    
                }

                ShootingGalleryButton l_GalleryButton = l_RaycastHit.collider.GetComponentInParent<ShootingGalleryButton>();
                if (l_GalleryButton != null)
                {
                    l_GalleryButton.Hit();
                    return;
                }

                ShootingGalleryTarget l_GalleryTarget = l_RaycastHit.collider.GetComponentInParent<ShootingGalleryTarget>();

                if (l_GalleryTarget != null)
                {
                    l_GalleryTarget.Hit();
                }
            } //ARA
        }        
        UpdateLifeBarEnemies();
    }
    void CreateShootHitParticles(Vector3 Position, Vector3 p_Normal)
    {
        GameObject l_GameObject = m_HitParticlesPool.GetNextElement();      
        l_GameObject.SetActive(true);
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

    void HurtPlayer(int damage)
    {
        if (m_Shield > 0)
        {
            m_Shield -= damage * 0.75;
            m_Health -= damage * 0.25;
        }
        else
        {
            m_Shield = 0;
            m_Health -= damage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item l_Item = other.GetComponent<Item>();
            if(l_Item.CanPick() && l_Item!=null)
            {
                l_Item.Pick(this);
            }
        }
    }
    private void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }
    void UpdateLifeBarEnemies()
    {
        GameController l_GameController = GameController.GetGameController();
        List<DroneEnemy> l_Enemies = l_GameController.GetEnemies();

        LifeBarElementUI l_LifeBarElementUI = l_GameController.GetInGameUI().m_LifeBarElementUI;
        l_LifeBarElementUI.Hide();

        foreach(DroneEnemy l_Enemy in l_Enemies)
        {
            Vector3 l_ViewportPoint = m_Camera.WorldToViewportPoint(l_Enemy.m_LifeBarPosition.position);
            bool l_EnemyIsVisible = l_ViewportPoint.z > 0.0f && l_ViewportPoint.x > 0 && l_ViewportPoint.x <= 1 && l_ViewportPoint.y > 0 && l_ViewportPoint.y < 1;

            if (l_EnemyIsVisible)
            {
                Vector2 l_Position = new Vector2(l_ViewportPoint.x * 1920.0f,l_ViewportPoint.y * 1080.0f);
                l_LifeBarElementUI.Show(l_Position, l_Enemy.GetLife()/(float)l_Enemy.m_MaxLife);  
                break;
            }
        }
    }
}