using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    public bool walking;

    [Space] public Transform currentCube;
    public Transform clickedCube;
    public Transform indicator;
    

    [Space] public List<Transform> ToPath = new List<Transform>();

    
    private void Start()
    {
        RayCastDown();
    }

    private void Update()
    {
        RayCastDown();
        transform.parent = currentCube.GetComponent<WalkablePath>().movingGround ? currentCube.parent : null;

        if (Input.GetMouseButtonDown(0))
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var mouseHit))
            {

                if (mouseHit.transform.GetComponent<WalkablePath>() != null)
                {
                    clickedCube = mouseHit.transform;//Gets the clicked cube
                    if (clickedCube == mouseHit.transform.CompareTag("Lever"))
                    {
                        ToPath.Clear();
                        clickedCube = mouseHit.transform.GetComponent<Lever>().nearByBlock.transform;
                        mouseHit.transform.GetComponent<Lever>().turnOnLever = true;
                    }
                    DOTween.Kill(gameObject.transform);//kill the transform previous tween if clicked another block
                    ToPath.Clear();//clears the path if clicked another block
                    FindPath();//again find the path to the new clicked block
                    
                    
                    //Mouse Indicator & Effect 
                    indicator.position = mouseHit.transform.GetComponent<WalkablePath>().GetWalkPoint();
                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(() => indicator.GetComponentInChildren<ParticleSystem>().Play());
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.black, .3f).SetDelay(.2f));
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
                }
                
            }
        }
    }



    #region PathFinding

    private void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<WalkablePath>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<WalkablePath>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);
        ExploreCube(nextCubes, pastCubes);
        BuildPath(); //
    }

    private void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        if (current == clickedCube)
        {
            return;
        }

        foreach (WalkPath path in current.GetComponent<WalkablePath>().possiblePaths)
        {
            if (!visitedCubes.Contains(path.target) && path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<WalkablePath>().previousBlock = current;
            }
        }

        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    private void BuildPath()
    {
        Transform cube = clickedCube;
        while (cube != currentCube) //Checks if the cube is not the current cube
        {
            ToPath.Add(cube); //Adds the cube to the final destination of list 
            if (cube.GetComponent<WalkablePath>().previousBlock != null)
                cube = cube.GetComponent<WalkablePath>().previousBlock;
            else
                return;
        }

        ToPath.Insert(0, clickedCube);
        FollowPath();
    }

    private void FollowPath()
    {
        var sequence = DOTween.Sequence();
        walking = true;
        for (int i = ToPath.Count - 1; i > 0; i--)
        {

            float timing = ToPath[i].GetComponent<WalkablePath>().isStair ? ToPath[i].GetComponent<WalkablePath>().stairSpeed : ToPath[i].GetComponent<WalkablePath>().walkPointSpeed; //change it
            sequence.Append(transform.DOMove(ToPath[i].GetComponent<WalkablePath>().GetWalkPoint(), .2f * timing)
                .SetEase(Ease.Linear));

            /*if(!finalPath[i].GetComponent<Walkable>().dontRotate) //while walking
                s.Join(transform.DOLookAt(finalPath[i].position, .1f, AxisConstraint.Y, Vector3.up));*/
        }

        /*if (clickedCube.GetComponent<Walkable>().isButton)
        {
            s.AppendCallback(()=>GameManager.instance.RotateRightPivot());
        }*/
        
        sequence.AppendCallback(Clear); //clears the path after the player has reached the destination or the path is blocked or the player clicked another block
    }

    private void Clear()
    {
        foreach (Transform t in ToPath)
        {
            t.GetComponent<WalkablePath>().previousBlock = null;
        }

        ToPath.Clear();
        walking = false;
    }

    #endregion

    private void RayCastDown()
    {
        Ray rayHit = new Ray(transform.GetChild(0).position, -transform.up);
        if (Physics.Raycast(rayHit, out var hitPoint))
        {
            if (hitPoint.transform.GetComponent<WalkablePath>() != null)
            {
                currentCube = hitPoint.transform;
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray ray = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(ray);
    }
}