using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public enum MeteorType{
    SMALL = 0,
    MEDIUM = 1,
    BIG = 2,
}

public class Meteor : MonoBehaviour
{
    // 通常隕石のSprite
    [SerializeField] private Sprite MediumSprite;
    // 大きな隕石のSprite
    [SerializeField] private Sprite BigSprite;
    // 小さな隕石のSprite
    [SerializeField] private Sprite SmallSprite;
    
    private GameObject parent;

    public MeteorType type{get; private set;}

    // 種類ごとの割合
    private static float[] typePercentage = {0.3f, 0.5f, 0.2f};

    // 種類ごとのパラメータ
    private static float[] speeds = {200f, 150f, 80f};
    private static float[] scales = {200f, 300f, 550f};

    // 速さ
    private float speed;

    // 寒い時どれだけ遅くなるか
    private const float SLOW_RATE = 0.5f;

    // 移動方向
    private Vector3 angle;

    // 寿命: とりあえず30秒
    // TODO?: 画面外への時間を算出
    private static float lifeTime = 30f;

	// ミーティア回転角
	private const float METEOR_ANGLE = 45.0f;

    void Start()
    {
        // Manager取得
        parent = GameObject.Find("Manager");

        // ランダムで種類を決定
        type = GetRandomType();

        // パラメータを初期化
        InitParameters(type);

        // 寿命設定
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        var nowSpeed = speed;

        // 現在のモードを取得
        var mode = parent.GetComponent<MainScene>().mode;
        switch(mode)
        {
            case PlayerMode.HOT:
                // 小さければ破壊
                if(type == MeteorType.SMALL)
                {
                    Destroy(gameObject);
                }
                break;
            case PlayerMode.COLD:
                // 遅くなる
                nowSpeed = SLOW_RATE * speed;
                break;
            default:
                // 何もしない
                break;
        }

        transform.position += angle * nowSpeed * Time.deltaTime;
		MeteorRotation();
    }

    // 各パラメータの初期化
    // Note: 本当はtypeを渡す必要はないが，typeに依存していることを明示するために引数にしています
    void InitParameters(MeteorType type)
    {
        // 種類依存のパラメータ
        speed = speeds[(int)type];
        var scale = scales[(int)type];
        transform.localScale = new Vector3(scale, scale, 0);
        // TODO: Spriteを変更する
        switch(type)
        {
            case MeteorType.SMALL:
                GetComponent<SpriteRenderer>().sprite = SmallSprite;
                break;
            case MeteorType.MEDIUM:
                GetComponent<SpriteRenderer>().sprite = MediumSprite;
                break;
            case MeteorType.BIG:
                GetComponent<SpriteRenderer>().sprite = BigSprite;
                break;       
        }
        
        // 位置・角度
        // 中心から見て，どの角度に隕石が発生するかをランダムで決定
        var PI = Mathf.PI;
        float baseRadian = Random.Range(PI/3, (2 + 2/3) * PI);
        // baseRadianを元に位置と角度を初期化
        transform.position = getInitFirstPos(baseRadian);
        angle = getInitAngle(baseRadian);
    }

    static MeteorType GetRandomType()
    {
        float random = Random.Range(0.0f, 1.0f);
        float sum = 0f;
        for (int i = 0; i < 3; i++)
        {
            sum += typePercentage[i];
            if (random < sum)
            {
                return (MeteorType)i;
            }
        }

        return MeteorType.MEDIUM;
    }

    static Vector3 getInitFirstPos(float rad)
    {
        // 対角線から半径計算
        int width = Screen.width;
        int height = Screen.height;
        float radius = Mathf.Sqrt(Mathf.Pow(width/2, 2) + Mathf.Pow(height/2, 2));

        Vector3 position = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        return position;
    }

    static Vector3 getInitAngle(float baseRad)
    {
        // ベースからπ(180度)回したものを軸とする
        float center = baseRad + Mathf.PI;
        // ±45度ランダムでぶれさせる
        float delta = Mathf.PI/4;
        float range = Random.Range(-delta, delta);
        float rad = center + range;

        var angle = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        return angle;
    }
	// ミーティア回転制御
	void MeteorRotation()
	{
		this.transform.rotation *= Quaternion.AngleAxis((METEOR_ANGLE * Time.deltaTime), (new Vector3(0.0f, 0.0f, 1.0f)));
	}
}
