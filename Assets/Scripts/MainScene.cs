using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public enum PlayerMode
{
	NEUTRAL,
	HOT,
	COLD,
}

public class MainScene : MonoBehaviour
{
	// 隕石のプレハブ
	[SerializeField] private GameObject meteorPrefab;
	// 土偶のプレハブ
	[SerializeField] private GameObject doguPrefab;
	// 地球のプレハブ
	[SerializeField] private GameObject playerPrefab;
	// 爆発のプレハブ
	[SerializeField] private GameObject explosionPrefab;
	// 背景ノード
	[SerializeField] private GameObject backGround;
	// 通常状態の背景
	[SerializeField] private Sprite NeutralBackGround;
	// 高温状態の背景
	[SerializeField] private Sprite HotBackGround;
	// 低温状態の背景
	[SerializeField] private Sprite ColdBackGround;
	// 地球の状態を表すテキスト
	[SerializeField] private Text statusText;
	// 現在のスコアを表すテキスト
	[SerializeField] private Text scoreText;

	// 隕石管理プール
	public List<GameObject> meteorPool{get;} = new List<GameObject>();

	// フェイズリスト
	private Queue<Phase> phases = new Queue<Phase>();

	// 現在のフェイズが始まった時間
	private float phaseStartTime = 0;

	// 前回隕石を追加した時間
	private float lastMeteorAddTime = 0;

	// プレイヤー
	GameObject player;

	// 現在のモード
	public PlayerMode mode{get; private set;}

    // HP何割でメリットモードになるか
    public const float MODE_CHANGE_PERCENTAGE = 0.65f;
    public const float MODE_CHANGE_THRESHOLD = Player.HP_THRESHOLD * MODE_CHANGE_PERCENTAGE;
	
	private float start_time;

	public float player_hp{ get{ return this.player.GetComponent<Player>().HP; } }
	public Vector3 player_pos{ get{ return this.player.GetComponent<Player>().transform.position; } }

	private GameObject background;
    private AudioSource SEsource;
    [SerializeField] private AudioClip onAllDestroy;
    [SerializeField] private AudioClip onSmallDestroy;
    


	// Start is called before the first frame update
	void Start()
	{
		// プレイヤーを召喚
		player = Instantiate(playerPrefab) as GameObject;

        // 最初に全Phaseの情報を設定
        phases.Enqueue(new Phase(8, 2, 1f, 6, 0.2f));
        phases.Enqueue(new Phase(8, 4, 1f, 5, 0.2f));
        phases.Enqueue(new Phase(8, 5, 0.7f, 4, 0.2f));
        phases.Enqueue(new Phase(8, 8, 0.8f, 4, 0.5f));
        phases.Enqueue(new Phase(8, 10, 0.7f, 3, 0.5f));
        phases.Enqueue(new Phase(8, 12, 0.7f, 3, 0.5f));
		phases.Enqueue(new Phase(8, 14, 0.7f, 3, 0.5f));
		phases.Enqueue(new Phase(8, 16, 0.7f, 3, 0.5f));
        phases.Enqueue(new Phase(16, 18, 0.7f, 2, 0.5f));
		phases.Enqueue(new Phase(8, 20, 0.7f, 2, 0.5f));

        // AudioSource取得
        SEsource = gameObject.GetComponent<AudioSource>();

		// 初期化
		SceneInit();
	}

	// Update is called once per frame
	void Update()
	{
		var nowTime = Time.time - this.start_time;
		var nowPhase = phases.Peek();

		// モードを更新
		UpdateMode();
		
		// スコアを更新
		updateScore();

		// 前回に隕石を追加してから時間が経っていれば追加
		if (nowTime - lastMeteorAddTime >= nowPhase.popInterval)
		{
			// 隕石追加
			AddMeteors(nowPhase.popCount, nowPhase.popProb);
			// 土偶を追加
			AddDogu(nowPhase.popDoguProb);
			// 最後に隕石発射した時刻を記憶
			lastMeteorAddTime = nowTime;
		}

		// 長さが1以下ならフェイズ移行はしないでフェイズのUpdate終了
		if (shouldChangePhase(nowTime, nowPhase))
		{
			ToNextPhase(nowTime);
		}
	}

