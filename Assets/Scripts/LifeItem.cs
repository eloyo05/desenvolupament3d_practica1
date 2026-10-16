using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints = 20;

    public override bool CanPick()
    {
        return true;
    }
    public override void Pick()
    {
        base.Pick();
        Debug.Log("pick life item "+m_LifePoints);
    }

}
