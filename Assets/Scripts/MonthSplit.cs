using System;

public class MonthSplit
{
    private Month month1;
    private Month month2;

    public MonthSplit(Month month1, Month month2)
    {
        this.month1 = month1;
        this.month2 = month2;
    }

    public Month GetDominantMonth()
    {
        int month1Mondays = month1.GetMondays().Length;
        int month2Mondays = month2.GetMondays().Length;
        if(month1Mondays == month2Mondays) return null;
        return month1Mondays > month2Mondays ? month1 : month2;
    }
}