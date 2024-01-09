using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Vertex<T>
{
    public T element { get; private set; }
    public int degree { get => edges.Count; }

    public Action<Vertex<T>> OnVertexUpdate;

    protected ISet<Edge<T>> edges;
    protected Graph<T> graph;

    public Vertex(T element, Graph<T> graph)
    {
        this.element = element;
        edges = new HashSet<Edge<T>>();
        this.graph = graph;
    }

    public virtual void AddLink(Edge<T> edge)
    {
        edges.Add(edge);
    }

    public virtual void RemoveLink(Edge<T> edge)
    {
        edges.Remove(edge);
    }

    public virtual void Destroy() 
    {

    }

    protected virtual void SignalUpdate()
    {
        OnVertexUpdate?.Invoke(this);
    }

}

public class VertexCreator<T>
{
    public virtual Vertex<T> CreateVertex(T element, Graph<T> graph) => new(element, graph);
}

public class Edge<T>
{
    protected Vertex<T> v1;
    protected Vertex<T> v2;


    public Edge(Vertex<T> v1, Vertex<T> v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public virtual void Create()
    {
        v1?.AddLink(this);
        v2?.AddLink(this);
    }
    
    public virtual void Clear()
    {
        v1?.RemoveLink(this);
        v2?.RemoveLink(this);
    }

    public virtual void Hide()
    {

    }

    public virtual void Show()
    {

    }

    public virtual void Destroy() 
    {

    }
}

public class EdgeCreator<T>
{
    public virtual Edge<T> CreateEdge(Vertex<T> v1, Vertex<T> v2) => new(v1, v2);
}

public class Graph<T>
{
    public static void GetGraph (ref Graph<T> graph, Func<T,T,bool> comparator, VertexCreator<T> vertexCreator, EdgeCreator<T> edgeCreator, T[] elems)
    {

        graph?.Destroy();
        graph = new Graph<T>();
        graph.Set(comparator);
        graph.Set(vertexCreator);
        graph.Set(edgeCreator);
        graph.Add(elems);
        graph.CalculateAll();
    }

    private List<Vertex<T>> vertices;
    private List<Edge<T>> edges;
    private Dictionary<T, Vertex<T>> refs;

    private Func<T, T, bool> comparator;
    private EdgeCreator<T> edgeCreator;
    private VertexCreator<T> vertexCreator;


    public Action OnGraphCreated;

    public bool[][] adjacenceMatrix { get; private set; }
    public bool[][] clusteringMatrix { get; private set; }
    public Dictionary<int,int> degreeDistribution { get; private set; }
    public Dictionary<int,int> clusteringDegreeDistribution { get; private set; }
    public int meanDegree { get => vertices.Aggregate(0, (s, elem) => s + elem.degree) / order; }
    public int maxDegree { get => degreeDistribution.Keys.Max(); }
    public int minDegree { get => degreeDistribution.Keys.Min(); }
    public int meanClusterDegree;
    public int maxClusterDegree { get => degreeDistribution.Keys.Max(); }
    public int minClusterDegree {  get => degreeDistribution.Keys.Min(); }
    public int order { get => vertices.Count; }


    private Graph()
    {
        vertices = new List<Vertex<T>>();
        edges = new List<Edge<T>>();
        refs = new Dictionary<T, Vertex<T>>();
    }

    public void Set (Func<T,T,bool> comparator) => this.comparator = comparator;
    public void Set (EdgeCreator<T> edgeCreator) => this.edgeCreator = edgeCreator;
    public void Set (VertexCreator<T> vertexCreator) => this.vertexCreator = vertexCreator;

    public T Get (int i) => (i >= 0 && i < order) ? vertices[i].element : default(T);

    public int IndexOf (T element) => 
        (refs.TryGetValue(element, out Vertex<T> value))? vertices.IndexOf(value) : -1;


    private void Add(T element)
    {
        Vertex<T> e = vertexCreator.CreateVertex(element, this);
        vertices.Add(e);
        refs.Add(element, e);
    }

    private void Add(T[] elements)
    {
        foreach (T element in elements) Add(element);
    }


    public void CalculateAll()
    {
        adjacenceMatrix = CalculateAdjacenceMatrix();
        CreateLinks();
        clusteringMatrix = CalculateClusteringMatrix();
        degreeDistribution = CalculateDegreeDistribution();
        clusteringDegreeDistribution = CalculateClusteringDegreeDistribution();
        OnGraphCreated?.Invoke();
    }

    
    private bool[][] CalculateAdjacenceMatrix()
    {
        bool[][] M = new bool[order][];
        for (int i = 0; i < order; i ++)
        {
            M[i] = new bool[order];
            for (int j = 0; j < order; j++) M[i][j] = comparator(Get(i), Get(j));
        }
        return M;
    }

    private bool[][] CalculateClusteringMatrix()
    {
        bool[][] M = new bool[adjacenceMatrix.Length][];
        for (int i = 0; i < adjacenceMatrix.Length; i++)
        {
            M[i] = new bool[adjacenceMatrix.Length];
            for (int j = 0; j < adjacenceMatrix.Length; j++)
            {
                M[i][j] = false;
                for (int k = 0; k < adjacenceMatrix.Length; k++)
                {
                    if (adjacenceMatrix[i][k] && adjacenceMatrix[k][j])
                    {
                        M[i][j] = adjacenceMatrix[i][j];
                        break;
                    }
                }
            }
        }
        return M;
    }

    private Dictionary<int, int> CalculateDegreeDistribution()
    {
        Dictionary<int, int> distrib = new();
        foreach (Vertex<T> vertex in vertices) {
            if (distrib.ContainsKey(vertex.degree)) distrib[vertex.degree]++;
            else distrib.Add(vertex.degree, 1);
        }
        return distrib;
    }

    private Dictionary<int,int> CalculateClusteringDegreeDistribution()
    {
        return null;
    }

    


    private void CreateLinks()
    {
        ClearLinks();
        for (int i = 0; i < order; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (i != j && adjacenceMatrix[i][j])
                {
                    Edge<T> edge = edgeCreator.CreateEdge(vertices[i], vertices[j]);
                    edges.Add(edge);
                    edge.Create();
                }
            }
        }
    }


    public void ShowLinks()
    {
        foreach (Edge<T> edge in edges) edge.Show();
    }

    public void HideLinks()
    {
        foreach (Edge<T> edge in edges) edge.Hide();
    }

    public void ClearLinks()
    {
        foreach (Edge<T> edge in edges)
        {
            edge.Clear();
        }
        edges.Clear();
    }

    public void ApplyTreatment(Action<Vertex<T>> treatment)
    {
        foreach (Vertex<T> vertex in vertices) treatment(vertex);
    }

    public void ApplyTreamen(Action<Edge<T>> treatment)
    {
        foreach (Edge<T> edge in edges) treatment(edge);
    }

    public void Destroy()
    {
        foreach (Vertex<T> vertex in vertices) vertex.Destroy();
        foreach (Edge<T> edge in edges) edge.Destroy();
    }
}
