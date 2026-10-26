using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldPoints = 20;

    public override bool CanPick(FPSController l_Player)
    {
        return l_Player.m_Shield < l_Player.m_MaxShield;
    }

    public override void Pick(FPSController l_Player)
    {
        l_Player.m_Shield = System.Math.Min(l_Player.m_Shield +m_ShieldPoints,l_Player.m_MaxShield );

        Debug.Log(
            "pick shield item " + m_ShieldPoints);

        base.Pick(l_Player);
    }
}