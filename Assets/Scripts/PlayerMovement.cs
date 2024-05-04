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
    private CameraMovement cameraMovement;
    


    [Space] public List<Transform> toPath = new List<Transform>();


    private void Awake()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
        RayCastDown();
    }

    private void Update()
    {
        //Application.targetFrameRate = 60;

        RayCastDown();
        //transform.parent = CurrentCube.GetComponent<WalkablePath>().movingGround ? CurrentCube.parent : null;
        if (cameraMovement.isRotating) return;
        if (Input.GetMouseButtonDown(0))
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var mouseHit))
            {

                if (mouseHit.transform.GetComponent<WalkablePath>() != null)
                {
                    clickedCube = mouseHit.transform; //Gets the clicked cube
                    if (clickedCube == mouseHit.transform.CompareTag("Lever"))
                    {
                        toPath.Clear();
                        clickedCube = mouseHit.transform.GetComponent<Lever>().nearByBlock.transform;
                        mouseHit.transform.GetComponent<Lever>().toggle = !mouseHit.transform.GetComponent<Lever>().toggle;
                    }

                    DOTween.Kill(gameObject.transform);
                    toPath.Clear();
                    FindPath();


                    //Mouse indicator & Effect 
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

        visitedCubes.Add(current); //Don't touch this

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
            toPath.Add(cube); //Adds the cube to the final destination of list 
            if (cube.GetComponent<WalkablePath>().previousBlock != null)
                cube = cube.GetComponent<WalkablePath>().previousBlock;
            else
                return;
        }

        toPath.Insert(0, clickedCube);
        FollowPath();
    }

    private void FollowPath()
    {
        var sequence = DOTween.Sequence();
        for (int i = toPath.Count - 1; i > 0; i--)
        {
            walking = true;
            var timing = toPath[i].GetComponent<WalkablePath>().isStair 
                ? toPath[i].GetComponent<WalkablePath>().stairSpeed
                : toPath[i].GetComponent<WalkablePath>().walkPointSpeed; //change it
            
            sequence.Append(transform.DOMove(toPath[i].GetComponent<WalkablePath>().GetWalkPoint(), .1f * timing).SetEase(Ease.Linear));
            //sequence.Append(transform.DOMove(toPath[i].GetComponent<WalkablePath>().GetWalkPoint(), .1f * timing).SetEase(Ease.Linear));
            

            /*if (!toPath[i].GetComponent<WalkablePath>().dontRotatePlayerInParticularBlock) //player rotation according to the path
            {
                sequence.Join(transform.DOLookAt(toPath[i].position, .2f, AxisConstraint.Y, Vector3.up));
            }*/
            //clears path after player clicks new block
            sequence.AppendCallback(Clear);
        }
    }

    private void Clear()
    {
        foreach (Transform t in toPath)
        {
            t.GetComponent<WalkablePath>().previousBlock = null;
        }

        toPath.Clear();
        walking = false;
    }
    
    #endregion


        private void RayCastDown()
        {
            var t = transform;
            Ray rayHit = new Ray(t.GetChild(0).position, -t.up);
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
            var t = transform;
            Gizmos.color = Color.blue;
            Ray ray = new Ray(t.GetChild(0).position, -t.up);
            Gizmos.DrawRay(ray);
        }
    }