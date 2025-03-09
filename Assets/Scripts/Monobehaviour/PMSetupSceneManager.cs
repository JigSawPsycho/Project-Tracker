using System;
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
    public Button exportButton;
    void Start()
    {
        startMonthInputField.onValueChanged.AddListener(OnStartDateInputFieldsValueChanged);
        startYearInputField.onValueChanged.AddListener(OnStartDateInputFieldsValueChanged);
    }

    private void OnStartDateInputFieldsValueChanged(string value)
    {
        if(string.IsNullOrEmpty(startMonthInputField.text) || string.IsNullOrEmpty(startYearInputField.text))
        {
            reportDateDropdown.interactable = false;
            exportButton.interactable = false;
        }
        Month month1 = new(int.Parse(startMonthInputField.text), int.Parse(startYearInputField.text) + 2000);
        Month month2 = month1.CreateFollowingMonth();
        reportDateDropdown.options = month1.ConvertMonthFridaysToOptionData().ToList();
        reportDateDropdown.AddOptions(month2.ConvertMonthFridaysToOptionData().ToList());
        reportDateDropdown.interactable = true;
        exportButton.interactable = true;
    }
}
