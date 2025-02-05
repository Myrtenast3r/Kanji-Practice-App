using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelectionManager : MonoBehaviour
{

    [SerializeField]
    private List<Button> sceneButtons = new List<Button>();

    private void Start()
    {
        for (int i = 0; i < sceneButtons.Count; i++)
        {
            int buttonIndex = i;
            int sceneIndex = buttonIndex + 1;

            sceneButtons[buttonIndex].onClick.AddListener(() => LoadScene(sceneIndex));
            Debug.Log($"Add listener for button {sceneButtons[buttonIndex]} to load scene at index {sceneIndex}");
        }
    }

    private void LoadScene(int sceneIndex)
    {
        Debug.Log($"Loading scene {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }
}
