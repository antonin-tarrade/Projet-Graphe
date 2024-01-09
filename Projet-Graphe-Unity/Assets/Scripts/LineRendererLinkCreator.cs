using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColoredVertex : Vertex<GameObject>
{
    private Gradient gradient;
    private MeshRenderer mesh;

    public ColoredVertex(GameObject element, Graph<GameObject> graph, Gradient gradient) : base(element, graph)
    {
        mesh = element.GetComponent<MeshRenderer>();
        this.gradient = gradient;
        graph.OnGraphCreated += SignalUpdate;
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
        base.SignalUpdate();
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