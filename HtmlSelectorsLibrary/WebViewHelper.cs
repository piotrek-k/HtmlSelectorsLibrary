using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class WebViewHelper
    {
        /// <summary>
        /// Znajduje obiekt HTML mając do dyspozycji podstawowe dane (pobrane z webView)
        /// </summary>
        /// <param name="info">Dane identyfikujące obiekt</param>
        /// <param name="currentHTMLDocument"></param>
        /// <param name="specialAttributeName">Nazwa specialnego atrybutu służącego identyfikacji obiektu (opcjonalnie)</param>
        /// <returns>HtmlNode, null jeśli jest więcej niż jedne obiekt</returns>
        public static HtmlNode quickFindByBasicInfo(InformationFromWebView info, HtmlNode currentHTMLDocument, string specialAttributeName="")
        {
            List<HtmlNode> results = currentHTMLDocument.Descendants(info.nodeName).Where(x => x.InnerText.Trim() == info.ObjectInnerText.Trim()).ToList(); //TODO: czasami zdarzają się tekście tablulacje i zakonczenia linii \t \n. Wtedy nie działa

            if (results.Count > 1 && info.specialAttribute != "")
            {
                results = results.Where(x => x.Attributes[specialAttributeName] != null ? x.Attributes[specialAttributeName].Value.Trim().ToLower() == info.specialAttribute.Trim().ToLower() : false).ToList();
            }
            if (results.Count > 1 && info.className != "")
            {
                results = results.Where(x => x.Attributes["class"] != null ? x.Attributes["class"].Value.Trim().ToLower() == info.className.Trim().ToLower() : false).ToList();
            }
            if (results.Count > 1 && info.idName != "")
            {
                results = results.Where(x => x.Id.Trim().ToLower() == info.idName.Trim().ToLower()).ToList();
            }
            if (results.Count > 1 && info.parentType != "")
            {
                results = results.Where(x => x.ParentNode.Name.Trim().ToLower() == info.parentType.Trim().ToLower()).ToList();
            }
            if (results.Count > 1 && info.hrefLink != "")
            {
                results = results.Where(x => x.Attributes["href"] != null ? x.Attributes["href"].Value.Trim().ToLower() == info.hrefLink.Trim().ToLower() : false).ToList();
            }
            if (results.Count > 1 && info.innerTextOfNearestChangedParent != "")
            {
                results = results.Where(x => Regex.Replace(x.ParentNode.ParentNode.InnerText, @"\s+", "") == Regex.Replace(info.innerTextOfNearestChangedParent, @"\s+", "")).ToList();
            }
            if (results.Count > 1)
            {
                Debug.WriteLine("Nie znaleziono szukanego obiektu. Za dużo potencjalnych danych.");
                return null;
            }
            else
            {
                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Dane pobrane z WebView, które pozwolą zidentyfikować obiekt w kodzie.
        /// </summary>
        public class InformationFromWebView
        {
            public string ObjectInnerText { get; set; }
            /// <summary>
            /// Typ szukanego obiektu np. body
            /// </summary>
            public string nodeName { get; set; }
            public string className { get; set; }
            public string idName { get; set; }
            /// <summary>
            /// np. body
            /// </summary>
            public string parentType { get; set; }
            public string hrefLink { get; set; }
            /// <summary>
            /// Wartość innerText obiektu znajdującego się 2 miejsca wyżej w hierarchii (x.ParentNode.ParentNode.InnerText)
            /// </summary>
            public string innerTextOfNearestChangedParent { get; set; }
            /// <summary>
            /// Wymaga utworzenia specjalnego atrybutu dla danego obiektu HTML. 
            /// Wartość atrybutu powinna być identyfikatorem. 
            /// Skrypt będzie szukał obiektu na podstawie wartości tego atrybutu.
            /// </summary>
            public string specialAttribute { get; set; }
        }

        /// <summary>
        /// Modyfikuje wybrane obiekty dodając im nowy atrybut pozwalający na ich późniejszą identyfikację.
        /// </summary>
        /// <param name="websiteHTML">Kod źródłowy pobranej strony</param>
        /// <param name="specialAttributeName">Nazwa atrybutu identyfikującego</param>
        /// <param name="typeOfObjectToAddAttribute">Typ obiektu który ma mieć ten atrybut</param>
        /// <returns>Zmodyfikowana strona w postaci HtmlNode</returns>
        HtmlNode LoadAndModifyWebPage(string websiteHTML, string specialAttributeName, string typeOfObjectToAddAttribute = "#text")
        {
            //ustawianie własnych atrybutów aby ułatwić lokalizację obiektów
            var hap = new HtmlDocument();
            hap.LoadHtml(websiteHTML);
            int i = 0;
            var objects = hap.DocumentNode.Descendants(typeOfObjectToAddAttribute);
            foreach (var x in objects)
            {
                x.ParentNode.SetAttributeValue(specialAttributeName, i + "");
                i++;
            }

            return hap.DocumentNode;
        }
    }
}
