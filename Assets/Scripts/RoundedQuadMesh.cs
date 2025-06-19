using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class RoundedQuadMesh : MonoBehaviour
{
    public float RoundEdges = 0.5f;
    public float RoundTopLeft = 0.0f;
    public float RoundTopRight = 0.0f;
    public float RoundBottomLeft = 0.0f;
    public float RoundBottomRight = 0.0f;
    public bool UsePercentage = true;
    public Rect rect = new Rect(-0.5f,-0.5f,1f,1f);
    public float Scale = 1f;
    public int CornerVertexCount = 8;
    public bool CreateUV = true;
    public bool FlipBackFaceUV = false;
    public bool DoubleSided = false;
    [Range(0f, 1f)]
    public float backsideOffset = 0.3f;
    public bool AutoUpdate = true;
    public bool AudoUpdateOnValidate = false;

    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;
    private Vector3[] m_Vertices;
    private Vector3[] m_Normals;
    private Vector2[] m_UV;
    private int[] m_Triangles;
    private int[] m_SideTriangles;
    private List<int> m_FinalTriangles = new List<int>();
    private int totalLen;
    private int halfLen;
    private int sideTriangleGroup;

    private void OnValidate()
    {
        if (AudoUpdateOnValidate)
        {
            Start();
            UpdateMesh();
        } 
     }

    void Start ()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        if (m_MeshFilter == null)
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
        m_Mesh = new Mesh();
        m_MeshFilter.sharedMesh = m_Mesh;
        UpdateMesh();
    }

    public Mesh UpdateMesh()
    {
        if (CornerVertexCount<2)
            CornerVertexCount = 2;
        int sides = DoubleSided ? 2 : 1;
        int vCount = CornerVertexCount * 4 * sides + sides; //+sides for center vertices

        int triCount = (CornerVertexCount * 4) * sides;
        //if (DoubleSided) triCount += (CornerVertexCount * 4) - 2;

        if (m_Vertices == null || m_Vertices.Length != vCount)
        {
            m_Vertices = new Vector3[vCount];
            m_Normals = new Vector3[vCount];
        }
        if (m_Triangles == null || m_Triangles.Length != triCount * 3)
            m_Triangles = new int[triCount * 3];
        if (CreateUV && (m_UV == null || m_UV.Length != vCount))
        { 
            m_UV = new Vector2[vCount];
        }
        int count = CornerVertexCount * 4;
        if (CreateUV)
        {
            m_UV[0] = Vector2.one *0.5f;
            if (DoubleSided)
                m_UV[count + 1] = m_UV[0];
        }
        float tl = Mathf.Max(0, RoundTopLeft + RoundEdges);
        float tr = Mathf.Max(0, RoundTopRight + RoundEdges);
        float bl = Mathf.Max(0, RoundBottomLeft + RoundEdges);
        float br = Mathf.Max(0, RoundBottomRight + RoundEdges);
        float f = Mathf.PI * 0.5f / (CornerVertexCount - 1);
        float a1 = 1f;
        float a2 = 1f;
        float x = 1f;
        float y = 1f;
        Vector2 rs = Vector2.one;
        if (UsePercentage)
        {
            rs = new Vector2(rect.width, rect.height) * 0.5f;
            if (rect.width > rect.height)
                a1 = rect.height / rect.width;
            else
                a2 = rect.width / rect.height;
            tl = Mathf.Clamp01(tl);
            tr = Mathf.Clamp01(tr);
            bl = Mathf.Clamp01(bl);
            br = Mathf.Clamp01(br);
        }
        else
        {
            x = rect.width * 0.5f;
            y = rect.height * 0.5f;
            if (tl + tr > rect.width)
            {
                float b = rect.width / (tl + tr);
                tl *= b;
                tr *= b;
            }
            if (bl + br > rect.width)
            {
                float b = rect.width / (bl + br);
                bl *= b;
                br *= b;
            }
            if (tl + bl > rect.height)
            {
                float b = rect.height / (tl + bl);
                tl *= b;
                bl *= b;
            }
            if (tr + br > rect.height)
            {
                float b = rect.height / (tr + br);
                tr *= b;
                br *= b;
            }
        }
        m_Vertices[0] = rect.center * Scale;

        if (DoubleSided)
        {
            m_Vertices[count + 1] = (rect.center * Scale);
            m_Vertices[count + 1] += new Vector3(0f, 0f, backsideOffset);
        }
           
        for (int i = 0; i < CornerVertexCount; i++ )
        {
            float s = Mathf.Sin((float)i * f);
            float c = Mathf.Cos((float)i * f);
            Vector2 v1 = new Vector3(-x + (1f - c) * tl * a1, y - (1f - s) * tl * a2);
            Vector2 v2 = new Vector3(x - (1f - s) * tr * a1, y - (1f - c) * tr * a2);
            Vector2 v3 = new Vector3(x - (1f - c) * br * a1, -y + (1f - s) * br * a2);
            Vector2 v4 = new Vector3(-x + (1f - s) * bl * a1, -y + (1f - c) * bl * a2);

            m_Vertices[1 + i] = (Vector2.Scale(v1, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount + i] = (Vector2.Scale(v2, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount * 2 + i] = (Vector2.Scale(v3, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount * 3 + i] = (Vector2.Scale(v4, rs) + rect.center) * Scale;
            if (CreateUV)
            {
                if(!UsePercentage)
                {
                    Vector2 adj = new Vector2(2f/rect.width, 2f/rect.height);
                    v1 = Vector2.Scale(v1, adj);
                    v2 = Vector2.Scale(v2, adj);
                    v3 = Vector2.Scale(v3, adj);
                    v4 = Vector2.Scale(v4, adj);
                }
                m_UV[1 + i] = v1 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 1 + i] = v2 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 2 + i] = v3 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 3 + i] = v4 * 0.5f + Vector2.one * 0.5f;
            }
            if (DoubleSided)
            {
                m_Vertices[1 + CornerVertexCount * 8 - i] = m_Vertices[1 + i] + new Vector3(0f, 0f, backsideOffset);
                m_Vertices[1 + CornerVertexCount * 7 - i] = m_Vertices[1 + CornerVertexCount + i] + new Vector3(0f, 0f, backsideOffset);
                m_Vertices[1 + CornerVertexCount * 6 - i] = m_Vertices[1 + CornerVertexCount * 2 + i] + new Vector3(0f, 0f, backsideOffset);
                m_Vertices[1 + CornerVertexCount * 5 - i] = m_Vertices[1 + CornerVertexCount * 3 + i] + new Vector3(0f, 0f, backsideOffset);
                if (CreateUV)
                {
                    m_UV[1 + CornerVertexCount * 8 - i] = v1 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 7 - i] = v2 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 6 - i] = v3 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 5 - i] = v4 * 0.5f + Vector2.one * 0.5f;
                }
            }
        }
        for (int i = 0; i < count + 1;i++ )
        {
            m_Normals[i] = -Vector3.forward;
            if (DoubleSided)
            {
                m_Normals[count + 1 + i] = Vector3.forward;
                if (FlipBackFaceUV)
                {
                    Vector2 uv = m_UV[count+1+i];
                    uv.x = 1f - uv.x;
                    m_UV[count+1+i] = uv;
                }
            }
        }
        for (int i = 0; i < count; i++)
        {
            m_Triangles[i*3    ] = 0;
            m_Triangles[i*3 + 1] = i + 1;
            m_Triangles[i*3 + 2] = i + 2;
            if (DoubleSided)
            {
                m_Triangles[(count + i) * 3] = count+1;
                m_Triangles[(count + i) * 3 + 1] = count+1 +i + 1;
                m_Triangles[(count + i) * 3 + 2] = count+1 +i + 2;
            }
        }

        m_Triangles[count * 3 - 1] = 1;
        if (DoubleSided)
        {
            m_Triangles[m_Triangles.Length - 1] = count + 1 + 1;
        }

        m_FinalTriangles.Clear();
        m_FinalTriangles.AddRange(m_Triangles);

        //Sort out second side
        if (DoubleSided)
        {  
            m_SideTriangles = new int[(CornerVertexCount * 4) * 6];

            totalLen = m_Vertices.Length;
            halfLen = (m_Vertices.Length / 2) - 1;
            sideTriangleGroup = 0;

            int lastVert = 1;
            for (int i = 1; i <= halfLen; i++)
            {
                //Debug Lines:
                /*Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - i], Color.red);
                
                if (i != halfLen)
                    Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - i - 1], Color.green);
                else
                    Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - 1], Color.cyan);*/

                //Set side triangles:
                m_SideTriangles[sideTriangleGroup] = i;
                m_SideTriangles[sideTriangleGroup + 1] = totalLen - i;
                m_SideTriangles[sideTriangleGroup + 2] = (i != halfLen) ? totalLen - i - 1 : totalLen - 1;

                m_SideTriangles[sideTriangleGroup + 3] = i;
                m_SideTriangles[sideTriangleGroup + 4] = (i != halfLen) ? totalLen - i - 1 : totalLen  - 1;
                m_SideTriangles[sideTriangleGroup + 5] = (i != halfLen) ? i + 1 : 1;

                sideTriangleGroup +=6;
                lastVert = totalLen - i;
            }

            m_FinalTriangles.AddRange(m_SideTriangles);
        }

    

        m_Mesh.Clear();
        m_Mesh.vertices = m_Vertices;
        m_Mesh.normals = m_Normals; 
        if (CreateUV)
            m_Mesh.uv = m_UV;
        m_Mesh.triangles = m_FinalTriangles.ToArray();
        m_Mesh.RecalculateNormals();
        return m_Mesh;
    }

    //public UnityEngine.Rendering.MeshUpdateFlags normalFlag;
    //public UnityEngine.Rendering.MeshUpdateFlags tangentFlag;


    //[Header("TEST")]
    //public int one = 0;
    //public int two = 0;


    void Update ()
    {
        if (AutoUpdate)
            UpdateMesh();


        /*int index = 0;
        foreach(Vector3 normal in m_Mesh.normals)
        {
            Debug.DrawRay(m_Vertices[index], normal, index < halfLen ? Color.red : Color.green);
            index++;
        }*/

        /*if (DoubleSided)
        {
            int totalLen = m_Vertices.Length;
            int halfLen = (m_Vertices.Length / 2) - 1;
             
           // Debug.DrawLine(m_Vertices[one], m_Vertices[two], Color.green);
            //Debug.Log(m_Vertices.Length);
            for (int i = 1; i <= halfLen; i++)
            {
                Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - i], Color.red);
                //Debug.DrawLine(m_Vertices[i + 1], m_Vertices[totalLen - i - 1], Color.red);
                if(i != halfLen)
                    Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - i - 1], Color.green);
                else
                    Debug.DrawLine(m_Vertices[i], m_Vertices[totalLen - 1], Color.cyan);
            } 
        }*/

    } 
}