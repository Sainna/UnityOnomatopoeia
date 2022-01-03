using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMAnimCircleCurve : TextMeshProAnimations
{
    [SerializeField, Tooltip("The radius of the text circle arc")]
    float _Radius = 1.0f;

    [SerializeField, Tooltip("Bring the text down when you want a bigger curve")]
    float _RadiusCompensation = 1.0f;

    [SerializeField, Tooltip("Circle radius animation curve")]
    AnimationCurve _RadiusCurve = AnimationCurve.Linear(0,0,1,1);


    [SerializeField, Tooltip("How much degrees the text arc should span")]
    float _ArcDegrees = 120.0f;
    [SerializeField, Tooltip("Arc degrees animation curve")]
    AnimationCurve _ArcDegreesCurve = AnimationCurve.Constant(0,1,1);

    [SerializeField, Tooltip("The angular offset at which the arc should be centered, in degrees")]
    float _AngularOffset = -90;
    [SerializeField, Tooltip("Angular offset movement speed")]
    float _AngularOffsetSpeed = 0.0f;

    public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
    {
        float boundsMinX = textComp.bounds.min.x;
        float boundsMaxX = textComp.bounds.max.x;
        int vertexIndex = charInfo.vertexIndex;
        Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
        vertices[vertexIndex + 0] += -charMidBaselinePos;
        vertices[vertexIndex + 1] += -charMidBaselinePos;
        vertices[vertexIndex + 2] += -charMidBaselinePos;
        vertices[vertexIndex + 3] += -charMidBaselinePos;

        //compute the horizontal position of the character relative to the bounds of the box, in a range [0, 1]
        //where 0 is the left border of the text and 1 is the right border
        float zeroToOnePos = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX);

        //get the transformation matrix, that maps the vertices, seen as offset from the central character point, to their final
        //position that follows the curve
        
        //compute the angle at which to show this character.
        //We want the string to be centered at the top point of the circle, so we first convert the position from a range [0, 1]
        //to a [-0.5, 0.5] one and then add m_angularOffset degrees, to make it centered on the desired point
        float angle = ((zeroToOnePos - 0.5f) * (_ArcDegrees * _ArcDegreesCurve.Evaluate(normalizedAnimProgress)) 
                                            + (_AngularOffset + (_AngularOffsetSpeed * Time.deltaTime))) * Mathf.Deg2Rad; //we need radians for sin and cos

        //compute the coordinates of the new position of the central point of the character. Use sin and cos since we are on a circle.
        //Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
        float x0 = Mathf.Cos(angle);            
        float y0 = Mathf.Sin(angle);
        float radiusForThisLine = (_Radius * _RadiusCurve.Evaluate(normalizedAnimProgress)) - textComp.textInfo.lineInfo[0].lineExtents.max.y * textComp.textInfo.characterInfo[charInfo.index].lineNumber;
        Vector2 newMideBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine); //actual new position of the character
        newMideBaselinePos *= _RadiusCompensation;

        //compute the trasformation matrix: move the points to the just found position, then rotate the character to fit the angle of the curve 
        //(-90 is because the text is already vertical, it is as if it were already rotated 90 degrees)
        matrix = Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0), Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90, Vector3.forward), Vector3.one);

        //apply the transformation, and obtain the final position and orientation of the 4 vertices representing this char
        vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
        vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
        vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
        vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
    }
}
