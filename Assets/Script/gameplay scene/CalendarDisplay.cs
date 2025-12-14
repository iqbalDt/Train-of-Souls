using UnityEngine;
using TMPro;
using System;

public class CalendarDisplay : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dateText;

    [Header("Format")]
    [Tooltip("Gunakan nama bulan (December) atau angka (12)")]
    public bool useMonthName = true;

    [Tooltip("Gunakan bahasa Indonesia (Desember)")]
    public bool useIndonesian = true;

    void Start()
    {
        UpdateDate();
    }

    void UpdateDate()
    {
        DateTime now = DateTime.Now;

        int day = now.Day;
        int month = now.Month;

        string monthText;

        if (useMonthName)
        {
            if (useIndonesian)
            {
                monthText = GetIndonesianMonth(month);
            }
            else
            {
                monthText = now.ToString("MMMM");
            }

            dateText.text = $"{day} {monthText}";
        }
        else
        {
            dateText.text = $"{day:D2}/{month:D2}";
        }
    }

    string GetIndonesianMonth(int month)
    {
        string[] months =
        {
            "JAN",
            "FEB",
            "MAR",
            "APR",
            "MEI",
            "JUN",
            "JUL",
            "AGS",
            "SEP",
            "OKT",
            "NOV",
            "DES"
        };

        return months[Mathf.Clamp(month - 1, 0, 11)];
    }
}
