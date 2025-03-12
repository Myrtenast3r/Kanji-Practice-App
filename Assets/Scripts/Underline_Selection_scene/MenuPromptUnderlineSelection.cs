using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPromptUnderlineSelection : MonoBehaviour
{
    [SerializeField]
    private Button saveAndReturnButton;
    [SerializeField]
    private Button yesButton;
    [SerializeField]
    private Button noButton;
    [SerializeField]
    private KanjiSentenceHandler kanjiSentenceHandler;

    private void Start()
    {
        if (kanjiSentenceHandler == null)
        {
            kanjiSentenceHandler = FindAnyObjectByType<KanjiSentenceHandler>();
        }

        saveAndReturnButton.onClick.AddListener(OnReturnClick);
        yesButton.onClick.AddListener(OnYesClick);
        noButton.onClick.AddListener(OnNoClick);

        ActivateSelf(false);
    }
    private void ActivateSelf(bool isOn)
    {
        this.transform.gameObject.SetActive(isOn);
    }

    private void OnReturnClick()
    {
        ActivateSelf(true);
    }
    private void OnNoClick()
    {
        ActivateSelf(false);
    }
    private void OnYesClick()
    {
        kanjiSentenceHandler.SaveProgress();
        ActivateSelf(false);
        SceneManager.LoadScene("MainMenu_Scene");
    }
}
