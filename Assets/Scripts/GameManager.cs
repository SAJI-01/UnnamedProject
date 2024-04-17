using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public PlayerMovement player;
    public List<PathCondition> pathConditions = new List<PathCondition>();
    public WalkablePath walkablePath;
    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        /*foreach (var path in FindObjectsOfType<WalkablePath>())
        {
            path.possiblePa
        }*/
        foreach (PathCondition pc in pathConditions)
        {
            int count = 0;
            for (int i = 0; i < pc.conditions.Count; i++)
            {
                if (pc.conditions[i].conditionObject.eulerAngles == pc.conditions[i].eulerAngle)
                {
                    count++;
                }
            }

            foreach (SinglePath sp in pc.paths)
                sp.walkablePathBlock.possiblePaths[sp.index].active = (count == pc.conditions.Count);
        }

        if (player.walking) return; //if the player is walking, don't do anything



        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
    
}

[System.Serializable]
public class PathCondition
{
    public string pathConditionName;
    public List<Condition> conditions;//conditions to check
    public List<SinglePath> paths;//paths to change
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
    public int index;//index of element in the block order
}



