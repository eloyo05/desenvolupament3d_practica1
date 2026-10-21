using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoCount = 20;

    public override bool CanPick()
    {
        return true;
    }
    public override void Pick(FPSController p_Player)
    {
        p_Player.m_Magazine = Mathf.Min(
            p_Player.m_Magazine + m_AmmoCount,
            p_Player.m_MaxMagazine
        );
        
        Debug.Log("pick ammo item " + m_AmmoCount);
        base.Pick(p_Player);
    }

}
