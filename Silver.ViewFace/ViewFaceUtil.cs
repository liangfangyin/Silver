using Silver.ViewFace.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using ViewFaceCore;
using ViewFaceCore.Core;
using ViewFaceCore.Model;

namespace Silver.ViewFace
{
    public class ViewFaceUtil
    {
        /// <summary>
        /// 会员特征码
        /// </summary>
        private static List<FaceFeatureModel> listGloadFaceFeature = new List<FaceFeatureModel>();

        /// <summary>
        /// 初始化特征码
        /// </summary>
        /// <param name="faceFeatures"></param>
        public static void Initialization([NotNull]List<FaceFeatureModel> faceFeatures)
        {
            if (faceFeatures.Count <= 0)
            {
                return;
            }
            listGloadFaceFeature.AddRange(faceFeatures);
        }

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="faceType">人脸类型</param>
        /// <returns></returns>
        public static (bool, float[], string) Registration([NotNull]string imagePath, [NotNull] string memberId, [NotNull] string faceType)
        {
            using (var faceImage = SKBitmap.Decode(imagePath).ToFaceImage())
            {
                //检测人脸信息
                using (FaceDetector faceDetector = new FaceDetector())
                {
                    FaceInfo[] infos = faceDetector.Detect(faceImage);
                    if (infos.Length <= 0)
                    {
                        return (false, default, "人脸不存在");
                    }
                    using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
                    //标记人脸位置
                    using (FaceLandmarker faceMark = new FaceLandmarker())
                    {
                        FaceMarkPoint[] points = faceMark.Mark(faceImage, infos[0]);
                        if (points.Length <= 0)
                        {
                            return (false, default, "未找到人脸");
                        }
                        //活体检测
                        var resultLiving = faceAntiSpoofing.AntiSpoofing(faceImage, infos[0], points);
                        if (resultLiving.Status == AntiSpoofingStatus.Error)
                        {
                            return (false, default, "未找到人脸");
                        }
                        else if (resultLiving.Status == AntiSpoofingStatus.Spoof)
                        {
                            return (false, default, "非法人脸注册");
                        }
                        else if (resultLiving.Status == AntiSpoofingStatus.Fuzzy)
                        {
                            return (false, default, "人脸太模糊");
                        }
                        //提取特征值
                        using (FaceRecognizer faceRecognizer = new FaceRecognizer())
                        {
                            float[] data = faceRecognizer.Extract(faceImage, points);
                            var listFilterFaceFeature = listGloadFaceFeature.Where(t => t.MemberId == memberId && t.FaceType == faceType).ToList();
                            if (listFilterFaceFeature.Count() <= 0)
                            {
                                listGloadFaceFeature.Add(new FaceFeatureModel() { MemberId = memberId, FaceType = faceType, Feature = data });
                            }
                            else
                            {
                                listFilterFaceFeature[0].Feature = data;
                            }
                            return (true, data, "成功");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="faceType">人脸类型</param>
        /// <returns></returns>
        public static (bool, float[], string) Registration([NotNull] System.Drawing.Bitmap bitmap, [NotNull] string memberId, [NotNull] string faceType)
        {
            //检测人脸信息
            using (FaceDetector faceDetector = new FaceDetector())
            {
                FaceInfo[] infos = faceDetector.Detect(bitmap);
                if (infos.Length <= 0)
                {
                    return (false, default, "人脸不存在");
                }
                using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
                //标记人脸位置
                using (FaceLandmarker faceMark = new FaceLandmarker())
                {
                    FaceMarkPoint[] points = faceMark.Mark(bitmap, infos[0]);
                    if (points.Length <= 0)
                    {
                        return (false, default, "未找到人脸");
                    }
                    //活体检测
                    var resultLiving = faceAntiSpoofing.AntiSpoofing(bitmap, infos[0], points);
                    if (resultLiving.Status == AntiSpoofingStatus.Error)
                    {
                        return (false, default, "未找到人脸");
                    }
                    else if (resultLiving.Status == AntiSpoofingStatus.Spoof)
                    {
                        return (false, default, "非法人脸注册");
                    }
                    else if (resultLiving.Status == AntiSpoofingStatus.Fuzzy)
                    {
                        return (false, default, "人脸太模糊");
                    }
                    //提取特征值
                    using (FaceRecognizer faceRecognizer = new FaceRecognizer())
                    {
                        float[] data = faceRecognizer.Extract(bitmap, points);
                        var listFilterFaceFeature = listGloadFaceFeature.Where(t => t.MemberId == memberId && t.FaceType == faceType).ToList();
                        if (listFilterFaceFeature.Count() <= 0)
                        {
                            listGloadFaceFeature.Add(new FaceFeatureModel() { MemberId = memberId, FaceType = faceType, Feature = data });
                        }
                        else
                        {
                            listFilterFaceFeature[0].Feature = data;
                        }
                        return (true, data, "成功");
                    }
                }
            }
        }

        /// <summary>
        /// 获取人脸信息
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns></returns>
        public static FaceInfo[] GetFaceInfo([NotNull] string imagePath)
        {
            using var bitmap = SKBitmap.Decode(imagePath);
            using FaceDetector faceDetector = new FaceDetector();
            FaceInfo[] infos = faceDetector.Detect(bitmap);
            return infos;
        }

        /// <summary>
        /// 获取人脸信息
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static FaceInfo[] GetFaceInfo([NotNull] System.Drawing.Bitmap bitmap)
        {
            using (FaceDetector faceDetector = new FaceDetector())
            {
                FaceInfo[] infos = faceDetector.Detect(bitmap);
                return infos;
            }
        }

        /// <summary>
        /// 绘制人脸画框
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="outputPath">输出图片路径</param>
        public static void DrawFace([NotNull] string imagePath, [NotNull]string outputPath)
        {
            using var bitmap = (Bitmap)Image.FromFile(imagePath);
            using FaceDetector faceDetector = new FaceDetector();
            FaceInfo[] infos = faceDetector.Detect(bitmap);
            //画方框，标记人脸
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawRectangles(new Pen(Color.Red, 4), infos.Select(p => new RectangleF(p.Location.X, p.Location.Y, p.Location.Width, p.Location.Height)).ToArray());
            }
            bitmap.Save(outputPath);
        }

        /// <summary>
        /// 绘制人脸画框
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap DrawFace([NotNull] System.Drawing.Bitmap bitmap)
        {
            using FaceDetector faceDetector = new FaceDetector();
            FaceInfo[] infos = faceDetector.Detect(bitmap);
            //画方框，标记人脸
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawRectangles(new Pen(Color.Red, 4), infos.Select(p => new RectangleF(p.Location.X, p.Location.Y, p.Location.Width, p.Location.Height)).ToArray());
            }
            return bitmap;
        }

        /// <summary>
        /// 人脸对比
        /// </summary>
        /// <param name="imagePath">人脸路径</param>
        /// <param name="faceType">人脸类型</param>
        /// <returns></returns>
        public static (int,string ,string) CompareFace([NotNull] string imagePath, [NotNull] string faceType)
        {
            using var faceImage = SKBitmap.Decode(imagePath).ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
            using FaceLandmarker faceMark = new FaceLandmarker();
            //检测人脸信息
            FaceInfo[] infos = faceDetector.Detect(faceImage);
            if (infos.Length <= 0)
            {
                return (500, "", "人脸不存在");
            }
            //标记人脸位置
            FaceMarkPoint[] points = faceMark.Mark(faceImage, infos[0]);
            if (points.Length <= 0)
            {
                return (500, "", "未找到人脸");
            }
            //活体检测
            var resultLiving = faceAntiSpoofing.AntiSpoofing(faceImage, infos[0], points);
            if (resultLiving.Status == AntiSpoofingStatus.Error)
            {
                return (500, "", "未找到人脸");
            }
            else if (resultLiving.Status == AntiSpoofingStatus.Spoof)
            {
                return (500, "", "非法人脸注册");
            }
            else if (resultLiving.Status == AntiSpoofingStatus.Fuzzy)
            {
                return (500, "", "人脸太模糊");
            }
            //提取特征值
            using (FaceRecognizer faceRecognizer = new FaceRecognizer())
            {
                float[] data = faceRecognizer.Extract(faceImage, points);

                // 使用PLINQ进行并行搜索以找到最相似的特征向量  
                var bestMatch = listGloadFaceFeature.Where(t => t.FaceType.ToLower() == faceType.ToLower()).Select(t => t.Feature).AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount) // 设置并行度  
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism) // 强制并行执行  
                    .Select((feature, index) => new { Feature = feature, Index = index, Similarity = faceRecognizer.Compare(data, feature) })
                    .OrderByDescending(x => x.Similarity) // 根据相似度排序  
                    .FirstOrDefault(); // 获取最相似的特征向量（如果有的话）
                if (bestMatch == null)
                {
                    return (2, "", "未找到用户");
                }
                var keysWithValue = listGloadFaceFeature.Where(t => t.FaceType.ToLower() == faceType.ToLower() && t.Feature == bestMatch.Feature).FirstOrDefault();
                if (keysWithValue == null)
                {
                    return (2, "", "未找到用户");
                }
                return (1, keysWithValue.MemberId, "成功");
            }
        }

        /// <summary>
        /// 人脸对比
        /// </summary>
        /// <param name="faceImage"></param>
        /// <param name="faceType">人脸类型</param>
        /// <returns></returns>
        public static (int, string, string) CompareFace([NotNull] System.Drawing.Bitmap faceImage, [NotNull] string faceType)
        {
            //检测人脸信息
            using FaceDetector faceDetector = new FaceDetector();
            using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
            using FaceLandmarker faceMark = new FaceLandmarker();
            FaceInfo[] infos = faceDetector.Detect(faceImage);
            if (infos.Length <= 0)
            {
                return (500, "", "人脸不存在");
            }
            //标记人脸位置
            FaceMarkPoint[] points = faceMark.Mark(faceImage, infos[0]);
            if (points.Length <= 0)
            {
                return (500, "", "未找到人脸");
            }
            //活体检测
            var resultLiving = faceAntiSpoofing.AntiSpoofing(faceImage, infos[0], points);
            if (resultLiving.Status == AntiSpoofingStatus.Error)
            {
                return (500, "", "未找到人脸");
            }
            else if (resultLiving.Status == AntiSpoofingStatus.Spoof)
            {
                return (500, "", "非法人脸注册");
            }
            else if (resultLiving.Status == AntiSpoofingStatus.Fuzzy)
            {
                return (500, "", "人脸太模糊");
            }
            //提取特征值
            using FaceRecognizer faceRecognizer = new FaceRecognizer();
            float[] data = faceRecognizer.Extract(faceImage, points);
            // 使用PLINQ进行并行搜索以找到最相似的特征向量  
            var bestMatch = listGloadFaceFeature.Where(t => t.FaceType.ToLower() == faceType.ToLower()).Select(t => t.Feature).AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount) // 设置并行度  
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism) // 强制并行执行  
                .Select((feature, index) => new { Feature = feature, Index = index, Similarity = faceRecognizer.Compare(data, feature) })
                .OrderByDescending(x => x.Similarity) // 根据相似度排序  
                .FirstOrDefault(); // 获取最相似的特征向量（如果有的话）
            if (bestMatch == null)
            {
                return (2, "", "未找到用户");
            }
            var keysWithValue = listGloadFaceFeature.Where(t => t.FaceType.ToLower() == faceType.ToLower() && t.Feature == bestMatch.Feature).FirstOrDefault();
            if (keysWithValue == null)
            {
                return (2, "", "未找到用户");
            }
            return (1, keysWithValue.MemberId, "成功");
        }

