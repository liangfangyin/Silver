using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Silver.Basic
{
    public static class XmlUtil
    {

        // 读取XML文件
        public static XmlDocument LoadXml(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load XML file: " + ex.Message);
            }
            return xmlDocument;
        }

        // 创建XML文件
        public static void CreateXml(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode declarationNode = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.AppendChild(declarationNode);
            XmlNode rootNode = xmlDocument.CreateElement("root");
            xmlDocument.AppendChild(rootNode);
            try
            {
                xmlDocument.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create XML file: " + ex.Message);
            }
        }

        // 添加节点到XML文件
        public static void AddNode(string filePath, string parentNodeXPath, string nodeName, string nodeValue = null)
        {
            XmlDocument xmlDocument = LoadXml(filePath);
            XmlNode parentNode = xmlDocument.SelectSingleNode(parentNodeXPath);
            if (parentNode != null)
            {
                XmlNode newNode = xmlDocument.CreateElement(nodeName);
                if (!string.IsNullOrEmpty(nodeValue))
                {
                    newNode.InnerText = nodeValue;
                }
                parentNode.AppendChild(newNode);
            }
            try
            {
                xmlDocument.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add node to XML file: " + ex.Message);
            }
        }

        // 更新XML节点的值
        public static void UpdateNode(string filePath, string nodeXPath, string newValue)
        {
            XmlDocument xmlDocument = LoadXml(filePath);
            XmlNode nodeToUpdate = xmlDocument.SelectSingleNode(nodeXPath);
            if (nodeToUpdate != null)
            {
                nodeToUpdate.InnerText = newValue;
            }
            try
            {
                xmlDocument.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to update XML node: " + ex.Message);
            }
        }

        // 删除XML节点
        public static void DeleteNode(string filePath, string nodeXPath)
        {
            XmlDocument xmlDocument = LoadXml(filePath);
            XmlNode nodeToDelete = xmlDocument.SelectSingleNode(nodeXPath);
            if (nodeToDelete != null)
            {
                XmlNode parentNode = nodeToDelete.ParentNode;
                parentNode.RemoveChild(nodeToDelete);
            }
            try
            {
                xmlDocument.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete XML node: " + ex.Message);
            }
        }

        /// <summary>
        /// 实体类转XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="xmlFilePath"></param>
        public static string ObjectToXml<T>(T value) where T : class,new()
        {
            string xmlFilePath = Directory.GetCurrentDirectory() + "/temp/";
            if (!Directory.Exists(xmlFilePath))
            {
                Directory.CreateDirectory(xmlFilePath);
            }
            xmlFilePath += Guid.NewGuid().ToString() + ".xml";
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter streamWriter = new StreamWriter(xmlFilePath))
            {
                serializer.Serialize(streamWriter, value);
            }
            XmlDocument xmlDocument = LoadXml(xmlFilePath);
            File.Delete(xmlFilePath);
            return xmlDocument.InnerXml;
        }

        /// <summary>
        /// 将XML转换为实体对象
        /// </summary>
        /// <typeparam name="T">
        /// [XmlRoot("Person")]
        /// public class Person{}
        /// </typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
