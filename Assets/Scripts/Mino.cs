using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{
    [SerializeField] private MinoMap minoMap;
    [SerializeField] private string minoName;

    private Vector2Int[,] rotateClockWiseDelta;

    private Vector2Int[,] rotateAnticlockWiseDelta;

    private Vector2Int normalGridXMeaningin45Grid = new Vector2Int(1, 1);
    private Vector2Int normalGridYMeaningin45Grid = new Vector2Int(-1, 1);

    public OneBoxMino[] oneBoxMinos { get; private set; }

    private int degreeState;
    public Vector2Int gridPosition { get; private set; }
    private bool movable;

    private const int putCancelCount = 25;

    private int putCancelCounter;
    private float fallTimer;
    private float putTimer;

    private const float fallTimeMulti = 10f;

    private int reachedPosMinY = 1000000;

    // Start is called before the first frame update
    protected void Start()
    {
        ResetFallTimer();
        ResetPutTimer();
        ResetPutCancelCounter();

        gridPosition = new Vector2Int(9, 29);
        degreeState = 0;
        movable = true;

        if (minoName == "I")
        {
            rotateClockWiseDelta = Define.rotateIminoClockWiseDelta;
            rotateAnticlockWiseDelta = Define.rotateIminoAnticlockWiseDelta;
        }
        else
        {
            rotateClockWiseDelta = Define.rotatenonIminoClockWiseDelta;
            rotateAnticlockWiseDelta = Define.rotatenonIminoAnticlockWiseDelta;
        }

        oneBoxMinos = GetComponentsInChildren<OneBoxMino>();
    }

    public void Initialize(MinoMap map)
    {
        minoMap = map;
        if(!map.CheckCanMoveHere(new Vector2Int(9, 29)))
        {
            GameManager.instance.Gameover();
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (movable)
        {
            FallUpdate();
        }
    }

    private void FallUpdate()
    {
        if (GameManager.instance != null)   //GameManagerの生成後にMinoが生成されることが約束されればこのcheckは消してよい。
        {
            if (minoMap.ChackOnMino(this))
            {
                if (putTimer >= GameManager.instance.minoPutTime || putCancelCounter >= putCancelCount)
                {
                    minoMap.RegisterMinoToMap(this);
                    movable = false;
                    Destroy(GetComponent<MinoController>());

                    GameManager.instance.NotifyMinoSeted();
                }
                putTimer += Time.deltaTime;
            }
            else
            {
                if (fallTimer >= GameManager.instance.minoFallTime)     //真なら下に落とす
                {
                    MoveDown();
                    ResetFallTimer();
                }
                fallTimer += Input.GetButton("Vertical") ? Time.deltaTime * fallTimeMulti : Time.deltaTime;
            }

            ////MovedState fallState = MovedState.None;
            //if (fallTimer >= GameManager.instance.minoFallTime)     //真なら下に落とすあるいは設置
            //{
            //    //fallState = MoveDown();
            //    if (MoveDown() != MovedState.Moved)
            //    {
            //        minoMap.RegisterMinoToMap(this);
            //        movable = false;
            //        Destroy(GetComponent<MinoController>());
            //    }
            //    ResetFallTimer();
            //}
            //fallTimer += Time.deltaTime;

            ////if(putTimer >= GameManager.instance.minoPutTime)
            ////{
            ////    if(fallState != MovedState.Moved)
            ////    {
            ////        minoMap.RegisterMinoToMap(this);
            ////        movable = false;
            ////        Destroy(GetComponent<MinoController>());
            ////    }
            ////}
            ////putTimer += Time.deltaTime;
        }
    }

    private void UpdateRealPosition()
    {
        transform.position = MinoMap.ConverseFromGridToReal(gridPosition);
    }

    private MovedState MoveNewGridPosition(Vector2Int deltaPos)
    {
        bool canMove = true;
        foreach(OneBoxMino mino in oneBoxMinos)
        {
            if (!minoMap.CheckCanMoveHere(gridPosition + mino.LocalGridPosition + deltaPos))
            {
                canMove = false;
                break;
            }
        }
        if (canMove)
        {
            gridPosition += deltaPos;
            UpdateRealPosition();
            return MovedState.Moved;
        }
        return MovedState.StopdByNormalBlock;
    }

    private void ResetFallTimer()
    {
        fallTimer = 0;
    }

    private void ResetPutTimer()
    {
        putTimer = 0;
    }

    private void ResetPutCancelCounter()
    {
        putCancelCounter = 0;
    }

    private void UpdateReachedPosMinY()
    {
        if (gridPosition.y < reachedPosMinY)
        {
            reachedPosMinY = gridPosition.y;
            ResetPutCancelCounter();
        }
    }

    private void MoveDown()
    {
        if(MoveNewGridPosition(new Vector2Int(0, -1)) == MovedState.Moved)
        {
            ResetPutTimer();
            UpdateReachedPosMinY();
        }
    }

    public void MoveRight()
    {
        bool preIsOnBlock = minoMap.ChackOnMino(this);
        bool check = false;
        check = MoveNewGridPosition(new Vector2Int(1, 0)) == MovedState.Moved;
        if (!check) check = MoveNewGridPosition(new Vector2Int(1, -1)) == MovedState.Moved;
        if (!check) check = MoveNewGridPosition(new Vector2Int(1, 1)) == MovedState.Moved;

        if (check && preIsOnBlock)
        {
            ResetPutTimer();
            putCancelCounter++;
            UpdateReachedPosMinY();
        }
    }

    public void MoveLeft()
    {
        bool preIsOnBlock = minoMap.ChackOnMino(this);
        bool check = false;
        check = MoveNewGridPosition(new Vector2Int(-1, 0)) == MovedState.Moved;
        if (!check) check = MoveNewGridPosition(new Vector2Int(-1, -1)) == MovedState.Moved;
        if (!check) check = MoveNewGridPosition(new Vector2Int(-1, 1)) == MovedState.Moved;

        if (check && preIsOnBlock)
        {
            ResetPutTimer();
            putCancelCounter++;
            UpdateReachedPosMinY();
        }
    }


    public void RotateClockwise()
    {
        bool preIsOnBlock = minoMap.ChackOnMino(this);
        bool check = false;
        check = RotateClockwiseInNewPosition(gridPosition);
        if (minoName != "O")
        {
            for (int i = 0; i < 4; i++)
            {
                if (!check)
                {
                    Vector2Int delta = rotateClockWiseDelta[degreeState, i].x * normalGridXMeaningin45Grid + rotateClockWiseDelta[degreeState, i].y * normalGridYMeaningin45Grid;
                    check = RotateClockwiseInNewPosition(gridPosition + delta);
                }
            }
        }

        if (check)
        {
            degreeState++;
            if (degreeState == 4)
            {
                degreeState = 0;
            }
            if (preIsOnBlock)
            {
                ResetPutTimer();
                putCancelCounter++;
                UpdateReachedPosMinY();
            }
        }
    }

    private bool RotateClockwiseInNewPosition(Vector2Int newPos)
    {
        bool canMove = true;
        Vector2Int[] nextLocalGrid = new Vector2Int[oneBoxMinos.Length];
        for (int i = 0; i < oneBoxMinos.Length; i++)
        {
            nextLocalGrid[i] = new Vector2Int(oneBoxMinos[i].LocalGridPosition.y, -oneBoxMinos[i].LocalGridPosition.x);
            if (!minoMap.CheckCanMoveHere(newPos + nextLocalGrid[i]))
            {
                canMove = false;
                break;
            }
        }
        if (canMove)
        {
            gridPosition = newPos;
            UpdateRealPosition();
            for (int i = 0; i < oneBoxMinos.Length; i++)
            {
                oneBoxMinos[i].SetLocalGridPosition(nextLocalGrid[i]);
            }
        }
        return canMove;
    }
    public void RotateAnticlockwise()
    {
        bool preIsOnBlock = minoMap.ChackOnMino(this);
        bool check = false;
        check = RotateAnticlockwiseInNewPosition(gridPosition);
        if (minoName != "O")
        {
            for (int i = 0; i < 4; i++)
            {
                if (!check)
                {
                    Vector2Int delta = rotateAnticlockWiseDelta[degreeState, i].x * normalGridXMeaningin45Grid + rotateAnticlockWiseDelta[degreeState, i].y * normalGridYMeaningin45Grid;
                    check = RotateAnticlockwiseInNewPosition(gridPosition + delta);
                }
            }
        }

        if (check)
        {
            degreeState--;
            if (degreeState == -1)
            {
                degreeState = 3;
            }
            if (preIsOnBlock)
            {
                ResetPutTimer();
                putCancelCounter++;
                UpdateReachedPosMinY();
            }
        }
    }

    private bool RotateAnticlockwiseInNewPosition(Vector2Int newPos)
    {
        bool canMove = true;
        Vector2Int[] nextLocalGrid = new Vector2Int[oneBoxMinos.Length];
        for (int i = 0; i < oneBoxMinos.Length; i++)
        {
            nextLocalGrid[i] = new Vector2Int(-oneBoxMinos[i].LocalGridPosition.y, oneBoxMinos[i].LocalGridPosition.x);
            if (!minoMap.CheckCanMoveHere(newPos + nextLocalGrid[i]))
            {
                canMove = false;
                break;
            }
        }
        if (canMove)
        {
            gridPosition = newPos;
            UpdateRealPosition();
            for (int i = 0; i < oneBoxMinos.Length; i++)
            {
                oneBoxMinos[i].SetLocalGridPosition(nextLocalGrid[i]);
            }
        }
        return canMove;
    }

    public string GetMinoName()
    {
        return minoName;
    }

    public enum MovedState
    {
        None,
        StopdByNormalBlock,
        StopdByWallBlock,
        Moved
    }
}
