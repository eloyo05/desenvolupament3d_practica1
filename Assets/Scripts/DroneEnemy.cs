
using Unity.VisualScripting;
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
    //CREAR EMPTY DE WAYPOYINT I DINS CREAR ELS WAYPOYINS QUE RECORRERÀ L'ENEMIC PER PATRULLAR (FOTO AL MOBIL 16/10) ARROSSEGARLOS A LA LLISTA DE L'INSPECTOR DEL SCRIPT DE DRONE ENEMY

    [Header("Perception")]
    public float m_HearingDistance = 10.0f;
    public float m_AlertTime = 3.0f;
    public float m_AlertRotationSpeed = 120.0f;

    float m_AlertTimer;

    public Transform m_Eyes;
    public float m_ViewDistance = 20.0f;

    [Range(-1.0f, 1.0f)]
    public float m_MinViewDot = 0.5f;

    private void Awake()
    {

        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch(m_CurrentState)
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
    /*bool HasArrivedToWaypoint()
    {
        return m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete;
    }*/ //AIXO ES EL QUE TENIEM DE CLASSE QUE NO FUNCIONA

    bool HasArrivedToWaypoint()
    {
        if (m_NavMeshAgent.pathPending)
            return false;

        if (m_NavMeshAgent.remainingDistance >
            m_NavMeshAgent.stoppingDistance + 0.1f)
            return false;

        return !m_NavMeshAgent.hasPath ||
               m_NavMeshAgent.velocity.sqrMagnitude < 0.01f;
    } //AIXO HO HA FET EL XAT I SI FUNCIONA
    void MoveToNextPatrolPosition()
    {
        /*m_CurrentPatrolId++;
        if (m_CurrentPatrolId == m_Waypoints.Count)
            m_CurrentPatrolId = 0;*/
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
    }
    void SetChaseState()
    {
        m_CurrentState = TState.CHASE;
        m_NavMeshAgent.isStopped = false;

        SetNextChasePosition();
    }
    void SetHitState()
    {
        m_CurrentState = TState.HIT;
    }
    void SetDieState()
    {
        m_CurrentState = TState.DIE;
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
        // Implement attack behavior
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
        // Implement hit behavior
    }
    void UpdateDieState()
    {
        // Implement die behavior
    }

    bool HearsPlayer()
    {
        FPSController l_Player =
            GameController.GetGameController().GetPlayer();

        if (l_Player == null)
            return false;

        Vector3 l_Difference =
            l_Player.transform.position - transform.position;

        return l_Difference.sqrMagnitude <=
               m_HearingDistance * m_HearingDistance;
    }

    bool SeesPlayer()
    {
        FPSController l_Player =
            GameController.GetGameController().GetPlayer();

        if (l_Player == null)
            return false;

        Vector3 l_PlayerPosition = l_Player.transform.position;
        Vector3 l_Direction = l_PlayerPosition - m_Eyes.position;

        float l_Distance = l_Direction.magnitude;

        if (l_Distance > m_ViewDistance)
            return false;

        l_Direction.Normalize();

        float l_Dot =
            Vector3.Dot(transform.forward, l_Direction);

        if (l_Dot < m_MinViewDot)
            return false;

        if (Physics.Raycast(
            m_Eyes.position,
            l_Direction,
            out RaycastHit l_Hit,
            l_Distance))
        {
            return l_Hit.transform == l_Player.transform ||
                   l_Hit.transform.IsChildOf(l_Player.transform);
        }

        return false;
    }



}
