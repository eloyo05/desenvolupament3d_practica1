using UnityEngine;

public class ShootingGalleryZone : MonoBehaviour
{
    public ShootingGalleryController m_Gallery;

    private void OnTriggerEnter(Collider other)
    {
        FPSController l_Player =
            other.GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            m_Gallery.SetPlayerInsideGallery(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FPSController l_Player =
            other.GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            m_Gallery.SetPlayerInsideGallery(false);
        }
    }
}