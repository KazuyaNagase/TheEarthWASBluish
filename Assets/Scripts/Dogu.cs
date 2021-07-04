using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dogu : MonoBehaviour
{
    private static float speed = 50f;

	private Vector3 angle;

	// Dogu Bullet
	[SerializeField] private GameObject dogu_bullet_prefab;
	
	private const float bullet_range = 4.0f; // 発射頻度
	private float bullet_time = 0.0f; // 発射経過時間
	private List<GameObject> dungan_list = new List<GameObject>();

	private AudioSource SEsource;

	// Start is called before the first frame update
	void Start()
	{
		// 適当に初期位置と行き先を決め，角度を出す
		// TODO: 画面内を通るようにゴールを設定
		var start = getRandomInitPos();
		var goal = getRandomInitPos();
		angle = (goal - start).normalized;

		// 位置と速度を反映
		transform.position = start;
		DirectionInit(start, goal);

		SEsource = gameObject.GetComponent<AudioSource>();
		SEsource.Play();
	}

	// Update is called once per frame
	void Update()
	{
		// 位置を更新
		transform.position += angle * speed * Time.deltaTime;

		// 球を発射
		LaunchBullet();
	}

	static Vector3 getRandomInitPos()
	{
		int width = Screen.width;
		int height = Screen.height;
		float radius = Mathf.Sqrt(Mathf.Pow(width/2, 2) + Mathf.Pow(height/2, 2));
		float rad = Random.Range(0, 2 * Mathf.PI);

		Vector3 position = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

		return position;
	}

	void LaunchBullet()
	{
		bullet_time += (Time.deltaTime);
		if(bullet_time > bullet_range){
			bullet_time = 0.0f;
			GameObject bullets = Instantiate(dogu_bullet_prefab, this.transform.position, this.transform.rotation) as GameObject;
			dungan_list.Add(bullets);
		}
	}
	// Dogu消滅時
	void OnDestroy()
	{
		foreach(GameObject item in dungan_list){
			Destroy(item);
		}
	}
	// Dogu初期方向
	void DirectionInit(Vector3 start, Vector3 goal)
	{
		transform.rotation *= Quaternion.AngleAxis(Vector3.Angle(goal, start), (new Vector3(0.0f, 0.0f, 1.0f)));
	}
}
