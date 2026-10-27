
using UnityEngine;

public class SpecialKeyItem : Item
{
    public override bool CanPick(FPSController l_Player)
    {
        return true;
    }

    public override void Pick(FPSController l_Player)
    {
        KeyInventory l_Inventory =
            l_Player.GetComponent<KeyInventory>();

        if (l_Inventory != null)
        {
            l_Inventory.m_HasSpecialKey = true;

            Debug.Log("Has aconseguit la clau especial!");

            base.Pick(l_Player);
        }
    }
}
