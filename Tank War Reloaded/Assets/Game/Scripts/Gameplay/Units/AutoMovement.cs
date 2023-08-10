using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoMovement : MonoBehaviour
{
    //[HideInInspector]
    //public GameObject player;
    //[HideInInspector]
    //public GameObject target;

    //protected Vector3 destination;

    //[SerializeField]
    //protected List<Transform> raycastPointList;
    ////list of direction can move to from raycast
    //protected List<GlobalVar.direction> dirList;
    ////direction for next movement
    //protected GlobalVar.direction currentDir;
    //protected string[] collideableLayerArr;
    //protected int layersToCheck;


    //protected bool reachedDes = true;
    //protected bool bodyRotated = false;
    //protected bool noTargetLeft = false;
    ////if this true && rally des set -> the obj wont move once reached rally point
    //protected bool stayAtRally = false;
    ////ultimately decide whether this can move 
    //protected bool canMove = true;

    //protected float distanceToPlayer;
    //protected float distanceToTarget;

    //public float speed = 4f;
    //public float turnSpeed = 3f;
    //public float engageRange = 35f;
    //public float effectiveEngageRange = 25f;

    //[SerializeField]
    //protected float minRandomDistance = 5f;
    //[SerializeField]
    //protected float maxRandomDistance = 12f;
    ////calculated by sqrt (2 * maxRandomDistance^2)
    //protected float maxRandomDistancePossible = 17f;

    //protected float maxRandomDistanceWhenEngage = 9f;

    ////for anti-stuck behavior
    ////if collision happened -> cache last position -> if have stay there for more than var value -> change destination
    //protected Vector3 lastCollisionPos;
    ////set this with lastpos in collision enter//only change this cache after time - this > changedDesafterTimeAmount
    //protected float lastPosCachedTime;
    //protected float lastPosCachedDuration = 5f;

    //protected float collisionStartTime;
    //protected float collisionDuration;

    //protected int normalLayer = -1;
    //protected int layerToUseWhenMoveFromOutOfBound;

    //protected enum action { ENGAGE, SCRAMBLE, RALLY}
    //protected action currentAction; 

    
    ////called this on Start() of inherit class
    //protected void SetRallyLayerByTag()
    //{
    //    if (gameObject.CompareTag(GlobalVar.allyTag) && normalLayer < 0)
    //    {
    //        normalLayer = GlobalVar.aTankLayer;
    //        layerToUseWhenMoveFromOutOfBound = GlobalVar.aIgnoreLowBlockLayer;
    //    }
    //    if (gameObject.CompareTag(GlobalVar.eTag) && normalLayer < 0)
    //    {
    //        normalLayer = GlobalVar.eTankLayer;
    //        layerToUseWhenMoveFromOutOfBound = GlobalVar.eIgnoreLowBlockLayer;
    //    }
    //    collideableLayerArr = new string[GlobalVar.blockMovementLayerArr.Length];
    //    for (int i=0; i<collideableLayerArr.Length; i++)
    //    {
    //        //Debug.Log(GlobalVar.blockMovementLayerArr[i]);
    //        collideableLayerArr[i] = LayerMask.LayerToName(GlobalVar.blockMovementLayerArr[i]);
    //    }
        
    //    layersToCheck = LayerMask.GetMask(collideableLayerArr);
    //    Debug.Log(layersToCheck);
    //}

    //protected void MoveToDes()
    //{
    //    if (!reachedDes && bodyRotated)
    //    {
    //        Vector3 dirToTarget = (destination - transform.position).normalized;
    //        Vector2 pos = transform.position;
    //        pos.x += Time.deltaTime * speed * dirToTarget.x;
    //        pos.y += Time.deltaTime * speed * dirToTarget.y;
    //        transform.position = pos;
    //    }
    //}

    //protected void FindPlayer()
    //{
    //    if (!player)
    //    {
    //        player = GameObject.FindGameObjectWithTag(GlobalVar.pTag);
    //    }
    //}

    //#region decide which direction to move to
    //protected void CheckMoveableDirection()
    //{
    //    foreach (Transform i in raycastPointList)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(i.position, i.up, effectiveEngageRange, layersToCheck);
    //        if (hit)
    //        {
    //            if (Vector2.Distance(hit.point, transform.position) <= 10f)
    //            {
    //                //do nothing
    //            }
    //            else
    //            {
    //                //add a weight to decide how likely u want to move in that direction
    //                dirList.Add((GlobalVar.direction)raycastPointList.IndexOf(i));
    //            }
    //        }
    //        //if nothing block the raycast -> add to move direction list
    //        else
    //        {
    //            dirList.Add((GlobalVar.direction)raycastPointList.IndexOf(i));
    //        }
    //    }
    //}
    ////compare the position of target then get the direction
    //protected void CheckMoveableDirectionOnEngage()
    //{

    //}
    ////if you want more complex behavior, add a weight to each in dirList then apply a formula
    //protected void DecideMoveDirectionVector()
    //{
    //    if (dirList.Count == 1)
    //    {
    //        currentDir = dirList[0];
    //    }
    //    if (dirList.Count > 1)
    //    {
    //        int index = Random.Range(0, dirList.Count);
    //        currentDir = dirList[index];
    //    }
    //    dirList.Clear();
    //}
    //#endregion


    //protected void SetNewRandomDes()
    //{
    //    if (reachedDes)
    //    {
    //        float randX = Random.Range(minRandomDistance, maxRandomDistance);
    //        float randY = Random.Range(minRandomDistance, maxRandomDistance);
    //        float dirX = Random.Range(-1f, 1f);
    //        float dirY = Random.Range(-1f, 1f);
    //        if (dirX <= 0) randX = -randX;
    //        if (dirY <= 0) randY = -randY;

    //        destination = new Vector3(transform.position.x + randX, transform.position.y + randY, 0);
    //        reachedDes = false;
    //        bodyRotated = false;

    //        currentAction = action.SCRAMBLE;
    //    }
    //}

    //protected void SetNewEngageDes()
    //{
    //    if (target && reachedDes)
    //    {
    //        Vector3 targetPos = target.transform.position;
    //        float randX = Random.Range(minRandomDistance, maxRandomDistanceWhenEngage);
    //        float randY = Random.Range(minRandomDistance, maxRandomDistanceWhenEngage);
    //        float dirX = Random.Range(-1f, 1f);
    //        float dirY = Random.Range(-1f, 1f);
    //        if (dirX <= 0) randX = -randX;
    //        if (dirY <= 0) randY = -randY;

    //        destination = new Vector3(targetPos.x + randX, targetPos.y + randY, 0);
    //        reachedDes = false;
    //        bodyRotated = false;

    //        currentAction = action.ENGAGE;
    //    }
    //}

    ////called this when object spawn out of bounds, make sure u have a layer ignoring obstacle to set this object to, then return it to normal layer when reachedDes
    //virtual public void SetRallyDes(Vector3 rallyPos, bool stayAtRallyPoint)
    //{
    //    SetRallyLayerByTag();
    //    gameObject.layer = layerToUseWhenMoveFromOutOfBound;
    //    destination = rallyPos;
    //    canMove = true;
    //    reachedDes = false;
    //    bodyRotated = false;
    //    stayAtRally = stayAtRallyPoint;
    //    currentAction = action.RALLY;
    //}

    //virtual public void SetRallyDes(Vector3 rallyPos, bool stayAtRallyPoint, int layerToUse)
    //{
    //    normalLayer = gameObject.layer;
    //    gameObject.layer = layerToUse;
    //    destination = rallyPos;
    //    canMove = true;
    //    reachedDes = false;
    //    bodyRotated = false;
    //    stayAtRally = stayAtRallyPoint;
    //    currentAction = action.RALLY;
    //}

    //protected void RotateTowardDes()
    //{
    //    Vector2 lookAtPos = (destination - transform.position).normalized;
    //    float rotateAngle = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg - 90;
    //    //use .Angle to compare quaternion, not straight-up rotation
    //    if (Quaternion.Angle(Quaternion.Euler(0, 0, rotateAngle), transform.rotation) <= 0.5f)
    //    {
    //        bodyRotated = true;
    //    }
    //    else
    //    {
    //        Quaternion to = Quaternion.Euler(0, 0, rotateAngle);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, to, turnSpeed * Time.fixedDeltaTime);
    //    }
    //}

    //protected void SetNewDesManually(Vector3 pos)
    //{
    //    destination = pos;
    //    reachedDes = false;
    //    bodyRotated = false;

    //    currentAction = action.SCRAMBLE;
    //}

    //virtual protected void CheckReachedDes()
    //{
    //    //if last action moving to engage -> if distance to target <= engage range -> reached des
    //    if (currentAction == action.ENGAGE)
    //    {
    //        if (distanceToTarget <= engageRange)
    //        {
    //            reachedDes = true;
    //        }
    //    }
    //    //if last action = scramble
    //    if (Vector3.Distance(destination, transform.position) <= 0.5f)
    //    {
    //        reachedDes = true;
    //        if (currentAction == action.RALLY)
    //        {
    //            gameObject.layer = normalLayer;
    //            currentAction = action.SCRAMBLE;

    //            if (stayAtRally) canMove = false;
    //        }
    //    }
    //}

    //virtual public void ResetUponKilled()
    //{
    //    canMove = true;
    //    reachedDes = false;
    //    bodyRotated = false;
    //    stayAtRally = false;
    //    noTargetLeft = false;
    //    currentAction = action.SCRAMBLE;
    //}

    //virtual protected void SetRandomTargetToEngage() { }

    //virtual protected void SetTargetToEngage() { }
}
