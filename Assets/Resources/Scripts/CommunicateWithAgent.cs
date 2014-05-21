using UnityEngine;
using System.Collections;

public class CommunicateWithAgent : MonoBehaviour {

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 2;
    public Material mat;

    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;

    // Use this for initialization

    void Awake()
    {
        lineRenderer = transform.gameObject.AddComponent<LineRenderer>();
    }

    void Start()
    {
        lineRenderer.material = mat;
        lineRenderer.SetColors(c1, c2);
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetWidth(0.13F, 0.13F);
        lineRenderer.SetVertexCount(lengthOfLineRenderer);
    }

    void OnTriggerEnter(Collider col)
    {
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter")
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, col.transform.position);
            }
            else if (col.gameObject.tag == "Builder")
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, col.transform.position);
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter")
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, col.transform.position);
            }
            else if (col.gameObject.tag == "Builder")
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, col.transform.position);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter")
            {
                lineRenderer.enabled = false;
            }
            else if (col.gameObject.tag == "Builder")
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
