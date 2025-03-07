using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MonthTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void GetMondays_GivenMonth_ReturnsCorrectMondayDates()
    {
        Month january = new Month(1, 2025);

        Assert.That(january.GetMondays(), Is.EquivalentTo(new[] { 6, 13, 20, 27}));

        Month february = new Month(2, 2025);

        Assert.That(february.GetMondays(), Is.EquivalentTo(new[] { 3, 10, 17, 24}));
    }
}
