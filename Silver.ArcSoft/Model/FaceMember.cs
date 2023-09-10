using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.ArcSoft.Model
{
    public class FaceMember
    {

        /// <summary>
        /// 人员ID
        ///</summary>
        public string FaceId { get; set; } = "";

        /// <summary>
        /// 人员id
        ///</summary> 
        public string Memnerid { get; set; } = "";

        /// <summary>
        /// 用户名
        ///</summary> 
        public string Name { get; set; } = "";

        /// <summary>
        /// 唯一识别号
        ///</summary> 
        public string No { get; set; } = "";

        /// <summary>
        /// 第三方卡号
        ///</summary> 
        public string Cardid { get; set; } = "";

        /// <summary>
        /// 性别  1：男  0：女  2：未知
        ///</summary> 
        public int Sex { get; set; } = 1;

        /// <summary>
        /// 商户号
        ///</summary> 
        public long Mchid { get; set; } = 0;
         
        /// <summary>
        /// 生日
        ///</summary> 
        public string Birthday { get; set; } = "";

        /// <summary>
        /// 注册头像
        ///</summary> 
        public string Headurl { get; set; } = "";

        /// <summary>
        /// 职位
        ///</summary> 
        public string Position { get; set; } = "";

        /// <summary>
        /// 手机号码
        ///</summary> 
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 电子邮箱
        ///</summary> 
        public string Email { get; set; } = "";

        /// <summary>
        /// 身份证号码
        ///</summary> 
        public string Idcard { get; set; } = "";

        /// <summary>
        /// 特征码
        /// </summary>
        public string Feature { get; set; } = "";


    }
}
