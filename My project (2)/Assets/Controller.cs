using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    public bool overrideMovementCtrl;

    public bool isTurnRound;
    private bool hasSetTurnRoundDes = false;
    public float turnRadius = 4;
    public int turnDegreeStepValue = 120;
    public int turnAngleThreshold = 90;

    public void Start()
    {
    }
    private Vector3 realDest;
    private Vector3 d1;



    public void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; // ??m b?o v?t ch? xoay theo tr?c x và z

                navMeshAgent.SetDestination(targetPosition);
                Debug.DrawRay(targetPosition, Vector3.up, Color.cyan, 10f);
            }
        }
        if (overrideMovementCtrl == false || navMeshAgent.enabled == false || HasArrived() ||
            navMeshAgent.isStopped)
            return;

        Vector3 targetDir = navMeshAgent.destination - transform.position;
        if (Vector3.Angle(targetDir, transform.forward) > turnAngleThreshold && isTurnRound == false)
        {
            realDest = navMeshAgent.destination;
            isTurnRound = true; 
            navMeshAgent.autoBraking = false;
        }


        if (isTurnRound)
        {
            if (hasSetTurnRoundDes == false)
            {
                d1 = FindTurnPoint(realDest);
                hasSetTurnRoundDes = true;
                navMeshAgent.SetDestination(d1);
            }

            if (Vector3.Distance(transform.position, d1) <= navMeshAgent.stoppingDistance * 1.2f)
            {
                if (Vector3.Angle(realDest - transform.position, transform.forward) > turnAngleThreshold)
                {
                    d1 = FindTurnPoint(realDest);
                    navMeshAgent.SetDestination(d1);
                }
            }

            if (Vector3.Angle(realDest - transform.position, transform.forward) < turnAngleThreshold)
            {
                navMeshAgent.SetDestination(realDest);
                hasSetTurnRoundDes = false;
                isTurnRound = false;
                navMeshAgent.autoBraking = true;
            }
        }

        Vector3 FindTurnPoint(Vector3 realDest)
        {
            Vector3 direction = realDest - transform.position;
            var cross = Vector3.Cross(transform.forward, direction); 
            Vector3 targetPos;
            NavMeshHit navMeshHit;
            if (cross.y < 0) 
            {
                targetPos = transform.position + Quaternion.Euler(0, -60, 0)*transform.forward * turnRadius;
            }
            else 
            {
                targetPos = transform.position + Quaternion.Euler(0, +60, 0) * transform.forward * turnRadius;
            }
            if (NavMesh.SamplePosition(targetPos, out navMeshHit, 2f, -1))
            {
                targetPos = navMeshHit.position; 
            }
            else 
            {
                targetPos = this.realDest;
            }

            return targetPos;
        }
    }

    public void SetRealDest(Vector3 pos)
    {
        realDest = pos;
    }



    private bool HasArrived()
    {
        float remainingDistance;
        if (navMeshAgent.pathPending)
        {
            remainingDistance = float.PositiveInfinity;
        }
        else
        {
            remainingDistance = navMeshAgent.remainingDistance;
        }

        return remainingDistance <= navMeshAgent.stoppingDistance;
    }
}

