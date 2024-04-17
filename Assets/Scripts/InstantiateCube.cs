using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCube : MonoBehaviour
{
    public int gridX, gridZ;
    public int gridCellSize = 1;
    private GameObject[,] gameGrid;
    

    private void Start()
    {
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        gameGrid = new GameObject[gridX, gridZ];
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                var position = new Vector3(x * gridCellSize, 0, z * gridCellSize);
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.parent = transform;
                gameGrid[x, z] = cube;
            }
        }
    }
    
}
