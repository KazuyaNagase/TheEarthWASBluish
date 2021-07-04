using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    // タイトルとLPのBGM
    [SerializeField] private AudioClip TitleBgm;
    // ゲーム中のBGM
    [SerializeField] private AudioClip MainBgm;
    // リザルトのBGM
    [SerializeField] private AudioClip ResultBgm;

    // 各シーンのボリューム
    private static Dictionary<string, float> volumes = new Dictionary<string, float>
    {
        {"Title", 1.0f},
        {"Main", 0.8f},
        {"Result", 0.6f}
    };

    private AudioSource bgm;

    static private bool isCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        // もういるなら消す
        if (isCreated)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            isCreated = true;
        }

        // シーン移動で破壊しない
        DontDestroyOnLoad(this);

        // イベントハンドラの登録
        SceneManager.sceneLoaded += OnSceneLoad;

        // AudioSourceの取得
        bgm = gameObject.GetComponent<AudioSource>();

        // 最初はTitle
        playBgm(TitleBgm, "Title");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void playBgm(AudioClip clip, string SceneName)
    {
        if(bgm.isPlaying) bgm.Stop();
        bgm.clip = clip;
        bgm.volume = volumes[SceneName];
        bgm.Play();
    }

    private void OnSceneLoad(Scene newScene, LoadSceneMode mode)
    {
        var sceneName = newScene.name;
        switch(sceneName)
        {
            case "Title":
                playBgm(TitleBgm, sceneName);
                break;
            case "LP":
                // そのまま
                break;
            case "Main":
                playBgm(MainBgm, sceneName);
                break;
            case "Result":
                playBgm(ResultBgm, sceneName);
                break;
            default:
                break;
        }
    }
}
