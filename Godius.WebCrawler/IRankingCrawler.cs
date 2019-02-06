using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godius.WebCrawler
{
	public interface IRankingCrawler
	{
		string GetCharacterRanking(string characterName);

		Task<string> GetCharacterRankingAsync(string characterName);
	}
}
