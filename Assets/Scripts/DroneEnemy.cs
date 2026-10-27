
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DroneEnemy : MonoBehaviour
{
    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        ATTACK,
        CHASE,
        HIT,
        DIE
    }
    NavMeshAgent m_NavMeshAgent;
    TState m_CurrentState=TState.IDLE;
    public List<Transform> m_Waypoints;
    int m_CurrentPatrolId;
    public float m_MinAttackDistance = 5.0f;
    public float m_MaxChaseDistance = 20.0f;
    public float m_ShootMaxDistance = 8.0f;
     
    public int m_MaxLife = 50;
    int m_Life = 50;
    public Transform m_LifeBarPosition;

    [Header("Hits")]
    public int m_HelixColliderAmount = 10;
    public int m_HeadColliderAmount = 50;
    public int m_BodyColliderAmount = 20;

    [Header("Perception")]
    public float m_HearingDistance = 10.0f;
    public float m_AlertTime = 3.0f;
    public float m_AlertRotationSpeed = 120.0f;
    float m_AlertTimer;

    [Header("Attack")]
    public int m_AttackDamage = 20;
    public float m_AttackCooldown = 1.0f;
    float m_AttackTimer = 0.0f;

    [Header("Hit")]
    public float m_HitTime = 0.25f;
    float m_HitTimer = 0.0f;
    TState m_StateAfterHit = TState.ALERT;

    [Header("Die")]
    public float m_DieFadeTime = 1.0f;

    float m_DieTimer = 0.0f;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    Vector3 m_StartScale;

    public Transform m_Eyes;
    public float m_ViewDistance = 20.0f;

    [Range(-1.0f, 1.0f)]
    public float m_MinViewDot = 0.5f;

    [Header("Drop item")]
    public GameObject m_AmmoItemPrefab;
    private bool m_DroppedItem = false;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_StartScale = transform.localScale;
    }
    private void Start()
    {
        m_Life = m_MaxLife;
        SetIdleState();
        GameController.GetGameController().AddEnemy(this);
    }
    private void Update()
    {
        if (m_AttackTimer > 0.0f)
        {
            m_AttackTimer -= Time.deltaTime;
        }

        switch (m_CurrentState)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;

        }
        
    }
    void SetIdleState()
    {
        m_CurrentState=TState.IDLE;
    }
    void SetPatrolState()
    {
        m_CurrentState = TState.PATROL;

        m_NavMeshAgent.isStopped = false;

        m_CurrentPatrolId = GetClosestPatrolWaypoint();

        m_NavMeshAgent.SetDestination(
            m_Waypoints[m_CurrentPatrolId].position
        );
    }
    void UpdatePatrolState()
    {
        if (HearsPlayer())
        {
            SetAlertState();
            return;
        }

        if (HasArrivedToWaypoint())
            MoveToNextPatrolPosition();
    }
   
    bool HasArrivedToWaypoint()
    {
        if (m_NavMeshAgent.pathPending)
            return false;

        if (m_NavMeshAgent.remainingDistance >
            m_NavMeshAgent.stoppingDistance + 0.1f)
            return false;

        return !m_NavMeshAgent.hasPath ||
               m_NavMeshAgent.velocity.sqrMagnitude < 0.01f;
    }
    void MoveToNextPatrolPosition()
    {
        m_CurrentPatrolId = (m_CurrentPatrolId + 1) % m_Waypoints.Count;
        m_NavMeshAgent.destination = m_Waypoints[m_CurrentPatrolId].position;
    }
    int GetClosestPatrolWaypoint()
    {
        int l_WaypointId = 0;
        float l_MaxDistance = 0.0f;

        for (int i =0; i < m_Waypoints.Count; i++)
        {
            float l_Distance=(transform.position - m_Waypoints[i].position).sqrMagnitude;
            if (i==0 ||l_Distance<l_MaxDistance)
            {
                l_MaxDistance = l_Distance;
                l_WaypointId = i;
            }
        }
        return l_WaypointId;

    }
    void SetAlertState()
    {
        m_CurrentState = TState.ALERT;

        m_NavMeshAgent.isStopped = true;

        m_AlertTimer = m_AlertTime;
    }
    void SetAttackState()
    {
        m_CurrentState = TState.ATTACK;

        m_NavMeshAgent.isStopped = true;

        
    }
    void SetChaseState()
    {
        m_CurrentState = TState.CHASE;
        m_NavMeshAgent.isStopped = false;

        SetNextChasePosition();
    }
    void SetHitState(TState l_StateAfterHit)
    {
        m_StateAfterHit = l_StateAfterHit;

        m_CurrentState = TState.HIT;

        m_NavMeshAgent.isStopped = true;

        m_HitTimer = m_HitTime;
    }
    void SetDieState()
    {
        if (m_CurrentState == TState.DIE)
            return;

        m_CurrentState = TState.DIE;

        if (m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.enabled = false;
        }

        Collider[] l_Colliders = GetComponentsInChildren<Collider>();

        foreach (Collider l_Collider in l_Colliders)
        {
            l_Collider.enabled = false;
        }

        m_DieTimer = m_DieFadeTime;
    }
    void UpdateIdleState()
    {
        SetPatrolState();
    }

    void UpdateAlertState()
    {
        transform.Rotate(
            0.0f,
            m_AlertRotationSpeed * Time.deltaTime,
            0.0f
        );

        if (SeesPlayer())
        {
            SetChaseState();
            return;
        }

        m_AlertTimer -= Time.deltaTime;

        if (m_AlertTimer <= 0.0f)
            SetPatrolState();
    }
    void UpdateAttackState()
    {
        FPSController l_Player = GameController.GetGameController().GetPlayer();
        
        if (l_Player == null)
            return;

        Vector3 l_Direction = l_Player.transform.position - transform.position;

        l_Direction.y = 0.0f;

        float l_Distance = l_Direction.magnitude;

        if (l_Distance > m_ShootMaxDistance)
        {
            SetChaseState();
            return;
        }

        if (!SeesPlayer())
        {
            SetChaseState();
            return;
        }

        if (l_Direction.sqrMagnitude > 0.01f)
        {
            Quaternion l_TargetRotation = Quaternion.LookRotation(l_Direction.normalized);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,l_TargetRotation,m_AlertRotationSpeed * Time.deltaTime);
        }

        

        if (m_AttackTimer <= 0.0f)
        {
            l_Player.HurtPlayer(m_AttackDamage);

            m_AttackTimer =m_AttackCooldown;
        }
    }
    void UpdateChaseState()
    {
        Transform l_Player =
            GameController.GetGameController().GetPlayer().transform;

        float l_Distance =
            Vector3.Distance(transform.position, l_Player.position);

        if (l_Distance > m_MaxChaseDistance)
        {
            SetPatrolState();
            return;
        }

        if (l_Distance <= m_MinAttackDistance && SeesPlayer())
        {
            SetAttackState();
            return;
        }

        SetNextChasePosition();
    }
    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_Direction = (l_PlayerPosition - transform.position).normalized;
        m_NavMeshAgent.destination = l_PlayerPosition - l_Direction * m_MinAttackDistance;
    }
    void UpdateHitState()
    {
        m_HitTimer -= Time.deltaTime;

        if (m_HitTimer > 0.0f)
            return;

        switch (m_StateAfterHit)
        {
            case TState.ALERT:
                SetAlertState();
                break;

            case TState.PATROL:
                SetPatrolState();
                break;

            case TState.CHASE:
                SetChaseState();
                break;

            case TState.ATTACK:
                SetAttackState();
                break;

            case TState.IDLE:
                SetIdleState();
                break;

            default:
                SetAlertState();
                break;
        }
    }
    void UpdateDieState()
    {
        m_DieTimer -= Time.deltaTime;

        float l_NormalizedTime = Mathf.Clamp01( m_DieTimer /m_DieFadeTime);

        transform.localScale = m_StartScale *l_NormalizedTime;

        Vector3 l_DropPosition = transform.position;
        l_DropPosition.y += 0.75f;

        if (!m_DroppedItem)
        {
            m_DroppedItem = true;

            Instantiate(
                m_AmmoItemPrefab,
                l_DropPosition,
                Quaternion.identity
            );
        }

        if (m_DieTimer <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }

    bool HearsPlayer()
    {
        FPSController l_Player = GameController.GetGameController().GetPlayer();

        if (l_Player == null)
            return false;

        Vector3 l_Difference =l_Player.transform.position - transform.position;

        return l_Difference.sqrMagnitude <=  m_HearingDistance * m_HearingDistance;
    }

    bool SeesPlayer()
    {
        FPSController l_Player =  GameController.GetGameController().GetPlayer();

        if (l_Player == null)
            return false;

        Vector3 l_PlayerPosition = l_Player.transform.position;
        Vector3 l_Direction = l_PlayerPosition - m_Eyes.position;

        float l_Distance = l_Direction.magnitude;

        if (l_Distance > m_ViewDistance)
            return false;

        l_Direction.Normalize();

        float l_Dot = Vector3.Dot(transform.forward, l_Direction);

        if (l_Dot < m_MinViewDot)
            return false;

        if (Physics.Raycast( m_Eyes.position, l_Direction, out RaycastHit l_Hit, l_Distance))
        {
            return l_Hit.transform == l_Player.transform ||  l_Hit.transform.IsChildOf(l_Player.transform);
        }

        return false;
    }
    public void Hit(HitCollider.THitColliderType l_HitColliderType)
    {
        int l_LifePoints = 0;

        switch (l_HitColliderType)
        {
            case HitCollider.THitColliderType.BODY:
                l_LifePoints = 0;
                break;

            case HitCollider.THitColliderType.HEAD:
                l_LifePoints =
                    m_HeadColliderAmount;
                break;

            case HitCollider.THitColliderType.HELIX:
                l_LifePoints =
                    m_HelixColliderAmount;
                break;

            default:
                Debug.LogError("Unknown hit collider type: " + l_HitColliderType);
                return;
        }

        TState l_PreviousState = m_CurrentState;

        bool l_DidDamage = l_LifePoints > 0;

        if (l_DidDamage)
        {
            m_Life -= l_LifePoints;
        }

        if (m_Life <= 0)
        {
            SetDieState();
            return;
        }

        if (l_PreviousState == TState.PATROL || l_PreviousState == TState.IDLE)
        {
            SetHitState(TState.ALERT);
        }
        else if (l_DidDamage)
        {
            SetHitState(l_PreviousState);
        }

        Debug.Log("Drone hit! Remaining life: " + m_Life);
    }
    public int GetLife()
    {
        return m_Life;
    }

    public void RestartEnemy()
    {
        gameObject.SetActive(true);

        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        transform.localScale = m_StartScale;

        m_Life = m_MaxLife;

        Collider[] l_Colliders =
            GetComponentsInChildren<Collider>();

        foreach (Collider l_Collider in l_Colliders)
        {
            l_Collider.enabled = true;
        }

        if (!m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.enabled = true;
        }

        m_NavMeshAgent.Warp(m_StartPosition);

        m_AttackTimer = 0.0f;
        m_DroppedItem = false;

        SetPatrolState();
    }
}
