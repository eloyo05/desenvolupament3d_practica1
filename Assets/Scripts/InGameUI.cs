using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public LifeBarElementUI m_LifeBarElementUI;

    private void Start()
    {
       GameController.GetGameController().SetInGameUI(this);
    }
}
