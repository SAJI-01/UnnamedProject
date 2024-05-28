using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private const float Tolerance = 0.1f;
    public PlayerMovement player;
    public List<PathCondition> pathConditions = new List<PathCondition>();
    public List<Enabler> enablers = new List<Enabler>();
    public List<PivotEnabler> Animation_Pivots = new List<PivotEnabler>();
    public List<ObjectToHideCondition> objectToHideConditions = new List<ObjectToHideCondition>();





    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        PathConditions();
        Animation_Trigger();
        HideObjectCondition();
    }


    private void PathConditions()
    {
        foreach (var pc in pathConditions)
        {
            var count = pc.conditions.Count(t => Mathf.Abs(t.conditionObject.eulerAngles.y - t.eulerAngle.y) < Tolerance);

            foreach (var sp in pc.paths)
                sp.walkablePathBlock.possiblePaths[sp.index].active = count == pc.conditions.Count;
        }

    }

    private void Animation_Trigger()
    {
        foreach (var pivot in Animation_Pivots)
        {
            var count = pivot.conditions.Count(t => t.pivot.localRotation == Quaternion.Euler(t.eulerAngle));

            foreach (var sp in pivot.paths)
                sp.walkablePathBlock.possiblePaths[sp.index].active = count == pivot.conditions.Count;
        }
    }

    public void Link(bool mbLaserStart, bool mbLaserEnd, bool mbNormalStart, bool mbNormalEnd)
    {
        foreach (var enabler in enablers)
        {
            var count = 0;
            foreach (var ec in enabler.conditions)
            {
                if (ec.startOfMbWithLaser && mbLaserStart) count++;
                if (ec.endOfMbWithLaser && mbLaserEnd) count++;
                if (ec.startOfNormalMb && mbNormalStart) count++;
                if (ec.endOfNormalMb && mbNormalEnd) count++;
            }

            foreach (var ep in enabler.paths)
            {
                if (ep.link)
                    ep.walkablePathBlock.possiblePaths[ep.index].active = count == enabler.conditions.Count;
                else if(ep.unLink)
                    ep.walkablePathBlock.possiblePaths[ep.index].active = count != enabler.conditions.Count;
            }
        }
    }
    
    public void TriggerToAnimation()
    {
        foreach (var pivotCondition in Animation_Pivots.SelectMany(pivot => pivot.conditions))
        {
            pivotCondition.pivot.DOComplete();
            pivotCondition.pivot.DOLocalRotate(pivotCondition.eulerAngle, .6f).SetEase(Ease.OutBack);
        }
    }
    
    private void HideObjectCondition()
    {
        foreach (var objectToHideCondition in objectToHideConditions)
        {
            var count = objectToHideCondition.eulerAngles.Count(t => Mathf.Abs(objectToHideCondition.ConditionObject.eulerAngles.y - t.y) < Tolerance);
            foreach (var go in objectToHideCondition.gameObjectsToHide)
            {
                //set meshRenderer enabled to false if hide is true, else true
                go.GetComponent<MeshRenderer>().enabled = objectToHideCondition.hide ? 
                    count == objectToHideCondition.eulerAngles.Length : 
                    count != objectToHideCondition.eulerAngles.Length;
            }
        }
    }

    [Serializable]
    public class PathCondition
    {
        public string pathConditionName;
        public List<Condition> conditions;
        public List<SinglePath> paths;
    }

    [Serializable]
    public class Condition
    {
        public Transform conditionObject;
        public Vector3 eulerAngle;
    }

    [Serializable]
    public class SinglePath
    {
        public WalkablePath walkablePathBlock;
        public int index; //index of element in the block order
    }

    [Serializable]
    public class Enabler
    {
        public string enablerName;
        public List<EnablerCondition> conditions;
        public List<EnablerPath> paths;
    }

    [Serializable]
    public class EnablerPath
    {
        public bool link,unLink;
        public WalkablePath walkablePathBlock;
        public int index;
    }

    [Serializable]
    public class EnablerCondition
    {
        public bool startOfMbWithLaser;
        public bool endOfMbWithLaser;
        public bool startOfNormalMb;
        public bool endOfNormalMb;
    }
    
    [Serializable]
    public class PivotEnabler
    {
        public string pivotName;
        public List<PivotCondition> conditions;
        public List<SinglePath> paths;
    }

    [Serializable]
    public class PivotCondition
    {
        public Transform pivot;
        public Vector3 eulerAngle;
    }
    
    [Serializable]
    public class ObjectToHideCondition
    {
        public Transform ConditionObject;
        public GameObject[] gameObjectsToHide;// list of objects to hide or show in Certain ConditionObject's Angle
        public Vector3[] eulerAngles; // list of eulerAngle To Hide or Show Object in Certain ConditionObject's Angle
        public bool hide;// if hide is true (hide object), else (show Object)
    }
}





