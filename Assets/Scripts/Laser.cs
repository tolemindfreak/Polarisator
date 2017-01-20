using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Laser : MonoBehaviour {

    
    public ParticleSystem endEffect;

    public Mirror ContactedMirror;

    private LineRenderer lineRenderer;
    private Vector3[] LinePosition;
    private Transform endEffectTransform;
    private Vector3 MaxPosition;

	void Start () {
        LinePosition = new Vector3[2];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(GameManager.instance.LaserWidth, GameManager.instance.LaserWidth);

        /// Set End Effect
        endEffect = GetComponentInChildren<ParticleSystem>();
        if (endEffect) endEffectTransform = endEffect.transform;

        /// Set Max Distance
        MaxPosition = new Vector3(transform.position.x + 50, transform.position.y, transform.position.z);
	}
	
	void Update () {
        RenderLaser();
	}

    void RenderLaser () {
        UpdateLength();
        lineRenderer.SetColors(GameManager.instance.LaserColor, GameManager.instance.LaserColor);
        LinePosition[0] = transform.position;
        lineRenderer.SetVertexCount(LinePosition.Length);
        //lineRenderer.numPositions = LinePosition.Length;
        for (int i = 0; i < LinePosition.Length; i++)
        {
            lineRenderer.SetPosition(i, LinePosition[i]);
        }
    }

    void UpdateLength () {

        float DegreeRotation = transform.eulerAngles.z;

        MaxPosition = new Vector3(transform.position.x + (float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, transform.position.y + (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, transform.position.z);

        RaycastHit2D hit2D = Physics2D.Linecast(transform.position,MaxPosition);

        if (hit2D)
        {
            Collider2D theObj = hit2D.collider;
            if (theObj.CompareTag("mirror"))
            {
                if(theObj.GetComponent<Mirror>() != null)
                {
                    if (!ContactedMirror)
                        ContactedMirror = hit2D.collider.GetComponent<Mirror>();
                    else
                    {
                        ContactedMirror.SourcePosition = transform.position;
                        Vector3 ThePointOfContact = new Vector3((hit2D.point.x - transform.position.x) > 0 ? hit2D.point.x - 0.05f : hit2D.point.x + 0.05f, (hit2D.point.y - transform.position.y) > 0 ? hit2D.point.y + 0.05f : hit2D.point.y - 0.05f, transform.position.z);
                        ContactedMirror.PointOfContact = new Vector3((hit2D.point.x - transform.position.x) > 0 ? hit2D.point.x - 0.05f: hit2D.point.x + 0.05f, (hit2D.point.y - transform.position.y) > 0 ? hit2D.point.y + 0.05f : hit2D.point.y - 0.05f,transform.position.z);
                        ContactedMirror.StartMirroring();
                        LinePosition[1] = ThePointOfContact;
                        if (endEffect)
                        {
                            endEffectTransform.position = hit2D.point;
                            if (!endEffect.isPlaying) endEffect.Play();
                            return;
                        }
                    }
                }
            }
            else
            {
                LinePosition[1] = MaxPosition;
                if (ContactedMirror)
                    ContactedMirror.StopMirroring();

                ContactedMirror = null;
            }
        }
        else
        {
            LinePosition[1] = MaxPosition;
            if (ContactedMirror)
                ContactedMirror.StopMirroring();

            ContactedMirror = null;
        }
    }
}
