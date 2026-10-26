using System.Collections.Generic;
using UnityEngine;

public class ShootingGalleryController : MonoBehaviour
{
    [Header("Game")]
    public float m_GameDuration = 30.0f;
    public int m_ScoreToWin = 1000;
    public int m_PointsPerTarget = 50;
    public float m_ResetDelay = 2.0f;

    [Header("Targets")]
    public List<ShootingGalleryTarget> m_TargetSequence;

    [Header("Door")]
    public GalleryDoor m_Door;

    bool m_IsPlaying = false;
    bool m_IsResetting = false;
    bool m_PlayerInsideGallery = false;

    int m_Score = 0;
    int m_CurrentTargetId = -1;

    float m_TimeLeft = 0.0f;
    float m_TargetTimeLeft = 0.0f;
    float m_ResetTimer = 0.0f;

    ShootingGalleryTarget m_CurrentTarget = null;

    string m_Message = "";

    private void Start()
    {
        for (int i = 0; i < m_TargetSequence.Count; ++i)
        {
            if (m_TargetSequence[i] != null)
            {
                m_TargetSequence[i].m_Gallery = this;
                m_TargetSequence[i].InitializeTarget();
            }
        }

        m_TimeLeft = m_GameDuration;
    }

    private void Update()
    {
        if (m_IsPlaying)
        {
            UpdateGame();
        }
        else if (m_IsResetting)
        {
            UpdateReset();
        }
    }

    void UpdateGame()
    {
        m_TimeLeft -= Time.deltaTime;
        m_TargetTimeLeft -= Time.deltaTime;

        if (m_TimeLeft <= 0.0f)
        {
            FinishGame(false);
            return;
        }

        if (m_TargetTimeLeft <= 0.0f)
        {
            if (m_CurrentTarget != null)
            {
                m_CurrentTarget.HideTarget();
                m_CurrentTarget = null;
            }

            ShowNextTarget();
        }
    }

    public void StartGame()
    {
        if (m_IsPlaying || m_IsResetting)
            return;

        if (m_TargetSequence.Count == 0)
        {
            Debug.LogWarning("Shooting Gallery: no targets assigned");
            return;
        }

        m_Score = 0;
        m_TimeLeft = m_GameDuration;
        m_CurrentTargetId = -1;
        m_Message = "";

        ResetAllTargets();

        m_IsPlaying = true;

        ShowNextTarget();
    }

    public void StopGame()
    {
        if (!m_IsPlaying)
            return;

        FinishGame(false);
    }

    void ShowNextTarget()
    {
        if (!m_IsPlaying)
            return;

        m_CurrentTargetId =
            (m_CurrentTargetId + 1) % m_TargetSequence.Count;

        m_CurrentTarget =
            m_TargetSequence[m_CurrentTargetId];

        if (m_CurrentTarget == null)
        {
            ShowNextTarget();
            return;
        }

        m_CurrentTarget.ActivateTarget();

        m_TargetTimeLeft =
            m_CurrentTarget.m_TimeOnScreen;
    }

    public void TargetHit(ShootingGalleryTarget l_Target)
    {
        if (!m_IsPlaying)
            return;

        if (l_Target != m_CurrentTarget)
            return;

        m_Score += m_PointsPerTarget;

        l_Target.HideTarget();
        m_CurrentTarget = null;

        if (m_Score >= m_ScoreToWin)
        {
            FinishGame(true);
            return;
        }

        ShowNextTarget();
    }

    void FinishGame(bool l_PlayerWon)
    {
        m_IsPlaying = false;

        ResetAllTargets();

        if (l_PlayerWon)
        {
            m_Message =
                "Felicitats! Repte superat i porta oberta";

            if (m_Door != null)
            {
                m_Door.OpenDoor();
            }
        }
        else
        {
            m_Message =
                "Torna a intentar-ho!";
        }

        m_IsResetting = true;
        m_ResetTimer = m_ResetDelay;
    }

    void UpdateReset()
    {
        m_ResetTimer -= Time.deltaTime;

        if (m_ResetTimer <= 0.0f)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        m_IsResetting = false;

        m_Score = 0;
        m_TimeLeft = m_GameDuration;
        m_CurrentTargetId = -1;
        m_CurrentTarget = null;

        m_Message = "";

        ResetAllTargets();
    }

    void ResetAllTargets()
    {
        for (int i = 0; i < m_TargetSequence.Count; ++i)
        {
            if (m_TargetSequence[i] != null)
            {
                m_TargetSequence[i].ResetTarget();
            }
        }
    }

    public void SetPlayerInsideGallery(bool l_Inside)
    {
        m_PlayerInsideGallery = l_Inside;
    }

    private void OnGUI()
    {
        if (!m_PlayerInsideGallery)
            return;

        GUIStyle l_HUDStyle =
            new GUIStyle(GUI.skin.label);

        l_HUDStyle.fontSize = 25;
        l_HUDStyle.fontStyle = FontStyle.Bold;
        l_HUDStyle.normal.textColor = Color.white;
        l_HUDStyle.alignment = TextAnchor.MiddleCenter;

        if (m_IsPlaying)
        {
            GUI.Label(
                new Rect(
                    Screen.width / 2.0f - 200,
                    20,
                    400,
                    35),
                "Punts: " + m_Score +
                " / " + m_ScoreToWin,
                l_HUDStyle
            );

            GUI.Label(
                new Rect(
                    Screen.width / 2.0f - 200,
                    55,
                    400,
                    35),
                "Temps: " +
                Mathf.CeilToInt(m_TimeLeft),
                l_HUDStyle
            );
        }

        if (m_Message != "")
        {
            GUIStyle l_MessageStyle =
                new GUIStyle(GUI.skin.label);

            l_MessageStyle.fontSize = 36;
            l_MessageStyle.fontStyle = FontStyle.Bold;
            l_MessageStyle.normal.textColor = Color.white;
            l_MessageStyle.alignment =
                TextAnchor.MiddleCenter;
            l_MessageStyle.wordWrap = true;

            GUI.Label(
                new Rect(
                    Screen.width / 2.0f - 400,
                    Screen.height / 2.0f - 75,
                    800,
                    150),
                m_Message,
                l_MessageStyle
            );
        }
    }
}