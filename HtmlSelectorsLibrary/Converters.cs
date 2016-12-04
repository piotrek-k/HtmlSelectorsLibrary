using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class Converters
    {
        /// <summary>
        /// Wbudowane Descendants() zwraca obiekty w kolejności pojawiania się w pliku (od góry do dołu). 
        /// Ta fukcja zwraca najpierw obiekty wyżej w hierarchii, potem te niżej.
        /// </summary>
        /// <param name="hn">obiekt od którego zaczynamy</param>
        /// <param name="howDeep">Jak daleko szukać (w drzewie obiektow)</param>
        /// <returns></returns>
        public static List<HtmlNode> GetDescendantsSortedByHierarchy(HtmlNode hn, int howDeep)
        {
            List<HtmlNode> result = new List<HtmlNode>();

            if (howDeep > 1)
            {
                List<HtmlNode> nextStep = hn.ChildNodes.ToList();
                foreach (var n in nextStep)
                {
                    result.Add(n);
                }
                foreach (var n in nextStep)
                {
                    result.AddRange(GetDescendantsSortedByHierarchy(n, howDeep - 1));
                }
            }

            return result;
        }

        /// <summary>
        /// Selector podzielony na części do selektora w postaci stringa
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static string SelectorAsListToString(List<string> selectors)
        {
            string result = "";

            foreach (var s in selectors)
            {
                if (result != "")
                {
                    result = s + " > " + result;
                }
                else
                {
                    result = s;
                }
            }

            return result;
        }
    }
}
