using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoCount = 20;

    public override bool CanPick(FPSController l_Player)
    {
        return l_Player.m_TotalAmmo < l_Player.m_MaxTotalAmmo;
    }
    public override void Pick(FPSController l_Player)
    {
        l_Player.m_TotalAmmo = Mathf.Min(l_Player.m_TotalAmmo + m_AmmoCount,l_Player.m_MaxTotalAmmo);

        Debug.Log("pick ammo item " + m_AmmoCount);
        base.Pick(l_Player);
    }

}
