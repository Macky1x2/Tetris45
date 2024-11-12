using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBoxMino : MonoBehaviour
{
    [SerializeField] private Vector2Int localGridPosition;

    private Vector2 localGridXMeaningInLocalReal = new Vector2(0.25f, -0.25f);
    private Vector2 localGridYMeaningInLocalReal = new Vector2(0.25f, 0.25f);

    // Start is called before the first frame update
    void Start()
    {
        UpdateLocalRealPosition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateLocalRealPosition()
    {
        transform.localPosition = localGridPosition.x * localGridXMeaningInLocalReal + localGridPosition.y * localGridYMeaningInLocalReal;
    }

    public void GoDownRow(int deltaY)
    {
        transform.position = new Vector3(transform.position.x , transform.position.y - deltaY * MinoMap.MinoOneDistanceXY);
    }

    public void SetLocalGridPosition(Vector2Int newPos)
    {
        localGridPosition = newPos;
        UpdateLocalRealPosition();
    }

    public Vector2Int LocalGridPosition
    {
        get
        {
            return localGridPosition;
        }
    }
}