        /// <summary>
        /// 获取人脸基本信息
        /// </summary>
        /// <param name="imagePath">人脸路径</param>
        /// <returns></returns>
        public static (bool, FaceBaseModel) GetFaceBase([NotNull] string imagePath)
        {
            FaceBaseModel infoFaceBaseModel = new FaceBaseModel();
            using var faceImage = SKBitmap.Decode(imagePath).ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
            using FaceLandmarker faceMark = new FaceLandmarker();
            using MaskDetector maskDetector = new MaskDetector();
            using AgePredictor agePredictor = new AgePredictor();
            using GenderPredictor genderPredictor = new GenderPredictor();
            using EyeStateDetector eyeStateDetector = new EyeStateDetector();
            //检测人脸信息
            FaceInfo[] infos = faceDetector.Detect(faceImage);
            if (infos.Length <= 0)
            {
                return (false, infoFaceBaseModel);
            }
            //标记人脸位置
            FaceMarkPoint[] points = faceMark.Mark(faceImage, infos[0]);
            if (points.Length <= 0)
            {
                return (false, infoFaceBaseModel);
            }
            //活体检测
            var resultLiving = faceAntiSpoofing.AntiSpoofing(faceImage, infos[0], points);
            //口罩检测
            var resultPlotMask= maskDetector.PlotMask(faceImage, infos[0]);
            //年龄检测
            var resultAge= agePredictor.PredictAge(faceImage, points);
            //性别检测
            var resultGender= genderPredictor.PredictGender(faceImage, points);
            //眼睛状态检测
            var resultEyeState= eyeStateDetector.Detect(faceImage, points);
            infoFaceBaseModel.SpoofingStatus = resultLiving.Status;
            infoFaceBaseModel.Masked = resultPlotMask.Masked;
            infoFaceBaseModel.Age = resultAge;
            infoFaceBaseModel.Gender= resultGender;
            infoFaceBaseModel.EyeState = resultEyeState;
            return (true, infoFaceBaseModel);
        }

