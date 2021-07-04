using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SEInstance : MonoBehaviour
{
    private AudioSource SEsource;

    public void Create(AudioClip clip)
    {
        SEsource = gameObject.GetComponent<AudioSource>();
        SEsource.clip = clip;
    }

    void Start()
    {
        // シーン移動で破壊しない
        DontDestroyOnLoad(this);

        SEsource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SEsource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
