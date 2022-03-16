using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;

namespace ImageRecognitionService
{
    public static class ImageService
    {
        public class Result
        {
            public int success { get; set; }
            public List<string> Lables { get; set; }
        }

        public static Result RecognizeImage(string imageFolder, string fileName, string modelPath)
        {
            Result result = new Result();
            try
            {
                using var image = Image.FromFile(imageFolder + "/input/" + fileName);

                using var scorer = new YoloScorer<YoloCocoP5Model>(modelPath);

                List<YoloPrediction> predictions = scorer.Predict(image);
                if (predictions.Count == 0)
                {
                    result.success = 100;
                    return result;
                }
                else
                {
                    List<string> loLables = new List<string>();
                    foreach (YoloPrediction label in predictions)
                    {
                        loLables.Add(label.Label.Name);
                    }
                    result.Lables = loLables;
                    result.success = 101;
                   
                }
                using var graphics = Graphics.FromImage(image);

                foreach (var prediction in predictions) // iterate predictions to draw results
                {
                    double score = Math.Round(prediction.Score, 2);

                    graphics.DrawRectangles(new Pen(prediction.Label.Color, 1),
                        new[] { prediction.Rectangle });

                    var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

                    graphics.DrawString($"{prediction.Label.Name} ({score})",
                        new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
                        new PointF(x, y));
                }
                if (!Directory.Exists(imageFolder + "/output"))
                    Directory.CreateDirectory(imageFolder + "/output");
                image.Save(imageFolder + "/output/" + fileName);
                result.success = 101;
                return result;
            }
            catch (Exception ex)
            {
                result.success = 0;
                return result;
            }
        }
    }
}
