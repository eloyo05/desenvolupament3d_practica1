using UnityEngine;

public class GalleryDoor : MonoBehaviour
{
    public Vector3 m_OpenOffset =
        new Vector3(0.0f, 5.0f, 0.0f);

    public float m_OpenSpeed = 2.0f;

    Vector3 m_ClosedLocalPosition;
    Vector3 m_OpenLocalPosition;

    bool m_IsOpen = false;

    private void Start()
    {
        m_ClosedLocalPosition =
            transform.localPosition;

        m_OpenLocalPosition =
            m_ClosedLocalPosition +
            m_OpenOffset;
    }

    private void Update()
    {
        if (!m_IsOpen)
            return;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_OpenLocalPosition,m_OpenSpeed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        m_IsOpen = true;
    }
}