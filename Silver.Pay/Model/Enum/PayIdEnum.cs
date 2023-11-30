using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Pay.Model.Enum
{
    public enum PayIdEnum
    {
        //当面付-扫码支付
        AliPayPreCreate = 1,
        //当面付-二维码/条码/声波支付
        AliPayPay = 2,
        //APP支付
        AliPayAPP = 3,
        //电脑网站支付
        AliPayWeb = 4,
        //手机网站支付
        AliPayWap = 5,
        //商户扫描客户支付码
        WeChatMicro = 6,
        //APP支付-App下单API
        WeChatAPP = 7,
        //公众号支付-JSAPI下单
        WeChatPub = 8,
        //扫码支付-Native下单API
        WeChatQrCode = 9,
        //H5支付-H5下单API
        WeChatH5 = 10,
        //小程序支付-JSAPI下单
        WeChatMiniProgram = 11 
    }
}
