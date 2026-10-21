using UnityEngine;

public class ShootingGalleryTarget : MonoBehaviour
{
    public enum TMovementType
    {
        STATIC = 0,
        UP_DOWN,
        LEFT_RIGHT
    }

    public ShootingGalleryController m_Gallery;

    [Header("Target")]
    public float m_TimeOnScreen = 1.5f;

    [Header("Movement")]
    public TMovementType m_MovementType =
        TMovementType.STATIC;

    public float m_MovementDistance = 3.0f;
    public float m_MovementSpeed = 2.0f;

    Vector3 m_StartLocalPosition;
    Vector3 m_EndLocalPosition;

    bool m_IsActiveTarget = false;
    bool m_MovingToEnd = true;

    public void InitializeTarget()
    {
        m_StartLocalPosition = transform.localPosition;

        if (m_MovementType == TMovementType.UP_DOWN)
        {
            m_EndLocalPosition =
                m_StartLocalPosition +
                Vector3.up * m_MovementDistance;
        }
        else if (m_MovementType == TMovementType.LEFT_RIGHT)
        {
            m_EndLocalPosition =
                m_StartLocalPosition +
                Vector3.right * m_MovementDistance;
        }
        else
        {
            m_EndLocalPosition = m_StartLocalPosition;
        }

        m_IsActiveTarget = false;
        m_MovingToEnd = true;

        gameObject.SetActive(false);
    }

    public void ActivateTarget()
    {
        transform.localPosition = m_StartLocalPosition;

        m_MovingToEnd = true;
        m_IsActiveTarget = true;

        gameObject.SetActive(true);
    }

    public void HideTarget()
    {
        m_IsActiveTarget = false;

        transform.localPosition = m_StartLocalPosition;

        gameObject.SetActive(false);
    }

    public void ResetTarget()
    {
        m_IsActiveTarget = false;
        m_MovingToEnd = true;

        transform.localPosition = m_StartLocalPosition;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_IsActiveTarget)
            return;

        if (m_MovementType == TMovementType.STATIC)
            return;

        Vector3 l_TargetPosition;

        if (m_MovingToEnd)
            l_TargetPosition = m_EndLocalPosition;
        else
            l_TargetPosition = m_StartLocalPosition;

        transform.localPosition =
            Vector3.MoveTowards(
                transform.localPosition,
                l_TargetPosition,
                m_MovementSpeed * Time.deltaTime
            );

        if (Vector3.Distance(
            transform.localPosition,
            l_TargetPosition) < 0.01f)
        {
            m_MovingToEnd = !m_MovingToEnd;
        }
    }

    public void Hit()
    {
        if (!m_IsActiveTarget)
            return;

        if (m_Gallery == null)
            return;

        m_Gallery.TargetHit(this);
    }
}