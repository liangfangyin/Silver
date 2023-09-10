using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.ArcSoft.Model
{
    public class FaceDistinguish
    { 
        /// <summary>
        /// 人员图片
        ///</summary>
        public string Imageurl { get; set; } = "";

        /// <summary>
        /// 商户id
        ///</summary>
        public long Mchid { get; set; } = 0;

        /// <summary>
        /// 识别会员id
        ///</summary>
        public long Memberid { get; set; } = 0;

        /// <summary>
        /// 识别会员名称
        ///</summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 识别用时毫秒
        ///</summary>
        public int Timeout { get; set; } = 0;

        /// <summary>
        /// 识别率
        ///</summary>
        public decimal Rate { get; set; } = 0;

        /// <summary>
        /// 算法  1：虹软
        ///</summary>
        public int Algorithm { get; set; } = 0;
 
        /// <summary>
        /// 年龄
        ///</summary>
        public int Age { get; set; } = 0;

        /// <summary>
        /// 性别 1：男 0：女 2：未知
        ///</summary>
        public int Sex { get; set; } = 0;

        /// <summary>
        /// 是否活体  1：是 0：否
        ///</summary>
        public int Isliving { get; set; } = 1;

        /// <summary>
        /// 人脸数
        ///</summary>
        public int Face { get; set; } = 1;

        /// <summary>
        /// 人脸角度
        ///</summary>
        public int Orient { get; set; } = 0;

        /// <summary>
        /// 错误码
        ///</summary>
        public string Errcode { get; set; } = "";

        /// <summary>
        /// 错误详情
        ///</summary>
        public string Errmsg { get; set; } = "";



    }
}
