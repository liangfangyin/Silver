using Silver.ArcSoft.Model;
using Silver.ArcSoft.Utils;
using Silver.Basic;
using System.Drawing;

namespace Silver.ArcSoft
{
    /// <summary>
    /// 虹软人脸
    /// </summary>
    public class ArcSoftFace : IArcSoftFace
    {

        #region 基础参数

        /// <summary>
        /// 人脸列表
        /// </summary>
        public static Dictionary<string, FaceMember> Faces = new Dictionary<string, FaceMember>();

        /// <summary>
        /// 引擎Handle
        /// </summary>
        private IntPtr pImageEngine = IntPtr.Zero;

        /// <summary>
        /// 应用APPID
        /// </summary>
        private string SdkAppID = "";

        /// <summary>
        /// 应用密钥
        /// </summary>
        private string SdkAppKey = "";

        /// <summary>
        /// 终端唯一识别号
        /// </summary>
        private string IdentityNo = "";

        /// <summary>
        /// 最小识别度
        /// </summary>
        private double MinRecognition = 0.8;

        #endregion

        #region 构造函数

        public ArcSoftFace()
        {
            SdkAppID = ConfigurationUtil.GetSection("ArcSoft:SdkAppID");
            SdkAppKey = ConfigurationUtil.GetSection("ArcSoft:SdkAppKey");
        }


        public ArcSoftFace(string AppID, string AppKey)
        {
            SdkAppID = AppID;
            SdkAppKey = AppKey;
        }

        #endregion


        /// <summary>
        /// 增加人脸
        /// </summary>
        /// <param name="faceId"></param>
        /// <param name="softFace"></param>
        public void AddFace(FaceMember softFace)
        {
            string faceKey = $"{softFace.Mchid}:{softFace.FaceId}";
            if (!string.IsNullOrEmpty(faceKey))
            {
                Faces[faceKey] = softFace;
            }
        }

