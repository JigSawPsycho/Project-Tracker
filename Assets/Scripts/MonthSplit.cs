using System;
using System.Collections.Generic;

public class MonthSplit
{
    private List<Month> months;

    public MonthSplit(List<Month> months)
    {
        this.months = months;
    }

    public List<Month> GetDominantMonths()
    {
        int dominantMonthThreshold = 0;
        List<Month> dominantMonths = new List<Month>();

        foreach(var month in months)
        {
            int mondayCount = month.GetMondays().Length;
            if (mondayCount > dominantMonthThreshold)
            {
                dominantMonths.Clear();
                dominantMonths.Add(month);
                dominantMonthThreshold = mondayCount;
            }
            else if (mondayCount == dominantMonthThreshold)
            {
                dominantMonths.Add(month);
            }
        }
        return dominantMonths;
    }
}