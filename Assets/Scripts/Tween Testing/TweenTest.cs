using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; 
public class TweenTest : MonoBehaviour
{
    [SerializeField] GameObject sampleButton; 

    private void Start()
    {
        var uiSequence = DOTween.Sequence();
        uiSequence.Append(sampleButton.transform.DOScale(new Vector3(1.2f, 1.2f), 1.0f).SetLoops(500, LoopType.Yoyo).SetEase(Ease.InOutSine));  
    }

}
