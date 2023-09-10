using Silver.ArcSoft.Model;
using System.Drawing;

namespace Silver.ArcSoft
{
    public interface IArcSoftFace : IDisposable
    {


        /// <summary>
        /// 增加人脸
        /// </summary>
        /// <param name="faceId"></param>
        /// <param name="softFace"></param>
        public void AddFace(FaceMember softFace);

        /// <summary>
        /// 批量添加人脸
        /// </summary>
        /// <param name="face"></param>
        public void AddFaces(List<FaceMember> face);

        /// <summary>
        /// 激活
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Activation();

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitEngines();

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="image"></param>
        /// <returns>bool:是否成功，string：成功特征码，识别原因</returns>
        public (bool, string) Register(Image image);

        /// <summary>
        /// 人脸识别
        /// </summary>
        /// <param name="srcImage"></param> 
        /// <returns>bool：是否成功  int：识别成功索引   float：相似度  </returns>
        public (bool, FaceMember, float) Distinguish(Image srcImage);

        /// <summary>
        /// 人脸识别-返回人员信息
        /// </summary>
        /// <param name="Feature">特征码</param> 
        /// <returns>bool：是否成功  int：识别成功索引   float：相似度  </returns>
        public (bool, FaceMember, float) Distinguish(string Feature);

        /// <summary>
        /// 人脸识别-返回特征
        /// </summary>
        /// <param name="srcImage"></param>
        /// <returns></returns>
        public (bool, FaceDistinguish, string) DistinguishFeatures(Image srcImage);

        /// <summary>
        /// 人脸对比相似度
        /// </summary>
        /// <param name="image1">人脸图片1</param>
        /// <param name="image2">人脸图片2</param>
        /// <returns></returns>
        public (bool, float) Contrast(Image image1, Image image2);

        /// <summary>
        /// 活体检测
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>是否活体 </returns>
        public bool LivingThing(Image bitmap);

        /// <summary>
        /// 人脸识别详情
        /// </summary>
        /// <param name="srcImage"></param>
        /// <returns>bool：是否成功 object：人脸详情  </returns>
        public (bool, List<Disting>, string) DistinguishInfo(Image srcImage);


    }
}
