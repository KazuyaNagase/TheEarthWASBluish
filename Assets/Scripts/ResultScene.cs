using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    // スコアのTEXT
    [SerializeField] private Text score;
    
    // スコアと西暦の変換率
    public const float yearRate = 50;

    [SerializeField] private GameObject SEPrefab;
    [SerializeField] private AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        // 年数に変換
        int year = (int)(Player.GetScore() * yearRate);
        // Scoreを設定
        score.text = year.ToString() + '年';
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            GameObject SE = Instantiate(SEPrefab);
            SEInstance script = SE.GetComponent<SEInstance>();
            script.Create(clip);
            
            SceneManager.LoadScene("Title");
        }

    }
}
