using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimSquishy : TextMeshProAnimations
    {
        int DilatePropertyID = -1;

        [SerializeField]
        Vector2 _MinMaxDilatation = Vector2.up;

        [SerializeField]
        float _Speed = 1.0f;

        [SerializeField]
        bool _UseNormalizedTime = true;

        void Start()
        {
            DilatePropertyID = Shader.PropertyToID("_FaceDilate");
        }


        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            float t = _UseNormalizedTime ? _Speed * normalizedAnimProgress : Time.time * _Speed;
            textComp.fontMaterial.SetFloat(DilatePropertyID, _MinMaxDilatation.x + Mathf.PingPong(t, _MinMaxDilatation.y));
        }
    }
}