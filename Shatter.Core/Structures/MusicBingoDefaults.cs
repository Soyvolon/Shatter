using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shatter.Core.Structures
{
	public struct MusicBingoDefaults
	{
		[JsonProperty("introduction")]
		public Uri Introduction { get; set; }
		[JsonProperty("winner")]
		public Uri Winner { get; set; }
		[JsonProperty("no_winner")]
		public Uri NoWinner { get; set; }
	}
}
