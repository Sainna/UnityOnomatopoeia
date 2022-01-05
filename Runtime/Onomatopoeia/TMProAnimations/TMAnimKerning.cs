using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimKerning : TextMeshProAnimations
    {


        public float KerningScale = 0.01f;

        public AnimationCurve KerningCurve = null;

        float InitialKerning = float.NaN;

        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            if(float.IsNaN(InitialKerning))
                InitialKerning = textComp.characterSpacing;

            textComp.characterSpacing = InitialKerning + (KerningCurve.Evaluate(normalizedAnimProgress) * KerningScale);
        }
    }
}