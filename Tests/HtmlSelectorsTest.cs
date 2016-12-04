using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlSelectorsLibrary;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class HtmlSelectorsTest
    {
        public HtmlDocument htmlDoc;
        List<string> selectorAsList;
        string selectorAsString = "";
        HtmlNode exampleObj;
        [TestMethod]
        public void LoadPage()
        {
            string _filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string finalPath = _filePath.ToString() + "" + "\\Debug\\Resources\\exampleHtml.html";
            htmlDoc = new HtmlDocument();
            string html = WebPageLoader.LoadHTMLFileFromResource(finalPath);
            htmlDoc.LoadHtml(html);
            Assert.IsNotNull(html);
        }

        [TestMethod]
        public void GeneratingSelector()
        {
            LoadPage();
            exampleObj = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes["href"] != null ? x.Attributes["href"].Value == "http://go.microsoft.com/fwlink/?LinkID=615541" : false).FirstOrDefault();
            selectorAsList = ObjectToSelector.GenerateSelector(exampleObj, new List<string>());
            Assert.AreEqual(5, selectorAsList.Count);
        }

        [TestMethod]
        public void ConvertSelectorToString()
        {
            GeneratingSelector();
            selectorAsString = Converters.SelectorAsListToString(selectorAsList);
            Assert.AreEqual("#main > div:nth-child(2) > ul:nth-child(1) > li:nth-child(1) > a:nth-child(1)", selectorAsString);
        }

        [TestMethod]
        public void FindObjectUsingSelector()
        {
            ConvertSelectorToString();
            List<HtmlNode> foundObjs = SelectorToObject.FindBySelector(htmlDoc.DocumentNode, selectorAsString);
            Assert.AreEqual(1, foundObjs.Count);
            Assert.AreEqual(foundObjs.FirstOrDefault(), exampleObj);
        }
    }
}
