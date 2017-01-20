using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    public string id;
    public string SourceID;

    public ParticleSystem endEffect;

    public bool HasContact;
    public Vector3 PointOfContact;
    public Vector3 SourcePosition;

    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private Vector3[] LinePosition;
    public Vector3 MaxPosition;
    public Mirror ContactedMirror;
    public TheTarget ContactedTarget;

    private Transform endEffectTransform;

    void Start()
    {
        LinePosition = new Vector3[2];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(GameManager.instance.LaserWidth, GameManager.instance.LaserWidth);

        ///Set The Sprite Renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        /// Set End Effect
        endEffect = GetComponentInChildren<ParticleSystem>();
        if (endEffect) endEffectTransform = endEffect.transform;

        /// Set Max Distance
        MaxPosition = new Vector3(transform.position.x + 50, transform.position.y, transform.position.z);
    }

    public void StartMirroring () {
        if (spriteRenderer != null)
            spriteRenderer.color = GameManager.instance.MirrorHitColor;

        if (!HasContact)
        {
            HasContact = true;
            GameManager.instance.AddConnectedMirror(this);
        }

        RenderLeser();
    }

    public void StopMirroring()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = GameManager.instance.MirrorNormalColor;

        if(HasContact)
        {
            HasContact = false;
            //GameManager.instance.RemoveConnectedMirror(this);
        }

        lineRenderer.SetVertexCount(0);
        SourceID = "";
        SourcePosition = Vector3.zero;
        for(int i = 0; i < LinePosition.Length; i++)
        {
            LinePosition[i] = Vector3.zero;
        }

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
    }

    void UpdateLength()
    {
        float DegreePhi = 0;
        if(SourcePosition.x < PointOfContact.x)
        {
            DegreePhi = 180;
        }
        else
        {
            DegreePhi = 0;
        }

        float DegreeRotation = DegreePhi + (transform.eulerAngles.z * 2) + (Mathf.Atan(((Mathf.PI/2) + ((PointOfContact.x - SourcePosition.x) / (PointOfContact.y - SourcePosition.y)))));

        MaxPosition = new Vector3(PointOfContact.x + (float)Mathf.Cos(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, PointOfContact.y + (float)Mathf.Sin(Mathf.Deg2Rad * DegreeRotation) * GameManager.instance.MaxLength, PointOfContact.z);

        RaycastHit2D hit2D = Physics2D.Linecast(PointOfContact, MaxPosition);

        if (hit2D)
        {
            Collider2D theObj = hit2D.collider;
            if (theObj.CompareTag("mirror"))
            {
                if (theObj.GetComponent<Mirror>() != null)
                {
                    if(theObj.GetComponent<Mirror>().id != id && theObj.GetComponent<Mirror>().id != SourceID)
                    {
                        if (!ContactedMirror)
                        {
                            ContactedMirror = hit2D.collider.GetComponent<Mirror>();
                        }
                        else
                        {
                            ContactedMirror.SourcePosition = PointOfContact;
                            Vector3 ThePointOfContact = new Vector3((hit2D.point.x - PointOfContact.x) > 0 ? hit2D.point.x - 0.05f : hit2D.point.x + 0.05f, (hit2D.point.y - PointOfContact.y) > 0 ? hit2D.point.y + 0.05f : hit2D.point.y - 0.05f, PointOfContact.z);
                            ContactedMirror.PointOfContact = ThePointOfContact;
                            ContactedMirror.SourceID = id;
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
            }else if (theObj.CompareTag("target"))
            {
                if (theObj.GetComponent<TheTarget>() != null)
                {

                    if (!ContactedTarget)
                    {
                        ContactedTarget = hit2D.collider.GetComponent<TheTarget>();
                    }
                    else
                    {
                        ContactedTarget.CheckFinish();
                    }

                    Vector3 ThePointOfContact = new Vector3((hit2D.point.x - PointOfContact.x) > 0 ? hit2D.point.x - 0.05f : hit2D.point.x + 0.05f, (hit2D.point.y - PointOfContact.y) > 0 ? hit2D.point.y + 0.05f : hit2D.point.y - 0.05f, PointOfContact.z);
                    LinePosition[1] = ThePointOfContact;
                    if (endEffect)
                    {
                        endEffectTransform.position = hit2D.point;
                        if (!endEffect.isPlaying) endEffect.Play();
                        return;
                    }
                }
                else
                {
                    LinePosition[1] = MaxPosition;
                    ContactedTarget = null;
                }
            }
            else
            {
                LinePosition[1] = MaxPosition;
                if (ContactedMirror)
                    ContactedMirror.StopMirroring();

                ContactedTarget = null;
                ContactedMirror = null;
            }
        }
        else
        {
            LinePosition[1] = MaxPosition;
            if (ContactedMirror)
                ContactedMirror.StopMirroring();


            ContactedTarget = null;
            ContactedMirror = null;
        }
    }
}
