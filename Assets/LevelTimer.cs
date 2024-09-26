using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI completionTimeText; // Use TextMeshProUGUI for TextMeshPro

    private bool levelCompleted;
    private float startTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;
            float completionTime = Time.time - startTime;
            DisplayLevelCompletionTime(completionTime);
        }
    }

    private void DisplayLevelCompletionTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string formattedTime = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";

        Debug.Log($"Level completed in {formattedTime}");

        // Update the TextMeshProUGUI component with the completion time
        completionTimeText.text = $"Level completed in {formattedTime}";
    }

    private void Start()
    {
        startTime = Time.time;
    }
}



