using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMAnimShakeB : TextMeshProAnimations
{

    [Header("Shake")]
    public float ScaleMultiplier = 1.0f;
    public float RotationMultiplier = 1.0f;

    Vector3 centerOfLine;
    Quaternion rotation;

    //todo: multiline?
    //todo: finish
    public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
    {
        TMP_TextInfo textInfo = textComp.textInfo;

        if(charInfo.index == 0)
        {
            centerOfLine = (textInfo.characterInfo[textInfo.lineInfo[0].firstCharacterIndex].bottomLeft +
                                textInfo.characterInfo[textInfo.lineInfo[0].lastCharacterIndex].topRight) / 2.0f;
            rotation = Quaternion.Euler(0, 0, Random.Range(-RotationMultiplier, RotationMultiplier));
        }

        // Get the index of the first vertex used by this text element.
        int vertexIndex = charInfo.vertexIndex;

        // Determine the center point of each character at the baseline.
        Vector3 charCenter = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2.0f;

        // Need to translate all 4 vertices of each quad to aligned with center of character.
        // This is needed so the matrix TRS is applied at the origin for each character.
        vertices[vertexIndex + 0] = vertices[vertexIndex + 0] - charCenter;
        vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - charCenter;
        vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - charCenter;
        vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - charCenter;

        // Determine the random scale change for each character.
        float randomScale = Random.Range(1f - ScaleMultiplier, 1 + ScaleMultiplier);

        // Setup the matrix for the scale change.
        var matrix = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * randomScale);

        // Apply the scale change relative to the center of each character.
        vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
        vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
        vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
        vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

        // Revert the translation change.
        vertices[vertexIndex + 0] += charCenter;
        vertices[vertexIndex + 1] += charCenter;
        vertices[vertexIndex + 2] += charCenter;
        vertices[vertexIndex + 3] += charCenter;

        // Need to translate all 4 vertices of each quad to aligned with the center of the line.
        // This is needed so the matrix TRS is applied from the center of the line.
        vertices[vertexIndex + 0] -= centerOfLine;
        vertices[vertexIndex + 1] -= centerOfLine;
        vertices[vertexIndex + 2] -= centerOfLine;
        vertices[vertexIndex + 3] -= centerOfLine;

        // Setup the matrix rotation.
        matrix = Matrix4x4.TRS(Vector3.one, rotation, Vector3.one);

        // Apply the matrix TRS to the individual characters relative to the center of the current line.
        vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
        vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
        vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
        vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

        // Revert the translation change.
        vertices[vertexIndex + 0] += centerOfLine;
        vertices[vertexIndex + 1] += centerOfLine;
        vertices[vertexIndex + 2] += centerOfLine;
        vertices[vertexIndex + 3] += centerOfLine;
    }
}
