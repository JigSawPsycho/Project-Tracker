using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Report
{
    public int reportWeek;
    public Month reportMonth;
    public List<Month> months;
    public Team team;
}
