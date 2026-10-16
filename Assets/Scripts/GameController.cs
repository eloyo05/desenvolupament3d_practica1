using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    public static GameController m_GameController = null;
    public KeyCode m_RestartKeyCode = KeyCode.F5;
    public FPSController m_Player;

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
    public void SetPlayer(FPSController Player)
    {
        m_Player = Player;
    }
    public FPSController GetPlayer()
    {
        return m_Player;
    }
}
