using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FermentJar : MonoBehaviour
{
    [Header("Jar Settings")]
    public float durationInSeconds = 900f; // เช่น 15 นาที = 900 วินาที
    public double rewardAmount = 500;
    public string jarID = "Jar_15m";

    [Header("UI")]
    public Button jarButton;
    public TextMeshProUGUI timerText;

    private bool isFermenting = false;
    private DateTime completeTime;

    private void Start()
    {
        CheckSavedState();
    }

    private void Update()
    {
        if (isFermenting)
        {
            TimeSpan remaining = completeTime - DateTime.UtcNow;

            if (remaining.TotalSeconds <= 0)
            {
                // หมักเสร็จแล้ว
                isFermenting = false;
                timerText.text = "READY!";
                jarButton.interactable = true;
            }
            else
            {
                timerText.text = string.Format("{0:D2}:{1:D2}", remaining.Minutes, remaining.Seconds);
            }
        }
    }

    // เริ่มหมักโหล
    public void StartFermenting()
    {
        completeTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
        PlayerPrefs.SetString(jarID + "_EndTime", completeTime.ToBinary().ToString());
        PlayerPrefs.Save();

        isFermenting = true;
        jarButton.interactable = false;
    }

    // กดรับของเมื่อหมักเสร็จ
    public void ClaimReward()
    {
        GameManager.Instance.pickles += rewardAmount;
        GameManager.Instance.UpdateUI();

        PlayerPrefs.DeleteKey(jarID + "_EndTime");
        timerText.text = "START";
        jarButton.onClick.RemoveAllListeners();
        jarButton.onClick.AddListener(StartFermenting);
    }

    private void CheckSavedState()
    {
        if (PlayerPrefs.HasKey(jarID + "_EndTime"))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(jarID + "_EndTime"));
            completeTime = DateTime.FromBinary(temp);

            if (DateTime.UtcNow >= completeTime)
            {
                timerText.text = "READY!";
                jarButton.onClick.AddListener(ClaimReward);
            }
            else
            {
                isFermenting = true;
                jarButton.interactable = false;
            }
        }
        else
        {
            timerText.text = "START";
            jarButton.onClick.AddListener(StartFermenting);
        }
    }
}