using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineRendererLink : GraphLink<GameObject>
{
    private LineRenderer line;

    public LineRendererLink(GraphElement<GameObject> e1, GraphElement<GameObject> e2, LineRenderer linePrefab) : base(e1, e2)
    {
        line = GameObject.Instantiate(linePrefab);
        line.gameObject.SetActive(false);
    }

    public override void Create()
    {
        base.Create();
        line.gameObject.SetActive(true);
        line.SetPosition(0, e1.element.transform.position);
        line.SetPosition(1, e2.element.transform.position);
    }

    public override void Clear()
    {
        base.Clear();
        line.gameObject.SetActive(false);
    }

    public override void Destroy()
    {
        base.Destroy();
        GameObject.Destroy(line.gameObject);
    }
}


public class LineRendererLinkCreator : GraphLinkCreator<GameObject>
{
    private LineRenderer linePrefab;
    public LineRendererLinkCreator(LineRenderer linePrefab)
    {
        this.linePrefab = linePrefab;
    }
    public override GraphLink<GameObject> CreateGraphLink(GraphElement<GameObject> e1, GraphElement<GameObject> e2)
    {
        return new LineRendererLink(e1, e2, linePrefab);
    }
}