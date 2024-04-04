using System;
using System.Collections.Generic;
using System.Text;
using ViewFaceCore.Model;

namespace Silver.ViewFace.Model
{
    public class FaceBaseModel
    {
        /// <summary>
        /// 活体检测
        /// </summary>
        public AntiSpoofingStatus SpoofingStatus { get; set; }

        /// <summary>
        /// 是否佩戴口罩
        /// </summary>
        public bool Masked { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 眼睛
        /// </summary>
        public EyeStateResult EyeState { get; set; }

    }
}
