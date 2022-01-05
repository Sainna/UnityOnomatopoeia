using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimRotationShake : TextMeshProAnimations
    {

        [SerializeField]
        float _Angle = 25.0f;

        [SerializeField]
        float _Speed = 1.0f;

        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            textComp.rectTransform.localRotation *= Quaternion.RotateTowards(Quaternion.identity, Quaternion.AngleAxis(_Angle, Vector3.forward), _Speed * Time.deltaTime);

            if(Mathf.Approximately(textComp.rectTransform.localRotation.eulerAngles.z, _Angle))
            {
                _Angle = -_Angle;
            }
        }
    }
}