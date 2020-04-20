using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SliceTrigger : MonoBehaviour
{
    public int count = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (count == 0)
        {
            count++;
            SplitMesh(other.gameObject);
            Destroy(other.gameObject);
        }
    }
    void SplitMesh(GameObject go)
    {
        GameObject leftHalf = MakeHalf(go, true);
        GameObject rightHalf = MakeHalf(go, false);
    }
    GameObject MakeHalf(GameObject go, bool isLeft)
    {
        float sign = isLeft ? -1 : 1;
        GameObject half = Instantiate(go);
        MeshFilter filter = half.GetComponent<MeshFilter>();
        Plane cuttingPlane = GetPlane(go);
        filter.mesh = CloneMesh(cuttingPlane, filter.mesh, isLeft);
        half.transform.position = go.transform.position + transform.rotation * new Vector3(sign * 0.05f, 0, 0);
        half.GetComponent<Rigidbody>().isKinematic = false;
        half.GetComponent<Rigidbody>().useGravity = true;
        half.GetComponent<Collider>().isTrigger = false;
        Destroy(half, 2);
        return half;
    }
    private Plane GetPlane(GameObject go)
    {
        Vector3 pt1 = transform.rotation * new Vector3(0, 0, 0);
        Vector3 pt2 = transform.rotation * new Vector3(0, 1, 0);
        Vector3 pt3 = transform.rotation * new Vector3(0, 0, 1);

        Plane rv = new Plane();
        rv.Set3Points(pt1, pt2, pt3);
        return rv;
    }
    private Mesh CloneMesh(Plane p, Mesh oMesh, bool halve)
    {
        Mesh cMesh = new Mesh();
        cMesh.name = "slicedMesh";
        Vector3[] vertices = oMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            bool side = p.GetSide(vertices[i]);

            if (side == halve)
            {
                vertices[i] = p.ClosestPointOnPlane(vertices[i]);
            }
        }
        cMesh.vertices = vertices;
        cMesh.triangles = oMesh.triangles;
        cMesh.normals = oMesh.normals;
        cMesh.uv = oMesh.uv;
        return cMesh;
    }
}

