using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoController : MonoBehaviour
{
    private Mino mino;

    private const float lrInputTime = 0.1f;
    private const float DashTimeMulti = 3f;
    private float lrInputTimer;

    // Start is called before the first frame update
    void Start()
    {
        mino = GetComponent<Mino>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
        UpdateRotate();
        UpdateReplace();
    }

    private void UpdateMove()
    {
        float inputX = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Horizontal"))
        {
            MoveLorR();
            lrInputTimer = 0;
        }
        else if (Input.GetButton("Horizontal"))
        {
            lrInputTimer += Input.GetButton("Dash") ? Time.deltaTime * DashTimeMulti : Time.deltaTime;
            if (lrInputTimer >= lrInputTime)
            {
                MoveLorR();
                lrInputTimer = 0;
            }
        }

        //“ü—Í‚É‰ž‚¶‚½•û‚Ö“®‚­
        void MoveLorR()
        {
            if (inputX == 1)
            {
                mino.MoveRight();
            }
            else if (inputX == -1)
            {
                mino.MoveLeft();
            }
        }
    }

    private void UpdateRotate()
    {
        if (Input.GetButtonDown("RotateClockwise"))
        {
            mino.RotateClockwise();
        }
        else if (Input.GetButtonDown("RotateAnticlockwise"))
        {
            mino.RotateAnticlockwise();
        }
    }

    private void UpdateReplace()
    {
        if (Input.GetButtonDown("Replace"))
        {
            if (GameManager.instance.NotifyWantedReplace(mino.GetMinoName()))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
