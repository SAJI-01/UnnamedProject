using DG.Tweening;
using UnityEngine;


public class MovableBlock : MonoBehaviour
{
    public bool isMoveAble;
    private bool isPlayerInside;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private Vector3 boxPosition;
    private bool isMoving;
    private PlayerMovement player;
    private MovableBlock thisBlock;
    
    private void Awake()
    {
        thisBlock = FindObjectOfType<MovableBlock>();
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        isPlayerInside = Physics.CheckBox(transform.position + boxPosition, boxSize, Quaternion.identity, LayerMask.GetMask("Player"));
        if (isPlayerInside)
        { 
            Invoke(nameof(IsMove), 0.5f);
        }
    }

    private bool IsMove()
    {
        return isMoveAble = true;
    }

    public void MoveAbleBlock(RaycastHit hit, Lever l)
    {
        if(isMoving) return;

        isMoving = true;
        if (isMoving)//prevent multiple click
        { 
            l.previousBlocks.GetComponent<WalkablePath>().possiblePaths[1].active = false;
            player.gameObject.transform.DOMoveX(transform.position.x + 2, 2).SetEase(Ease.InOutCubic);
            hit.transform.GetComponent<MovableBlock>().transform.DOMoveX(transform.position.x + 2, 2)
                .SetEase(Ease.InOutCubic).onComplete = () =>
            {
                isMoveAble = false;
                l.objectToToggle.GetComponent<WalkablePath>().possiblePaths[0].active =
                    true;
                l.objectToToggle.GetComponent<WalkablePath>().possiblePaths[1].active =
                    false; // finish path true
            };
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + boxPosition, boxSize);
    }
}
