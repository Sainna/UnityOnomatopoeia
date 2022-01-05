using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimShaderVertexOffset : TextMeshProAnimations
    {
    
        [SerializeField] private Vector3 _TranslateDir = Vector3.up;
        [SerializeField] private AnimationCurve _TranslateSpeedCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
        [SerializeField] private float _TranslateAmount = 5.0f;
        [SerializeField] private float _TranslateSpeed = 5.0f;

        private static readonly int VertexOffsetID =  Shader.PropertyToID("_VertexOffsetVec");

        public void SetWorldTranslateDir(Vector3 newDir)
        {
            _TranslateDir = newDir.normalized;
        }
    
        public override void AnimationSetup(TMP_Text textComp)
        {
            textComp.fontMaterial.SetVector(VertexOffsetID, Vector4.zero);
        }
    
        public override void AnimationStart(TMP_Text textComp, TMP_TextInfo textInfo, float normalizedAnimProgress)
        {
            Vector3 position = textComp.fontMaterial.GetVector(VertexOffsetID);
            var newPos = Vector3.MoveTowards(position, position + (_TranslateDir * _TranslateAmount),
                _TranslateSpeedCurve.Evaluate(normalizedAnimProgress) * _TranslateSpeed * Time.deltaTime);
            textComp.fontMaterial.SetVector(VertexOffsetID, newPos);

        }
    
        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress,
            ref Vector3[] vertices)
        {
        }

        public override void AnimationEnd(TMP_Text textComp, TMP_TextInfo textInfo, float normalizedAnimProgress)
        {
        
        }
    }
}