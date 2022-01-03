using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpriteFiller : MonoBehaviour
{

    [SerializeField]
    UnityEngine.UI.Image _img;


    SpeechBubble parentBubble;

    void Awake()
    {
        parentBubble = GetComponentInParent<SpeechBubble>();
    }

    public void SetFill(float v)
    {
        _img.fillAmount = parentBubble.OwningOnomatopoeia.SpeechBubbleSizeCurve.Evaluate(v);
    }
}
