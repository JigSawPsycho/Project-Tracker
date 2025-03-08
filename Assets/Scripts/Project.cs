using System;
using UnityEngine;

public enum ProjectStatus
{
    OnTrack,
    Behind,
    Critical
}

[Serializable]
public class Project
{
    public string name;
    public int startWeek;
    public int startMonth;
    public int endWeek;
    public int endMonth;
    public int progress;
    public ProjectStatus status;
    public string[] notes;
}
