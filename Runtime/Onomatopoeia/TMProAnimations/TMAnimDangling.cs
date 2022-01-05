using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public class TMAnimDangling : TextMeshProAnimations
    {

        [SerializeField, Tooltip("Angle range")]
        Vector2Int _AngleRange = new Vector2Int(10, 25);

        [SerializeField, Tooltip("Speed range")]
        Vector2 _SpeedRange = new Vector2(1.5f, 3.0f);

        float[] AngleRanges = new float[20];
        float[] Speed = new float[20];
        public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
        {
            // if(Time.time - StartTime < _Refresh)
            //     return;

            int charIndexMod = charInfo.index%20;

            // Setup initial random values
            if(AngleRanges[charIndexMod] == 0 && Speed[charIndexMod] == 0)
            {
                AngleRanges[charIndexMod] = Random.Range(_AngleRange.x, _AngleRange.y);
                Speed[charIndexMod] = Random.Range(_SpeedRange.x, _SpeedRange.y);
            }

            int vertexIndex = charInfo.vertexIndex;

            Vector2 charMidTopline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.topRight.y);
            // Vector2 charMidBasline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
            Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            Vector3 offset = charMidTopline;
            // Vector3 offset = charMidBasline;

            vertices[vertexIndex + 0] += -offset;
            vertices[vertexIndex + 1] += -offset;
            vertices[vertexIndex + 2] += -offset;
            vertices[vertexIndex + 3] += -offset;

            float angle = Mathf.SmoothStep(-AngleRanges[charIndexMod], AngleRanges[charIndexMod], Mathf.PingPong(Time.time * Speed[charIndexMod], 1f));
            //Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);
            //matrix = Matrix4x4.TRS(jitterOffset, Quaternion.identity, Vector3.one);
            //matrix = Matrix4x4.TRS(jitterOffset, Quaternion.Euler(0, 0, Random.Range(-5f, 5f)), Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);


            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;

            // if(charInfo.index == textComp.textInfo.characterCount-1)
            //     StartTime = Time.time;
        }
    }
}