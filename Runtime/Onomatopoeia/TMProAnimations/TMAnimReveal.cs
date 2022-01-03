using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMAnimReveal : TextMeshProAnimations
{
    //todo: doesn't work
    float[] Angles = null;
    int CurrentCharIndex = 0;
    int Direction = 1;

    int vertexIndex;
    Vector2 charMidTopline;
    Vector3 offset;

    float startTime = -0.1f;

    public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
    {
        if(Angles == null)
        {
            Angles = new float[textComp.textInfo.characterCount];
            CurrentCharIndex = 0;
        }

        if(charInfo.index != CurrentCharIndex || Time.time - startTime > 0.1f)
            return;


        if(Angles[CurrentCharIndex] == 0)
        {
            vertexIndex = charInfo.vertexIndex;
            charMidTopline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.topRight.y);
            // Vector2 charMidBasline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            offset = charMidTopline;
            // Vector3 offset = charMidBasline;
        }

        

        if (Angles[CurrentCharIndex] < 90)
        {
            vertices[vertexIndex + 0] += -offset;
            vertices[vertexIndex + 1] += -offset;
            vertices[vertexIndex + 2] += -offset;
            vertices[vertexIndex + 3] += -offset;

            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 15 * Direction, 0), Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);


            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;

            //m_TextComponent.mesh.vertices = vertices;
            //m_TextComponent.mesh.uv = m_TextComponent.textInfo.meshInfo[0].uvs0;
            //m_TextComponent.mesh.uv2 = m_TextComponent.textInfo.meshInfo[0].uvs2;
            //m_TextComponent.mesh.colors32 = m_TextComponent.textInfo.meshInfo[0].colors32;

            Angles[CurrentCharIndex] += 15;
            return;
        }
        else
        {
            CurrentCharIndex++;
        }

        if (CurrentCharIndex == textComp.textInfo.characterCount)
        {
            System.Array.Clear(Angles, 0, Angles.Length);
            Direction *= -1;
            CurrentCharIndex = 0;

            startTime = Time.time;
        }
    }
}
