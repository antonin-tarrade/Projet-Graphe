using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class GraphElement<T>
{
    public T element { get; private set; }
    private ISet<GraphLink<T>> links;

    public GraphElement(T element)
    {
        this.element = element;
        links = new HashSet<GraphLink<T>>();
    }

    public void AddLink(GraphLink<T> link)
    {
        links.Add(link);
    }

    public void RemoveLink(GraphLink<T> link)
    {
        links.Remove(link);
    }

    public virtual void Destroy() 
    {

    }

}

public class GraphLink<T>
{
    protected GraphElement<T> e1;
    protected GraphElement<T> e2;

    public GraphLink(GraphElement<T> e1, GraphElement<T> e2)
    {
        this.e1 = e1;
        this.e2 = e2;
    }

    public virtual void Create()
    {
        e1?.AddLink(this);
        e2?.AddLink(this);
    }
    
    public virtual void Clear()
    {
        e1?.RemoveLink(this);
        e2?.RemoveLink(this);
    }

    public virtual void Destroy() 
    {

    }
}

public class GraphLinkCreator<T>
{
    public virtual GraphLink<T> CreateGraphLink(GraphElement<T> e1, GraphElement<T> e2)
    {
        return new GraphLink<T>(e1, e2);
    }
}

public class Graph<T>
{

    public static Graph<T> GetGraph (ref Graph<T> graph)
    {
        if (graph == null)
        {
            return new Graph<T>();
        }
        T[] elems = graph.refs.Keys.ToArray<T>();
        graph.Destroy();
        return new Graph<T>(elems);
    }

    public static Graph<T> GetGraph(ref Graph<T> graph, T[] elems)
    {
        if (graph == null)
        {
            return new Graph<T>(elems);
        }
        graph.Destroy();
        return new Graph<T>(elems);
    }


    private List<GraphElement<T>> elements;
    private List<GraphLink<T>> links;
    private Dictionary<T, GraphElement<T>> refs;

    private Graph()
    {
        elements = new List<GraphElement<T>>();
        links = new List<GraphLink<T>>();
        refs = new Dictionary<T, GraphElement<T>>();
    }

    private Graph(T[] elements) : this()
    {
        foreach (T e in elements)
        {
            Add(e);
        }
    }

    public int GetSize()
    {
        return elements.Count;
    }

    public T Get(int i)
    {
        return (i >= 0 && i < GetSize()) ? elements[i].element : default(T);
    }

    public int IndexOf(T element)
    {
        if (refs.TryGetValue(element, out GraphElement<T> value)) return elements.IndexOf(value);
        return -1;
    }


    public void Add(T element)
    {
        GraphElement<T> e = new GraphElement<T>(element);
        elements.Add(e);
        refs.Add(element, e);
    }

    public bool[][] GetAdjacenceMatrix(Func<T, T, bool> comparator)
    {
        int size = GetSize();
        bool[][] m = new bool[size][];
        for (int i = 0; i < size; i ++)
        {
            m[i] = new bool[size];
            for (int j = 0; j < size; j++) m[i][j] = comparator(Get(i), Get(j));
        }
        return m;
    }

    public void ClearLinks()
    {
        foreach (GraphLink<T> link in links)
        {
            link.Destroy();
        }
        links.Clear();
    }

    public void SetLinks(Func<T, T, bool> comparator, GraphLinkCreator<T> linkCreator)
    {
        ClearLinks();
        bool[][] adjacenceMatrix = GetAdjacenceMatrix(comparator);
        int size = adjacenceMatrix.Length;
        int n = Mathf.Min(size, GetSize());
        for (int i = 0; i < n; i ++)
        {
            for (int j = 0; j < n; j ++)
            {
                if (i != j && adjacenceMatrix[i][j])
                {
                    GraphLink<T> link = linkCreator.CreateGraphLink(elements[i], elements[j]);
                    links.Add(link);
                    link.Create();
                }
            }
        }
    }

    public void Destroy() {
        foreach (GraphElement<T> e in elements) e.Destroy();
        foreach (GraphLink<T> l in links) l.Destroy();
    }
}
