using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public FPSController m_Player;

    [Header("Texts")]
    public TMP_Text m_HealthText;
    public TMP_Text m_ShieldText;
    public TMP_Text m_AmmoText;

    private void Update()
    {
        if (m_Player == null)
            return;

        if (m_HealthText != null)
        {
            m_HealthText.text =
                Mathf.CeilToInt((float)m_Player.m_Health) +
                " / " +
                Mathf.CeilToInt((float)m_Player.m_MaxHealth);
        }

        if (m_ShieldText != null)
        {
            m_ShieldText.text =
                Mathf.CeilToInt((float)m_Player.m_Shield) +
                " / " +
                Mathf.CeilToInt((float)m_Player.m_MaxShield);
        }

        if (m_AmmoText != null)
        {
            m_AmmoText.text =
                m_Player.m_Magazine +
                " / " +
                m_Player.m_TotalAmmo;
        }
    }
}