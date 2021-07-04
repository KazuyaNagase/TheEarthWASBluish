using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoguBullet : MonoBehaviour
{
	public GameObject dogu_data;
    
	private static float speed = 120f;
	private Vector3 angle;

	private AudioSource SEsource;
    
	// Start is called before the first frame update
    void Start()
    {
        angle = InitBulletAngle();
		SEsource = gameObject.GetComponent<AudioSource>();
		SEsource.Play();
    }

    // Update is called once per frame
    void Update()
    {
		TransBullet();  // 玉の移動処理
		DiposeBullet(); // 玉の消滅処理
    }

	Vector3 InitBulletAngle()
	{
		return (GameObject.Find("Manager").GetComponent<MainScene>().player_pos - this.transform.position).normalized;
	}

	void TransBullet()
	{
       this.transform.position += angle * speed * Time.deltaTime; 
	}
	void DiposeBullet()
	{
		if(!GetComponent<Renderer>().isVisible){ Destroy(this.gameObject); }
	}
}
