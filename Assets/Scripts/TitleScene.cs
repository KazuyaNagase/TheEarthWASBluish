using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject SEPrefab;
    [SerializeField] private AudioClip clip;
    
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Screen.SetResolution(1024, 768, false, 30);

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject SE = Instantiate(SEPrefab);
            SEInstance script = SE.GetComponent<SEInstance>();
            script.Create(clip);

            SceneManager.LoadScene("LP");
        }
    }
}
