using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private MinoMap minoMap;
    [SerializeField] private Mino[] minoPrefabs = new Mino[7];

    [SerializeField] private Text scoreText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text gameoverText;
    [SerializeField] private MinoDisplayer containerMinoDisplay;
    [SerializeField] private MinoDisplayer[] nextMinoDisplays = new MinoDisplayer[5];

    public float minoFallTime{ get; private set; }
    public float minoPutTime { get; private set; }

    private int replacedCounter;
    private int containerMinoID = -1;

    private int score;
    private int wave;

    private List<int> minoTable = new List<int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void subroutine()
    {
        Debug.Log("サブルーチンコール");
    }

    // Start is called before the first frame update
    void Start()
    {
        //ミノ系変数初期化
        minoFallTime = 0.5f;
        minoPutTime = 0.5f;

        Add7IDToMinoTable();
        Add7IDToMinoTable();
        UpdateNextDisplays();
        SpawnMino();
        replacedCounter = 0;

        score = 0;
        UpdateScoreText();

        wave = 1;
        UpdateWaveText();
    }

    private void Add7IDToMinoTable()
    {
        List<int> tmpList = new List<int>();
        for (int i = 0; i < 7; i++)
        {
            while (true)
            {
                int randomID = UnityEngine.Random.Range(0, 7);
                if (!tmpList.Contains(randomID))
                {
                    tmpList.Add(randomID);
                    break;
                }
            }
        }

        for(int i = 0; i < 7; i++)
        {
            minoTable.Add(tmpList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Gameover()
    {
        Time.timeScale = 0f;
        gameoverText.gameObject.SetActive(true);
    }

    public void NotifyMinoSeted()
    {
        SpawnMino();
    }

    private void SpawnMino()
    {
        Instantiate(minoPrefabs[minoTable[0]]).Initialize(minoMap);
        minoTable.RemoveAt(0);
        if (minoTable.Count <= 7)
        {
            Add7IDToMinoTable();
        }
        UpdateNextDisplays();

        replacedCounter = 0;
    }

    public void AddScore(int delta)
    {
        score += delta * wave;
        UpdateScoreText();

        if(score>=(int)Math.Pow(wave, 3 / 2.0f) * 50)
        {
            wave++;
            minoFallTime = 0.5f / wave;
            UpdateWaveText();
        }
    }

    public bool NotifyWantedReplace(string pushMinoName)
    {
        if (replacedCounter > 0) return false;
        int pushMinoID = ConvertNameToID(pushMinoName);

        if (containerMinoID == -1)
        {
            Instantiate(minoPrefabs[minoTable[0]]).Initialize(minoMap);
            minoTable.RemoveAt(0);
            if (minoTable.Count <= 7)
            {
                Add7IDToMinoTable();
            }
            UpdateNextDisplays();
        }
        else
        {
            Instantiate(minoPrefabs[containerMinoID]).Initialize(minoMap);
        }
        containerMinoID = pushMinoID;
        containerMinoDisplay.UpdateDisplayer(containerMinoID);

        replacedCounter++;
        return true;
    }

    private void UpdateNextDisplays()
    {
        for(int i = 0; i < nextMinoDisplays.Length; i++)
        {
            nextMinoDisplays[i].UpdateDisplayer(minoTable[i]);
        }
    }

    private int ConvertNameToID(string name)
    {
        int ret = 0;
        switch (name)
        {
            case "I": ret = 0; break;
            case "J": ret = 1; break;
            case "L": ret = 2; break;
            case "O": ret = 3; break;
            case "S": ret = 4; break;
            case "T": ret = 5; break;
            case "Z": ret = 6; break;
        }
        return ret;
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"{score}";
    }

    private void UpdateWaveText()
    {
        waveText.text = $"wave: {wave}";
    }
}
