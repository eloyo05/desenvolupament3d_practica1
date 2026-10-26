using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    static GameController m_GameController = null;
    public KeyCode m_RestartKeyCode = KeyCode.F5;
    
    FPSController m_Player;
    List<DroneEnemy> m_Enemies = new List<DroneEnemy>();
    InGameUI m_InGameUI;

    //Fade m_Fade;

    private void Awake()
    {
       if (m_GameController == null)
       {
            m_GameController = this;
            DontDestroyOnLoad(this);
       }
       else if (m_GameController != this)
        {
           Destroy(gameObject);
       }
    }
    static public GameController GetGameController()
    {
        return m_GameController;
    }
    public void Restart()
    {
        Debug.Log("Restarting game...");

        if (m_DestroyObjects != null)
        {
            for (int i = 0; i < m_DestroyObjects.childCount; ++i)
            {
                Destroy(m_DestroyObjects.GetChild(i).gameObject);
            }
        }

            if (m_Player != null)
            {
                m_Player.Restart();
            }

        for (int i = 0; i < m_Enemies.Count; ++i)
        {
            if (m_Enemies[i] != null)
            {
                m_Enemies[i].RestartEnemy();
            }
        }


        /*if (m_Fade != null)
        {
            StartCoroutine(m_Fade.FadeOut());
        }*/
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
    public void AddEnemy(DroneEnemy Enemy)
    {
        m_Enemies.Add(Enemy);
    }
    public void RemoveEnemy(DroneEnemy l_Enemy)
    {
        m_Enemies.Remove(l_Enemy);
    }
    public void SetInGameUI(InGameUI InGameUI)
    {
        m_InGameUI = InGameUI;
    }
    public InGameUI GetInGameUI()
    {
        return m_InGameUI;
    }
    public List<DroneEnemy> GetEnemies()
    {
        return m_Enemies;
    }

    /*public void SetFade(Fade _Fade)
    {
        m_Fade = _Fade;
    }
    public Fade GetFade()
    {
        return m_Fade;
    } */  
}