        /// <summary>
        /// 获取人脸基本信息
        /// </summary>
        /// <param name="imagePath">人脸路径</param>
        /// <returns></returns>
        public static (bool, FaceBaseModel) GetFaceBase([NotNull] System.Drawing.Bitmap faceImage)
        {
            FaceBaseModel infoFaceBaseModel = new FaceBaseModel();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceAntiSpoofing faceAntiSpoofing = new FaceAntiSpoofing();
            using FaceLandmarker faceMark = new FaceLandmarker();
            using MaskDetector maskDetector = new MaskDetector();
            using AgePredictor agePredictor = new AgePredictor();
            using GenderPredictor genderPredictor = new GenderPredictor();
            using EyeStateDetector eyeStateDetector = new EyeStateDetector();
            //检测人脸信息
            FaceInfo[] infos = faceDetector.Detect(faceImage);
            if (infos.Length <= 0)
            {
                return (false, infoFaceBaseModel);
            }
            //标记人脸位置
            FaceMarkPoint[] points = faceMark.Mark(faceImage, infos[0]);
            if (points.Length <= 0)
            {
                return (false, infoFaceBaseModel);
            }
            //活体检测
            var resultLiving = faceAntiSpoofing.AntiSpoofing(faceImage, infos[0], points);
            //口罩检测
            var resultPlotMask = maskDetector.PlotMask(faceImage, infos[0]);
            //年龄检测
            var resultAge = agePredictor.PredictAge(faceImage, points);
            //性别检测
            var resultGender = genderPredictor.PredictGender(faceImage, points);
            //眼睛状态检测
            var resultEyeState = eyeStateDetector.Detect(faceImage, points);
            infoFaceBaseModel.SpoofingStatus = resultLiving.Status;
            infoFaceBaseModel.Masked = resultPlotMask.Masked;
            infoFaceBaseModel.Age = resultAge;
            infoFaceBaseModel.Gender = resultGender;
            infoFaceBaseModel.EyeState = resultEyeState;
            return (true, infoFaceBaseModel);
        }

    }
}
