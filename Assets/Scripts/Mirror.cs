using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    public string id;

    public ParticleSystem endEffect;

    public bool HasContact;
    public Vector3 PointOfContact;
    public Vector3 SourcePosition;

    private LineRenderer lineRenderer;
    private Vector3[] LinePosition;
    private Vector3 MaxPosition;
    public Mirror ContactedMirror;

    private Transform endEffectTransform;

    void Start()
    {
        LinePosition = new Vector3[2];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(GameManager.instance.LaserWidth, GameManager.instance.LaserWidth);

        /// Set End Effect
        endEffect = GetComponentInChildren<ParticleSystem>();
        if (endEffect) endEffectTransform = endEffect.transform;

        /// Set Max Distance
        MaxPosition = new Vector3(transform.position.x + 50, transform.position.y, transform.position.z);
    }

    public void StartMirroring () {
        Debug.Log("Masuk Start Mirroring");
        RenderLeser();
        //Vector3 dir = (SourcePosition - PointOfContact).normalized;
        //Debug.DrawLine(PointOfContact, PointOfContact + dir * 10, Color.red, Mathf.Infinity);

    }

    void RenderLeser()
    {
        UpdateLength();
        lineRenderer.SetColors(GameManager.instance.LaserColor, GameManager.instance.LaserColor);
        LinePosition[0] = PointOfContact;
        lineRenderer.SetVertexCount(LinePosition.Length);
        //lineRenderer.numPositions = LinePosition.Length;
        for (int i = 0; i < LinePosition.Length; i++)
        {
            lineRenderer.SetPosition(i, LinePosition[i]);
        }

        if (ContactedMirror)
            ContactedMirror.HasContact = true;
    }

    void UpdateLength()
    {
        float DegreeRotation = 180 + (transform.eulerAngles.z * 2) + (Mathf.Atan(((Mathf.PI/2) + ((PointOfContact.x - SourcePosition.x) / (PointOfContact.y - SourcePosition.y)))));

        MaxPosition = new Vector3(PointOfContact.x + (float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, PointOfContact.y + (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, PointOfContact.z);


        RaycastHit2D hit2D = Physics2D.Raycast(PointOfContact, new Vector2((float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation), (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation)), GameManager.instance.MaxLength);

        if (hit2D)
        {

            Collider2D theObj = hit2D.collider;
            if (hit2D.collider.GetComponent<Mirror>() != null)
            {

                if (!ContactedMirror)
                {
                    if(hit2D.collider.GetComponent<Mirror>().id != id)
                        ContactedMirror = hit2D.collider.GetComponent<Mirror>();
                }
                    
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
