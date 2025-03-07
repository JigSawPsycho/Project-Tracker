using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MonthSplitTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void GetDominantMonth_GivenOneLongAndOneShortMonth_ReturnsLongMonth()
    {
        Month march = new Month(3, 2025);
        Month april = new Month(4, 2025);
        MonthSplit marchAprilSplit = new MonthSplit(march, april);
        
        Assert.That(marchAprilSplit.GetDominantMonth(), Is.EqualTo(march));
    }

    [Test]
    public void GetDominantMonth_GivenSameLengthMonths_ReturnsNull()
    {
        Month april = new Month(4, 2025);
        Month may = new Month(5, 2025);
        MonthSplit marchAprilSplit = new MonthSplit(april, may);
        
        Assert.That(marchAprilSplit.GetDominantMonth(), Is.Null);
    }
}