using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LPScene : MonoBehaviour
{
    public Sprite secondSprite;

    // 現在のページ
    private int pageCount;
    
    [SerializeField] private GameObject SEPrefab;
    [SerializeField] private AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        pageCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject SE = Instantiate(SEPrefab);
            SEInstance script = SE.GetComponent<SEInstance>();
            script.Create(clip);

            if (pageCount == 0)
            {
                ToNextPage();
            }
            else
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    void ToNextPage()
    {
        pageCount++;

        Image img = GameObject.Find("Background/Image").GetComponent<Image>();
        img.sprite = secondSprite;
    }
}
