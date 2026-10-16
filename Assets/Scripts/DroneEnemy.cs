using NUnit.Framework;
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
    public float m_ChaseDistance=5.0f;
    //CREAR EMPTY DE WAYPOYINT I DINS CREAR ELS WAYPOYINS QUE RECORRERÀ L'ENEMIC PER PATRULLAR (FOTO AL MOBIL 16/10) ARROSSEGARLOS A LA LLISTA DE L'INSPECTOR DEL SCRIPT DE DRONE ENEMY

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
        m_CurrentPatrolId = GetClosestPatrolWaypoint();
        m_NavMeshAgent.destination = m_Waypoints[m_CurrentPatrolId].position;
    }
    void UpdatePatrolState()
    {
        if (HasArrivedToWaypoint())
            MoveToNextPatrolPosition();
    }
    bool HasArrivedToWaypoint()
    {
        return m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete;
    }
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
    }
    void SetAttackState()
    {
        m_CurrentState = TState.ATTACK;
    }
    void SetChaseState()
    {
        m_CurrentState = TState.CHASE;
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
        // Implement alert behavior
    }
    void UpdateAttackState()
    {
        // Implement attack behavior
    }
    void UpdateChaseState()
    {
        // Implement chase behavior
    }
    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_Direction = (l_PlayerPosition - transform.position).normalized;
        m_NavMeshAgent.destination = l_PlayerPosition - l_Direction * m_ChaseDistance;
    }
    void UpdateHitState()
    {
        // Implement hit behavior
    }
    void UpdateDieState()
    {
        // Implement die behavior
    }
    
}
