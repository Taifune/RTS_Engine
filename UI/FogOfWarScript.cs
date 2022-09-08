using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarScript : MonoBehaviour
{
    public GameObject m_fogOfWarPlane;
    public Transform reviewer;
    public LayerMask m_fog_layer;
    public float view_distance;
    public float m_radiusofviewSqr { get { return view_distance * view_distance; } }

    private Mesh m_mesh;
    private Vector3[] m_verticles;
    private Color[] m_colors;
    


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {

        Ray r = new Ray(transform.position, reviewer.position - transform.position);
        RaycastHit foghit;
        if(Physics.Raycast(r, out foghit,1000,m_fog_layer,QueryTriggerInteraction.Collide))
        {
            for (int i=0; i<m_verticles.Length; i++)
            {
                Vector3 v = m_fogOfWarPlane.transform.TransformPoint(m_verticles[i]);
                float dist = Vector3.SqrMagnitude(v - foghit.point);
                if(dist< m_radiusofviewSqr)
                {
                    float alpha = Mathf.Min(m_colors[i].a, dist / m_radiusofviewSqr);
                    m_colors[i].a = alpha;

                }
            }
            UpdateColor();

        }
        
    }

    void Initialize()
    {

        //view_distance = 10f;


        m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_verticles = m_mesh.vertices;
        m_colors = new Color[m_verticles.Length];
        for (int i =0; i < m_colors.Length; i++)
        {
            m_colors[i] = Color.black;
        }
        UpdateColor();

    }

    void UpdateColor()
    {

        m_mesh.colors = m_colors;
    }
}
