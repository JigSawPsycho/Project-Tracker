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
        SceneManager.LoadScene("TimelineScene");
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
        teamNamesDropdown.options = new List<TMP_Dropdown.OptionData>(new TMP_Dropdown.OptionData[1] { new(name = report.team.name)});
        teamNamesDropdown.value = 0;
        foreach (var project in report.team.projects)
        {
            ProjectSetupUI projectSetupUI = GenerateProjectSetupUI();
            projectSetupUIs.Add(projectSetupUI);

            projectSetupUI.projectNameInputField.text = project.name;
            projectSetupUI.startWeekDropdown.value = projectSetupUI.startWeekDropdown.options.FindIndex(x => x.text.Contains($"{project.startWeek} {project.startMonth}"));
            projectSetupUI.endWeekDropdown.value = projectSetupUI.endWeekDropdown.options.FindIndex(x => x.text.Contains($"{project.endWeek} {project.endMonth}"));
            projectSetupUI.notesInputField.text = string.Join("\n", project.notes);
            projectSetupUI.progressInputField.text = project.progress.ToString();
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
            months = pmConfig.months,
            reportMonth = pmConfig.reportMonth,
            reportWeek = pmConfig.reportFriday,
            team = team
        };

        Main.report = report;
        ReportSaver.report = report;
    }

    public void OnFileDownload() {

    }

    private Project ConvertToProject(ProjectSetupUI projectSetupUI)
    {
        GetProjectDateInfo(projectSetupUI.startWeekDropdown, out int startWeekInt, out Month startMonth);

        GetProjectDateInfo(projectSetupUI.endWeekDropdown, out int endWeekInt, out Month endMonth);

        return new Project() 
        { 
            startMonth = startMonth,
            startWeek = startWeekInt,
            endMonth = endMonth,
            endWeek = endWeekInt,
            name = projectSetupUI.projectNameInputField.text,
            notes = projectSetupUI.notesInputField.text.Split('\n'),
            progress = Mathf.Clamp(int.Parse(projectSetupUI.progressInputField.text), 0, 100),
            status = (ProjectStatus) projectSetupUI.statusDropdown.value
        };
    }

    private void GetProjectDateInfo(TMP_Dropdown possibleWeeksDropdown, out int weekInt, out Month month)
    {
        string dateStr = possibleWeeksDropdown.options[possibleWeeksDropdown.value].text;
        string[] splits = dateStr.Split(" ");
        weekInt = int.Parse(splits[0]);
        month = new Month(Month.ConvertStringToMonthInt(splits[1]), int.Parse(splits[2]));
    }

    private void AddProject_OnClick()
    {
        projectSetupUIs.Add(GenerateProjectSetupUI());
        StartCoroutine(RefreshUI());
    }

    private ProjectSetupUI GenerateProjectSetupUI()
    {
        ProjectSetupUI projectSetupUI = Instantiate(projectSetupUIPrefab, projectSetupUIContainer);
        List<TMP_Dropdown.OptionData> mondayOptionDatas = new List<TMP_Dropdown.OptionData>();
        pmConfig.months.ForEach(x => mondayOptionDatas.AddRange(x.ConvertMonthMondaysToOptionData().ToList()));
        projectSetupUI.progressInputField.onValueChanged.AddListener(str => ProgressInputField_OnValueChanged(projectSetupUI.progressInputField, str));
        projectSetupUI.startWeekDropdown.options = mondayOptionDatas;
        UpdateEndWeekDropdownOptions(projectSetupUI);
        projectSetupUI.startWeekDropdown.onValueChanged.AddListener(_ => UpdateEndWeekDropdownOptions(projectSetupUI));
        projectSetupUI.removeButton.onClick.AddListener(() => FlushProjectSetupUI(projectSetupUI));
        projectSetupUI.increasePrioButton.onClick.AddListener(() => ChangeProgressPriority(projectSetupUI, -1));
        projectSetupUI.decreasePrioButton.onClick.AddListener(() => ChangeProgressPriority(projectSetupUI, 1));
        return projectSetupUI;
    }

    private void UpdateEndWeekDropdownOptions(ProjectSetupUI projectSetupUI)
    {
        List<TMP_Dropdown.OptionData> mondayOptionDatas = new List<TMP_Dropdown.OptionData>();
        pmConfig.months.ForEach(x => mondayOptionDatas.AddRange(x.ConvertMonthMondaysToOptionData().ToList()));
        projectSetupUI.endWeekDropdown.options = mondayOptionDatas.GetRange(projectSetupUI.startWeekDropdown.value, mondayOptionDatas.Count - projectSetupUI.startWeekDropdown.value);
    }

    private void ProgressInputField_OnValueChanged(TMP_InputField inputField, string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            inputField.text = "0";
        }
    }

    private void ChangeProgressPriority(ProjectSetupUI projectSetupUI, int priorityDelta)
    {
        int currentPrio = projectSetupUI.transform.GetSiblingIndex() + 1;
        int newPrio = Mathf.Clamp(currentPrio + priorityDelta, 1, projectSetupUIContainer.childCount);
        int prioIndex = newPrio - 1;
        projectSetupUI.transform.SetSiblingIndex(prioIndex);
        projectSetupUIs.Remove(projectSetupUI);
        projectSetupUIs.Insert(prioIndex, projectSetupUI);
        StartCoroutine(RefreshUI());
    }

    private void FlushProjectSetupUI(ProjectSetupUI projectSetupUI)
    {
        projectSetupUIs.Remove(projectSetupUI);
        projectSetupUI.removeButton.onClick.RemoveAllListeners();
        projectSetupUI.increasePrioButton.onClick.RemoveAllListeners();
        projectSetupUI.decreasePrioButton.onClick.RemoveAllListeners();
        projectSetupUI.progressInputField.onValueChanged.RemoveAllListeners();
        Destroy(projectSetupUI.gameObject);
        StartCoroutine(RefreshUI());
    }

    private IEnumerator RefreshUI()
    {
        projectSetupUIs.ForEach(x => x.projectPrioText.text = (x.transform.GetSiblingIndex() + 1).ToString());
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
