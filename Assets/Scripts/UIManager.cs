using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool isFreeView = false;
    public bool isFreeViewInverse = true;
    public void FreeView()
    {
        isFreeView = !isFreeView;
        isFreeViewInverse = !isFreeViewInverse;
        Debug.Log("FreeView Button Clicked");
    }
}