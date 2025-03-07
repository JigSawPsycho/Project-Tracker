using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TextMeshProUGUI teamNameText;
    public TextContainer weekPrefab;
    public Transform weekContainer;
    public GameObject rowPrefab;
    public Transform rowContainer;
    public GameObject cellPrefab;
    public Transform monthContainer;
    public MonthUI monthPrefab;
    public TextContainer projectPrefab;
    public Transform projectContainer;
    public TextContainer projectNoteBoxPrefab;
    public Transform projectNoteBoxContainer;
    public TextContainer projectProgressPrefab;
    void Start()
    {
        string teamJson = File.ReadAllText(Application.persistentDataPath + "/report.json");
        Report report = JsonUtility.FromJson<Report>(teamJson);
        teamNameText.text = "Timeline " + report.team.name;
        // TODO: Update 'As at' date

        GenerateMonths(report, out Month startMonth, out Month endMonth);
        GenerateWeeks(startMonth, endMonth, out int weekCount);

        for (int i = 0; i < report.team.projects.Length; i++)
        {
            Project project = report.team.projects[i];
            TextContainer projectTextContainer = Instantiate(projectPrefab, projectContainer);
            projectTextContainer.texts[0].text = i.ToString();
            Month projMonth = project.endMonth == startMonth.month ? startMonth : endMonth;
            projectTextContainer.texts[1].text = $"<b>{project.name}</b>\nDue: {project.endWeek} {projMonth}";
            GameObject row = Instantiate(rowPrefab, rowContainer);
            for (int j = 0; j < weekCount; j++) Instantiate(cellPrefab, row.transform);
            Instantiate(projectNoteBoxPrefab, projectNoteBoxContainer).texts[0].text = string.Join("\n", project.notes);
            // TODO: Spawning in project progress bar
        }

        // TODO: Add confidential marker
    }

    private void GenerateWeeks(Month startMonth, Month endMonth, out int weekCount)
    {
        int[] startMonthMondays = startMonth.GetMondays();
        int[] endMonthMondays = endMonth.GetMondays();

        weekCount = startMonthMondays.Length + endMonthMondays.Length;
        foreach (int day in startMonthMondays)
        {
            Instantiate(weekPrefab, weekContainer).texts[0].text = day.ToString();
        }

        foreach (int day in endMonthMondays)
        {
            Instantiate(weekPrefab, weekContainer).texts[0].text = day.ToString();
        }
    }

    private void GenerateMonths(Report report, out Month startMonth, out Month endMonth)
    {
        startMonth = new Month(report.startingMonth, report.startingYear);
        endMonth = new Month(report.endingMonth, report.endingYear);

        MonthUI startMonthUI = Instantiate(monthPrefab, monthContainer);
        MonthUI endMonthUI = Instantiate(monthPrefab, monthContainer);

        Month dominantMonth = new MonthSplit(startMonth, endMonth).GetDominantMonth();

        float offsetTime = dominantMonth != null ? dominantMonth.GetMondays().Length == 4 ? MonthUI.PREFERRED_WIDTH_4_WEEKS : MonthUI.PREFERRED_WIDTH_5_WEEKS : -1;

        if (dominantMonth == startMonth) startMonthUI.layoutElement.minWidth = offsetTime;
        else if (dominantMonth == endMonth) endMonthUI.layoutElement.minWidth = offsetTime;

        startMonthUI.textContainer.texts[0].text = startMonth.ToString();
        endMonthUI.textContainer.texts[0].text = endMonth.ToString();
    }
}
