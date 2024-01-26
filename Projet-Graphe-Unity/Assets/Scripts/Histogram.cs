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

    public Dictionary<int, int> GenerateHistogram(Dictionary<int,int> values)
    {
        Dictionary<int, int> histogram = new();
        for (int i = 0; i < abcisses.Count - 1; i++)
        {
            histogram.Add(i, 0);
        }
        foreach (KeyValuePair<int,int> kvp in values)
        {
            int index = 0;
            while (index<abcisses.Count && kvp.Key > abcisses[index])
            {
                index++;
            }
            index = Mathf.Clamp(index - 1, 0, abcisses.Count - 1);
            if (histogram.TryGetValue(index, out int v)) histogram[index] = v+kvp.Value;
            else histogram.Add(index, kvp.Value);
        }
        return histogram;
    }

}