	private void UpdateMode()
	{
		float hp = player.GetComponent<Player>().HP;
		if (hp < -MODE_CHANGE_THRESHOLD)
		{
            // 小隕石の爆発が起きる
            if (mode != PlayerMode.HOT)
            {
                ExplodeSmallMeteors();
            }
			mode = PlayerMode.HOT;
			// モードで背景を切り替える
			// TODO?: Modeみて諸々を切り替える(モードが切り替わったときだけ変更するようにする)
			backGround.GetComponent<Image>().sprite = HotBackGround;
			statusText.text = "地球が高温状態 小さな隕石が消滅";

		}
		else if(hp > MODE_CHANGE_THRESHOLD)
		{
			mode = PlayerMode.COLD;
			// モードで背景を切り替える
			// TODO?: Modeみて諸々を切り替える(モードが切り替わったときだけ変更するようにする)
			backGround.GetComponent<Image>().sprite = ColdBackGround;
			statusText.text = "地球が低温状態 すべての隕石のスピードダウン";
		}
		else
		{
			mode = PlayerMode.NEUTRAL;
			// モードで背景を切り替える
			// TODO?: Modeみて諸々を切り替える(モードが切り替わったときだけ変更するようにする)
			backGround.GetComponent<Image>().sprite = NeutralBackGround;
			statusText.text = ""; // 空にする
		}
	}

	private bool shouldChangePhase(float nowTime, Phase nowPhase)
	{
		// 最後の1つならフェイズ移行無し
		if (phases.Count <= 1) return false;
		// フェイズが始まってから時間が経っていれば次のフェイズへ
		return (nowTime - phaseStartTime >= nowPhase.time);
	}

	// 次のフェイズに移行
	private void ToNextPhase(float currentTime)
	{
		// フェイズ更新
		phases.Dequeue();
		// フェイズ開始時間をセット
		phaseStartTime = currentTime;
	}

	// count数の隕石を，確率probabilityで追加
	void AddMeteors(int count, float probability)
	{
		for (int i=0; i<count; i++)
		{
			if (Random.Range(0f,1f) < probability)
			{
				AddMeteor();
			}
		}
	}

	// 確率で土偶を追加
	void AddDogu(float probability)
	{
		if (Random.Range(0f,1f) < probability)
		{
			GameObject dogu = Instantiate(doguPrefab);
			meteorPool.Add(dogu);
		}
	}

	// 隕石を一つ生成し，隕石管理プールに追加
	void AddMeteor()
	{
		GameObject meteor = Instantiate(meteorPrefab);
		meteorPool.Add(meteor);
	}
	// Explosion追加
	public void DestroyAllMeteors()
	{
        SEsource.PlayOneShot(onAllDestroy);
        
        // スコア加算
        // NOTE: 隕石の数 * 10
        Player.AddScore(meteorPool.Count);
        
        // NOTE: 土偶に当たったら次弾発射までの時間を戻す
        var nowTime = Time.time - this.start_time;
        lastMeteorAddTime = nowTime;
        
		foreach(GameObject item in meteorPool){
			if(item != null){ Instantiate(explosionPrefab, item.transform.position, item.transform.rotation); }	
			Destroy(item);
		}
	}
	public void ExplodeSmallMeteors()
	{
		var Destroyed = false;
		foreach(GameObject item in meteorPool){
			if(item == null) continue;
			
			var meteor = item.GetComponent<Meteor>();
			if (meteor == null) continue;
			if (meteor.type != MeteorType.SMALL) continue;

			Destroyed = true;
			Instantiate(explosionPrefab, item.transform.position, item.transform.rotation); 
			// Destroy自体はsmallMeteor自体がやる
		}

		if (Destroyed)
		{
			SEsource.PlayOneShot(onSmallDestroy);
		}
	}

	void SceneInit()
	{
		this.start_time = Time.time;
		phaseStartTime = lastMeteorAddTime = 0.0f;
	}

	void updateScore()
	{
		// Scoreを設定
		// TODO: ここら辺よくないが最後なので許容
		int year = (int)(Player.GetScore() * ResultScene.yearRate);
		scoreText.text = year.ToString();
	}
}
