using Silver.ArcSoft.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ArcSoft.Utils
{
    public class ShapeUtil
    {

        /// <summary>
        /// byte数组转hex
        /// </summary>
        /// <param name="comByte"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] comByte)
        {
            string returnStr = "";
            if (comByte != null)
            {
                for (int i = 0; i < comByte.Length; i++)
                {
                    returnStr += comByte[i].ToString("X2") + " ";
                }
            }
            return returnStr;
        }

        /// <summary>
        /// hex转byte[]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public byte[] HexToByte(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }


        /// <summary>
        /// 将特征码转换成指针
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IntPtr byteToIntPtr(string s)
        {
            byte[] bytes = HexToByte(s);
            IntPtr pLocalFeature = IntPtr.Zero;
            ASF_FaceFeature localFeature = new ASF_FaceFeature();
            localFeature.feature = MemoryUtil.Malloc(bytes.Length);
            MemoryUtil.Copy(bytes, 0, localFeature.feature, bytes.Length);
            localFeature.featureSize = bytes.Length;
            pLocalFeature = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_FaceFeature>());
            MemoryUtil.StructureToPtr(localFeature, pLocalFeature);
            return pLocalFeature;
        }



    }
}
