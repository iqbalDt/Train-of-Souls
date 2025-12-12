using UnityEngine;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager Instance;

    public TMP_Text dayText;
    public int currentDay = 1;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void NextDay()
    {
        if (currentDay < 2)   // untuk sekarang hanya 2 hari
            currentDay++;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (dayText != null)
            dayText.text = "Day " + currentDay;
    }
}
