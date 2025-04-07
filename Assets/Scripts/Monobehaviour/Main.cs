using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codice.Client.BaseCommands;
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
    public RectTransform tableContainer;
    public static Report report;
    private IEnumerator Start()
    {
        yield return GenerateReportUI();
    }

    private IEnumerator GenerateReportUI()
    {
        if(report == null)
        {
            report = new Report()
            {
                reportWeek = 1,
                reportMonth = new Month(5, 2023),
                months = new List<Month>()
                {
                    new Month(5, 2023),
                    new Month(6, 2023),
                    new Month(7, 2023)
                },
                team = new Team()
                {
                    name = "Test Team",
                    projects = new Project[]
                    {
                        new Project()
                        {
                            name = "Test Project",
                            startWeek = 1,
                            startMonth = new Month(5, 2023),
                            endWeek = 1,
                            endMonth = new Month(5, 2023),
                            progress = 50,
                            status = ProjectStatus.OnTrack,
                            notes = new string[]
                            {
                                "Test note"
                            }
                        }
                    }
                }
            };
        }
        teamNameText.text = "Timeline " + report.team.name;

        GenerateMonths(report);
        GenerateWeeks(report.months, out int weekCount);

        int date = report.reportMonth.GetFollowingFridayFromMonday(report.reportWeek, out Month fridayMonth);
        asAtTimeText.text = $"As at {date} {fridayMonth} {fridayMonth.year}";

        for (int i = 0; i < report.team.projects.Length; i++)
        {
            yield return SetupProject(report, report.reportMonth, report.months, weekCount, i);
        }
    }

    private int GetWeekIndexForMonthAndIndex(int weekDate, Month targetMonth)
    {
        return targetMonth.GetMondays().ToList().FindIndex(x => x == weekDate);
    }

    private int GetWeekIndexAcrossMonths(int weekDate, Month targetMonth, List<Month> months)
    {
        int index = 0;
        foreach(var month in months)
        {
            if(!targetMonth.Equals(month))
            {
                index += month.GetMondays().Length;
            }
            else
            {
                index += targetMonth.GetMondays().ToList().FindIndex(x => x == weekDate);
                break;
            }
        }
        return index;
    }

    private IEnumerator SetupProject(Report report, Month reportMonth, List<Month> months, int weekCount, int i)
    {
        Project project = report.team.projects[i];
        TextContainer projectTextContainer = Instantiate(projectPrefab, projectContainer);
        projectTextContainer.texts[0].text = (i + 1).ToString();
        int projEndDate = project.endMonth.GetFollowingFridayFromMonday(project.endWeek, out Month projEndMonthFriday);
        projectTextContainer.texts[1].text = $"<b>{project.name}</b>\nDue: {projEndDate} {projEndMonthFriday}";
        GameObject row = Instantiate(rowPrefab, rowContainer);
        List<RectTransform> cellRectTransforms = new List<RectTransform>();
        for (int j = 0; j < weekCount; j++)
        {
            cellRectTransforms.Add(Instantiate(cellPrefab, row.transform).GetComponent<RectTransform>());
        }
        yield return null;
        yield return null;
        if(i == 0)
        {
            int index = GetWeekIndexAcrossMonths(report.reportWeek, reportMonth, months);
            RectTransform cellRectTransform = row.transform.GetChild(index) as RectTransform;
            RectTransform reportWeekMarker = Instantiate(reportWeekMarkerPrefab, tableContainer);
            float cellSize = cellRectTransform.sizeDelta.x + (weekPadding/2f);
            Vector3 targetPos = new Vector3(cellRectTransform.position.x, reportWeekMarker.position.y, 0);
            reportWeekMarker.position = targetPos;
            Vector3 offset = new Vector3(cellSize, 0, -reportWeekMarker.localPosition.z);
            reportWeekMarker.localPosition += offset;
        }
        InstantiateProjectProgressBars(months, project, project.startMonth, project.endMonth, row);
        Instantiate(projectNoteBoxPrefab, projectNoteBoxContainer).texts[0].text = string.Join("\n", project.notes);
    }

    private void InstantiateProjectProgressBars(List<Month> months, Project project, Month projStartMonth, Month projEndMonth, GameObject row)
    {
        ProgressBarUI projectProgressBarUI = Instantiate(projectProgressPrefab, row.transform);
        SetupProgressBarTransform(months, project, row, projectProgressBarUI.rectTransform);
        projectProgressBarUI.progressSlider.value = (float)project.progress/100f;
        projectProgressBarUI.progressPercentageText.text = $"{((int)project.progress).ToString()}%";
        projectProgressBarUI.SetStatusColours(project.status);
    }

    private void SetupProgressBarTransform(List<Month> months, Project project, GameObject row, RectTransform projectProgressRect)
    {
        RectTransform targetStartPos;
        int startWeekRowIndex = GetWeekIndexAcrossMonths(project.startWeek, project.startMonth, months);
        int endWeekRowIndex = GetWeekIndexAcrossMonths(project.startWeek, project.startMonth, months);

        targetStartPos = row.transform.GetChild(startWeekRowIndex).transform as RectTransform;
        projectProgressRect.localPosition = targetStartPos.localPosition;

        int weeksToSpan = endWeekRowIndex - startWeekRowIndex;
        projectProgressRect.sizeDelta = new Vector2(CalculateProgressBarSizeDeltaX(weeksToSpan, (row.transform.GetChild(0).transform as RectTransform).sizeDelta.x, weekPadding), projectProgressRect.sizeDelta.y);
    }

    private static float CalculateProgressBarSizeDeltaX(int weeksToSpan, float oneWeekXSize, float weekPadding)
    {
        return oneWeekXSize * (weeksToSpan + 1) + (weekPadding * (weeksToSpan - 1));
    }

    private void GenerateWeeks(List<Month> months, out int weekCount)
    {
        List<int> mondays = new List<int>();
        months.ForEach(m => mondays.AddRange(m.GetMondays()));

        weekCount = mondays.Count;
        foreach (int day in mondays)
        {
            Instantiate(weekPrefab, weekContainer).texts[0].text = day.ToString();
        }
    }

    private void GenerateMonths(Report report)
    {
        List<Month> dominantMonths = new MonthSplit(report.months).GetDominantMonths();
        foreach (var month in report.months)
        {
            MonthUI monthUI = GenerateMonthUI(month);
            SetMonthLayoutSize(month, monthUI, dominantMonths);
        }
    }

    private static void SetMonthLayoutSize(Month month, MonthUI monthUI, List<Month> dominantMonths)
    {
        float offsetTime = month.GetMondays().Length == 4 ? MonthUI.PREFERRED_WIDTH_4_WEEKS : MonthUI.PREFERRED_WIDTH_5_WEEKS;
        if(report.months.Count == 3 && dominantMonths[0].GetMondays().Length == 5 && dominantMonths.Count >= 2) offsetTime = MonthUI.PREFERRED_WIDTH_5_WEEKS_3_MONTHS_2_LONG_MONTHS;

        if (dominantMonths.Contains(month)) monthUI.layoutElement.minWidth = offsetTime;
    }

    private MonthUI GenerateMonthUI(Month month)
    {
        MonthUI monthUI = Instantiate(monthPrefab, monthContainer);
        monthUI.textContainer.texts[0].text = month.ToString();
        return monthUI;
    }
}
