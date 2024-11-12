using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoDisplayer : MonoBehaviour
{
    [SerializeField] GameObject[] minoImages = new GameObject[7];
    private GameObject mino;

    private int minoID = -1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateDisplayer(int newID)
    {
        if (mino != null) Destroy(mino);

        if (newID == -1)
        {
            
        }
        else
        {
            minoID = newID;
            mino = Instantiate(minoImages[minoID], this.transform);
        }
    }
}
