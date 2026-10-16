using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoCount = 20;

    public override bool CanPick()
    {
        return true;
    }
    public override void Pick()
    {
        base.Pick();
        Debug.Log("pick ammo item " + m_AmmoCount);
    }

}
