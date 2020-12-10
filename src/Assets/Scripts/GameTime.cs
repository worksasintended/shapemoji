using System;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    private static Timer timer;

    [SerializeField] private int finishTime;
    [SerializeField] private GameObject textTimeCountdown;
    [SerializeField] private GameObject textTimeRemaining1;
    [SerializeField] private GameObject textTimeRemaining2;

    /**
     * current time in unix format, will be updated by timer
     */
    private int currentTime;


    /**
     * game start time in unix format
     */
    private int startTime;

    private Text timeRemainingText2;
    private Text timeRemainingText1;
    private Text timeCountdownText;


    /**
     * Called in the beginning of the game, once
     */
    private void Start()
    {
        timeCountdownText = textTimeCountdown.GetComponent<Text>();
        timeRemainingText1 = textTimeRemaining1.GetComponent<Text>();
        timeRemainingText2 = textTimeRemaining2.GetComponent<Text>();
        timeCountdownText.text = "";
        startTime = (int) DateTimeOffset.Now.ToUnixTimeSeconds();
        currentTime = (int) DateTimeOffset.Now.ToUnixTimeSeconds();

        timer = new Timer(1000);
        timer.Elapsed += OnTimerUpdate;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    /**
     * Update is called once per frame.
     * Time display is refreshed.
     * 
     */
    private void Update()
    {
        TimeDisplay();
        CountdownDisplay();
        CheckGameOver();
    }

    /**
     * Update elapsed time
     */
    private void OnTimerUpdate(object source, ElapsedEventArgs e)
    {
        currentTime = (int) DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    /**
     * find out remaining time until game end
     * @return remaining time
     */
    private int GetRemainingTime()
    {
        return finishTime - currentTime + startTime;
    }

    /**
     * remaining time display
     */
    private void TimeDisplay()
    {
        int time = GetRemainingTime();
        int seconds = time % 60;
        int minutes = time / 60;
        string toDisplay = $"{minutes:D2}:{seconds:D2}";
        timeRemainingText2.text = toDisplay;
        timeRemainingText1.text = toDisplay;
    }

    /**
     * Countdown display for remaining 5 seconds
     */
    private void CountdownDisplay()
    {
        if (GetRemainingTime() <= 5)
        {
            timeCountdownText.text = Convert.ToString(GetRemainingTime());
        }
    }

    /**
     * check, if time is over and load end scene
     */
    private void CheckGameOver()
    {
        if (GetRemainingTime() <= 0)
        {
            timer.Enabled = false;
            GameSceneManager.Instance.LoadEndScene();
        }
    }

    /**
     * SetFinishTime sets the finish time. Used in test
     *
     * @param time
     */
    public void SetFinishTime(int time)
    {
        finishTime = time;
    }
}