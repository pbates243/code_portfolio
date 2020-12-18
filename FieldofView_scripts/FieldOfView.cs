using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public float meshResoultion;
    public int edgeResolveIterations;
    public float edgeDistanceThreshhold;

    public float maskCutAwayDst = .1f;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public Renderer rend;

    



    [HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>(); // list of visible targets

    IEnumerator FindTargetsWithDelay(float delay)
	{
        while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

    void Start()
	{
		StartCoroutine("FindTargetsWithDelay", .2f);

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
	}

    void LateUpdate()
    { 
        DrawFieldOfView();
        
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) // takes in angle and gives us a direction of it
    {
        if (!angleIsGlobal) 
		{
			angleInDegrees += transform.eulerAngles.y;
		}
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0 , Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)); // 
    }

    void FindVisibleTargets()  
	{
        
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask); // array of targets in view

        
        for (int i = 0; i < targetsInViewRadius.Length; i ++)
		{
			Transform target = targetsInViewRadius[i].transform;

			Vector3 dirToTarget = (target.position - transform.position).normalized; // finds the location of the target
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float disToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast (transform.position, dirToTarget, disToTarget, obstacleMask)) // checks if there is an obstacle in between player and target
				{
					visibleTargets.Add(target);
         

				}
             
			}
		}
	}


    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResoultion); // raycount
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>(); // all the points the raycast hits
        ViewCastInfo oldViewCast = new ViewCastInfo();



        for (int i = 0; i < stepCount; i++) // loop through step count
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i; // rotates clockwise until we get to rightmost angle

            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle); // new view cast info

            if ( i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistanceThreshhold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) // if old hit and new didnt, or new did, and old didnt
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA); 
                    }

                    if(edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount]; // array of vertices
        int[] triangles = new int[(vertexCount - 2) * 3]; // number of triangles

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount-1; i++) // loop through vertices and set to positions of viewpoints list
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * maskCutAwayDst); // convert the vertices to local space points

            if (i < vertexCount - 2) // keeps in bounds of array
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) // edge detection
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2; // finds new angle
            ViewCastInfo newViewCast = ViewCast(angle); // casts new viewcast

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshhold;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);


    }

    ViewCastInfo ViewCast(float globalAngle) // struct of view cast info
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle); // passes in relevant info to constructor
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle); 
        }
    }

    public struct ViewCastInfo // info about the raycast
    {
        public bool hit; //if it hit something
        public Vector3 point;
        public float dst; // length of ray
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }

    }

    public struct EdgeInfo // information to locate the edge of obstacles
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }

    }
}
