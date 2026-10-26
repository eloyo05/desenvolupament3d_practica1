using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints = 20;

    public override bool CanPick(FPSController l_Player)
    {
        return l_Player.m_Health < l_Player.m_MaxHealth;
    }
    public override void Pick(FPSController l_Player)
    {
        l_Player.m_Health = System.Math.Min(
            l_Player.m_Health + m_LifePoints,
            l_Player.m_MaxHealth
        );

        Debug.Log("pick life item "+m_LifePoints);
        base.Pick(l_Player);
    }

}
