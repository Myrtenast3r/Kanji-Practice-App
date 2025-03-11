using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class KanjiSentenceHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sentenceText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI feedbackText;
    //[SerializeField] private TextMeshProUGUI attemptsText;
    //[SerializeField] private TextMeshProUGUI correctRateText;
    [SerializeField] private Button reloadButton;

    [SerializeField]
    private TextAsset sceneCsvFile; // assign in the editor

    private List<KanjiSentenceData> sentenceDataList = new List<KanjiSentenceData>();
    private int totalAttempts;
    private int correctAnswers;

    private void Start()
    {
        //LoadProgress();
        LoadCSVData();
        DisplayRandomSentence();
    }

    private void LoadCSVData()
    {
        string[] lines = sceneCsvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) //Start from 1 to skip the header line
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length == 10)
            {
                sentenceDataList.Add(new KanjiSentenceData(parts));
            }
        }

        if (sentenceDataList != null)
        {
            for (int i = 0; i < sentenceDataList.Count; i++)
            {
                Debug.Log($"{sentenceDataList[i]} sentence: {sentenceDataList[i].Sentence}");
                Debug.Log($"{sentenceDataList[i].Options}");
                Debug.Log($"{sentenceDataList[i].Meanings}");
                Debug.Log($"{sentenceDataList[i]} correct answer: {sentenceDataList[i].CorrectAnswer}");
            }

            Debug.Log($"Number of sentences in the list: {sentenceDataList.Count}");
        }
    }

    private void DisplayRandomSentence()
    {
        ResetUI();

        KanjiSentenceData question = sentenceDataList[Random.Range(0, sentenceDataList.Count)];
        sentenceText.SetText(question.Sentence);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.Options[i];
            int index = i; // Capture the index
            //answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(question.Options[index], question.CorrectAnswer, question.Meanings[index], answerButtons[index].transform.GetChild(1)));
            //answerButtons[i].transform.GetChild(1).transform.GetChild(0).TryGetComponent(out TextMeshProUGUI text);
            //text.SetText(question.Meanings[index]);
            Debug.Log($"Set the text for button {i} to {question.Meanings[index]}");
        }
    }

    private void CheckAnswer(string selectedAnswer, string correctAnswer, string meaning, Transform background)
    {
        totalAttempts++;

        if (selectedAnswer == correctAnswer)
        {
            correctAnswers++;
            feedbackText.SetText($"Correct! Meaning: {meaning}");
            background.gameObject.SetActive(true);
        }
        else
        {
            feedbackText.SetText($"Incorrect.");
        }

        //UpdateUI();

        // Deactivate the option buttons after answering
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
        }
        reloadButton.onClick.RemoveAllListeners();
        reloadButton.onClick.AddListener(() => DisplayRandomSentence());
        reloadButton.transform.gameObject.SetActive(true);
    }

    private void ResetUI()
    {
        feedbackText.SetText($"Select the correct kanji.");

        foreach (var button in answerButtons)
        {
            button.onClick.RemoveAllListeners();
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            button.interactable = true;
            //button.transform.GetChild(1).TryGetComponent(out TextMeshProUGUI meaningText);
            button.transform.GetChild(1).gameObject.SetActive(false);
            //meaningText.gameObject.SetActive(false);
        }

        reloadButton.transform.gameObject.SetActive(false);
    }

    //private void UpdateUI()
    //{
    //    attemptsText.text = $"Total Attempts: {totalAttempts}";
    //    float correctRate = totalAttempts > 0 ? ((float)correctAnswers / totalAttempts) * 100f : 0f;
    //    correctRateText.text = $"Correct Rate: {correctRate:F2}%";
    //}

    //private void SaveProgress()
    //{
    //    SaveDataManager.Instance.SaveSentenceProgress("KanjiSentenceProgress", totalAttempts, correctAnswers);
    //}

    //private void LoadProgress()
    //{
    //    (totalAttempts, correctAnswers) = SaveDataManager.Instance.LoadSentenceProgress("KanjiSentenceProgress");
    //    UpdateUI();
    //}
}


[System.Serializable]
public class KanjiSentenceData
{
    public string Sentence;
    public string[] Options = new string[4];
    public string CorrectAnswer;
    public string[] Meanings = new string[4];
    // reading??

    public KanjiSentenceData(string[] parts)
    {
        Sentence = parts[0];
        Options[0] = parts[1];
        Options[1] = parts[2];
        Options[2] = parts[3];
        Options[3] = parts[4];
        CorrectAnswer = parts[5];
        Meanings[0] = parts[6];
        Meanings[1] = parts[7];
        Meanings[2] = parts[8];
        Meanings[3] = parts[9];
    }
}
