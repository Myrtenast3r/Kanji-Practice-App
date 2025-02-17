using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionManager : MonoBehaviour
{

    //[SerializeField]
    //private List<Button> sceneButtons = new List<Button>();

    [SerializeField]
    private Button scene1Button;
    [SerializeField]
    private Button scene2Button;
    [SerializeField]
    private Button scene3Button;
    [SerializeField]
    private Button scene4Button;
    [SerializeField]
    private Button scene5Button;
    [SerializeField]
    private Button scene6Button;
    [SerializeField]
    private Button scene7Button;

    private void Start()
    {
        //for (int i = 0; i < sceneButtons.Count; i++)
        //{
        //    int buttonIndex = i;
        //    int sceneIndex = buttonIndex + 1;

        //    sceneButtons[buttonIndex].onClick.AddListener(() => LoadScene(sceneIndex));
        //    Debug.Log($"Add listener for button {sceneButtons[buttonIndex]} to load scene at index {sceneIndex}");
        //}

        scene1Button.onClick.AddListener(() => LoadScene("")); // Add scene name
        scene2Button.onClick.AddListener(() => LoadScene("Homophones_Scene")); // Homophones scene
        scene3Button.onClick.AddListener(() => LoadScene(""));  // Add scene name
        scene4Button.onClick.AddListener(() => LoadScene(""));  // Add scene name
        scene5Button.onClick.AddListener(() => LoadScene(""));  // Add scene name
        scene6Button.onClick.AddListener(() => LoadScene(""));  // Add scene name
        scene7Button.onClick.AddListener(() => LoadScene(""));  // Add scene name
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"Scene name is null or empty. No scene to load. ");
            return;
        }
        else
        {
            Debug.Log($"Loading scene {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }

    //private void LoadScene(int sceneIndex)
    //{
    //    Debug.Log($"Loading scene {sceneIndex}");
    //    SceneManager.LoadScene(sceneIndex);
    //}
}
