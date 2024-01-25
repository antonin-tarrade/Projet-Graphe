using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Vertex<T>
{
    public T element { get; private set; }
    public int degree { get => edges.Count; }

    public event Action<Vertex<T>> OnVertexUpdate;

    public ISet<Edge<T>> edges { get; protected set; }
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

    public float weight => weightFunc(v1.element, v2.element);
    public Func<T,T,float> weightFunc;

    public Edge(Vertex<T> v1, Vertex<T> v2)
    {
        this.v1 = v1;
        this.v2 = v2;
        weightFunc = ((u,v) => 0);
    }

    public Edge(Vertex<T> v1, Vertex<T> v2, Func<T,T,float> weightFunc) : this(v1,v2)
    {
        this.weightFunc = weightFunc;
    }

    public Vertex<T> GetNeighbour(Vertex<T> vertex)
    {
        if (vertex == v1) return v2;
        if (vertex == v2) return v1;
        return null;
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
    private Edge<T>[][] edgeMatrix;

    private Func<T, T, bool> comparator;
    private EdgeCreator<T> edgeCreator;
    private VertexCreator<T> vertexCreator;


    public event Action OnGraphCreated;

    public bool[][] adjacenceMatrix { get; private set; }
    public float[][] shortestDistanceMatrix { get; private set; }
    public bool[][] clusteringMatrix { get; private set; }
    public Dictionary<int,int> degreeDistribution { get; private set; }
    public Dictionary<int,int> clusteringDegreeDistribution { get; private set; }
    public List<List<int>> connexComponents { get; private set; }
    public Dictionary<int,int> connexComponentsDistribution { get; private set; }
    public ISet<ISet<int>> clicks { get; private set; }
    public int meanDegree { get => vertices.Aggregate(0, (s, elem) => s + elem.degree) / order; }
    public int maxDegree { get => degreeDistribution.Keys.Max(); }
    public int minDegree { get => degreeDistribution.Keys.Min(); }
    public int meanClusterDegree { get => clusteringDegreeDistribution.Aggregate(0, (s,elem) => elem.Value*elem.Key + s) / order; }
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

    private List<int> GetNeighbours(bool[][] matrix, int i)
    {
        List<int> neighbours = new();
        for (int j = 0; j < order; j++)
        {
            if (i != j && matrix[i][j]) neighbours.Add(j);
        }
        return neighbours;
    }


    public void CalculateAll()
    {
        adjacenceMatrix = CalculateAdjacenceMatrix();
        CreateLinks();
        shortestDistanceMatrix = CalculateShortestDistanceMatrix(adjacenceMatrix, edgeMatrix);
        clusteringMatrix = CalculateClusteringMatrix();
        degreeDistribution = CalculateDegreeDistribution(adjacenceMatrix);
        clusteringDegreeDistribution = CalculateDegreeDistribution(clusteringMatrix);
        connexComponents = CalculateConnexComponents(adjacenceMatrix);
        connexComponentsDistribution = CalculateConnexComponentsDistribution(connexComponents);
        

        OnGraphCreated?.Invoke();     
    }

    //public void CalculateClicks()
    //{
    //    clicks = CalculateClicks(adjacenceMatrix);
    //}

    
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

    private float[][] CalculateShortestDistanceMatrix(bool[][] matrix, Edge<T>[][] edgeMatrix)
    {
        float[][] M = new float[order][];
        for (int i = 0; i < order; i++)
        {
            M[i] = new float[order];
            for (int j = 0; j < order; j++)
                M[i][j] = float.PositiveInfinity;
        }

        for (int i = 0; i < order; i++)
        {
            M[i][i] = 0;
            List<int> availables = new();
            for (int e = 0; e < order; e++)
                availables.Add(e);
            int pivot = i;
            while(availables.Count > 0)
            {
                pivot = Array.IndexOf(M[i], availables.Select(j => M[i][j]).Min());
                if (!availables.Contains(pivot)) break;
                availables.Remove(pivot);
                for (int j = 0; j < order; j++)
                {
                    
                    if (availables.Contains(j) && matrix[pivot][j] && M[i][pivot] + edgeMatrix[pivot][j].weight < M[i][j])
                    {
                        M[i][j] = M[i][pivot] + edgeMatrix[pivot][j].weight;
                        M[j][i] = M[i][pivot] + edgeMatrix[pivot][j].weight;
                    }
                        
                }                       
            }
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

    private Dictionary<int,int> CalculateDegreeDistribution(bool[][] matrix)
    {
        int length = matrix.Length;
        Dictionary<int, int> distrib = new();
        for (int i = 0; i < length;  i++)
        {
            int degree = 0;
            for (int j = 0; j < length; j++)
            {
                if (matrix[i][j]) degree++;
            }
            if (distrib.ContainsKey(degree)) distrib[degree]++;
            else distrib.Add(degree, 1);
        }
        return distrib;
    }


    private List<List<int>> CalculateConnexComponents(bool[][] matrix)
    {

        List<List<int>> components = new();
        
        List<Vertex<T>> freeVertices = new(vertices);

        while (freeVertices.Count > 0)
        {
            List<int> component = new();
            Vertex<T> origin = freeVertices[0];
            Queue<Vertex<T>> toHandle = new();
            toHandle.Enqueue(origin);

            while (toHandle.TryDequeue(out Vertex<T> next)) {
                
                component.Add(IndexOf(next.element));
                freeVertices.Remove(next);
                foreach (Edge<T> edge in next.edges)
                {
                    Vertex<T> neighbour = edge.GetNeighbour(next);
                    if (!toHandle.Contains(neighbour) && freeVertices.Contains(neighbour))
                    {
                        toHandle.Enqueue(neighbour);
                    }
                }
            }
            components.Add(component);
        }
        return components;
    }

    private Dictionary<int,int> CalculateConnexComponentsDistribution(List<List<int>> components)
    {
        Dictionary<int, int> distrib = new();
        foreach (List<int> l in components)
        {
            if (distrib.TryGetValue(l.Count, out int v)) distrib[l.Count] = v+1;
            else distrib.Add(l.Count,1);
        }
        return distrib;
    }

    //private ISet<ISet<int>> CalculateClicks(bool[][] matrix)
    //{
    //    ISet<ISet<int>> clicks = new HashSet<ISet<int>>();
    //    BB(matrix, ref clicks, new HashSet<int>(), new HashSet<int>(refs.Keys.Select<T, int>(e => IndexOf(e))), new HashSet<int>());
    //    return clicks;
    //}

    //private void BB(bool[][] matrix, ref ISet<ISet<int>> clicks, ISet<int> click, ISet<int> vertices, ISet<int> handled)
    //{
    //    if (vertices.Count == 0 && handled.Count == 0)
    //        if (click.Count > 0) clicks.Add(click);
    //    int pivot = vertices.Union(handled).OrderBy(i => refs[Get(i)].degree).Last();
    //    IEnumerable<int> iterable = vertices.Except(GetNeighbours(matrix, pivot));
    //    foreach (int v in iterable)
    //    {
    //        List<int> neighbours = GetNeighbours(matrix, v);
    //        BB(matrix, ref clicks, new HashSet<int>(click.Union(new HashSet<int>(v))), new HashSet<int>(vertices.Except(neighbours)), new HashSet<int>(handled.Union(neighbours)));
    //        vertices = (ISet<int>)vertices.Except(new List<int>(v));
    //        handled = (ISet<int>)handled.Except(new List<int>(v));
    //    }
    //}


    


    private void CreateLinks()
    {
        ClearLinks();
        edgeMatrix = new Edge<T>[order][];
        for (int i = 0; i < order; i++) edgeMatrix[i] = new Edge<T>[order];
        for (int i = 0; i < order; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (i != j && adjacenceMatrix[i][j])
                {
                    Edge<T> edge = edgeCreator.CreateEdge(vertices[i], vertices[j]);
                    edges.Add(edge);
                    edgeMatrix[i][j] = edge;
                    edgeMatrix[j][i] = edge;
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

    public void ApplyTreatment(Action<Edge<T>> treatment)
    {
        foreach (Edge<T> edge in edges) treatment(edge);
    }

    public void Destroy()
    {
        foreach (Vertex<T> vertex in vertices) vertex.Destroy();
        foreach (Edge<T> edge in edges) edge.Destroy();
    }
}
