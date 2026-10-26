/*using UnityEngine;

public class TCDebugtimeScaler : MonoBehaviour
{
    public KeyCode m_AcceleratorKeyCode;
    public KeyCode m_DecceleratorKeyCode;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(m_AcceleratorKeyCode))
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 10f;
            }
            else
                Time.timeScale = 1f;
        }
        else if (Input.GetKeyDown(m_DecceleratorKeyCode))
        {
            if (Time.timeScale == 1f)
            Time.timeScale = 0.1f;
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

#endif
}*/
