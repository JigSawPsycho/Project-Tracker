using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class Month
{
    // experimentation led to this number
    public readonly int month;
    public readonly int year;

    public Month(int month, int year)
    {
        this.month = month;
        this.year = year;
    }

    public int[] GetMondays()
    {
        return AllDatesInMonth(month, year).Where(i => i.DayOfWeek == DayOfWeek.Monday).ToList().ConvertAll<int>(x => x.Day).ToArray();
    }

    private static List<DateTime> AllDatesInMonth(int month, int year)
    {
        List<DateTime> result = new List<DateTime>();
        int days = DateTime.DaysInMonth(year, month);
        for (int day = 1; day <= days; day++)
        {
            result.Add(new DateTime(year, month, day));
        }
        return result;
    }

    //TODO: Month is showing weird
    public override string ToString()
    {
        CultureInfo ci = new CultureInfo("en-US");
        return DateTime.Now.ToString("MMMM", ci);
    }
}
