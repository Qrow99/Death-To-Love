using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private RawImage _backgroundImg;
    [SerializeField] private float _x, _y; 
    void Update()
    {
        _backgroundImg.uvRect = new Rect(_backgroundImg.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _backgroundImg.uvRect.size); 
    }
}
