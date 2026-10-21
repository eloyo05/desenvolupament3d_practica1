using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldPoints = 20;

    public override bool CanPick()
    {
        return true;
    }
    public override void Pick(FPSController p_Player)
    {
        p_Player.m_Shield = System.Math.Min(
          p_Player.m_Shield + m_ShieldPoints,
          p_Player.m_MaxShield
        );
        
        Debug.Log("pick shield item "+ m_ShieldPoints);
        base.Pick(p_Player);
    }

}
