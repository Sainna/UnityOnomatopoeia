using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimSpeedBasedLinearTranslate : TextMeshProAnimations
    {

        [SerializeField] private Vector3 _TranslateDir = Vector3.up;
        [SerializeField] private AnimationCurve _TranslateSpeedCurve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
        [SerializeField] private float _TranslateAmount = 5.0f;
        [SerializeField] private float _TranslateSpeed = 5.0f;

        private void Awake()
        {
            _TranslateDir = transform.up;
        }


        public void SetWorldTranslateDir(Vector3 newDir)
        {
            _TranslateDir = newDir.normalized;
        }
    
    
        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress,
            ref Vector3[] vertices)
        {
            var position = textComp.transform.position;
            var newPos = Vector3.MoveTowards(position, position + (_TranslateDir * _TranslateAmount),
                _TranslateSpeedCurve.Evaluate(normalizedAnimProgress) * _TranslateSpeed * Time.deltaTime);
            textComp.transform.position = newPos;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (_TranslateDir * _TranslateAmount));
        }
    }
}