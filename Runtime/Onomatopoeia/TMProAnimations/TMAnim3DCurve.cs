using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMAnim3DCurve : TextMeshProAnimations
{
    [SerializeField] private BezierSurface BezierSurf;
    private TMP_Text TextComponent;

    [SerializeField] private AnimationCurve _LetterHeightDeformation;
    [SerializeField] private AnimationCurve _LetterWidthDeformation;
    [SerializeField, Range(-2.0f, 2.0f)] private float spacing = 0.0f;
    
    private bool hasTextChanged;

    private float CurveWidth = 0;
    private float LineWidth = 0;
    private float LineWidthProgress = 0;
    public override void AnimationStart(TMP_Text textComp, TMP_TextInfo textInfo, float normalizedAnimProgress)
    {
        LineWidth = textInfo.lineInfo[0].width;
        CurveWidth  = BezierSurf.TopCurve.ArcLength;
        LineWidthProgress = 0;
    }


    public override void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress,
        ref Vector3[] vertices)
    {
        // Skip characters that are not visible and thus have no geometry to manipulate.
        if (!charInfo.isVisible)
        {
            return;
        }
        
        //TODO: SPACES
            

            // Retrieve the pre-computed animation data for the given character.
            // VertexAnim vertAnim = vertexAnim[i];

            
        var lineHeight = textComp.textInfo.lineInfo[0].lineHeight;
        var charHeight = Vector3.Distance(charInfo.bottomLeft, charInfo.topLeft);
        var charWidth = Vector3.Distance(charInfo.bottomLeft, charInfo.bottomRight);

        // lineWidthProgress += charWidth / 2;

        var tx = BezierSurf.TopCurve.NormDistToT(LineWidthProgress / CurveWidth);
        

        // 0 = bl
        // 1 = ul
        // 2 = ur
        // 3 = br
        Vector3 p1 = BezierSurf.GetPoint(tx, 0);
        Vector3 p2 = BezierSurf.GetPoint(tx, 1);
        float curveHeight = Vector3.Distance(p1, p2);
        

        float halfSizeX = (charWidth / 2) / CurveWidth;
        float halfSizeY = (charHeight / 2) / curveHeight;


        // var weightTest = 0.15f * ((-4 * tx * tx + 4 * tx));
        var weigthHeight = _LetterHeightDeformation.Evaluate(tx);
        var weigthWidth = _LetterWidthDeformation.Evaluate(tx);
        
        var point1 = BezierSurf.GetPoint(tx, 0.5f + halfSizeY);
        var point2 = BezierSurf.GetPoint(tx, 0.5f - halfSizeY - weigthHeight);
        var point3 = BezierSurf.GetPoint(tx + halfSizeX + weigthWidth, 0.5f - halfSizeY - weigthHeight);
        var point4 = BezierSurf.GetPoint(tx + halfSizeX + weigthWidth, 0.5f + halfSizeY);
        
        
        LineWidthProgress += charWidth + spacing + weigthWidth;

        // Get the index of the first vertex used by this text element.
        int vertexIndex = charInfo.vertexIndex;
        




        // 0 = bl
        // 1 = ul
        // 2 = ur
        // 3 = br
        vertices[vertexIndex + 0] = point1;
        vertices[vertexIndex + 1] = point2;
        vertices[vertexIndex + 2] = point3;
        vertices[vertexIndex + 3] = point4;
    }
    
    
}
