using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimStretch : TextMeshProAnimations
    {


        public float StretchScale = 0.01f;

        public AnimationCurve StretchCurve = null;

        [SerializeField]
        float initScale = 1.0f;

        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            // // Setup initial random values
            // int vertexIndex = charInfo.vertexIndex;

            // Vector2 charMidBasline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);

            // // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            // //Vector3 offset = charMidTopline;
            // Vector3 offset = charMidBasline;

            // vertices[vertexIndex + 0] += -offset;
            // vertices[vertexIndex + 1] += -offset;
            // vertices[vertexIndex + 2] += -offset;
            // vertices[vertexIndex + 3] += -offset;

            // Vector3 stretchOffset = new Vector3(1.0f * normalizedAnimProgress, 0, 0);
        
            // matrix = Matrix4x4.TRS(stretchOffset * StretchScale, Quaternion.identity, Vector3.one);
            // Matrix4x4 matrixMin = Matrix4x4.TRS(-stretchOffset * StretchScale, Quaternion.identity, Vector3.one);

        

            // vertices[vertexIndex + 0] = matrixMin.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            // vertices[vertexIndex + 1] = matrixMin.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            // vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            // vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);


            // vertices[vertexIndex + 0] += offset;
            // vertices[vertexIndex + 1] += offset;
            // vertices[vertexIndex + 2] += offset;
            // vertices[vertexIndex + 3] += offset;
        
        
        
            Vector3 scale = textComp.transform.localScale;
            scale.x = initScale + (StretchCurve.Evaluate(normalizedAnimProgress) * StretchScale);
            textComp.transform.localScale = scale;
        }
    }
}