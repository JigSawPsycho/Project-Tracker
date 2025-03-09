using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PMSetupSceneManager : MonoBehaviour
{
    public TMP_InputField teamNamesInputField;
    public TMP_InputField startMonthInputField;
    public TMP_InputField startYearInputField;
    public TMP_Dropdown reportDateDropdown;
    public Button importButton;
    public Button exportButton;
    Month startMonth;
    Month endMonth;
    void Start()
    {
        startMonthInputField.onValueChanged.AddListener(OnStartDateInputFieldsValueChanged);
        startYearInputField.onValueChanged.AddListener(OnStartDateInputFieldsValueChanged);
        exportButton.onClick.AddListener(OnExportButtonClick);
        importButton.onClick.AddListener(OnImportButtonClick);
    }

    private void OnImportButtonClick()
    {
        new PMConfigLoader(this, "pmcfg", OnPMConfigLoad).StartLoad();
    }

    private void OnPMConfigLoad(ProjectManagerConfig pmConfig)
    {
        teamNamesInputField.text = string.Join(",", pmConfig.teamNames);
        startMonthInputField.text = pmConfig.startMonth.ToString();
        startYearInputField.text = pmConfig.startYear.ToString();
    }

    private void OnExportButtonClick()
    {
        new PMConfigSaver(GeneratePMConfigFromUI()).Save();
    }

    private ProjectManagerConfig GeneratePMConfigFromUI()
    {
        string[] reportDate = reportDateDropdown.options[reportDateDropdown.value].text.Split(" ");
        int reportFriday = int.Parse(reportDate[0]);
        int reportMonth = Month.ConvertStringToMonthInt(reportDate[1]);
        int reportYear = int.Parse(reportDate[2]);
        int[] startMonthFridays = startMonth.GetFridays();
        int[] endMonthFridays = endMonth.GetFridays();
        List<int> fridays = new List<int>(startMonthFridays);
        fridays.AddRange(endMonthFridays);
        return new()
        {
            teamNames = teamNamesInputField.text.Split(","),
            startMonth = startMonth.month,
            startYear = startMonth.year,
            endMonth = endMonth.month,
            endYear = endMonth.year,
            reportFriday = reportFriday,
            reportMonth = reportMonth,
            reportYear = reportYear
        };
    }

    private void OnStartDateInputFieldsValueChanged(string value)
    {
        if(string.IsNullOrEmpty(startMonthInputField.text) || string.IsNullOrEmpty(startYearInputField.text))
        {
            reportDateDropdown.interactable = false;
            exportButton.interactable = false;
            return;
        }
        startMonth = new(int.Parse(startMonthInputField.text), int.Parse(startYearInputField.text) + 2000);
        endMonth = startMonth.CreateFollowingMonth();
        reportDateDropdown.options = startMonth.ConvertMonthFridaysToOptionData().ToList();
        reportDateDropdown.AddOptions(endMonth.ConvertMonthFridaysToOptionData().ToList());
        reportDateDropdown.interactable = true;
        exportButton.interactable = true;
    }
}
