using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerMovement player;
    public List<PathCondition> pathConditions = new List<PathCondition>();
    public List<Enabler> enablers = new List<Enabler>();




    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        PathConditions();



        if (player.walking) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }


    private void PathConditions()
    {
        foreach (var pc in pathConditions)
        {
            var count = 0;
            for (var i = 0; i < pc.conditions.Count; i++)
                if (pc.conditions[i].conditionObject.eulerAngles == pc.conditions[i].eulerAngle)
                    count++;

            foreach (var sp in pc.paths)
                sp.walkablePathBlock.possiblePaths[sp.index].active = count == pc.conditions.Count;
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

    [System.Serializable]
    public class PathCondition
    {
        public string pathConditionName;
        public List<Condition> conditions;
        public List<SinglePath> paths;
    }

    [System.Serializable]
    public class Condition
    {
        public Transform conditionObject;
        public Vector3 eulerAngle;

    }

    [System.Serializable]
    public class SinglePath
    {
        public WalkablePath walkablePathBlock;
        public int index; //index of element in the block order
    }

    [System.Serializable]
    public class Enabler
    {
        public string enablerName;
        public List<EnablerCondition> conditions;
        public List<EnablerPath> paths;
    }

    [System.Serializable]
    public class EnablerPath
    {
        public bool link,unLink;
        public WalkablePath walkablePathBlock;
        public int index;
    }

    [System.Serializable]
    public class EnablerCondition
    {
        public bool startOfMbWithLaser;
        public bool endOfMbWithLaser;
        public bool startOfNormalMb;
        public bool endOfNormalMb;
    }
}





