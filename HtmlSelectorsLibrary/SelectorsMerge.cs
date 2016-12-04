using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class SelectorsMerge
    {
        static char[] firstStepDivision = { '>' };
        static char[] secondStepDivision = { '.', '#', ':', '(', ')' };

        private static Regex forObjectWithClass = new Regex(@"\..+");
        private static Regex forNthChildInObject = new Regex(@".+\:nth-child\(\d+\)");
        private static Regex forObjectWithID = new Regex(@"\#.+");
        private static Regex forObjectType = new Regex(@"^[^#\.\(\}]+$");

        /// <summary>
        /// Enum dla GenerateSelectorSchema. Określa stan operacji odgadywania schematu na zasadzie ilości obiektów w jakim jest dana
        /// </summary>
        enum StateOfSteppingBack : int { None, InProgress, Finish };

        /// <summary>
        /// Porównuje dwa selectory w postaci list
        /// </summary>
        /// <param name="sample1"></param>
        /// <param name="sample2"></param>
        /// <returns>zwraca (jeśli to możliwe) wspólny schemat</returns>
        public static List<string> GenerateSelectorSchema(List<string> sample1, List<string> sample2)
        {
            List<string> result = new List<string>();
            //obiekty najnizsze w hierarchii są na początku

            sample1.Reverse();
            sample2.Reverse();

            int biggestSize = (sample1.Count > sample2.Count) ? sample1.Count : sample2.Count;
            int s1Index = 0;
            int s2Index = 0;
            string previousS1 = "";
            string previousS2 = "";
            int steppingBack = (int)StateOfSteppingBack.None;
            int previousSteppingBack = -1;

            while (s1Index < sample1.Count && s2Index < sample2.Count)
            {
                string s1 = sample1[s1Index];
                string s2 = sample2[s2Index];

                string newSelectorStep = s1;

                if (s1 != s2)
                {
                    string[] selectorAsParts1 = s1.Split(secondStepDivision, StringSplitOptions.RemoveEmptyEntries);
                    string[] selectorAsParts2 = s2.Split(secondStepDivision, StringSplitOptions.RemoveEmptyEntries);

                    if (forNthChildInObject.IsMatch(s1) && forNthChildInObject.IsMatch(s2) && selectorAsParts1[0] == selectorAsParts2[0]) //table:nth-child(10), typ obiektu jest ten sam, roznica w indeksie
                    {
                        newSelectorStep = selectorAsParts1[0] + ":nth-child({x})";

                        if (steppingBack == (int)StateOfSteppingBack.InProgress && s1 != previousS1 && s2 != previousS2)
                        {
                            steppingBack = (int)StateOfSteppingBack.Finish;
                        }
                        else
                        {
                            s1Index++;
                            s2Index++;
                        }
                    }
                    else
                    {
                        //możliwe że schemat każdy kolejny obiekt znajduje się w o jednym kontenerze więcej. Sprawdźmy:
                        if (s1 == previousS2)
                        {
                            s2Index--;
                            steppingBack = (int)StateOfSteppingBack.InProgress;
                        }
                        else if (s2 == previousS1)
                        {
                            s1Index--;
                            steppingBack = (int)StateOfSteppingBack.InProgress;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if (steppingBack == (int)StateOfSteppingBack.InProgress && s1 != previousS1 && s2 != previousS2)
                    {
                        steppingBack = (int)StateOfSteppingBack.Finish;
                    }
                    else
                    {
                        s1Index++;
                        s2Index++;
                    }
                }

                if (steppingBack == (int)StateOfSteppingBack.Finish)
                {
                    string[] prevPartedS1 = previousS1.Split(secondStepDivision, StringSplitOptions.RemoveEmptyEntries);
                    //string[] prevPartedS2 = previousS2.Split(secondStepDivision, StringSplitOptions.RemoveEmptyEntries);
                    if (forObjectWithClass.IsMatch(previousS1) || forObjectWithID.IsMatch(previousS1)) //index=1
                    {
                        newSelectorStep = "::" + prevPartedS1[1] + "::";
                    }
                    else
                    {
                        newSelectorStep = "::" + prevPartedS1[0] + "::";
                    }

                    steppingBack = (int)StateOfSteppingBack.None;
                }

                if (steppingBack != (int)StateOfSteppingBack.InProgress)
                {
                    result.Insert(0, newSelectorStep);
                }

                if (steppingBack == (int)StateOfSteppingBack.InProgress && result[0] == previousS1)
                {
                    result.RemoveAt(0);
                }

                previousS1 = s1;
                previousS2 = s2;
                previousSteppingBack = steppingBack;
            }

            return result;
        }
    }
}
