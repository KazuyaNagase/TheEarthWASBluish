using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderThreshold : MonoBehaviour
{
    public bool isHot; //ごめん
    const float threshold = MainScene.MODE_CHANGE_PERCENTAGE;
    const int width = 1260;

    // Start is called before the first frame update
    void Start()
    {
        var x = (width/2) * threshold;
        if(isHot) x *= -1;
        else x -= 25; //すまん
        var position = transform.position;
        transform.position = new Vector3(x, position.y, position.z);
    }
}
