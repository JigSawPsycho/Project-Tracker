using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class Month : IEquatable<Month>
{
    // experimentation led to this number
    public int month;
    public int year;

    public Month(int month, int year)
    {
        this.month = month;
        this.year = year;
    }

    public int[] GetMondays()
    {
        return AllDatesInMonth(month, year).Where(i => i.DayOfWeek == DayOfWeek.Monday).ToList().ConvertAll<int>(x => x.Day).ToArray();
    }

    public int[] GetFridays()
    {
        return AllDatesInMonth(month, year).Where(i => i.DayOfWeek == DayOfWeek.Friday).ToList().ConvertAll<int>(x => x.Day).ToArray();
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

    public Month CreateFollowingMonth()
    {
        int newMonth = month + 1;
        int newYear = year;
        if(newMonth == 13)
        {
            newYear++;
            newMonth = 1;
        }
        return new(newMonth, newYear);
    }

    //TODO: Month is showing weird
    public override string ToString()
    {
        CultureInfo ci = new CultureInfo("en-US");
        return new DateTime(year, month, 1).ToString("MMMM", ci);
    }

    public static int ConvertStringToMonthInt(string str)
    {
        CultureInfo ci = new CultureInfo("en-US");
        return DateTime.ParseExact(str, "MMMM", ci).Month;
    }

    public int GetFollowingFridayFromMonday(int day, out Month fridayMonth)
    {
        fridayMonth = this;
        DateTime dt = new DateTime(year, month, day).AddDays(4);
        if(dt.Month != month || dt.Year != year) fridayMonth = new Month(dt.Month, dt.Year);
        return dt.Day;
    }

    public TMPro.TMP_Dropdown.OptionData[] ConvertMonthMondaysToOptionData()
    {
        return GetMondays().ToList().ConvertAll(x => new TMPro.TMP_Dropdown.OptionData($"{x} {this} {year}")).ToArray();
    }

    public TMPro.TMP_Dropdown.OptionData[] ConvertMonthFridaysToOptionData()
    {
        return GetFridays().ToList().ConvertAll(x => new TMPro.TMP_Dropdown.OptionData($"{x} {this} {year}")).ToArray();
    }

    public bool Equals(Month other)
    {
        return other.month == month && other.year == year;
    }
}
