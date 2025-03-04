using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgress(string key, HashSet<string> correctAnswers, int totalAttempts)
    {
        string savedData = string.Join(",", correctAnswers.ToArray());
        PlayerPrefs.SetString(key, savedData);
        PlayerPrefs.SetInt(key + "_TotalAttempts", totalAttempts); // Save attempts
        PlayerPrefs.Save();
        Debug.Log($"Saved data for {key}: {savedData}, \n attempts: {totalAttempts}");
    }

    public HashSet<string> LoadProgress(string key)
    {
        string data = PlayerPrefs.GetString(key, "");
        Debug.Log($"Loaded data for {key}: '{data}'");

        // Make sure empty strings do not contribute to the HashSet count
        if (string.IsNullOrEmpty(data))
        {
            return new HashSet<string>(); // Return an empty set properly
        }

        return new HashSet<string>(data.Split(',').Where(s => !string.IsNullOrEmpty(s)));
    }

    public void ResetProgress(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(key + "_TotalAttempts"); // Reset attempts
        PlayerPrefs.Save();
        Debug.Log($"Reset data for {key}");

        // Verify that data is cleared
        string data = PlayerPrefs.GetString(key, "");
        Debug.Log($"Data after reset for {key}: {data}");
    }
}
