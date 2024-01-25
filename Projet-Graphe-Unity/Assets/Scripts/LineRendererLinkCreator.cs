using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColoredVertex : Vertex<GameObject>
{
    private Gradient gradient;
    private MeshRenderer mesh;

    public ColoredVertex(GameObject element, Graph<GameObject> graph, Gradient gradient) : base(element, graph)
    {
        if (!element.TryGetComponent<MeshRenderer>(out mesh))
        {
            CombineMeshes();
            mesh = element.GetComponent<MeshRenderer>();
        }
        this.gradient = gradient;
        graph.OnGraphCreated += SignalUpdate;
    }

    private void CombineMeshes()
    {
        Vector3 pos = element.transform.position;
        Quaternion rot = element.transform.rotation;

        element.transform.position = Vector3.zero;
        element.transform.rotation = Quaternion.identity;

        MeshFilter[] meshFilters = element.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = (meshFilters[i].sharedMesh);
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); 
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true);
        combinedMesh.name = element.name + "Mesh";

        MeshFilter meshFilter = element.gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = combinedMesh;

        MeshRenderer meshRenderer = element.gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        meshRenderer.SetMaterials(
            new List<Material>(
                new Material[]
                { meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial, Resources.Load<Material>("OutlineMat") }));
        //meshRenderer.materials[1].SetFloat("_Float", 0);

        element.transform.position = pos;
        element.transform.rotation = rot;


        foreach (MeshFilter filter in meshFilters)
        {
            GameObject.Destroy(filter.gameObject);
        }

    }

    public void UpdateColor()
    {
        if (graph.maxDegree > 0) mesh.material.color = gradient.Evaluate((float)(degree - graph.minDegree) / (float)(graph.maxDegree - graph.minDegree));
        else mesh.material.color = Color.white;
    }

    public override void AddLink(Edge<GameObject> edge)
    {
        base.AddLink(edge);
    }

    public override void RemoveLink(Edge<GameObject> edge)
    {
        base.RemoveLink(edge);
    }

    protected override void SignalUpdate()
    {
        UpdateColor();
        Satellite satellite = element.GetComponent<Satellite>();
        satellite.degree = degree;
        satellite.tailleComposanteConnexe = ConnexComponent().Count;
        base.SignalUpdate();
    }

    private List<int> ConnexComponent()
    {
        List<List<int>> components = graph.connexComponents;
        foreach (List<int> component in components)
        {
            if (component.Contains(graph.IndexOf(element))) return component;
        }
        return null;
    }

    public override void Destroy()
    {
        base.Destroy();
        graph.OnGraphCreated -= SignalUpdate;
    }
}

public class ColoredVertexCreator : VertexCreator<GameObject>
{
    private Gradient gradient;

    public ColoredVertexCreator(Gradient gradient)
    {
        this.gradient = gradient;
    }

    public override Vertex<GameObject> CreateVertex(GameObject element, Graph<GameObject> graph) => new ColoredVertex(element, graph, gradient);
}

public class LineRendererEdge : Edge<GameObject>
{
    private LineRenderer line;

    public LineRendererEdge(Vertex<GameObject> e1, Vertex<GameObject> e2, LineRenderer linePrefab) : base(e1, e2)
    {
        line = GameObject.Instantiate(linePrefab);
        line.gameObject.SetActive(false);
    }

    public override void Create()
    {
        base.Create();
        line.gameObject.SetActive(true);
        line.SetPosition(0, v1.element.transform.position);
        line.SetPosition(1, v2.element.transform.position);
        line.startColor = v1.element.GetComponent<MeshRenderer>().material.color;
        line.endColor = v2.element.GetComponent<MeshRenderer>().material.color;
        v1.OnVertexUpdate += UpdateColor;
        v2.OnVertexUpdate += UpdateColor;
    }

    public override void Clear()
    {
        base.Clear();
        line.gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        line.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();
        line.gameObject.SetActive(false);
    }

    public override void Destroy()
    {
        base.Destroy();
        GameObject.Destroy(line.gameObject);
        v1.OnVertexUpdate -= UpdateColor;
        v2.OnVertexUpdate -= UpdateColor;
    }

    private void UpdateColor(Vertex<GameObject> v)
    {
        if ( v == v1) line.startColor = v1.element.GetComponent<MeshRenderer>().material.color;
        if ( v == v2) line.endColor = v2.element.GetComponent<MeshRenderer>().material.color;
    }
}


public class LineRendererEdgeCreator : EdgeCreator<GameObject>
{
    private LineRenderer linePrefab;
    public LineRendererEdgeCreator(LineRenderer linePrefab)
    {
        this.linePrefab = linePrefab;
    }
    public override Edge<GameObject> CreateEdge(Vertex<GameObject> v1, Vertex<GameObject> v2) => new LineRendererEdge(v1, v2, linePrefab);
}

public class WeightedEdgeCreator : EdgeCreator<GameObject>
{
    private Func<GameObject, GameObject, float> weightFunc;
    private EdgeCreator<GameObject> subCreator;

    public WeightedEdgeCreator(Func<GameObject, GameObject, float> weightFunc, EdgeCreator<GameObject> subCreator)
    {
        this.weightFunc = weightFunc;
        this.subCreator = subCreator;
    }

    public override Edge<GameObject> CreateEdge(Vertex<GameObject> v1, Vertex<GameObject> v2)
    {
        Edge<GameObject> edge  = subCreator.CreateEdge(v1, v2);
        edge.weightFunc = weightFunc;
        return edge;
    }
}