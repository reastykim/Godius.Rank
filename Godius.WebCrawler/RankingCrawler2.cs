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
	public class RankingCrawlerV2 : IRankingCrawler
	{
		const string API_ADDRESS = "http://www.godius.co.kr/community_3?m_info=1";
		const string CONTENT_TYPE = "application/x-www-form-urlencoded";
		const string PARAMETER_KEY = "m_info2";
		const string PARSE_BEGIN_TEXT = "<span style='color:#ff0000'>";
		const string PARSE_END_TEXT = "</span>";

		readonly Encoding UsedEncoding = Encoding.GetEncoding(949);

		public string GetCharacterRanking(string characterName)
		{
			var request = WebRequest.Create($"{API_ADDRESS}&m_info2={characterName}") as HttpWebRequest;
			var response = request.GetResponse() as HttpWebResponse;
			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				var responseText = sr.ReadToEnd();

				var begin = responseText.IndexOf("<tbody>") + "<tbody>".Length;
				var end = responseText.IndexOf("</tbody>", begin);
				if (begin < 0 || end < 0)
					return null;

				var tbodyText = responseText.Substring(begin, end - begin).Replace("\r\n", "").Replace(" ", "");
				if (tbodyText.Contains("\"no-result\""))
					return null;

				begin = tbodyText.IndexOf("<tr><td>") + "<tr><td>".Length;
				end = tbodyText.IndexOf("</td>", begin);

				var rankingText = tbodyText.Substring(begin, end - begin);
				return rankingText;
			}
		}

		public async Task<string> GetCharacterRankingAsync(string characterName)
		{
			var request = WebRequest.Create($"{API_ADDRESS}&m_info2={characterName}") as HttpWebRequest;
			var response = (await request.GetResponseAsync()) as HttpWebResponse;
			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				var responseText = sr.ReadToEnd();

				var begin = responseText.IndexOf("<tbody>") + "<tbody>".Length;
				var end = responseText.IndexOf("</tbody>", begin);
				if (begin < 0 || end < 0)
					return null;

				var tbodyText = responseText.Substring(begin, end - begin).Replace("\r\n", "").Replace(" ", "");
				if (tbodyText.Contains("\"no-result\""))
					return null;

				begin = tbodyText.IndexOf("<tr><td>") + "<tr><td>".Length;
				end = tbodyText.IndexOf("</td>", begin);

				var rankingText = tbodyText.Substring(begin, end - begin);
				return rankingText;
			}
		}
	}
}
