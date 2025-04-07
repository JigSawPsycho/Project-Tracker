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
    public Month startMonth;
    public int endWeek;
    public Month endMonth;
    public int progress;
    public ProjectStatus status;
    public string[] notes;
}
