using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currency")]
    public double pickles = 0;
    public double picklesPerClick = 1;
    public double picklesPerSecond = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI pickleText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (picklesPerSecond > 0)
        {
            pickles += picklesPerSecond * Time.deltaTime;
            UpdateUI();
        }
    }
    
    public void OnPickleClicked()
    {
        pickles += picklesPerClick;
        UpdateUI();
    }

    public void UpdateUI()
    {
        pickleText.text = FormatNumber(pickles);
    }

    private string FormatNumber(double num)
    {
        if (num >= 1e12) return (num / 1e12).ToString("F2") + " T";
        if (num >= 1e9) return (num / 1e9).ToString("F2") + " B";
        if (num >= 1e6) return (num / 1e6).ToString("F2") + " M";
        if (num >= 1e3) return (num / 1e3).ToString("F2") + " K";
        return Mathf.FloorToInt((float)num).ToString();
    }
}