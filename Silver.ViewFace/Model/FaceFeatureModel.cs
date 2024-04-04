using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ViewFace.Model
{
    /// <summary>
    /// 会员特征值
    /// </summary>
    public class FaceFeatureModel
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public string MemberId { get; set; } = "";

        /// <summary>
        /// 人脸类型
        /// </summary>
        public string FaceType { get; set; } = "";

        /// <summary>
        /// 特征值
        /// </summary>
        public float[] Feature { get; set; }

    }
}
