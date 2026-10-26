using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("UI")]
    public GameObject m_GalleryHUD;
    public TMP_Text m_ScoreText;
    public TMP_Text m_TimeText;
    public TMP_Text m_MessageText;

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

        UpdateUI();
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

    void UpdateUI()
    {
        if (m_GalleryHUD == null)
            return;

        m_GalleryHUD.SetActive(m_PlayerInsideGallery);

        if (!m_PlayerInsideGallery)
            return;

        if (m_ScoreText != null)
        {
            m_ScoreText.gameObject.SetActive(m_IsPlaying);

            m_ScoreText.text ="Punts: " +m_Score +  " / " +  m_ScoreToWin;
        }

        if (m_TimeText != null)
        {
            m_TimeText.gameObject.SetActive(m_IsPlaying);

            m_TimeText.text = "Temps: " + Mathf.CeilToInt(m_TimeLeft); //QUE ES CeilToInt?
        }

        if (m_MessageText != null)
        {
            bool l_ShowMessage = m_Message != "";

            m_MessageText.gameObject.SetActive(l_ShowMessage);

            if (l_ShowMessage)
            {
                m_MessageText.text = m_Message;
            }
        }
    }
}