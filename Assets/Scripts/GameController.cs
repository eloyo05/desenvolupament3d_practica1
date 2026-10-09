using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    public static GameController m_GameController = null;
    public KeyCode m_RestartKeyCode = KeyCode.F5;

    private void Start()
    {
       if (m_GameController == null)
       {
            m_GameController = this;
            DontDestroyOnLoad(this);
       }
       else
       {
           GameObject.Destroy(gameObject);
       }
    }
    static public GameController GetGameController()
    {
        return m_GameController;
    }
    public void Restart()
    {
        Debug.Log("Restarting game...");
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
        {
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(m_RestartKeyCode))
        {
            Restart();
        }
    }
}
