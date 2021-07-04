using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	// Define
	private const float MOVE_RATE   = 550.0f;  // 移動率 (移動量)
	private const float DAMAGE_RATE = 40.0f;   // 気温変化率 (気温変化速度)

	private const float SCORE_DIGIT  = 100.0f; // スコアの有効数字
	private const float DAMAGE_DIGIT = 10.0f;  // ダメージ計算時の有効数字
	public  const float HP_THRESHOLD = 50.0f;  // HPの閾値 (-HP_THRESHOLD <= x <= HP_THRESHOLD)

	private const float PLAYER_INIT_HP = 0.0f; // 初期HP
	private const float PLAYER_ANGLE = 30.0f;  // プレイヤー回転角度

	// 移動を正規化デバイスへ
	private const float STAGE_UP_RANGE    = 0.85f; // 上: 移動範囲(割合)
	private const float STAGE_DOWN_RANGE  = 0.05f; // 下: 移動範囲(割合)
	private const float STAGE_RIGHT_RANGE = 0.95f; // 右: 移動範囲(割合)
	private const float STAGE_LEFT_RANGE  = 0.05f; // 左: 移動範囲(割合)

	private const int KEY_UP    = 0x01;
	private const int KEY_DOWN  = 0x02;
	private const int KEY_RIGHT = 0x04;
	private const int KEY_LEFT  = 0x08;

	private const float HP_PERCENTAGE = 0.80f; // 皮の切り替え率

	// Var : private
	private float hp;
	private Vector3 player_pos;
	private bool isExist;
	private GameObject parent; // manager
	private SpriteRenderer MainSpriteRenderer;

	[SerializeField] private GameObject explosionPrefab;

	private GameObject explosion;

	// var : public
	public static float player_score;
	public Sprite[] PlayerSprite = new Sprite[3];

	private AudioSource SEsource;
	
	[SerializeField] private AudioClip onDogu;
	[SerializeField] private AudioClip onDestroy;

	[SerializeField] private GameObject SEPrefab;

	public float HP
	{
		get{ return this.hp; }
		private set{ this.hp = value; }
	}
	public bool ExistFlag
	{
		get{ return this.isExist; }
		private set{ this.isExist = value; }
	}

	public static float GetScore(){ return player_score;}

	// スコア加算
	public static void AddScore(float value)
	{
		player_score += value;
	}

    // Start is called before the first frame update
    void Start()
    {
		// Playerステータス: 初期化
		// HP, 生存フラグ, スコア, 土偶衝突
		this.hp = PLAYER_INIT_HP;
		player_score = 0.0f;
		this.isExist = true;

		// Managerを取得
		this.parent = GameObject.Find("Manager");
		this.MainSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

		// SE
		this.SEsource = gameObject.GetComponent<AudioSource>();

		explosion = null;
    }

    // Update is called once per frame
    void Update()
    {
		PlayerMove(MoveDirection(GetKeyboard())); // Move
		PlayerCalcHP(CalcHP()); // HP
		PlayerJudgeExist();     // 生存判定
		PlayerScore(); // 生存時間計算
		PlayerRotation(); // プレイヤー回転
		ViewModel(); // 皮の変更

    	PlayerEnd(); // 終了判定の処理
	}

	// PRO:KEYBOARD
	int GetKeyboard()
	{
		int code = 0;
		if(Input.GetKey(KeyCode.UpArrow))    code |= KEY_UP;
		if(Input.GetKey(KeyCode.DownArrow))  code |= KEY_DOWN;
		if(Input.GetKey(KeyCode.RightArrow)) code |= KEY_RIGHT;
		if(Input.GetKey(KeyCode.LeftArrow))  code |= KEY_LEFT;
		return code;
	}
	Vector3 MoveDirection(int key_code)
	{
		Vector3 cp_pl = new Vector3(0.0f, 0.0f, 0.0f);
		if((key_code&KEY_UP) == KEY_UP) cp_pl.y += MOVE_RATE;
		if((key_code&KEY_DOWN) == KEY_DOWN) cp_pl.y -= MOVE_RATE;
		if((key_code&KEY_RIGHT) == KEY_RIGHT) cp_pl.x += MOVE_RATE;
		if((key_code&KEY_LEFT) == KEY_LEFT) cp_pl.x -= MOVE_RATE;
		return cp_pl * Time.deltaTime;
	}
	// PRO:HP
	float CalcHP()
	{
		float px = (Camera.main.WorldToViewportPoint(transform.position).x-0.5f); // Player座標をViewport変換して, 中央中心へ
		return (Mathf.Floor(px*DAMAGE_RATE*DAMAGE_DIGIT)/DAMAGE_DIGIT); // 有効数字を考慮したダメージ計算
	}
	// プレイヤーの移動: 正規化デバイス
	void PlayerMove(Vector3 m_pos)
	{
		if (!isExist) return;

		Vector3 dest_pos = Camera.main.WorldToViewportPoint(this.transform.position + m_pos);
		if(dest_pos.y > STAGE_UP_RANGE){ dest_pos.y = STAGE_UP_RANGE; }
		if(dest_pos.y < STAGE_DOWN_RANGE){ dest_pos.y = STAGE_DOWN_RANGE; }
		if(dest_pos.x > STAGE_RIGHT_RANGE){ dest_pos.x = STAGE_RIGHT_RANGE; }
		if(dest_pos.x < STAGE_LEFT_RANGE){ dest_pos.x = STAGE_LEFT_RANGE; }
		this.transform.position = Camera.main.ViewportToWorldPoint(dest_pos);
	}
	// ダメージ適応
	void PlayerCalcHP(float diff)
	{
		if (!isExist) return;

		// HPの限界値(HP_THRESHOLD)を超えないように, HPを更新する.
		diff *= Time.deltaTime;
		this.hp = ((Mathf.Abs(this.hp+diff) > HP_THRESHOLD) ? HP_THRESHOLD*Mathf.Sign(this.hp) : this.hp+diff);
	}
	// 生存判定
	void PlayerJudgeExist()
	{
		if (!isExist) return;

		// HPがゼロ -> 力尽きる
		if(Mathf.Abs(this.hp) >= HP_THRESHOLD) isExist = false;
	}
	// 衝突処理 
	void OnTriggerEnter2D (Collider2D c)
	{
		if (!isExist) return;

		GameObject collider = c.gameObject;
		// ダメージ
		if (collider.CompareTag("DamageObject")) {
			isExist = false;

			// シーン遷移で消えないSE再生
			GameObject SE = Instantiate(SEPrefab);
            SEInstance script = SE.GetComponent<SEInstance>();
            script.Create(onDestroy);

			// 爆発
			explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
		}
		if (collider.CompareTag("Dogu")){
			SEsource.PlayOneShot(onDogu);
			parent.GetComponent<MainScene>().DestroyAllMeteors();
		}
	}
	// プレイヤー回転制御
	void PlayerRotation()
	{
		this.transform.rotation *= Quaternion.AngleAxis((PLAYER_ANGLE * Time.deltaTime), (new Vector3(0.0f, 0.0f, 1.0f)));
	}
	// 終了判定の処理
	void PlayerEnd()
	{
		if (isExist) return;
		gameObject.SetActive(false); // 死んでいたら地球を消す
		// 1秒後にスコア結果に飛ばす
		Invoke("gotoScore", 1.0f); // TODO: シーン名一覧とかに逃すべき
	}
	
	void gotoScore()
	{
		SceneManager.LoadScene("Result");
	}
	// 生存時間の経過
	void PlayerScore()
	{
		// スコア加算
		AddScore(Time.deltaTime);
	}
	void ViewModel()
	{
		// 現在のモードを取得
		var mode = parent.GetComponent<MainScene>().mode;
		switch(mode)
		{
			// 温度で地球の見た目を変更(0 - Hot/ 1 - Neutral/ 2 - Cold)
			case PlayerMode.NEUTRAL:
				MainSpriteRenderer.sprite = PlayerSprite[1];
				break;
			case PlayerMode.HOT:
				MainSpriteRenderer.sprite = PlayerSprite[0];
				break;
			case PlayerMode.COLD:
				MainSpriteRenderer.sprite = PlayerSprite[2];
				break;
			default:
				// 何もしない
				break;
		}
	}

}
