using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TLSceneManager : MonoBehaviour
{
    public TMP_Dropdown teamNamesDropdown;
    public Button importConfigButton;
    public Button importReportButton;
    public GameObject importButtonsGroup;
    public Button addProjectButton;
    public Button generateButton;
    public Transform projectSetupUIContainer;
    public ProjectSetupUI projectSetupUIPrefab;
    public CanvasGroup contentCanvasGroup;
    public GameObject waitingForUploadGameObject;
    ProjectManagerConfig pmConfig;
    List<ProjectSetupUI> projectSetupUIs = new List<ProjectSetupUI>();

    public void Start()
    {
        PMConfigLoader.onLoadComplete += PMConfigLoader_OnLoad;
        ReportLoader.onLoadComplete += ReportLoader_OnLoadComplete;
        ReportSaver.onPreFileDownload += Generate_OnClick;
        ReportSaver.onFileDownloaded += ReportSaver_OnFileDownloaded;
        addProjectButton.onClick.AddListener(AddProject_OnClick);
    }

    private void ReportSaver_OnFileDownloaded()
    {
        SceneManager.LoadScene(3);
    }

    public void OnDestroy()
    {
        PMConfigLoader.onLoadComplete -= PMConfigLoader_OnLoad;
        ReportLoader.onLoadComplete -= ReportLoader_OnLoadComplete;
        ReportSaver.onPreFileDownload -= Generate_OnClick;
        ReportSaver.onFileDownloaded -= ReportSaver_OnFileDownloaded;
        addProjectButton.onClick.RemoveListener(AddProject_OnClick);
    }

    private void ReportLoader_OnLoadComplete(Report report)
    {
        contentCanvasGroup.interactable = false;
        waitingForUploadGameObject.SetActive(true);
        importButtonsGroup.gameObject.SetActive(false);
        if(report == null)
        {
            importButtonsGroup.gameObject.SetActive(true);
            waitingForUploadGameObject.SetActive(false);
            return;
        }
        contentCanvasGroup.interactable = true;
        contentCanvasGroup.gameObject.SetActive(true);
        waitingForUploadGameObject.SetActive(false);
        pmConfig = new ProjectManagerConfig()
        {
            teamNames = new string[1] { report.team.name },
            startMonth = report.startingMonth,
            startYear = report.startingYear,
            endMonth = report.endingMonth,
            endYear = report.endingYear,
            reportFriday = report.reportWeek,
            reportMonth = report.reportMonth,
            reportYear = report.reportYear
        };
        teamNamesDropdown.options = new List<TMP_Dropdown.OptionData>(new TMP_Dropdown.OptionData[1] { new(name = report.team.name)});
        teamNamesDropdown.value = 0;
        foreach (var project in report.team.projects)
        {
            ProjectSetupUI projectSetupUI = GenerateProjectSetupUI();
            projectSetupUIs.Add(projectSetupUI);

            projectSetupUI.projectNameInputField.text = project.name;
            Month projStartMonth = new(project.startMonth, 1);
            projectSetupUI.startWeekDropdown.value = projectSetupUI.startWeekDropdown.options.FindIndex(x => x.text.Contains($"{project.startWeek} {projStartMonth}"));
            Month projEndMonth = new(project.endMonth, 1);
            projectSetupUI.endWeekDropdown.value = projectSetupUI.endWeekDropdown.options.FindIndex(x => x.text.Contains($"{project.endWeek} {projEndMonth}"));
            projectSetupUI.notesInputField.text = string.Join("\n", project.notes);
            projectSetupUI.progressSlider.value = project.progress;
            projectSetupUI.statusDropdown.value = (int) project.status;
        }
        StartCoroutine(RefreshUI());
    }

    private void Generate_OnClick()
    {
        Project[] projects = projectSetupUIs.ConvertAll(ConvertToProject).ToArray(); 

        Team team = new()
        {
            name = teamNamesDropdown.options[teamNamesDropdown.value].text,
            projects = projects
        };

        Report report = new Report()
        {
            startingMonth = pmConfig.startMonth,
            startingYear = pmConfig.startYear,
            endingMonth = pmConfig.endMonth,
            endingYear = pmConfig.endYear,
            reportMonth = pmConfig.reportMonth,
            reportWeek = pmConfig.reportFriday,
            reportYear = pmConfig.reportYear,
            team = team
        };

        Main.report = report;
        ReportSaver.report = report;
    }

    public void OnFileDownload() {

    }

    private Project ConvertToProject(ProjectSetupUI projectSetupUI)
    {
        GetProjectDateInfo(projectSetupUI.startWeekDropdown, out int startWeekInt, out int startMonthInt);

        GetProjectDateInfo(projectSetupUI.endWeekDropdown, out int endWeekInt, out int endMonthInt);

        return new Project() 
        { 
            startMonth = startMonthInt,
            startWeek = startWeekInt,
            endMonth = endMonthInt,
            endWeek = endWeekInt,
            name = projectSetupUI.projectNameInputField.text,
            notes = projectSetupUI.notesInputField.text.Split('\n'),
            progress = (int) projectSetupUI.progressSlider.value,
            status = (ProjectStatus) projectSetupUI.statusDropdown.value
        };
    }

    private void GetProjectDateInfo(TMP_Dropdown possibleWeeksDropdown, out int weekInt, out int monthInt)
    {
        string dateStr = possibleWeeksDropdown.options[possibleWeeksDropdown.value].text;
        string[] splits = dateStr.Split(" ");
        weekInt = int.Parse(splits[0]);
        monthInt = Month.ConvertStringToMonthInt(splits[1]);
    }

    private void AddProject_OnClick()
    {
        projectSetupUIs.Add(GenerateProjectSetupUI());
        StartCoroutine(RefreshUI());
    }

    private ProjectSetupUI GenerateProjectSetupUI()
    {
        ProjectSetupUI projectSetupUI = Instantiate(projectSetupUIPrefab, projectSetupUIContainer);
        Month startMonth = new Month(pmConfig.startMonth, pmConfig.startYear);
        Month endMonth = new Month(pmConfig.endMonth, pmConfig.endYear);
        List<TMP_Dropdown.OptionData> mondayOptionDatas = startMonth.ConvertMonthMondaysToOptionData().ToList();
        mondayOptionDatas.AddRange(endMonth.ConvertMonthMondaysToOptionData().ToList());
        projectSetupUI.startWeekDropdown.options = mondayOptionDatas;
        projectSetupUI.endWeekDropdown.options = mondayOptionDatas;
        projectSetupUI.removeButton.onClick.AddListener(() => FlushProjectSetupUI(projectSetupUI));
        return projectSetupUI;
    }

    private void FlushProjectSetupUI(ProjectSetupUI projectSetupUI)
    {
        projectSetupUIs.Remove(projectSetupUI);
        projectSetupUI.removeButton.onClick.RemoveAllListeners();
        Destroy(projectSetupUI);
        StartCoroutine(RefreshUI());
    }

    private IEnumerator RefreshUI()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentCanvasGroup.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentCanvasGroup.transform as RectTransform);
    }

    private void PMConfigLoader_OnLoad(ProjectManagerConfig config)
    {
        contentCanvasGroup.interactable = false;
        waitingForUploadGameObject.SetActive(true);
        importButtonsGroup.gameObject.SetActive(false);
        if (config == null)
        {
            importButtonsGroup.SetActive(true);
            waitingForUploadGameObject.SetActive(false);
            return;
        }
        contentCanvasGroup.interactable = true;
        contentCanvasGroup.gameObject.SetActive(true);
        waitingForUploadGameObject.SetActive(false);
        pmConfig = config;
        teamNamesDropdown.options = pmConfig.teamNames.ToList().ConvertAll(x => new TMP_Dropdown.OptionData(x));
    }
}
