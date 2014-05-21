using UnityEngine;
using System.Collections;

public class DeliberativeCommunicateWithAgent : MonoBehaviour {

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 2;
    public Material mat;

    private LineRenderer lineRenderer;

    DeliberativeFireFighter scrpt;

    void Awake()
    {
        lineRenderer = transform.gameObject.AddComponent<LineRenderer>();
    }

    void Start()
    {
        scrpt = transform.parent.GetComponent<DeliberativeFireFighter>();
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
                DeliberativeFireFighter other = col.GetComponent<DeliberativeFireFighter>();
                if (other.objective == DeliberativeFireFighter.STATE.RECRUIT_A_HELPER)
                {
                    if(scrpt.objective == DeliberativeFireFighter.STATE.DEFAULT)
                    {
                        scrpt.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FIRE, other.placeTogo);
                    }
                }
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
