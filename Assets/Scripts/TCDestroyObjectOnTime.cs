using System.Collections;
using UnityEngine;

public class TCDestroyObjectOnTime : MonoBehaviour
{
    public float m_Time = 0.5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(m_Time);
        GameObject.Destroy(gameObject);
    }
}
