using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour
{
	private const float BAR_BIAS = 0.50f;   // プレイヤーHPと気温バーの変換のためのバイアス
	private const float HP_WEIGHT = 100.0f; // スクロールバーの比重に適応

	public GameObject player_data;
	
	Slider hp_slider;

	// Start is called before the first frame update
    void Start()
    {
		player_data  = GameObject.Find("Manager");
		hp_slider = GameObject.Find("HpSlider").GetComponent<Slider>(); // Init slider
    }

    // Update is called once per frame
    void Update()
    {
		UpdateSlider(); // スライダー更新
    }

	// スライダーの表示更新
	void UpdateSlider()
	{
		float bar_value = ((player_data.GetComponent<MainScene>().player_hp/HP_WEIGHT) + BAR_BIAS); // プレイヤーのHPを気温バーの表記に合わせる
		hp_slider.value = bar_value; // 気温バー更新: (0.0 <= Value <= 1.0)
	}
}
