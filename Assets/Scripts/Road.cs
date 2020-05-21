using UnityEngine;

public class Road : MonoBehaviour
{
    public Tile Tile1 { get; set; }
    public Tile Tile2 { get; set; }

    private Mesh _mesh;
    private Vector3[] _vertices;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _vertices = _mesh.vertices;
    }

    private void Update()
    {
        if (Tile1 != null && Tile2 != null)
        {
            //_vertices[0] = _vertices[1] = Tile1.transform.position;
            //_vertices[2] = _vertices[3] = Tile2.transform.position;
            //_mesh.vertices = _vertices;
            //_mesh.RecalculateBounds();


            //transform.localPosition = (Tile1.transform.localPosition + Tile2.transform.localPosition) * 0.5f;
            //float distance = Vector3.Distance(Tile1.transform.position, Tile2.transform.position);
            //Vector3 scale = transform.localScale;
            //scale.x = distance;
            //transform.localScale = scale;
        }
    }
}
