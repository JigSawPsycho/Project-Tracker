using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public float weekPadding;
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI asAtTimeText;
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
    public ProgressBarUI projectProgressPrefab;
    public RectTransform reportWeekMarkerPrefab;
    public static Report report;
    private IEnumerator Start()
    {
        if(report == null)
        {
            string teamJson = File.ReadAllText(Application.persistentDataPath + "/report.json");
            report = JsonUtility.FromJson<Report>(teamJson);
        }
        teamNameText.text = "Timeline " + report.team.name;

        GenerateMonths(report, out Month startMonth, out Month endMonth);
        GenerateWeeks(startMonth, endMonth, out int weekCount);

        Month reportMonth = new Month(report.reportMonth, report.reportYear);
        asAtTimeText.text = $"As at {report.reportWeek} {reportMonth} {report.reportYear}";

        for (int i = 0; i < report.team.projects.Length; i++)
        {
            yield return SetupProject(report, reportMonth, startMonth, endMonth, weekCount, i);
        }

        // TODO: Add confidential marker
    }

    private int GetWeekIndexForMonthAndIndex(int weekDate, Month targetMonth)
    {
        return targetMonth.GetMondays().ToList().FindIndex(x => x == weekDate);
    }

    private int GetWeekIndexAcrossTwoMonths(int weekDate, Month targetMonth, Month laterMonth)
    {
        int index = targetMonth.GetMondays().ToList().FindIndex(x => x == weekDate);
        if(targetMonth.month == laterMonth.month && targetMonth.year == laterMonth.year) index += laterMonth.GetMondays().Length;
        return index;
    }

    private IEnumerator SetupProject(Report report, Month reportMonth, Month startMonth, Month endMonth, int weekCount, int i)
    {
        Project project = report.team.projects[i];
        TextContainer projectTextContainer = Instantiate(projectPrefab, projectContainer);
        projectTextContainer.texts[0].text = (i + 1).ToString();
        Month projStartMonth = project.startMonth == startMonth.month ? startMonth : endMonth;
        Month projEndMonth = project.endMonth == startMonth.month ? startMonth : endMonth;
        projectTextContainer.texts[1].text = $"<b>{project.name}</b>\nDue: {project.endWeek} {projEndMonth}";
        GameObject row = Instantiate(rowPrefab, rowContainer);
        List<RectTransform> cellRectTransforms = new List<RectTransform>();
        for (int j = 0; j < weekCount; j++)
        {
            cellRectTransforms.Add(Instantiate(cellPrefab, row.transform).GetComponent<RectTransform>());
        }
        yield return null;
        if(i == 0)
        {
            int index = GetWeekIndexAcrossTwoMonths(report.reportWeek, reportMonth, endMonth);
            RectTransform reportWeekMarker = Instantiate(reportWeekMarkerPrefab, row.transform);
            reportWeekMarker.position = new Vector3(row.transform.GetChild(index).position.x - weekPadding/2f, reportWeekMarker.position.y, 0);
        }
        InstantiateProjectProgressBars(startMonth, project, projStartMonth, projEndMonth, row);
        Instantiate(projectNoteBoxPrefab, projectNoteBoxContainer).texts[0].text = string.Join("\n", project.notes);
    }

    private void InstantiateProjectProgressBars(Month startMonth, Project project, Month projStartMonth, Month projEndMonth, GameObject row)
    {
        List<int> startMonthMondays = projStartMonth.GetMondays().ToList();
        List<int> endMonthMondays = projEndMonth.GetMondays().ToList();
        int startWeekIndex = startMonthMondays.FindIndex(x => x == project.startWeek);
        int endWeekIndex = endMonthMondays.FindIndex(x => x == project.endWeek);
        ProgressBarUI projectProgressBarUI = Instantiate(projectProgressPrefab, row.transform);
        SetupProgressBarTransform(startMonth, projStartMonth, projEndMonth, row, startWeekIndex, endWeekIndex, projectProgressBarUI.rectTransform);
        projectProgressBarUI.progressSlider.value = (float)project.progress/100f;
        projectProgressBarUI.progressPercentageText.text = $"{((int)project.progress).ToString()}%";
    }

    private void SetupProgressBarTransform(Month startMonth, Month projStartMonth, Month projEndMonth, GameObject row, int startWeekIndex, int endWeekIndex, RectTransform projectProgressRect)
    {
        RectTransform targetStartPos;
        int startWeekRowIndex;
        int endWeekRowIndex;
        if (projStartMonth == startMonth)
        {
            startWeekRowIndex = startWeekIndex;
            targetStartPos = row.transform.GetChild(startWeekRowIndex).transform as RectTransform;
        }
        else
        {
            startWeekRowIndex = startMonth.GetMondays().Length + startWeekIndex;
            targetStartPos = row.transform.GetChild(startWeekRowIndex).transform as RectTransform;
        }

        if (projEndMonth == startMonth)
        {
            endWeekRowIndex = endWeekIndex;
        }
        else
        {
            endWeekRowIndex = startMonth.GetMondays().Length + endWeekIndex;
        }
        projectProgressRect.localPosition = targetStartPos.localPosition;

        int weeksToSpan = endWeekRowIndex - startWeekRowIndex;
        projectProgressRect.sizeDelta = new Vector2(CalculateProgressBarSizeDeltaX(weeksToSpan, (row.transform.GetChild(0).transform as RectTransform).sizeDelta.x, weekPadding), projectProgressRect.sizeDelta.y);
    }

    private static float CalculateProgressBarSizeDeltaX(int weeksToSpan, float oneWeekXSize, float weekPadding)
    {
        return oneWeekXSize * (weeksToSpan + 1) + (weekPadding * (weeksToSpan - 1));
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
