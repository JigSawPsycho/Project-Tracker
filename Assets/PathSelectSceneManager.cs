using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathSelectSceneManager : MonoBehaviour
{
    public Button projectManagerButton;
    public Button teamLeadButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectManagerButton.onClick.AddListener(ProjectManagerButton_OnClick);
        teamLeadButton.onClick.AddListener(TeamLeadButton_OnClick);
    }

    void OnDestroy()
    {
        projectManagerButton.onClick.RemoveListener(ProjectManagerButton_OnClick);
        teamLeadButton.onClick.RemoveListener(TeamLeadButton_OnClick);
    }

    private void ProjectManagerButton_OnClick()
    {
        SceneManager.LoadScene(1);
    }

    private void TeamLeadButton_OnClick()
    {
        SceneManager.LoadScene(2);
    }
}
