
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public FPSController m_Player;


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
            "Munició: " + m_Player.m_Magazine + " / " + m_Player.m_MaxMagazine, l_Style);
    }

}
