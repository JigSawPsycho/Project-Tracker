using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public RectTransform rectTransform;
    public Slider progressSlider;
    public Image sliderBackground;
    public Image sliderFill;
    public Image sliderOutline;
    public TextMeshProUGUI progressPercentageText;
    public List<StatusColourProfile> statusColourProfiles;

    public void SetStatusColours(ProjectStatus projectStatus)
    {
        StatusColourProfile statusColourProfile = statusColourProfiles.Find(x => x.status == projectStatus);
        sliderFill.color = new Color(statusColourProfile.backgroundColor.r, statusColourProfile.backgroundColor.g, statusColourProfile.backgroundColor.b, 75);
        sliderBackground.color = statusColourProfile.backgroundColor;
        sliderOutline.color = statusColourProfile.outlineColor;
    }
}

[Serializable]
public class StatusColourProfile
{
    public ProjectStatus status;
    public Color backgroundColor;
    public Color outlineColor;
}