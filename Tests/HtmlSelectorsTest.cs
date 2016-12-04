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

        [TestMethod]
        public string MergeTwoSelectors()
        {
            LoadPage();
            HtmlNode obj1 = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes["href"] != null ? x.Attributes["href"].Value == "http://go.microsoft.com/fwlink/?LinkID=615541" : false).FirstOrDefault();
            HtmlNode obj2 = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes["href"] != null ? x.Attributes["href"].Value == "http://go.microsoft.com/fwlink/?LinkID=615530" : false).FirstOrDefault();

            List<string> selector1 = ObjectToSelector.GenerateSelector(obj1, new List<string>());
            List<string> selector2 = ObjectToSelector.GenerateSelector(obj2, new List<string>());

            List<string> result = SelectorsMerge.GenerateSelectorSchema(selector1, selector2);
            string resultAsString = Converters.SelectorAsListToString(result);

            Assert.AreEqual("#main > div:nth-child(2) > ul:nth-child(1) > li:nth-child({x}) > a:nth-child(1)", resultAsString);

            return resultAsString;
        }

        [TestMethod]
        public void ResultOfMergedSelector()
        {
            string mergedselector = MergeTwoSelectors();
            List<HtmlNode> objs = SelectorToObject.FindBySelector(htmlDoc.DocumentNode, mergedselector);
            Assert.AreEqual(12, objs.Count);
        }
    }
}
