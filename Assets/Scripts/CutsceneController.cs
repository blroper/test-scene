using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("Leave empty to use the Camera on this GameObject.")]
    [SerializeField] private Camera targetCamera;

    [Header("Sequence")]
    [Tooltip("Add empty objects as waypoints in the exact order they should be visited.")]
    [SerializeField] private Transform[] views;

    [Tooltip("Higher values move and rotate the camera more quickly.")]
    [SerializeField] private float transitionSpeed = 3f;

    [SerializeField] private bool wrapAround;

    [Header("Scene Transition")]
    [SerializeField] private bool loadNextSceneAfterFinalView = true;

    [Tooltip("Leave empty to load the next scene in Build Settings.")]
    [SerializeField] private string nextSceneName;

    private int currentViewIndex;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            Debug.LogError("CutsceneController could not find a Camera.", this);
            enabled = false;
            return;
        }

        currentViewIndex = 0;

        if (views != null && views.Length > 0 && views[0] != null)
        {
            targetPosition = views[0].position;
            targetRotation = views[0].rotation;
        }
        else
        {
            targetPosition = targetCamera.transform.position;
            targetRotation = targetCamera.transform.rotation;
        }

        targetCamera.transform.position = targetPosition;
        targetCamera.transform.rotation = targetRotation;
    }

    private void Update()
    {
        if (views == null || views.Length == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToNextView();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToPreviousView();
        }

        float interpolation = 1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);
        targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetPosition, interpolation);
        targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, targetRotation, interpolation);
    }

    public void MoveToNextView()
    {
        if (views == null || views.Length == 0)
        {
            return;
        }

        if (currentViewIndex < views.Length - 1)
        {
            currentViewIndex++;
            SetView(currentViewIndex);
        }
        else if (loadNextSceneAfterFinalView)
        {
            LoadNextScene();
        }
        else if (wrapAround)
        {
            currentViewIndex = 0;
            SetView(currentViewIndex);
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("There is no next scene after this scene in Build Settings.", this);
        }
    }

    public void MoveToPreviousView()
    {
        if (views == null || views.Length == 0)
        {
            return;
        }

        if (currentViewIndex > 0)
        {
            currentViewIndex--;
        }
        else if (wrapAround)
        {
            currentViewIndex = views.Length - 1;
        }

        SetView(currentViewIndex);
    }

    private void SetView(int index)
    {
        if (views[index] == null)
        {
            Debug.LogWarning($"View {index} is empty. Assign a waypoint Transform in the views list.", this);
            return;
        }

        targetPosition = views[index].position;
        targetRotation = views[index].rotation;
    }

    public void MoveToView(int index)
    {
        if (views == null || views.Length == 0)
        {
            return;
        }

        currentViewIndex = Mathf.Clamp(index, 0, views.Length - 1);
        SetView(currentViewIndex);
    }

    public int CurrentViewIndex => currentViewIndex;
}
