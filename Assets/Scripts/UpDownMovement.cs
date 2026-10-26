using UnityEngine;

public class UpDownMovement : MonoBehaviour
{
    public float m_Distance = 2.0f;
    public float m_Speed = 2.0f;
    public float m_WaitTime = 1.0f;

    Vector3 m_StartPosition;
    Vector3 m_DownPosition;

    bool m_GoingDown = true;
    float m_WaitTimer = 0.0f;

    private void Start()
    {
        m_StartPosition = transform.position;
        m_DownPosition = m_StartPosition + Vector3.down * m_Distance;
    }

    private void Update()
    {
        if (m_WaitTimer > 0.0f)
        {
            m_WaitTimer -= Time.deltaTime;
            return;
        }

        Vector3 l_TargetPosition;

        if (m_GoingDown)
            l_TargetPosition = m_DownPosition;
        else
            l_TargetPosition = m_StartPosition;

        transform.position = Vector3.MoveTowards(transform.position,l_TargetPosition, m_Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, l_TargetPosition) < 0.01f)
        {
            m_GoingDown = !m_GoingDown;
            m_WaitTimer = m_WaitTime;
        }
    }
}