        /// <summary>
        /// 批量添加人脸
        /// </summary>
        /// <param name="face"></param>
        public void AddFaces(List<FaceMember> face)
        {
            if (face.Count <= 0)
            {
                return;
            }
            Task.Factory.StartNew(() =>
            {
                foreach (var item in face)
                {
                    string faceKey = $"{item.Mchid}:{item.FaceId}";
                    Faces[faceKey] = item;
                }
            });
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Activation()
        {
            IdentityNo = EncryptUtil.ToMD5_32($"{SdkAppID}{SdkAppKey}");
            string offOnlinePath = Directory.GetCurrentDirectory() + "/offlinefile/";
            if (!Directory.Exists(offOnlinePath))
            {
                Directory.CreateDirectory(offOnlinePath);
            }
            if (File.Exists(offOnlinePath + IdentityNo + ".txt"))
            {
                return true;
            }
            try
            {
                int retCode = ASFFunctions.ASFActivation(SdkAppID, SdkAppKey);
                if (retCode == 0)
                {
                    File.WriteAllText(offOnlinePath + IdentityNo + ".txt", (new { appid = SdkAppID, appkey = SdkAppKey, actiondate = DateTime.Now }).ToJson());
                    return true;
                }
                throw new Exception("激活引擎失败，错误码：" + retCode);
            }
            catch (Exception ex)
            {
                //禁用相关功能按钮
                if (ex.Message.Contains("无法加载 DLL"))
                {
                    throw new Exception("请将sdk相关DLL放入bin对应的x86或x64下的文件夹中!");
                }
                else
                {
                    throw new Exception("激活引擎失败!");
                }
            }
            return false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitEngines()
        {
            int retCode = 0;
            //初始化引擎
            uint detectMode = DetectionMode.ASF_DETECT_MODE_IMAGE;//Image模式下检测脸部的角度优先值
            int imageDetectFaceOrientPriority = ASF_OrientPriority.ASF_OP_0_ONLY;
            //人脸在图片中所占比例，如果需要调整检测人脸尺寸请修改此值，有效数值为2-32
            int detectFaceScaleVal = 16;
            //最大需要检测的人脸个数
            int detectFaceMaxNum = 5;
            //引擎初始化时需要初始化的检测功能组合
            int combinedMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_AGE | FaceEngineMask.ASF_GENDER | FaceEngineMask.ASF_FACE3DANGLE;
            //初始化引擎，正常值为0，其他返回值请参考http://ai.arcsoft.com.cn/bbs/forum.php?mod=viewthread&tid=19&_dsign=dbad527e
            retCode = ASFFunctions.ASFInitEngine(detectMode, imageDetectFaceOrientPriority, detectFaceScaleVal, detectFaceMaxNum, combinedMask, ref pImageEngine);
            //Console.WriteLine("InitEngine Result:" + retCode);
            if (retCode == 0)
            {
                Writelogs("引擎初始化成功!", "ArcSoftFace.InitEngines");
            }
            else
            {
                throw new Exception($"引擎初始化失败!错误码为:{retCode}!");
            }
        }

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="image"></param>
        /// <returns>bool:是否成功，string：成功特征码，识别原因</returns>
        public (bool, string) Register(Image image)
        {
            if (image.Width > 1536 || image.Height > 1536)
            {
                image = Utils.ImageUtil.ScaleImage(image, 1536, 1536);
            }
            if (image.Width % 4 != 0)
            {
                image = Utils.ImageUtil.ScaleImage(image, image.Width - (image.Width % 4), image.Height);
            }
            //人脸检测 
            ASF_MultiFaceInfo multiFaceInfo = FaceUtil.DetectFace(pImageEngine, image);
            //判断检测结果
            if (multiFaceInfo.faceNum <= 0)
            {
                image.Dispose();
                return (false, "检测到人脸为0");
            }
            ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
            MRECT rect = MemoryUtil.PtrToStructure<MRECT>(multiFaceInfo.faceRects);
            image = Utils.ImageUtil.CutImage(image, rect.left, rect.top, rect.right, rect.bottom);
            IntPtr feature = FaceUtil.ExtractFeature(pImageEngine, image, out singleFaceInfo);
            if (singleFaceInfo.faceRect.left == 0 && singleFaceInfo.faceRect.right == 0)
            {
                //释放指针
                MemoryUtil.Free(feature);
                image.Dispose();
                return (false, "未检测到人脸");
            }
            //人脸特征feature过滤
            ASF_FaceFeature faceFeature = MemoryUtil.PtrToStructure<ASF_FaceFeature>(feature);
            byte[] feature_b = new byte[faceFeature.featureSize];
            MemoryUtil.Copy(faceFeature.feature, feature_b, 0, faceFeature.featureSize);
            //释放指针
            MemoryUtil.Free(feature);
            image.Dispose();
            return (true, ShapeUtil.ByteToHex(feature_b).Replace(" ", ""));
        }

        /// <summary>
        /// 人脸识别-返回人员信息
        /// </summary>
        /// <param name="srcImage"></param>
        /// <returns>bool：是否成功  int：识别成功索引   float：相似度  </returns>
        public (bool, FaceMember, float) Distinguish(Image srcImage)
        {
            if (srcImage.Width > 1536 || srcImage.Height > 1536)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, 1536, 1536);
            }
            //调整图像宽度，需要宽度为4的倍数
            if (srcImage.Width % 4 != 0)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, srcImage.Width - (srcImage.Width % 4), srcImage.Height);
            }
            //调整图片数据，非常重要
            ImageInfo imageInfo = Utils.ImageUtil.ReadBMP(srcImage);
            //人脸检测
            ASF_MultiFaceInfo multiFaceInfo = FaceUtil.DetectFace(pImageEngine, imageInfo);
            if (multiFaceInfo.faceNum < 1)
            {
                MemoryUtil.Free(imageInfo.imgData);
                srcImage.Dispose();
                return (false, new FaceMember(), 0);
            }
            MRECT temp = new MRECT();
            int rectTemp = 0;
            //标记出检测到的人脸
            for (int i = 0; i < multiFaceInfo.faceNum; i++)
            {
                MRECT rect = MemoryUtil.PtrToStructure<MRECT>(multiFaceInfo.faceRects + MemoryUtil.SizeOf<MRECT>() * i);
                int orient = MemoryUtil.PtrToStructure<int>(multiFaceInfo.faceOrients + MemoryUtil.SizeOf<int>() * i);
                int rectWidth = rect.right - rect.left;
                int rectHeight = rect.bottom - rect.top;
                //查找最大人脸
                if (rectWidth * rectHeight > rectTemp)
                {
                    rectTemp = rectWidth * rectHeight;
                    temp = rect;
                }
            }
            float compareSimilarity = 0f;
            FaceMember infoMember = new FaceMember();
            ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
            //提取人脸特征
            IntPtr image1Feature = FaceUtil.ExtractFeature(pImageEngine, srcImage, out singleFaceInfo);
            foreach (string key in Faces.Keys)
            {
                IntPtr userFeature = ShapeUtil.byteToIntPtr(Faces[key].Feature);
                float similarity = 0f;
                int ret = ASFFunctions.ASFFaceFeatureCompare(pImageEngine, image1Feature, userFeature, ref similarity);
                //增加异常值处理
                if (similarity.ToString().IndexOf("E") > -1)
                {
                    similarity = 0f;
                }
                if (similarity > compareSimilarity && similarity >= MinRecognition)
                {
                    compareSimilarity = similarity;
                    infoMember = Faces[key];
                }
                MemoryUtil.Free(userFeature);
                if (compareSimilarity > 0.99)
                {
                    break;
                }
            }
            MemoryUtil.Free(imageInfo.imgData);
            MemoryUtil.Free(image1Feature);
            srcImage.Dispose();
            return (true, infoMember, compareSimilarity);
        }
        
        /// <summary>
        /// 人脸识别-返回人员信息
        /// </summary>
        /// <param name="Feature">特征码</param> 
        /// <returns>bool：是否成功  int：识别成功索引   float：相似度  </returns>
        public (bool, FaceMember, float) Distinguish(string Feature)
        {
            float compareSimilarity = 0f;
            FaceMember infoMember = new FaceMember();
            //提取人脸特征
            IntPtr image1Feature = ShapeUtil.byteToIntPtr(Feature);
            foreach (string key in Faces.Keys)
            {
                IntPtr userFeature = ShapeUtil.byteToIntPtr(Faces[key].Feature);
                float similarity = 0f;
                int ret = ASFFunctions.ASFFaceFeatureCompare(pImageEngine, image1Feature, userFeature, ref similarity);
                //增加异常值处理
                if (similarity.ToString().IndexOf("E") > -1)
                {
                    similarity = 0f;
                }
                if (similarity > compareSimilarity && similarity >= MinRecognition)
                {
                    compareSimilarity = similarity;
                    infoMember = Faces[key];
                }
                MemoryUtil.Free(userFeature);
                if (compareSimilarity > 0.99)
                {
                    break;
                }
            }
            MemoryUtil.Free(image1Feature);
            return (true, infoMember, compareSimilarity);
        }

        /// <summary>
        /// 人脸识别-返回特征
        /// </summary>
        /// <param name="srcImage"></param> 
        /// <returns>  </returns>
        public (bool, FaceDistinguish, string) DistinguishFeatures(Image srcImage)
        {
            FaceDistinguish infoDistinguish = new FaceDistinguish();
            infoDistinguish.Algorithm = 1;
            DateTime beginDate = DateTime.Now;
            if (srcImage.Width > 1536 || srcImage.Height > 1536)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, 1536, 1536);
            }
            //调整图像宽度，需要宽度为4的倍数
            if (srcImage.Width % 4 != 0)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, srcImage.Width - (srcImage.Width % 4), srcImage.Height);
            }
            //调整图片数据，非常重要
            ImageInfo imageInfo = Utils.ImageUtil.ReadBMP(srcImage);
            //人脸检测
            ASF_MultiFaceInfo multiFaceInfo = FaceUtil.DetectFace(pImageEngine, imageInfo);
            if (multiFaceInfo.faceNum < 1)
            {
                MemoryUtil.Free(imageInfo.imgData);
                srcImage.Dispose();
                infoDistinguish.Errmsg = "100";
                infoDistinguish.Errcode = "非人物图片";
                return (false, infoDistinguish, "非人物图片");
            }
            infoDistinguish.Face = multiFaceInfo.faceNum;
            if (multiFaceInfo.faceNum > 1)
            {
                MemoryUtil.Free(imageInfo.imgData);
                srcImage.Dispose();
                infoDistinguish.Errmsg = "101";
                infoDistinguish.Errcode = "存在多个人脸";
                return (false, infoDistinguish, "存在多个人脸");
            }
            //MRECT temp = new MRECT();
            //int rectTemp = 0;
            //年龄检测
            int retCode_Age = -1;
            ASF_AgeInfo ageInfo = FaceUtil.AgeEstimation(pImageEngine, imageInfo, multiFaceInfo, out retCode_Age);
            //性别检测
            int retCode_Gender = -1;
            ASF_GenderInfo genderInfo = FaceUtil.GenderEstimation(pImageEngine, imageInfo, multiFaceInfo, out retCode_Gender);
            int retCode_Liveness = -1;
            //RGB活体检测
            ASF_LivenessInfo liveInfo = FaceUtil.LivenessInfo_RGB(pImageEngine, imageInfo, multiFaceInfo, out retCode_Liveness);
            infoDistinguish.Isliving = 0;
            //判断检测结果
            if (retCode_Liveness == 0 && liveInfo.num > 0)
            {
                infoDistinguish.Isliving = MemoryUtil.PtrToStructure<int>(liveInfo.isLive);
            }
            //3DAngle检测
            int retCode_3DAngle = -1;
            ASF_Face3DAngle face3DAngleInfo = FaceUtil.Face3DAngleDetection(pImageEngine, imageInfo, multiFaceInfo, out retCode_3DAngle);
            //标记出检测到的人脸
            for (int i = 0; i < multiFaceInfo.faceNum; i++)
            {
                infoDistinguish.Age = MemoryUtil.PtrToStructure<int>(ageInfo.ageArray + MemoryUtil.SizeOf<int>() * i);
                infoDistinguish.Sex = MemoryUtil.PtrToStructure<int>(genderInfo.genderArray + MemoryUtil.SizeOf<int>() * i);
                if (infoDistinguish.Sex == 1)
                {
                    infoDistinguish.Sex = 0;
                }
                else
                {
                    infoDistinguish.Sex = 1;
                }
                infoDistinguish.Orient = Convert.ToInt32(MemoryUtil.PtrToStructure<float>(face3DAngleInfo.roll + MemoryUtil.SizeOf<float>() * i));
            }
            FaceMember infoMember = new FaceMember();
            float compareSimilarity = 0f;
            ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
            //提取人脸特征
            IntPtr image1Feature = FaceUtil.ExtractFeature(pImageEngine, srcImage, out singleFaceInfo);
            foreach (string key in Faces.Keys)
            {
                IntPtr userFeature = ShapeUtil.byteToIntPtr(Faces[key].Feature);
                float similarity = 0f;
                int ret = ASFFunctions.ASFFaceFeatureCompare(pImageEngine, image1Feature, userFeature, ref similarity);
                //增加异常值处理
                if (similarity.ToString().IndexOf("E") > -1)
                {
                    similarity = 0f;
                }
                if (similarity > compareSimilarity && similarity >= MinRecognition)
                {
                    compareSimilarity = similarity;
                    infoMember = Faces[key];
                } 
                MemoryUtil.Free(userFeature);
                if (compareSimilarity > 0.99)
                {
                    break;
                }
            }
            DateTime endDate = DateTime.Now;
            infoDistinguish.Imageurl = infoMember.Headurl;
            infoDistinguish.Mchid = infoMember.Mchid;
            infoDistinguish.Memberid = infoMember.FaceId.ToLong();
            infoDistinguish.Name = infoMember.Name;
            infoDistinguish.Timeout = Convert.ToInt32((endDate - beginDate).TotalMilliseconds);
            infoDistinguish.Rate = Convert.ToDecimal(compareSimilarity * 100);
            infoDistinguish.Errmsg = "0";
            infoDistinguish.Errcode = "成功";
            MemoryUtil.Free(image1Feature);
            MemoryUtil.Free(imageInfo.imgData);
            srcImage.Dispose();
            return (true, infoDistinguish, "成功");
        }


        /// <summary>
        /// 人脸对比相似度
        /// </summary>
        /// <param name="image1">人脸图片1</param>
        /// <param name="image2">人脸图片2</param>
        /// <returns></returns>
        public (bool, float) Contrast(Image image1, Image image2)
        {
            if (image1.Width > 1536 || image1.Height > 1536)
            {
                image1 = Utils.ImageUtil.ScaleImage(image1, 1536, 1536);
            }
            if (image1.Width % 4 != 0)
            {
                image1 = Utils.ImageUtil.ScaleImage(image1, image1.Width - (image1.Width % 4), image1.Height);
            }

            if (image2.Width > 1536 || image2.Height > 1536)
            {
                image2 = Utils.ImageUtil.ScaleImage(image2, 1536, 1536);
            }
            if (image2.Width % 4 != 0)
            {
                image2 = Utils.ImageUtil.ScaleImage(image1, image2.Width - (image2.Width % 4), image2.Height);
            }
            float similarity = 0f;
            //人脸检测
            ASF_SingleFaceInfo singleFaceInfo1 = new ASF_SingleFaceInfo();
            IntPtr feature1 = FaceUtil.ExtractFeature(pImageEngine, image1, out singleFaceInfo1);
            //人脸检测
            ASF_SingleFaceInfo singleFaceInfo2 = new ASF_SingleFaceInfo();
            IntPtr feature2 = FaceUtil.ExtractFeature(pImageEngine, image2, out singleFaceInfo2);
            int ret = ASFFunctions.ASFFaceFeatureCompare(pImageEngine, feature1, feature2, ref similarity);
            MemoryUtil.Free(feature1);
            MemoryUtil.Free(feature2);
            return (true, similarity);
        }

        /// <summary>
        /// 活体检测
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>是否活体 </returns>
        public bool LivingThing(Image bitmap)
        {
            //检测人脸，得到Rect框
            ASF_MultiFaceInfo multiFaceInfo = FaceUtil.DetectFace(pImageEngine, bitmap);
            //得到最大人脸
            ASF_SingleFaceInfo maxFace = FaceUtil.GetMaxFace(multiFaceInfo);
            //得到Rect
            MRECT rect = maxFace.faceRect;
            //调整图片数据，非常重要
            ImageInfo imageInfo = Utils.ImageUtil.ReadBMP(bitmap);
            int retCode_Liveness = -1;
            bool isLiveness = false;
            //RGB活体检测
            ASF_LivenessInfo liveInfo = FaceUtil.LivenessInfo_RGB(pImageEngine, imageInfo, multiFaceInfo, out retCode_Liveness);
            //判断检测结果
            if (retCode_Liveness == 0 && liveInfo.num > 0)
            {
                int isLive = MemoryUtil.PtrToStructure<int>(liveInfo.isLive);
                isLiveness = (isLive == 1) ? true : false;
            }
            bitmap.Dispose();
            return isLiveness;
        }

        /// <summary>
        /// 人脸识别详情
        /// </summary>
        /// <param name="srcImage"></param>
        /// <returns>bool：是否成功 object：人脸详情  </returns>
        public (bool, List<Disting>, string) DistinguishInfo(Image srcImage)
        {
            if (srcImage.Width > 1536 || srcImage.Height > 1536)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, 1536, 1536);
            }
            //调整图像宽度，需要宽度为4的倍数
            if (srcImage.Width % 4 != 0)
            {
                srcImage = Utils.ImageUtil.ScaleImage(srcImage, srcImage.Width - (srcImage.Width % 4), srcImage.Height);
            }
            //调整图片数据，非常重要
            ImageInfo imageInfo = Utils.ImageUtil.ReadBMP(srcImage);
            //人脸检测
            ASF_MultiFaceInfo multiFaceInfo = FaceUtil.DetectFace(pImageEngine, imageInfo);
            //年龄检测
            int retCode_Age = -1;
            ASF_AgeInfo ageInfo = FaceUtil.AgeEstimation(pImageEngine, imageInfo, multiFaceInfo, out retCode_Age);
            //性别检测
            int retCode_Gender = -1;
            ASF_GenderInfo genderInfo = FaceUtil.GenderEstimation(pImageEngine, imageInfo, multiFaceInfo, out retCode_Gender);
            //3DAngle检测
            int retCode_3DAngle = -1;
            ASF_Face3DAngle face3DAngleInfo = FaceUtil.Face3DAngleDetection(pImageEngine, imageInfo, multiFaceInfo, out retCode_3DAngle);
            MemoryUtil.Free(imageInfo.imgData);
            if (multiFaceInfo.faceNum < 1)
            {
                srcImage.Dispose();
                return (false, new List<Disting>(), "无人脸");
            }
            int rectTemp = 0;
            List<Disting> listdisting = new List<Disting>();
            //标记出检测到的人脸
            for (int i = 0; i < multiFaceInfo.faceNum; i++)
            {
                Disting infodisting = new Disting();
                MRECT rect = MemoryUtil.PtrToStructure<MRECT>(multiFaceInfo.faceRects + MemoryUtil.SizeOf<MRECT>() * i);
                infodisting.orient = MemoryUtil.PtrToStructure<int>(multiFaceInfo.faceOrients + MemoryUtil.SizeOf<int>() * i);
                infodisting.age = MemoryUtil.PtrToStructure<int>(ageInfo.ageArray + MemoryUtil.SizeOf<int>() * i);
                infodisting.gender = MemoryUtil.PtrToStructure<int>(genderInfo.genderArray + MemoryUtil.SizeOf<int>() * i);
                //角度状态 非0表示人脸不可信
                infodisting.face3DStatus = MemoryUtil.PtrToStructure<int>(face3DAngleInfo.status + MemoryUtil.SizeOf<int>() * i);
                //roll为侧倾角，pitch为俯仰角，yaw为偏航角
                infodisting.roll = MemoryUtil.PtrToStructure<float>(face3DAngleInfo.roll + MemoryUtil.SizeOf<float>() * i);
                infodisting.pitch = MemoryUtil.PtrToStructure<float>(face3DAngleInfo.pitch + MemoryUtil.SizeOf<float>() * i);
                infodisting.yaw = MemoryUtil.PtrToStructure<float>(face3DAngleInfo.yaw + MemoryUtil.SizeOf<float>() * i);

                int rectWidth = rect.right - rect.left;
                int rectHeight = rect.bottom - rect.top;
                //查找最大人脸
                if (rectWidth * rectHeight > rectTemp)
                {
                    rectTemp = rectWidth * rectHeight;
                }
                ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
                Image image = Utils.ImageUtil.CutImage(srcImage, rect.left, rect.top, rect.right, rect.bottom);
                IntPtr feature = FaceUtil.ExtractFeature(pImageEngine, image, out singleFaceInfo);
                //人脸特征feature过滤
                ASF_FaceFeature faceFeature = MemoryUtil.PtrToStructure<ASF_FaceFeature>(feature);
                byte[] feature_b = new byte[faceFeature.featureSize];
                MemoryUtil.Copy(faceFeature.feature, feature_b, 0, faceFeature.featureSize);
                //释放指针
                MemoryUtil.Free(feature);
                image.Dispose();
                infodisting.features = ShapeUtil.ByteToHex(feature_b).Replace(" ", "");
                infodisting.left = rect.left;
                infodisting.top = rect.top;
                infodisting.right = rect.right;
                infodisting.bottom = rect.bottom;
                listdisting.Add(infodisting);
            }
            srcImage.Dispose();
            return (true, listdisting, "成功");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //MemoryUtil.Free(pImageEngine);
            FaceUtil.CloseSignFace(pImageEngine);
            //GC.Collect();
        }

        private static void Writelogs(string message, string method)
        {
            string path = Directory.GetCurrentDirectory() + "//logs//";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string content = $"时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  \r\n  方法：{method}  \r\n  内容：{message} \r\n\r\n ";
            File.AppendAllText(path, content);
        }


    }
}
