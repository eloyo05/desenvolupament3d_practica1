using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract bool CanPick();
    public virtual void Pick(FPSController p_Player)
    {
        gameObject.SetActive(false);
    }
    
}
