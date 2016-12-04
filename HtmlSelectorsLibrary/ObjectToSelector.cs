using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class ObjectToSelector
    {
        /// <summary>
        /// Generuje seletor dla danego obiektu
        /// </summary>
        /// <param name="node">Obiekt dla którego generujemy selektor</param>
        /// <param name="selector">Nie modyfikować, wartość przydatna w rekurencji. Wstaw pustą listę.</param>
        /// <returns>Zwraca selector</returns>
        public static List<string> GenerateSelector(HtmlNode node, List<string> selector)
        {
            if (selector != null)
            {
                HtmlNode parent = node.ParentNode;
                string result = "";
                bool hasSelectorWithId = false; //to wystarczy zeby zakonczyc szukanie
                bool cameToEnd = false;

                if (parent == null)
                {
                    cameToEnd = true;
                }
                else
                {
                    if (node.Id != null && node.Id != "")
                    {
                        result = "#" + node.Id;
                        hasSelectorWithId = true;
                    }
                    else
                    {
                        string className = node.Attributes["class"] != null ? node.Attributes["class"].Value.Trim().ToLower() : "";
                        if (className != "" && parent.Descendants().Where(
                            x => x.Attributes["class"] != null ? x.Attributes["class"].Value.Trim().ToLower() == className : false
                            ).Count() == 1)
                        {
                            result = "." + node.Attributes["class"].Value;
                        }
                        else
                        {
                            List<HtmlNode> parentDescendants = parent.Descendants(node.Name).ToList();
                            if (parentDescendants.Count > 1)
                            {
                                int countDesc = 1;
                                foreach (var d in parentDescendants)
                                {
                                    if (d.StreamPosition == node.StreamPosition)
                                    {
                                        result = node.Name + ":nth-child(" + countDesc + ")";
                                        break;
                                    }
                                    countDesc++;
                                }
                            }
                            else
                            {
                                result = node.Name + ":nth-child(1)";
                            }
                        }
                    }
                }

                if (result != "")
                {
                    selector.Add(result);
                }

                if (!hasSelectorWithId && !cameToEnd)
                {
                    selector = GenerateSelector(node.ParentNode, selector);
                }
            }
            return selector;
        }
    }
}
