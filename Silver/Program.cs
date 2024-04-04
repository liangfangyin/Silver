// See https://aka.ms/new-console-template for more information
using System;
using Silver.Basic;
using Silver.ViewFace;

 
var result= ViewFaceUtil.GetFaceBase("D:\\imgs\\14.jpg");

Console.WriteLine((new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()).ToString());
Console.ReadLine();

