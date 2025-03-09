using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TLSceneManager : MonoBehaviour
{
    public TMP_Dropdown teamNamesDropdown;
    public Button importConfigButton;
    public Button addProjectButton;
    public Transform projectSetupUIContainer;
    public ProjectSetupUI projectSetupUIPrefab;
    ProjectManagerConfig pmConfig;
    List<ProjectSetupUI> projectSetupUIs = new List<ProjectSetupUI>();

    public void Start()
    {
        importConfigButton.onClick.AddListener(ImportConfig_OnClick);
        addProjectButton.onClick.AddListener(AddProject_OnClick);
    }

    private void AddProject_OnClick()
    {
        projectSetupUIs.Add(GenerateProjectSetupUI());
    }

    private ProjectSetupUI GenerateProjectSetupUI()
    {
        return Instantiate(projectSetupUIPrefab, projectSetupUIContainer);
    }

    private void ImportConfig_OnClick()
    {
        new PMConfigLoader(this, "pmcfg", PMConfigLoader_OnLoad).StartLoad();
    }

    private void PMConfigLoader_OnLoad(ProjectManagerConfig config)
    {
        if (config == null) return;
        pmConfig = config;
        teamNamesDropdown.options = pmConfig.teamNames.ToList().ConvertAll(x => new TMP_Dropdown.OptionData(x));
    }
}
