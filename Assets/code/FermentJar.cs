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
    private bool isReadyToClaim = false;
    private DateTime completeTime;

    private void Start()
    {
        if (jarButton != null)
        {
            jarButton.onClick.RemoveAllListeners();
            jarButton.onClick.AddListener(OnJarButtonClicked);
        }

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
                isReadyToClaim = true;
                if (timerText != null) timerText.text = "READY!";
                if (jarButton != null) jarButton.interactable = true;
            }
            else
            {
                if (timerText != null)
                {
                    if (remaining.TotalHours >= 1)
                    {
                        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)remaining.TotalHours, remaining.Minutes, remaining.Seconds);
                    }
                    else
                    {
                        timerText.text = string.Format("{0:D2}:{1:D2}", remaining.Minutes, remaining.Seconds);
                    }
                }
            }
        }
    }

    // ฟังก์ชันจัดการคลิกของโหลหมัก
    public void OnJarButtonClicked()
    {
        if (isReadyToClaim)
        {
            ClaimReward();
        }
        else if (!isFermenting)
        {
            StartFermenting();
        }
    }

    // เริ่มหมักโหล
    public void StartFermenting()
    {
        completeTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
        PlayerPrefs.SetString(jarID + "_EndTime", completeTime.ToBinary().ToString());
        PlayerPrefs.Save();

        isFermenting = true;
        isReadyToClaim = false;
        if (jarButton != null) jarButton.interactable = false;
    }

    // กดรับของเมื่อหมักเสร็จ
    public void ClaimReward()
    {
        if (GameManager.Instance != null)
        {
            // ได้รับแต้มหมักคูณตาม Rebirth Multiplier
            double finalReward = rewardAmount * GameManager.Instance.CurrentRebirthMultiplier;
            GameManager.Instance.pickles += finalReward;
            GameManager.Instance.UpdateUI();
        }

        PlayerPrefs.DeleteKey(jarID + "_EndTime");
        PlayerPrefs.Save();

        isReadyToClaim = false;
        isFermenting = false;
        if (timerText != null) timerText.text = "START";
        if (jarButton != null) jarButton.interactable = true;
    }

    private void CheckSavedState()
    {
        if (PlayerPrefs.HasKey(jarID + "_EndTime"))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(jarID + "_EndTime"));
            completeTime = DateTime.FromBinary(temp);

            if (DateTime.UtcNow >= completeTime)
            {
                isReadyToClaim = true;
                isFermenting = false;
                if (timerText != null) timerText.text = "READY!";
                if (jarButton != null) jarButton.interactable = true;
            }
            else
            {
                isFermenting = true;
                isReadyToClaim = false;
                if (jarButton != null) jarButton.interactable = false;
            }
        }
        else
        {
            isReadyToClaim = false;
            isFermenting = false;
            if (timerText != null) timerText.text = "START";
            if (jarButton != null) jarButton.interactable = true;
        }
    }
}
