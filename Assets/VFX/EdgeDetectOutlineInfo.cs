using UnityEngine;

public class EdgeDetectOutlineInfo : MonoBehaviour
{
    [System.NonSerialized]
    public MaterialPropertyBlock block;
    [System.NonSerialized]
    public MeshFilter meshFilter;

    public Material mat;
    private Camera _camera;

    public bool Enable;
    public Color EdgeColor;
    public float SampleDistance;
    public float NormalSensitivity;
    public float DepthSensitivity;

     void Awake()
    {
      _camera = GetComponent<Camera>();
      _camera.depthTextureMode = DepthTextureMode.DepthNormals;
        block = new MaterialPropertyBlock();
        
    }

     void Update()
    {
        setInfo();
        if(Enable)
        {
            DrawObject(this.meshFilter, block);
        }

    }

    void setInfo()
    {
        block = new MaterialPropertyBlock();
        meshFilter = GetComponent<MeshFilter>();
        block.SetColor("_EdgeColor", EdgeColor);
        block.SetFloat("_SampleDistance", SampleDistance);
        block.SetFloat("_NormalSensitivity", NormalSensitivity);
        block.SetFloat("_DepthSensitivity", DepthSensitivity);


    }

    void DrawObject(MeshFilter meshFilter, MaterialPropertyBlock block)
    {
        Matrix4x4 drawMatrix = meshFilter.transform.localToWorldMatrix;

        Graphics.DrawMesh(meshFilter.sharedMesh, drawMatrix, mat, 0, null, 0, block);

    }
}
