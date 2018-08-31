using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Godius.WebCrawler
{
    public class RankingCrawler
    {
        const string API_ADDRESS = "http://godius10.cafe24.com/comuser/commu/com_ranking.asp";
        const string METHOD = "POST";
        const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        const string PARAMETER_KEY = "SearchMan";
        const string PARSE_BEGIN_TEXT = "<span style='color:#ff0000'>";
        const string PARSE_END_TEXT = "</span>";

        static readonly Encoding UsedEncoding = Encoding.GetEncoding(949);

        public static string GetCharacterRanking(string characterName)
        {
            var request = WebRequest.Create(API_ADDRESS) as HttpWebRequest;
            request.Method = METHOD;
            request.ContentType = CONTENT_TYPE;

            var requestStream = request.GetRequestStream();
            using (var sw = new StreamWriter(requestStream))
            {
                sw.WriteLine($"{PARAMETER_KEY}={HttpUtility.UrlEncode(characterName, UsedEncoding)}");
            }

            var response = request.GetResponse() as HttpWebResponse;
            using (var sr = new StreamReader(response.GetResponseStream(), UsedEncoding))
            {
                var responseText = sr.ReadToEnd();

                var begin = responseText.IndexOf(PARSE_BEGIN_TEXT) + PARSE_BEGIN_TEXT.Length;
                var end = responseText.IndexOf(PARSE_END_TEXT, begin);
                if (begin < 0 || end < 0)
                    return null;

                var rankingText = responseText.Substring(begin, end - begin);
                return rankingText;
            }
        }

        public static async Task<string> GetCharacterRankingAsync(string characterName)
        {
            var request = WebRequest.Create(API_ADDRESS) as HttpWebRequest;
            request.Method = METHOD;
            request.ContentType = CONTENT_TYPE;

            var requestStream = await request.GetRequestStreamAsync();
            using (var sw = new StreamWriter(requestStream))
            {
                sw.WriteLine($"{PARAMETER_KEY}={HttpUtility.UrlEncode(characterName, UsedEncoding)}");
            }

            var response = (await request.GetResponseAsync()) as HttpWebResponse;
            using (var sr = new StreamReader(response.GetResponseStream(), UsedEncoding))
            {
                var responseText = sr.ReadToEnd();

                var begin = responseText.IndexOf(PARSE_BEGIN_TEXT) + PARSE_BEGIN_TEXT.Length;
                var end = responseText.IndexOf(PARSE_END_TEXT, begin);
                var rankingText = responseText.Substring(begin, end - begin);
                return rankingText;
            }
        }
    }
}
