using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionManager : MonoBehaviour
{
    [SerializeField]
    private Button sceneButton;
    [SerializeField]
    private Button resetDataButton;
    [SerializeField]
    private Transform confirmationPanel;
    [SerializeField]
    private Transform closeAppPanel;

    private string selectedSceneKey = string.Empty;

    private void Start()
    {
        confirmationPanel.gameObject.SetActive(false);
        closeAppPanel.gameObject.SetActive(false);
    }

    public void OnSelectScene(string sceneKey)
    {
        selectedSceneKey = sceneKey;
        bool hasSavedData = SaveDataManager.Instance.LoadProgress(sceneKey).Count > 0;
        resetDataButton.gameObject.SetActive(hasSavedData);
        confirmationPanel.gameObject.SetActive(true);
    }

    public void OnConfirmLoadScene()
    {
        if (!string.IsNullOrEmpty(selectedSceneKey))
        {
            SceneManager.LoadScene(selectedSceneKey);
        }
        else
        {
            Debug.LogError("No scene key has been selected.");
        }
    }

    public void OnConfirmResetData()
    {
        if (SaveDataManager.Instance != null && !string.IsNullOrEmpty(selectedSceneKey))
        {
            SaveDataManager.Instance.ResetProgress(selectedSceneKey);
           // Debug.Log($"After reset, SaveDataManager.Instance.LoadProgress(selectedSceneKey).Count: {SaveDataManager.Instance.LoadProgress(selectedSceneKey).Count}");
            resetDataButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("SaveDataManager instance or selected scene key is not valid.");
        }
    }

    public void OnCancelLoadScene()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Confirmation Panel is not assigned in the Inspector.");
        }
    }

    public void OnCloseAppButtonClicked()
    {
        if (closeAppPanel != null)
        {
            closeAppPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Found no reference to app close panel");
        }
    }

    public void OnConfirmApplicationShutdown()
    {
        ApplicationShutdown();
    }

    public void OnCancelApplicationShutdown()
    {
        closeAppPanel.gameObject.SetActive(false);
    }

    private void ApplicationShutdown()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
