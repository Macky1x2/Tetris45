using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMap : MonoBehaviour
{
    public static float MinoOneDistanceXY = 0.35355339059f;

    private const float gridToRialBiasX = -3.1819808f;
    private const float gridToRialBiasY = -4.5f;

    private int[] rdluVectorX = new int[]{ 1, 0, -1, 0 };   //�㉺���E�̃}�X���[�v�p
    private int[] rdluVectorY = new int[] { 0, -1, 0, 1 };

    private const int mapSizeX = 19;        //y���W�������̕����̈ړ��\�}�X��(��͈ړ��\�}�X����1������)
    private const int mapSizeY = 40;
    private OneBoxMino[,] map = new OneBoxMino[mapSizeX, mapSizeY];

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < (int)Math.Ceiling(mapSizeY / 2.0f); i++)
        //{
        //    map[0, i * 2 + 1] = -1;
        //    map[mapSizeX - 1, i * 2 + 1] = -1;
        //}
        //for(int i = 0; i < mapSizeX / 2; i++)
        //{
        //    map[i * 2 + 1, 0] = -1;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector2 ConverseFromGridToReal(Vector2Int pos)
    {
        Vector2 ret = new Vector2();
        ret.x = pos.x * MinoOneDistanceXY + gridToRialBiasX;
        ret.y = pos.y * MinoOneDistanceXY + gridToRialBiasY;
        return ret;
    }

    public void RegisterMinoToMap(Mino mino)
    {
        //�T�E���h
        SE.instance.PlaySoundByID(0);

        var xyList = new List<int>();
        var xyListrd = new List<int>();
        OneBoxMino[] minoBoxes = mino.oneBoxMinos;
        foreach(OneBoxMino minoBox in minoBoxes)
        {
            Vector2Int pos = mino.gridPosition + minoBox.LocalGridPosition;
            map[pos.x, pos.y] = minoBox;
            if (!xyList.Contains(pos.x - pos.y) && pos.x - pos.y <= 8)
            {
                xyList.Add(pos.x - pos.y);
            }
            if(!xyListrd.Contains(pos.x + pos.y) && pos.x + pos.y >= 10)
            {
                xyListrd.Add(pos.x + pos.y);
            }
        }

        var alighRowList = new List<int>();
        var alighRowListrd = new List<int>();
        bool roopCheck;
        do
        {
            roopCheck = false;
            //�����Ă�s(�E�����)��o�^
            foreach (int xy in xyList)
            {
                int y = mapSizeX - 1 - xy;
                bool canRemove = true;
                for (int i = 0; mapSizeX - 1 - i >= 0 && y - i >= 0; i++)
                {
                    int mapX = mapSizeX - 1 - i, mapY = y - i;
                    if (mapY >= mapSizeY || map[mapX, mapY] == null)
                    {
                        canRemove = false;
                        break;
                    }
                }

                if (canRemove)
                {
                    if (!alighRowList.Contains(y))
                    {
                        alighRowList.Add(y);
                    }
                }
            }
            //�����Ă�s(�E������)��o�^
            foreach (int xy in xyListrd)
            {
                int y = xy;
                bool canRemove = true;
                for (int i = 0; i < mapSizeX && y - i >= 0; i++)
                {
                    int mapX = i, mapY = y - i;
                    if (mapY >= mapSizeY || map[mapX, mapY] == null)
                    {
                        canRemove = false;
                        break;
                    }
                }

                if (canRemove)
                {
                    if (!alighRowListrd.Contains(y))
                    {
                        alighRowListrd.Add(y);
                    }
                }
            }
            if (alighRowList.Count > 0 || alighRowListrd.Count > 0)
            {
                roopCheck = true;

                //�T�E���h
                SE.instance.PlaySoundByID(1);

                //�o�^���ꂽ�s�̃X�R�A���Z(�G�t�F�N�g�o���Ȃ炱��), �o�^���ꂽ�s(�E�����)�̃I�u�W�F�N�g���폜
                foreach (int alignRow in alighRowList)
                {
                    int deltaScore = Math.Max(alignRow + 1, mapSizeX);
                    GameManager.instance.AddScore(deltaScore);
                    for (int i = 0; mapSizeX - 1 - i >= 0 && alignRow - i >= 0; i++)
                    {
                        if (map[mapSizeX - 1 - i, alignRow - i] != null) Destroy(map[mapSizeX - 1 - i, alignRow - i].gameObject);   //null�`�F�b�N�͕K�v�Ȃ����A�o�^���ꂽ�s(�E������)�Ɛ���������邽�߁B
                    }
                }
                //�o�^���ꂽ�s�̃X�R�A���Z(�G�t�F�N�g�o���Ȃ炱��), �o�^���ꂽ�s(�E������)�̃I�u�W�F�N�g���폜
                foreach (int alignRow in alighRowListrd)
                {
                    int deltaScore = Math.Max(alignRow + 1, mapSizeX);
                    GameManager.instance.AddScore(deltaScore);
                    for (int i = 0; i < mapSizeX && alignRow - i >= 0; i++)
                    {
                        if (map[i, alignRow - i] != null) Destroy(map[i, alignRow - i].gameObject);     //�o�^���ꂽ�s(�E�����)�ŏ����ꂽ�\�������邽��null�`�F�b�N
                    }
                }
                //�o�^�������ɗ��Ƃ�
                int tmpFallDistance;
                //x, y�����ɋ����̃}�X
                for (int x = 0; x < mapSizeX; x += 2)
                {
                    tmpFallDistance = 0;
                    for (int y = 2; y < mapSizeY; y += 2)
                    {
                        int fallDistance = tmpFallDistance;
                        int rowBaseY = mapSizeX - 1 - (x - y);
                        int rowBaseYrd = x + y;
                        if(alighRowList.Contains(rowBaseY - 2) || alighRowListrd.Contains(rowBaseYrd - 2))
                        {
                            fallDistance += 2;
                            tmpFallDistance = fallDistance;
                        }
                        if (alighRowList.Contains(rowBaseY) || alighRowListrd.Contains(rowBaseYrd)) continue;
                        if(fallDistance > 0)
                        {
                            map[x, y - fallDistance] = map[x, y];
                            if (map[x, y - fallDistance] != null) map[x, y - fallDistance].GoDownRow(fallDistance);
                            map[x, y] = null;   //?
                        }
                    }
                }
                //x, y�����Ɋ�̃}�X
                for (int x = 1; x < mapSizeX; x += 2)
                {
                    tmpFallDistance = 0;
                    for (int y = 3; y < mapSizeY; y += 2)
                    {
                        int fallDistance = tmpFallDistance;
                        int rowBaseY = mapSizeX - 1 - (x - y);
                        int rowBaseYrd = x + y;
                        if (alighRowList.Contains(rowBaseY - 2) || alighRowListrd.Contains(rowBaseYrd - 2))
                        {
                            fallDistance += 2;
                            tmpFallDistance = fallDistance;
                        }
                        if (alighRowList.Contains(rowBaseY) || alighRowListrd.Contains(rowBaseYrd)) continue;
                        if (fallDistance > 0)
                        {
                            map[x, y - fallDistance] = map[x, y];
                            if (map[x, y - fallDistance] != null) map[x, y - fallDistance].GoDownRow(fallDistance);
                            map[x, y] = null;   //?
                        }
                    }
                }
            }
            


            ////�����Ă�s��o�^
            //foreach (int xy in xyList)
            //{
            //    int y = mapSizeX - 1 - xy;
            //    bool canRemove = true;
            //    for (int i = 0; mapSizeX - 1 - i >= 0 && y - i >= 0; i++)
            //    {
            //        int mapX = mapSizeX - 1 - i, mapY = y - i;
            //        if (mapY >= mapSizeY || map[mapX, mapY] == null)
            //        {
            //            canRemove = false;
            //            break;
            //        }
            //    }

            //    if (canRemove)
            //    {
            //        if (!alighRowList.Contains(y))
            //        {
            //            alighRowList.Add(y);
            //        }
            //    }
            //}
            //if (alighRowList.Count > 0)
            //{
            //    //�o�^���ꂽ�s�̃X�R�A���Z(�G�t�F�N�g�o���Ȃ炱��), �o�^���ꂽ�s�̃I�u�W�F�N�g���폜
            //    foreach (int alignRow in alighRowList)
            //    {
            //        int deltaScore = Math.Max(alignRow + 1, mapSizeX);
            //        GameManager.instance.AddScore(deltaScore);
            //        for (int i = 0; mapSizeX - 1 - i >= 0 && alignRow - i >= 0; i++)
            //        {
            //            Destroy(map[mapSizeX - 1 - i, alignRow - i].gameObject);
            //        }
            //    }
            //    //�o�^���ꂽ�s����p���āA�S�}�X�̂��炵�������v�Z
            //    int[,] deltaMap = new int[mapSizeX, mapSizeY];
            //    foreach (int alignRow in alighRowList)
            //    {
            //        if (alignRow < mapSizeX - 1)
            //        {
            //            for (int i = 0; mapSizeX - 1 - i >= 0 && alignRow - i >= 0; i++)
            //            {
            //                int baseX = mapSizeX - 1 - i, baseY = alignRow - i;
            //                for (int j = baseY + 2; j < mapSizeY; j += 2)
            //                {
            //                    deltaMap[baseX, j] += 2;
            //                }
            //            }
            //        }
            //    }
            //    //
            //    int[] effectersLowerIDs = new int[(mapSizeY + 1) / 2 + mapSizeX / 2];
            //    effectersLowerIDs[0] = 0;
            //    for (int i = 1; i < effectersLowerIDs.Length; i++)
            //    {
            //        effectersLowerIDs[i] = effectersLowerIDs[i - 1];
            //        if ((i - 1) * 2 >= mapSizeX - 1 && alighRowList.Contains((i - 1) * 2))
            //        {
            //            effectersLowerIDs[i] += 2;
            //        }
            //    }
            //    //���炵���������Ƃ�
            //    //x, y�����ɋ����̃}�X
            //    for (int x = 0; x < mapSizeX; x += 2)
            //    {
            //        for (int y = 0; y < mapSizeY; y += 2)
            //        {
            //            int rowBaseY = mapSizeX - 1 - (x - y);
            //            if (alighRowList.Contains(rowBaseY)) continue;
            //            int fallDistance = deltaMap[x, y] + effectersLowerIDs[rowBaseY / 2];
            //            if (fallDistance > 0)
            //            {
            //                map[x, y - fallDistance] = map[x, y];
            //                if (map[x, y - fallDistance] != null) map[x, y - fallDistance].GoDownRow(fallDistance);
            //                //if (map[x, y] != null) Destroy(map[x, y].gameObject);
            //            }
            //        }
            //    }
            //    //x, y�����Ɋ�̃}�X
            //    for (int x = 1; x < mapSizeX; x += 2)
            //    {
            //        for (int y = 1; y < mapSizeY; y += 2)
            //        {
            //            int rowBaseY = mapSizeX - 1 - (x - y);
            //            if (alighRowList.Contains(rowBaseY)) continue;
            //            int fallDistance = deltaMap[x, y] + effectersLowerIDs[rowBaseY / 2];
            //            if (fallDistance > 0)
            //            {
            //                map[x, y - fallDistance] = map[x, y];
            //                if (map[x, y - fallDistance] != null) map[x, y - fallDistance].GoDownRow(fallDistance);
            //                //if (map[x, y] != null) Destroy(map[x, y].gameObject);
            //            }
            //        }
            //    }
            //}

            //������
            alighRowList.Clear();
            alighRowListrd.Clear();
            xyList.Clear();
            xyListrd.Clear();
            for (int i = 10; i < mapSizeY; i += 2)
            {
                xyList.Add(mapSizeX - 1 - i);
                xyListrd.Add(i);
            }
        } while (roopCheck);
        

        ////�����Ă�s���폜
        //foreach(int y in yList)
        //{
        //    CheckRemoveableRow(y);
        //}
    }

    private void CheckRemoveableRow(int y)
    {
        bool check = true;
        for (int x = y % 2 == 0 ? 0 : 1; x < mapSizeX; x += 2)
        {
            if (map[x, y] == null)
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            RemoveRow(y);
        }
    }

    private void RemoveRow(int y)
    {
        //�X�R�A���Z
        int deltaScore = y % 2 == 0 ? (mapSizeX + 1) / 2 : mapSizeX / 2;
        GameManager.instance.AddScore(deltaScore);

        for(int checkY = y; checkY + 2 < mapSizeY; checkY += 2)
        {
            for (int x = y % 2 == 0 ? 0 : 1; x < mapSizeX; x += 2)
            {
                if(map[x, checkY]!=null) Destroy(map[x, checkY].gameObject);
                if (map[x, checkY + 2] != null) map[x, checkY + 2].GoDownRow(2);
                map[x, checkY] = map[x, checkY + 2];
                map[x, checkY + 2] = null;
            }
        }
    }

    public bool ChackOnMino(Mino mino)
    {
        Vector2Int fallPos = mino.gridPosition + new Vector2Int(0, -1);
        bool nonret = true;
        foreach (OneBoxMino minoBox in mino.oneBoxMinos)
        {
            if(!CheckCanMoveHere(fallPos + minoBox.LocalGridPosition))
            {
                nonret = false;
                break;
            }
        }

        return !nonret;
    }

    public bool CheckCanMoveHere(Vector2Int pos)
    {
        //���͈͊O
        if (pos.x < 0 || pos.x == 0 && pos.y % 2 == 1)
        {
            return false;
        }
        //�E�͈͊O
        if (pos.x >= mapSizeX || pos.x == mapSizeX - 1 && pos.y % 2 == 1)
        {
            return false;
        }
        //���͈͊O
        if (pos.y < 0 || pos.y == 0 && pos.x % 2 == 1)
        {
            return false;
        }
        //��~�u���b�N�ɏd�Ȃ肤��}�X
        if (pos.x % 2 == 0 && pos.y % 2 == 0 || pos.x % 2 == 1 && pos.y % 2 == 1)
        {
            return map[pos.x, pos.y] == null;
        }
        //4�̒�~�u���b�N�ɏd�Ȃ肤��}�X
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            if (map[pos.x + rdluVectorX[i], pos.y + rdluVectorY[i]] != null)
            {
                ret = false;
                break;
            }
        }
        return ret;
    }

    //public Vector2Int CalcNextGridPosition(Vector2Int nowPos, Vector2Int delta)
    //{

    //}

    //private int InterpretMap(int x, int y)
    //{
    //    if (x < 0)
    //    {
    //        return -1;
    //    }
    //    if (x >= mapSizeX)
    //    {
    //        return -1;
    //    }
    //    if (y < 0)
    //    {
    //        return -1;
    //    }
    //    return map[x,y];
    //}
}
