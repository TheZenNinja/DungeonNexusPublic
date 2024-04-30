using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ExpTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ExpTestSimplePasses()
    {
        var threshes = new int[]
        {
            0,      // 1
            100,    // 2
            300,    // 3
            500,    // 4
            1000    // 5
        };


        Assert.AreEqual(1, GetLevel(threshes, 99));

        Assert.AreEqual(2, GetLevel(threshes, 100));
        Assert.AreEqual(2, GetLevel(threshes, 123));
        Assert.AreEqual(2, GetLevel(threshes, 299));

        Assert.AreEqual(3, GetLevel(threshes, 450));

        Assert.AreEqual(5, GetLevel(threshes, 1000));
        Assert.AreEqual(5, GetLevel(threshes, 2500));
    }

    protected int GetLevel(int[] thresholds, int exp)
    {
        if (exp < thresholds[0])
            return 1;
        if (exp >= thresholds[thresholds.Length - 1])
            return thresholds.Length;

        for (int i = 1; i < thresholds.Length; i++)
        {
            if (exp < thresholds[i] && exp >= thresholds[i-1])
                return i;

        }
        return -1;
    }
}
