using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSelectorsLibrary
{
    public class WebPageLoader
    {
        /// <summary>
        /// Zwraca HTML strony z pliku
        /// </summary>
        public string LoadHTMLFileFromResource(string pathToFile)
        {
            return System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
        }

        /// <summary>
        /// Zwraca html strony o podanym adresie
        /// </summary>
        public async Task<string> LoadHTMLFromWebsite(string webPageURL)
        {
            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(webPageURL))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                var htmlData = await content.ReadAsStringAsync();

                var hap = new HtmlDocument();
                hap.LoadHtml(htmlData);
                //currentHTMLDocument = hap.DocumentNode;

                return htmlData;
            }
        }
    }
}
