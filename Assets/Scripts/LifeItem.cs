using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints = 20;

    public override bool CanPick()
    {
        return true;
    }
    public override void Pick(FPSController p_Player)
    {
        p_Player.m_Health = System.Math.Min(
            p_Player.m_Health + m_LifePoints,
            p_Player.m_MaxHealth
        );

        Debug.Log("pick life item "+m_LifePoints);
        base.Pick(p_Player);
    }

}
