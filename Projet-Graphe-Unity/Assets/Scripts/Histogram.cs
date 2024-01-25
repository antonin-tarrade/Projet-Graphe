using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Histogram 
{
    private List<int> abcisses;

    public Histogram(List<int> abcisses)
    {
        this.abcisses = abcisses;
    }

    public Dictionary<int, int> GenerateHistogram(List<ValueTuple<int,int>> values)
    {
        Dictionary<int, int> histogram = new();
        for (int i = 0; i < abcisses.Count - 1; i++)
        {
            histogram.Add(i, 0);
        }
        foreach (ValueTuple<int,int> tup in values)
        {
            int index = 0;
            while (index<abcisses.Count && tup.Item1 < abcisses[index])
            {
                index++;
            }
            histogram.Add(Mathf.Clamp(index - 1, 0, abcisses.Count - 1), tup.Item2);
        }
        return histogram;
    }

}
