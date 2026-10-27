using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool m_Activated = false;

    private void OnTriggerEnter(Collider l_Other)
    {
        if (m_Activated)
            return;

        FPSController l_Player =
            l_Other.GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            m_Activated = true;

            l_Player.SetCheckpoint(transform);
        }
    }
}