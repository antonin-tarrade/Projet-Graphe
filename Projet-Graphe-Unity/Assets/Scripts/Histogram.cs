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

    public Dictionary<float,int> GenerateHistogram(float[] tab)
    {
        Dictionary<float, int> dic = new();
        foreach (float f in tab)
        {
            if (dic.TryGetValue(f, out int v)) dic[f] = v + 1;
            else dic.Add(f, 1);
        }
        return GenerateHistogram(dic);
    }

    public Dictionary<int, int> GenerateHistogram(int[] tab)
    {
        Dictionary<int, int> dic = new();
        foreach (int f in tab)
        {
            if (dic.TryGetValue(f, out int v)) dic[f] = v + 1;
            else dic.Add(f, 1);
        }
        return GenerateHistogram(dic);
    }

    public Dictionary<float, int> GenerateHistogram(Dictionary<float, int> values)
    {
        Dictionary<float, int> histogram = new();
        for (float i = 0; i < abcisses.Count - 1; i++)
        {
            histogram.Add(i, 0);
        }
        foreach (KeyValuePair<float, int> kvp in values)
        {
            int index = 0;
            while (index<abcisses.Count-1 && kvp.Key > abcisses[index])
            {
                index++;
            }
            index = Mathf.Clamp(index - 1, 0, abcisses.Count - 1);
            if (histogram.TryGetValue(index, out int v)) histogram[index] = v+kvp.Value;
            else histogram.Add(index, kvp.Value);
        }
        return histogram;
    }

    public Dictionary<int, int> GenerateHistogram(Dictionary<int, int> values)
    {
        Dictionary<int, int> histogram = new();
        for (int i = 0; i < abcisses.Count - 1; i++)
        {
            histogram.Add(i, 0);
        }
        foreach (KeyValuePair<int, int> kvp in values)
        {
            int index = 0;
            while (index < abcisses.Count - 1 && kvp.Key > abcisses[index])
            {
                index++;
            }
            index = Mathf.Clamp(index - 1, 0, abcisses.Count - 1);
            if (histogram.TryGetValue(index, out int v)) histogram[index] = v + kvp.Value;
            else histogram.Add(index, kvp.Value);
        }
        return histogram;
    }

}
