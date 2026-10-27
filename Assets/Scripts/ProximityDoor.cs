
using UnityEngine;

public class ProximityDoor : MonoBehaviour
{
    [Header("Doors")]
    public Transform m_LeftDoor;
    public Transform m_RightDoor;

    [Header("Movement")]
    public Vector3 m_LeftOpenOffset = new Vector3(-2.0f, 0.0f, 0.0f);
    public Vector3 m_RightOpenOffset = new Vector3(2.0f, 0.0f, 0.0f);
    public float m_OpenSpeed = 2.0f;

    [Header("Special Key")]
    public bool m_RequiresKey = false;

    Vector3 m_LeftClosedPosition;
    Vector3 m_RightClosedPosition;
    Vector3 m_LeftOpenPosition;
    Vector3 m_RightOpenPosition;

    bool m_PlayerInside = false;
    bool m_HasKey = false;

    private void Start()
    {
        m_LeftClosedPosition = m_LeftDoor.localPosition;
        m_RightClosedPosition = m_RightDoor.localPosition;

        m_LeftOpenPosition = m_LeftClosedPosition + m_LeftOpenOffset;
        m_RightOpenPosition = m_RightClosedPosition + m_RightOpenOffset;
    }

    private void Update()
    {
        if (m_PlayerInside && (!m_RequiresKey || m_HasKey))
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        m_LeftDoor.localPosition = Vector3.MoveTowards(
            m_LeftDoor.localPosition,
            m_LeftOpenPosition,
            m_OpenSpeed * Time.deltaTime
        );

        m_RightDoor.localPosition = Vector3.MoveTowards(
            m_RightDoor.localPosition,
            m_RightOpenPosition,
            m_OpenSpeed * Time.deltaTime
        );
    }

    void CloseDoor()
    {
        m_LeftDoor.localPosition = Vector3.MoveTowards(
            m_LeftDoor.localPosition,
            m_LeftClosedPosition,
            m_OpenSpeed * Time.deltaTime
        );

        m_RightDoor.localPosition = Vector3.MoveTowards(
            m_RightDoor.localPosition,
            m_RightClosedPosition,
            m_OpenSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider l_Other)
    {
        FPSController l_Player =
            l_Other.GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            m_PlayerInside = true;

            KeyInventory l_Inventory =
                l_Player.GetComponent<KeyInventory>();

            m_HasKey = l_Inventory != null &&
                       l_Inventory.m_HasSpecialKey;
        }
    }

    private void OnTriggerExit(Collider l_Other)
    {
        FPSController l_Player =
            l_Other.GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            m_PlayerInside = false;
        }
    }
}
