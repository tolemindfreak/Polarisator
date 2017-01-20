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
        transform.eulerAngles = new Vector3(0f, 90f, 0f);
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

        if (ContactedMirror)
            ContactedMirror.HasContact = true;
    }

    void UpdateLength () {

        float DegreeRotation = transform.eulerAngles.z;

        MaxPosition = new Vector3(transform.position.x + (float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, transform.position.y + (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, transform.position.z);


        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, new Vector2((float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation), (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation)), GameManager.instance.MaxLength);
        if (hit2D)
        {

            Collider2D theObj = hit2D.collider;
            if (hit2D.collider.GetComponent<Mirror>() != null)
            {
                if(!ContactedMirror)
                    ContactedMirror = hit2D.collider.GetComponent<Mirror>();
                else
                {
                    ContactedMirror.StartMirroring();
                }

            }
                

            if (theObj.CompareTag("mirror"))
            {
                LinePosition[1] = hit2D.point;
                if (endEffect)
                {
                    endEffectTransform.position = hit2D.point;
                    if (!endEffect.isPlaying) endEffect.Play();
                    return;
                }
            }
        }
        else
        {
            LinePosition[1] = MaxPosition;
            ContactedMirror = null;
        }
    }
}
