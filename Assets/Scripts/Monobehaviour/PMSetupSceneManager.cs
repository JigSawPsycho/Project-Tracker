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
    public TMP_Dropdown monthCountDropdown;
    public Button importButton;
    public Button exportButton;
    List<Month> months;
    void Start()
    {
        startMonthInputField.onValueChanged.AddListener(_ => UpdateMonths());
        startMonthInputField.onValueChanged.AddListener(_ => UpdateValidDateState());
        startMonthInputField.onValueChanged.AddListener(_ => UpdateReportDateOptions());
        startYearInputField.onValueChanged.AddListener(_ => UpdateMonths());
        startYearInputField.onValueChanged.AddListener(_ => UpdateValidDateState());
        startYearInputField.onValueChanged.AddListener(_ => UpdateReportDateOptions());
        monthCountDropdown.onValueChanged.AddListener(_ => UpdateMonths());
        monthCountDropdown.onValueChanged.AddListener(_ => UpdateReportDateOptions());
        PMConfigLoader.onLoadComplete += OnPMConfigLoad;
        PMConfigSaver.onPreFileDownload += PMConfigSaver_OnPreFileDownload;
    }

    public void OnDestroy()
    {
        PMConfigLoader.onLoadComplete -= OnPMConfigLoad;
        PMConfigSaver.onPreFileDownload -= PMConfigSaver_OnPreFileDownload;
    }

    private void OnPMConfigLoad(ProjectManagerConfig pmConfig)
    {
        teamNamesInputField.text = string.Join(",", pmConfig.teamNames);
        startMonthInputField.text = pmConfig.months[0].month.ToString();
        startYearInputField.text = pmConfig.months[0].year.ToString().Substring(2);
        monthCountDropdown.value = monthCountDropdown.options.FindIndex(x => x.text == pmConfig.months.Count.ToString());
    }

    public void PMConfigSaver_OnPreFileDownload()
    {
        PMConfigSaver.pmConfig = GeneratePMConfigFromUI();
    }

    private ProjectManagerConfig GeneratePMConfigFromUI()
    {
        string[] reportDate = reportDateDropdown.options[reportDateDropdown.value].text.Split(" ");
        int reportFriday = int.Parse(reportDate[0]);
        Month reportMonth = new Month(Month.ConvertStringToMonthInt(reportDate[1]), int.Parse(reportDate[2]));
        List<int> fridays = new List<int>();
        months.ForEach(m => fridays.AddRange(m.GetFridays()));
        var teamNames = teamNamesInputField.text.Split(",").ToList();
        teamNames.ForEach(t => t.Trim());
        return new()
        {
            teamNames = teamNames,
            months = months,
            reportFriday = reportFriday,
            reportMonth = reportMonth
        };
    }

    private void UpdateValidDateState()
    {
        bool enteredValidDate = HasValidDateBeenEntered();
        reportDateDropdown.interactable = enteredValidDate;
        exportButton.interactable = enteredValidDate;
    }

    private bool HasValidDateBeenEntered()
    {
        return !string.IsNullOrEmpty(startMonthInputField.text) && !string.IsNullOrEmpty(startYearInputField.text);
    }

    private void UpdateMonths()
    {
        if(!HasValidDateBeenEntered()) return;
        months = new List<Month>();
        int monthsAfterStart = int.Parse(monthCountDropdown.options[monthCountDropdown.value].text) - 1;
        Month currentMonth = new(int.Parse(startMonthInputField.text), int.Parse(startYearInputField.text) + 2000);

        months.Add(currentMonth);
        for(int i = 0; i < monthsAfterStart; i++)
        {
            currentMonth = currentMonth.CreateFollowingMonth();
            months.Add(currentMonth);
        }
    }

    private void UpdateReportDateOptions()
    {
        if(!HasValidDateBeenEntered()) return;
        reportDateDropdown.options = new List<TMP_Dropdown.OptionData>();
        months.ForEach(m => reportDateDropdown.AddOptions(m.ConvertMonthMondaysToOptionData().ToList()));
    }

}
