using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    public static SE instance;

    private AudioSource[] source;
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
        source = GetComponents<AudioSource>();
    }

    public void PlaySoundByID(int id)
    {
        source[id].Play();
    }
}
