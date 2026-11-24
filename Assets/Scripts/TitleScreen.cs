using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject navigationHint;
    [SerializeField] private CutsceneController cutsceneController;

    private void Awake()
    {
        titlePanel.SetActive(true);

        if (navigationHint != null)
        {
            navigationHint.SetActive(false);
        }

        if (cutsceneController != null)
        {
            cutsceneController.enabled = false;
        }
    }

    public void StartCutscene()
    {
        titlePanel.SetActive(false);

        if (navigationHint != null)
        {
            navigationHint.SetActive(true);
        }

        if (cutsceneController != null)
        {
            cutsceneController.enabled = true;
        }
    }
}