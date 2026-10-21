
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public FPSController m_Player;

    public float m_CrosshairSize = 10f;
    public float m_CrosshairThickness = 2f;
    public float m_CrosshairGap = 4f;

    void OnGUI()
    {
        GUIStyle l_Style = new GUIStyle(GUI.skin.label);
        l_Style.fontSize = 25;
        l_Style.fontStyle = FontStyle.Bold;
        l_Style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 300, 35),
            "Vida: " + m_Player.m_Health + " / " + m_Player.m_MaxHealth, l_Style);

        GUI.Label(new Rect(20, 55, 300, 35),
            "Escut: " + m_Player.m_Shield + " / " + m_Player.m_MaxShield, l_Style);

        GUI.Label(new Rect(20, 90, 350, 35),
            "Munició: " + m_Player.m_Magazine + " / " + m_Player.m_TotalAmmo, l_Style);

        GUI.color = Color.white;

        float l_CenterX = Screen.width / 2f;
        float l_CenterY = Screen.height / 2f;

        GUI.DrawTexture(new Rect(
            l_CenterX - m_CrosshairThickness / 2,
            l_CenterY - m_CrosshairGap - m_CrosshairSize,
            m_CrosshairThickness,
            m_CrosshairSize), Texture2D.whiteTexture);

        GUI.DrawTexture(new Rect(
            l_CenterX - m_CrosshairThickness / 2,
            l_CenterY + m_CrosshairGap,
            m_CrosshairThickness,
            m_CrosshairSize), Texture2D.whiteTexture);

        GUI.DrawTexture(new Rect(
            l_CenterX - m_CrosshairGap - m_CrosshairSize,
            l_CenterY - m_CrosshairThickness / 2,
            m_CrosshairSize,
            m_CrosshairThickness), Texture2D.whiteTexture);

        GUI.DrawTexture(new Rect(
            l_CenterX + m_CrosshairGap,
            l_CenterY - m_CrosshairThickness / 2,
            m_CrosshairSize,
            m_CrosshairThickness), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }
}
