// See https://aka.ms/new-console-template for more information
using System;
using Silver.Basic;

var result= DateTime.Now.ToLastWeek();

Console.WriteLine("Hello, World!");
Console.WriteLine((new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()).ToString());
Console.ReadLine();

