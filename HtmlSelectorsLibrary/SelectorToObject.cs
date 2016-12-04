using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class SelectorToObject
    {
        static char[] firstStepDivision = { '>' };
        static char[] secondStepDivision = { '.', '#', ':', '(', ')' };

        private static Regex forObjectWithClass = new Regex(@"\..+");
        private static Regex forNthChildInObject = new Regex(@".+\:nth-child\(\d+\)");
        private static Regex forObjectWithID = new Regex(@"\#.+");
        private static Regex forObjectType = new Regex(@"^[^#\.\(\}]+$");

        /// <summary>
        /// Znajduje wszystkie obiekty spełniające warunki z podanego selectora
        /// </summary>
        /// <param name="hn">Obiekt html od którego zaczynamy szukanie. Zwykle jest to 'document'.</param>
        /// <param name="selector"></param>
        /// <returns>Lista obiektow HTML</returns>
        public static List<HtmlNode> FindBySelector(HtmlNode hn, string selector)
        {
            string[] parts = selector.Split(firstStepDivision, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> selectors = parts.ToList();

            List<HtmlNode> foundObjects = FindNext(hn, selectors);

            return foundObjects;
        }

        /// <summary>
        /// Znajdź wszystkie obiekty html, które są zgodne z pierwszym selektorem z listy
        /// </summary>
        /// <param name="hn">Obiekt html od którego zaczynamy</param>
        /// <param name="selectors">Selektor podzielony na części</param>
        /// <returns></returns>
        private static List<HtmlNode> FindNext(HtmlNode hn, List<string> selectors)
        {
            string currentSelector = selectors[0].Trim();
            List<HtmlNode> nextSteps = new List<HtmlNode>();
            string[] selectorAsParts = currentSelector.Split(secondStepDivision, System.StringSplitOptions.RemoveEmptyEntries);

            if (forObjectWithClass.IsMatch(currentSelector)) //.jakasKlasa
            {
                nextSteps.AddRange(
                    hn.Descendants()
                    .Where(x => x.Attributes["class"] != null ? x.Attributes["class"].Value.Trim().ToLower() == selectorAsParts[0].Trim().ToLower() : false)
                    .ToList()
                    );
            }
            else if (forObjectWithID.IsMatch(currentSelector)) //#jakiesID
            {
                nextSteps.AddRange(
                    hn.Descendants()
                    .Where(x => x.Id.Trim().ToLower() == selectorAsParts[0].Trim().ToLower())
                    );
            }
            else if (forObjectType.IsMatch(currentSelector)) //body
            {
                nextSteps.AddRange(
                    hn.Descendants()
                    .Where(x => x.Name.Trim().ToLower() == selectorAsParts[0].Trim().ToLower())
                    );
            }
            else if (currentSelector.Contains("nth-child({x})"))
            {
                nextSteps.AddRange(
                    hn.Descendants()
                    .Where(x => x.Name.Trim().ToLower() == selectorAsParts[0].Trim().ToLower())
                    );
            }
            else if (forNthChildInObject.IsMatch(currentSelector)) //table:nth-child(10)
            {
                nextSteps.AddRange(
                        FindNearestObjectOfIndex(hn, selectorAsParts[0], int.Parse(selectorAsParts[2]))
                    );
            }
            else
            {
                throw new NotImplementedException();
            }

            List<HtmlNode> results = new List<HtmlNode>();
            if (selectors.Count > 1)
            {
                foreach (var nextStep in nextSteps)
                {
                    results.AddRange(
                        FindNext(
                            nextStep,
                            selectors.Skip(1).ToList()
                            )
                        );
                }
            }
            else
            {
                results.AddRange(nextSteps);
            }

            return results;
        }

        /// <summary>
        /// Znajduje obiekt danego typu, który jest najbliższy indexowi podanemu w selektorze (aby zniwelować różnice między tym co renderuje przeglądarka a faktycznym kodem, zagnieżdżone listy itp)
        /// </summary>
        /// <param name="startFrom">Obiekt html z którego zaczynamy szukanie</param>
        /// <param name="objectType">Typ obiektu (node name)</param>
        /// <param name="indexInHierarchy">Spodziewany index (nth-child)</param>
        /// <param name="startFromIndex">Nie zmieniać, parametr przydatny w rekurencji. Pierwsze użycie zawsze ma wartość 1</param>
        /// <returns></returns>
        public static List<HtmlNode> FindNearestObjectOfIndex(HtmlNode startFrom, string objectType, int indexInHierarchy, int? startFromIndex = 1)
        {
            var children = startFrom.ChildNodes.ToList();
            int countIndex = startFromIndex != null ? (int)startFromIndex : 1;
            bool finished = false;
            List<HtmlNode> result = new List<HtmlNode>();

            foreach (var c in children)
            {
                if (c.Name.Trim().ToLower() == objectType.Trim().ToLower())
                {
                    if (countIndex == indexInHierarchy)
                    {
                        result.Add(c);
                        finished = true;
                    }

                    countIndex++;
                }
            }

            if (!finished)
            {
                foreach (var c in children)
                {
                    result.AddRange(FindNearestObjectOfIndex(c, objectType, indexInHierarchy, countIndex));
                }
            }

            return result;
        }
    }
}
