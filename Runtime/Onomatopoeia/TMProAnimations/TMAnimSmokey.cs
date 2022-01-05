using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimSmokey : TextMeshProAnimations
    {

        [SerializeField]
        bool _UseUnderline = true;

        [SerializeField]
        Color _UnderlineColor = Color.white;

        [SerializeField]
        bool _UseTextColor = true;

        [SerializeField]
        Color _TextColor = Color.white;

        [SerializeField]
        bool _UseOutline = true;


        int TextFaceColorPropertyID = -1;
        int UnderlineColorPropertyID = -1;
        int UnderlineXPropertyID = -1;
        int UnderlineYPropertyID = -1;
        int UnderlineDilatePropertyID = -1;
        int UnderlineSoftnessPropertyID = -1;
        int SoftnessPropertyID = -1;



        [SerializeField]
        Vector2 _MinMaxDilatation = Vector2.up;

        [SerializeField]
        Vector2 _MinMaxUnderlineDilatation = Vector2.up;

        [SerializeField]
        float _Speed = 1.0f;

        [SerializeField]
        bool _UseNormalizedTime = true;

        void Start()
        {
            SoftnessPropertyID = Shader.PropertyToID("_OutlineSoftness");

            UnderlineColorPropertyID = Shader.PropertyToID("_UnderlayColor");
            UnderlineXPropertyID = Shader.PropertyToID("_UnderlayOffsetX");
            UnderlineYPropertyID = Shader.PropertyToID("_UnderlayOffsetY");
            UnderlineDilatePropertyID = Shader.PropertyToID("_UnderlayDilate");
            UnderlineSoftnessPropertyID = Shader.PropertyToID("_UnderlaySoftness");

            TextFaceColorPropertyID = Shader.PropertyToID("_FaceColor");
        
        }


        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            float t = _UseNormalizedTime ? _Speed * normalizedAnimProgress : Time.time * _Speed;
            if(_UseOutline)
            {
                textComp.fontMaterial.SetFloat(SoftnessPropertyID, _MinMaxDilatation.x + Mathf.PingPong(t, _MinMaxDilatation.y));
            }

            if(_UseUnderline)
            {
                textComp.fontMaterial.SetColor(UnderlineColorPropertyID, _UnderlineColor);
                // textComp.fontMaterial.SetFloat(UnderlineDilatePropertyID, 0.4f);

                textComp.fontMaterial.SetFloat(UnderlineXPropertyID, Mathf.Cos(t * 1.7f) * 0.7f);
                textComp.fontMaterial.SetFloat(UnderlineYPropertyID, Mathf.Abs(Mathf.Sin(t)) + 0.7f);

                textComp.fontMaterial.SetFloat(UnderlineSoftnessPropertyID, _MinMaxUnderlineDilatation.x + Mathf.PingPong(t, _MinMaxUnderlineDilatation.y));
            }

            if(_UseTextColor)
            {
                textComp.fontMaterial.SetColor(TextFaceColorPropertyID, _TextColor);
            }
        
        }
    }
}