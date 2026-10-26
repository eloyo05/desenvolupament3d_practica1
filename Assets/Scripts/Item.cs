using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract bool CanPick(FPSController l_Player);

    public virtual void Pick(FPSController l_Player)
    {
        gameObject.SetActive(false);
    }

}
