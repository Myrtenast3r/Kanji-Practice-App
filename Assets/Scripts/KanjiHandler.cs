using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class KanjiHandler : MonoBehaviour
{
    public TextMeshProUGUI mainKanjiText;
    public TextMeshProUGUI mainKanjiMeaningText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI numberOfKanjiText;
    public Button[] optionButtons;
    public Button reloadButton;

    private List<KanjiData> kanjiDatasList;
    private HashSet<string> correctAnswers;

    private void Start()
    {
        // Read the csv file
        string fileName = "kanjilist.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        kanjiDatasList = ReadKanjiData(filePath);
        //correctAnswers = new HashSet<string>();
        LoadProgress();
        DisplayRandomKanji();

        if (kanjiDatasList != null)
        {
            //for (int i = 0; i < kanjiDatasList.Count; i++)
            //{
            //    Debug.Log(kanjiDatasList[i].Kanji);
            //    Debug.Log(kanjiDatasList[i].Reading);
            //    Debug.Log(kanjiDatasList[i].Meaning);
            //}
            //Debug.Log($"Loaded csv data successfully");
            //Debug.Log($"Number of kanjis in the list: {kanjiDatasList.Count}");
        }
        else
        {
            Debug.Log($"Cannot get kanji data");
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        reloadButton.transform.gameObject.SetActive(false);

    }

    List<KanjiData> ReadKanjiData(string filePath)
    {
        List<KanjiData> data = new List<KanjiData>();
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++) //Start from 1 to skip the header line
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length == 4)
            {
                // Skip the ID column (parts[0])
                data.Add(new KanjiData(parts[1], parts[2], parts[3]));
            }
        }
        return data;
    }

    private void DisplayRandomKanji()
    {
        ResetUI();

        // Keep track of correct answers
        List<KanjiData> remainingKanji = kanjiDatasList.Where(k => !correctAnswers.Contains(k.Kanji)).ToList();

        //Debug.Log($"Number of remaining kanji: {remainingKanji.Count}");
        numberOfKanjiText.SetText(remainingKanji.Count.ToString());

        if (remainingKanji.Count == 0)
        {
            answerText.SetText($"All kanjis answered correctly!");
            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].interactable = false; // Deactivate option buttons
            }
            reloadButton.interactable = false;
            return;
        }

        // Select the main kanji
        KanjiData mainKanji = remainingKanji[Random.Range(0, remainingKanji.Count)];
        mainKanjiText.SetText(mainKanji.Kanji);
        mainKanjiMeaningText.SetText(mainKanji.Meaning);

        // Get the kanjis with the same reading
        List<KanjiData> sameReading = remainingKanji.Where(k => k.Reading == mainKanji.Reading && k.Kanji != mainKanji.Kanji).ToList();

        // Get kanjis with different readings
        List<KanjiData> differentReading = remainingKanji.Where(k => k.Reading != mainKanji.Reading).ToList();

        // Randomly select three kanjis with different readings
        List<KanjiData> options = differentReading.OrderBy(x => Random.Range(0, differentReading.Count)).Take(3).ToList();

        // Ensure there is at least one kanji in the options with the same reading as the main kanji
        if (sameReading.Count > 0)
        {
            KanjiData correctOption = sameReading[Random.Range(0, sameReading.Count)]; // Randomize selection
            options.Insert(Random.Range(0, options.Count + 1), correctOption); // Insert at random position
        }
        else
        {
            Debug.LogWarning($"No kanji with the same reading found in the list. ");
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i].Kanji;
            int index = i; // Capture index for use in the lambda
            optionButtons[i].onClick.AddListener(() => CheckAnswer(options[index], mainKanji, optionButtons[index].transform.GetChild(1)));
            optionButtons[i].transform.GetChild(1).TryGetComponent(out TextMeshProUGUI text);
            text.SetText(options[index].Meaning);
        }
    }

    void CheckAnswer(KanjiData selected, KanjiData main, Transform meaningText)
    {
        if (selected.Reading == main.Reading)
        {
            answerText.text = $"Correct! Reading: {selected.Reading}";
            meaningText.gameObject.SetActive(true);
            mainKanjiMeaningText.gameObject.SetActive(true);
            correctAnswers.Add(main.Kanji);
        }

        else
        {
            answerText.text = $"Incorrect. Try again";
        }

        // Deactivate the option buttons after answering
        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].interactable = false;
        }

        reloadButton.onClick.RemoveAllListeners();
        reloadButton.onClick.AddListener(() =>  DisplayRandomKanji());
        reloadButton.transform.gameObject.SetActive(true);
    }

    void ResetUI()
    {
        answerText.text = $"Select the kanji with the same reading. ";

        // Clear previous options
        foreach (var button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            button.interactable = true;
            button.transform.GetChild(1).TryGetComponent(out TextMeshProUGUI meaningText);
            meaningText.gameObject.SetActive(false);
        }

        reloadButton.transform.gameObject.SetActive(false);
        mainKanjiMeaningText.transform.gameObject.SetActive(false);
    }

    public void SaveProgress()
    {
        Debug.Log($"KanjiHandler SaveProgress");
        string sceneKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SaveDataManager.Instance.SaveProgress(sceneKey, correctAnswers);
    }

    public void LoadProgress()
    {
        Debug.Log($"KanjiHandler LoadProgress");
        string sceneKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        correctAnswers = SaveDataManager.Instance.LoadProgress(sceneKey);
    }
}

public class KanjiData
{
    public string Kanji;
    public string Reading;
    public string Meaning;

    public KanjiData(string kanji, string reading, string meaning) 
    { 
        Kanji = kanji;
        Reading = reading;
        Meaning = meaning;
    }
}
