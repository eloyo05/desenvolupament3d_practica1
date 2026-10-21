using UnityEngine;

public class ShootingGalleryButton : MonoBehaviour
{
    public enum TButtonType
    {
        START = 0,
        END
    }

    public ShootingGalleryController m_Gallery;

    public TButtonType m_ButtonType;

    public void Hit()
    {
        if (m_Gallery == null)
            return;

        if (m_ButtonType ==
            TButtonType.START)
        {
            m_Gallery.StartGame();
        }
        else if (m_ButtonType ==
                 TButtonType.END)
        {
            m_Gallery.StopGame();
        }
    }
}