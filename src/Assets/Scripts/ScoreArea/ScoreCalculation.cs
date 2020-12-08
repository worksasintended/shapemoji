﻿using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace ScoreArea
{
    /**
     * used to calculate score after login emoji
     */
    public class ScoreCalculation : MonoBehaviour
    {
        /**
         * Analyze scorable view, create virtual screenshot, read pixels, analyse pixels, read score, trigger ScoreArea.ChangeScore() in a coroutine
         *
         * @param scoreArea ScoreArea to analyze
         * @param renderer Renderer of score area
         * @param cam Main Camera
         */
        public IEnumerator AnalyzeScoreableView(ScoreArea scoreArea, Renderer renderer, Camera cam)
        {
            var result = new AnalyzeScoreAreaResult();
            var bounds = renderer.bounds;
            // get dimensions of Score Area
            var size = bounds.size;
            // get position and transform to screen point
            var position = cam.WorldToScreenPoint(bounds.min);
            // create texture to store "screenshot" in
            var img = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGB24, false);
            // create rectengular at score area
            var rect = new Rect((Vector2)position, (Vector2)size);
            // wait for frame ot be rendered
            yield return new WaitForEndOfFrame();
            // create the image
            img.ReadPixels(rect, 0, 0);
            img.Apply();
            var pixels = img.GetPixels();
            //analyze pixels
            result = AnalyzePixelMap(pixels);
            //calculate score
            
            //TODO push result into ScoreArea

            yield return null;
        }

        
        /**
         * analyzes pixel map for colors, identifying emojiCovered, emojiUncovered, scoreAreaCovered
         *
         * @param pixels 1D pixel Map to analyze
         *
         * @return results of analysis in AnalyzeScoreAreaResult
         */
        private static AnalyzeScoreAreaResult AnalyzePixelMap(Color[] pixels)
        {
            var result = new AnalyzeScoreAreaResult();
            int emojiCovered = 0;
            int scoreAreaCovered = 0;
            int emojiUncovered = 0;
            foreach (var pixel in pixels)
            {
                if (pixel.r > 0 && pixel.g > 0)
                {
                    scoreAreaCovered++;
                }
                else if (pixel.b > 0 && pixel.g > 0)
                {
                    emojiCovered++;
                }
                else if ( pixel.b > 0)
                {
                    emojiUncovered++;
                }
            }
            result.EmojiCovered = emojiCovered;
            result.BackgroundCovered = scoreAreaCovered;
            result.EmojiUncovered = emojiUncovered;
            return result;
        }
    }
}
