using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class KanjiSentenceHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sentenceText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Button reloadButton;
    [SerializeField] private TextMeshProUGUI numberOfRemainingSentences;

    [SerializeField]
    private TextAsset sceneCsvFile; // assign in the editor

    public TextMeshProUGUI totalAttempsText; // UI element for showing total attempts
    public TextMeshProUGUI correctAnswerRateText; // UI element for showing the rate of correct answers

    private int totalAttempts;
    private HashSet<string> correctAnswers;

    private List<KanjiSentenceData> sentenceDataList = new List<KanjiSentenceData>();

    private void Start()
    {
        LoadCSVData();
        LoadProgress();
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
            //for (int i = 0; i < sentenceDataList.Count; i++)
            //{
            //    Debug.Log($"{sentenceDataList[i]} sentence: {sentenceDataList[i].Sentence}");
            //    Debug.Log($"{sentenceDataList[i].Options}");
            //    Debug.Log($"{sentenceDataList[i].Meanings}");
            //    Debug.Log($"{sentenceDataList[i]} correct answer: {sentenceDataList[i].CorrectAnswer}");
            //}

            Debug.Log($"Number of sentences in the list: {sentenceDataList.Count}");
        }
    }

    private void DisplayRandomSentence()
    {
        ResetUI();

        // Keep track of correct answers
        List<KanjiSentenceData> remainingSentences = sentenceDataList.Where(k => !correctAnswers.Contains(k.CorrectAnswer)).ToList();

        numberOfRemainingSentences.SetText(remainingSentences.Count.ToString());

        if (remainingSentences.Count == 0)
        {
            feedbackText.SetText($"All questions anwered correctly!");
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].interactable = false;
            }
            reloadButton.interactable = false;
            return;
        }

        // Select the main sentence
        KanjiSentenceData selectedSentenceData = remainingSentences[Random.Range(0, remainingSentences.Count)];
        Debug.Log($"selected sentence data: {selectedSentenceData.Sentence}");
        sentenceText.SetText(selectedSentenceData.Sentence);

        // Create a list of answer-meaning-index tuples
        List<(string option, string meaning, int originalIndex)> answerPairs = new List<(string, string, int)>();
        for (int i = 0; i < selectedSentenceData.Options.Length; i++)
        {
            answerPairs.Add((selectedSentenceData.Options[i], selectedSentenceData.Meanings[i], i));
        }

        // Shuffle the answer pairs before assigning to buttons
        answerPairs = answerPairs.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int shuffledIndex = i; // Capture shuffled index for lambda
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answerPairs[i].option;
            answerButtons[i].onClick.AddListener(() =>
            CheckAnswer(selectedSentenceData, answerPairs[shuffledIndex].originalIndex, answerButtons[shuffledIndex].transform.GetChild(1)));
        }


        // old
        //for (int i = 0; i < answerButtons.Length; i++)
        //{
        //    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = selectedSentenceData.Options[i];
        //    int index = i; // Capture the index
        //    answerButtons[i].onClick.AddListener(() => CheckAnswer(selectedSentenceData, index, answerButtons[index].transform.GetChild(1)));
        //}
    }

    //new
    private void CheckAnswer(KanjiSentenceData selected, int index, Transform background)
    {
        totalAttempts++;

        if (selected.Options[index] == selected.CorrectAnswer)
        {
            feedbackText.SetText($"Correct! Meaning: {selected.Meanings[index]}");
            correctAnswers.Add(selected.Options[index]);
            background.gameObject.SetActive(true);
        }

        else
        {
            feedbackText.SetText($"Incorrect.");
        }

        UpdateStats(); // Update stats after answering

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
            button.transform.GetChild(1).gameObject.SetActive(false);
        }

        reloadButton.transform.gameObject.SetActive(false);
    }

    private void UpdateStats()
    {
        totalAttempsText.SetText(totalAttempts.ToString());
        float correctRate = totalAttempts > 0 ? ((float)correctAnswers.Count / totalAttempts) * 100 : 0f; // Calculate the correct rate
        correctAnswerRateText.SetText($"{correctRate:F2}%");
    }

    public void SaveProgress()
    {
        Debug.Log($"KanjiSentenceHandler SaveProgress");
        string sceneKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SaveDataManager.Instance.SaveProgress(sceneKey, correctAnswers, totalAttempts);
    }

    public void LoadProgress()
    {
        Debug.Log($"KanjiHandler LoadProgress");
        string sceneKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        correctAnswers = SaveDataManager.Instance.LoadProgress(sceneKey);
        totalAttempts = PlayerPrefs.GetInt(sceneKey + "_TotalAttempts", 0);
        UpdateStats();
    }
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
