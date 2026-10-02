using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currency")]
    public double pickles = 0;
    public double picklesPerClick = 1;
    public double picklesPerSecond = 0;

    [Header("Double Click / Critical Settings")]
    [Tooltip("โอกาสที่จะได้ผลผลิตดับเบิ้ลจากการกดคลิก (หน่วยเป็น %) เช่น 10 = 10%")]
    [Range(0f, 100f)]
    public float doubleChance = 10f; // โอกาสสุ่มติดดับเบิ้ล 10%
    public double doubleMultiplier = 2.0; // ตัวคูณ 2 เท่า

    [Header("Rebirth System")]
    public int rebirthCount = 0;
    public double baseRebirthCost = 10000; // แต้มขั้นต่ำในการ Rebirth ครั้งแรก
    public double rebirthCostMultiplier = 2.5; // ค่าใช้จ่ายเพิ่มขึ้นในแต่ละรอบ
    public double rebirthBonusPerLevel = 0.5; // โบนัสเพิ่มตัวคูณ +50% (+0.5) ต่อ 1 Rebirth

    [Header("UI Reference")]
    public TextMeshProUGUI pickleText;
    public TextMeshProUGUI rebirthText; // (Optional) เผื่อใช้แสดงผล Rebirth
    public TextMeshProUGUI multiplierText; // (Optional) เผื่อใช้แสดงตัวคูณปัจจุบัน

    // Events สำหรับให้อนิเมชันหรือ UI มารอรับ Callback
    public event Action<bool, double> OnPickleClickedEvent; // (isDouble, amountEarned) ไว้ทำ popup "+2" หรือ text เด้ง
    public event Action<int> OnRebirthTriggered; // (newRebirthCount) ไว้สั่งเล่นท่าอนิเมชัน Rebirth ตอนเสร็จ

    // คำนวณค่า Rebirth ณ ปัจจุบัน
    public double CurrentRebirthCost => baseRebirthCost * Math.Pow(rebirthCostMultiplier, rebirthCount);
    public double CurrentRebirthMultiplier => 1.0 + (rebirthCount * rebirthBonusPerLevel);
    public bool CanRebirth => pickles >= CurrentRebirthCost;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadRebirthData();
    }

    private void Update()
    {
        if (picklesPerSecond > 0)
        {
            pickles += (picklesPerSecond * CurrentRebirthMultiplier) * Time.deltaTime;
            UpdateUI();
        }
    }

    // ฟังก์ชันตอนกดคลิกแตงกวา
    public void OnPickleClicked()
    {
        // 1. สุ่มโอกาสได้ Double Click
        bool isDouble = UnityEngine.Random.Range(0f, 100f) < doubleChance;
        double multiplier = isDouble ? doubleMultiplier : 1.0;

        // คำนวณแต้มที่ได้รับ (คิดผลของ Rebirth Multiplier ด้วย)
        double earned = (picklesPerClick * multiplier) * CurrentRebirthMultiplier;
        pickles += earned;

        // แจ้งเตือน Event (สำหรับระบบแสดง Floating Text / Animation ในอนาคต)
        OnPickleClickedEvent?.Invoke(isDouble, earned);

        UpdateUI();
    }

    // 2. ฟังก์ชันระบบ Rebirth (เรียกใช้เมื่อกดปุ่ม Rebirth ที่จะทำ UI ในอนาคต)
    public bool TriggerRebirth()
    {
        if (!CanRebirth)
        {
            Debug.Log($"[Rebirth] แตงกวาไม่พอ! ต้องการ {FormatNumber(CurrentRebirthCost)} แต่มี {FormatNumber(pickles)}");
            return false;
        }

        // รีเซ็ตแต้มแตงกวา และเพิ่มเลเวล Rebirth
        pickles = 0;
        rebirthCount++;

        SaveRebirthData();

        // ยิง Event ให้ UI / Animation ด้านล่างเริ่มเล่นเอฟเฟกต์
        OnRebirthTriggered?.Invoke(rebirthCount);

        UpdateUI();
        Debug.Log($"[Rebirth] สำเร็จ! ปัจจุบัน Rebirth: {rebirthCount} (ตัวคูณ x{CurrentRebirthMultiplier:F1})");
        return true;
    }

    public void UpdateUI()
    {
        if (pickleText != null)
        {
            pickleText.text = FormatNumber(pickles);
        }

        if (rebirthText != null)
        {
            rebirthText.text = $"Rebirth: {rebirthCount} (Cost: {FormatNumber(CurrentRebirthCost)})";
        }

        if (multiplierText != null)
        {
            multiplierText.text = $"x{CurrentRebirthMultiplier:F1}";
        }
    }

    public string FormatNumber(double num)
    {
        if (num >= 1e12) return (num / 1e12).ToString("F2") + " T";
        if (num >= 1e9) return (num / 1e9).ToString("F2") + " B";
        if (num >= 1e6) return (num / 1e6).ToString("F2") + " M";
        if (num >= 1e3) return (num / 1e3).ToString("F2") + " K";
        return Mathf.FloorToInt((float)num).ToString();
    }

    private void SaveRebirthData()
    {
        PlayerPrefs.SetInt("RebirthCount", rebirthCount);
        PlayerPrefs.Save();
    }

    private void LoadRebirthData()
    {
        rebirthCount = PlayerPrefs.GetInt("RebirthCount", 0);
    }

    // ฟังก์ชันสำหรับทดสอบ Reset ข้อมูล Rebirth (เผื่อใช้เทสใน Editor)
    [ContextMenu("Reset Rebirth Data")]
    public void ResetRebirthData()
    {
        PlayerPrefs.DeleteKey("RebirthCount");
        PlayerPrefs.Save();
        rebirthCount = 0;
        UpdateUI();
    }
}
