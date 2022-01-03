using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMAnimJitter : TextMeshProAnimations
{

    public float AngleMultiplier = 0.01f;
    public float CurveScale = 0.01f;

    public float InitOffsetRange = 0.25f;

    public float PerlinSpeed = 1.5f;

    public bool UserPerlinNoise = true;


    public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
    {
        // Setup initial random values
        int vertexIndex = charInfo.vertexIndex;

        Vector2 charMidBasline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);

        // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
        //Vector3 offset = charMidTopline;
        Vector3 offset = charMidBasline;

        vertices[vertexIndex + 0] += -offset;
        vertices[vertexIndex + 1] += -offset;
        vertices[vertexIndex + 2] += -offset;
        vertices[vertexIndex + 3] += -offset;

        Vector3 jitterOffset;
        
        if(UserPerlinNoise)
        {
            jitterOffset = new Vector3((Mathf.PerlinNoise(PerlinSpeed * normalizedAnimProgress, vertexIndex) * (InitOffsetRange*2)) - InitOffsetRange,
                                        (Mathf.PerlinNoise(vertexIndex, PerlinSpeed * normalizedAnimProgress) * (InitOffsetRange*2)) - InitOffsetRange, 0);
            float angleOffset = (Mathf.PerlinNoise(PerlinSpeed * normalizedAnimProgress, vertexIndex) - 0.5f) * 2 * AngleMultiplier;
            matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, angleOffset), Vector3.one);
        }
        else
        {
            jitterOffset = new Vector3(Random.Range(-InitOffsetRange, InitOffsetRange), Random.Range(-InitOffsetRange, InitOffsetRange), 0);
            matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);
        }

        //matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, vertexAnim[i].angle), Vector3.one);
        //matrix = Matrix4x4.TRS(jitterOffset, Quaternion.identity, Vector3.one);
        

        vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
        vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
        vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
        vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);


        vertices[vertexIndex + 0] += offset;
        vertices[vertexIndex + 1] += offset;
        vertices[vertexIndex + 2] += offset;
        vertices[vertexIndex + 3] += offset;
    }
}
