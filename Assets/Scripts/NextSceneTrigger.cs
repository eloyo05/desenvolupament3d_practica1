using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    public string m_SceneName = "enemies";

    private void OnTriggerEnter(
        Collider l_Other)
    {
        FPSController l_Player =
            l_Other
            .GetComponentInParent<FPSController>();

        if (l_Player != null)
        {
            SceneManager.LoadSceneAsync(m_SceneName);
        }
    }
}