using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Godius.RankCollector
{
    class Program
    {
        const string API_ADDRESS = "http://godius10.cafe24.com/comuser/commu/com_ranking.asp";
        const string METHOD = "POST";
        const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        const string PARAMETER_KEY = "SearchMan";
        const string PARSE_BEGIN_TEXT = "<span style='color:#ff0000'>";
        const string PARSE_END_TEXT = "</span>";

        static readonly Encoding UsedEncoding = Encoding.GetEncoding(949);

        static void Main(string[] args)
        {
            var members = new string[] {
                "GISADAN",
                "악어",
                "토르",
                "마법의나라",
                "WMMW",
                "장대인",
                "수양대군",
                "현상태",
                "백장미",
                "뭔데이",
                "아이오페",
                "제제",
                "전설의마녀",
                "은가비",
                "게임의나라",
                "타하라미카",
                "제이앤비",
                "Firefighter",
                "음악",
                "까미",
                "시카고",
                "연잎",
                "국가",
                "기부",
                "그루트",
                "판타지의나라",
                "가족같은경우",
                "두물머리",
                "조병주"
            };

            foreach (var member in members)
            {
                var rank = GetCharacterRanking(member, UsedEncoding);
                //Console.WriteLine($"{member}\t{rank}");
                Console.WriteLine($"{rank}");
            }

            Console.ReadLine();
        }

        private static string GetCharacterRanking(string id, Encoding encoding)
        {
            try
            {
                var request = WebRequest.Create(API_ADDRESS) as HttpWebRequest;
                request.Method = METHOD;
                request.ContentType = CONTENT_TYPE;

                var requestStream = request.GetRequestStream();
                using (var sw = new StreamWriter(requestStream))
                {
                    sw.WriteLine($"{PARAMETER_KEY}={HttpUtility.UrlEncode(id, UsedEncoding)}");
                }
                //request.ContentLength = requestStream.Length;

                var response = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(response.GetResponseStream(), UsedEncoding))
                {
                    var responseText = sr.ReadToEnd();
                    
                    var begin = responseText.IndexOf(PARSE_BEGIN_TEXT) + PARSE_BEGIN_TEXT.Length;
                    var end = responseText.IndexOf(PARSE_END_TEXT, begin);

                    var rankingText = responseText.Substring(begin, end - begin);


                    //"<span style="color:#ff0000">1234</span>";
                    //< font color = "#A8BCC6" >< b > 토르 님의 순위는<span style= 'color:#ff0000' > 1234 </ span > 입니다. & nbsp; &nbsp; 타페리 & nbsp; &nbsp;< img src = '/pos/4.jpg' border = '0' height = '34' align = 'absmiddle' > &nbsp;< img src = '/mark/43.jpg' border = '0' title = '기사단' align = 'absmiddle' ></ b ></ font >
                    //토르 님의 순위는 <span style='color:#ff0000'>1234</span> 
                    return rankingText;
                }
            }
            catch (WebException we)
            {
                //Console.WriteLine(we);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }

            return String.Empty;
        }
    }
}
