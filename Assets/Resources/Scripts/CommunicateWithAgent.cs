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
        lineRenderer.SetPosition(0, transform.parent.position);
        lineRenderer.SetPosition(1, transform.parent.position);
    }

    void builderInteraction(Collider col)
    {
        Builder me;
        me = transform.parent.GetComponent<Builder>();
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter" && me.WarnSomebody)
            {
                DeliberativeFireFighter other = col.gameObject.GetComponent<DeliberativeFireFighter>();
                if (other.objective == DeliberativeFireFighter.STATE.DEFAULT && !other.leader)
                {
                    other.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FAR_AWAY_FIRE, me.firePosition);
                    Debug.Log("IM BEING CALLED HERE");
                    me.WarnSomebody = false;
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

    void firefigtherInteraction(Collider col)
    {
        DeliberativeFireFighter me;
        me = transform.parent.GetComponent<DeliberativeFireFighter>();
        
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter")
            {
                DeliberativeFireFighter other = col.gameObject.GetComponent<DeliberativeFireFighter>();
                if(me.objective == DeliberativeFireFighter.STATE.RECRUIT_A_HELPER)
                {
                    if (other.objective == DeliberativeFireFighter.STATE.DEFAULT && !other.leader)
                    {
                        other.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FAR_AWAY_FIRE, me.placeTogo);
                        me.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FAR_AWAY_FIRE, me.placeTogo);
                        Debug.Log("IM BEING CALLED HERE");
                    }
                }
                else
                {
                    if (me.objective == DeliberativeFireFighter.STATE.DEFAULT && !me.leader)
                    {
                        if (other.objective == DeliberativeFireFighter.STATE.RECRUIT_A_HELPER)
                        {
                            other.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FAR_AWAY_FIRE, other.placeTogo);
                            me.setState(DeliberativeFireFighter.STATE.GO_HELP_A_FAR_AWAY_FIRE, other.placeTogo);
                            Debug.Log("IM BEING CALLED HERE");
                        }
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

    void OnTriggerEnter(Collider col)
    {
        if (transform.parent.tag == "FireFighter")
            firefigtherInteraction(col);
        else if (transform.parent.tag == "Builder")
            builderInteraction(col);
    }

    void OnTriggerStay(Collider col)
    {
        if (transform.parent.tag == "FireFighter")
            firefigtherInteraction(col);
        else if (transform.parent.tag == "Builder")
            builderInteraction(col);
    }

    void OnTriggerExit(Collider col)
    {
        if (transform.parent.position != col.transform.position)
        {
            if (col.gameObject.tag == "FireFighter")
            {
                lineRenderer.enabled = false;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, transform.parent.position);
            }
            else if (col.gameObject.tag == "Builder")
            {
                lineRenderer.enabled = false;
                lineRenderer.SetPosition(0, transform.parent.position);
                lineRenderer.SetPosition(1, transform.parent.position);
            }
        }
    }
}
