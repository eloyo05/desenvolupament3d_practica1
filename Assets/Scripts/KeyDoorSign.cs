using UnityEngine;
using TMPro;

public class KeyDoorSign : MonoBehaviour
{
    public KeyInventory m_KeyInventory;
    public TMP_Text m_Text;

    private void Update()
    {
        if (m_KeyInventory == null || m_Text == null)
            return;

        if (m_KeyInventory.m_HasSpecialKey)
        {
            m_Text.text = "UNLOCKED";
            m_Text.color = Color.green;
        }
        else
        {
            m_Text.text = "LOCKED";
            m_Text.color = Color.red;
        }
    }
}