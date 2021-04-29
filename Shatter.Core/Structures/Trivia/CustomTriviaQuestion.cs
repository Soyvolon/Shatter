using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shatter.Core.Structures.Trivia
{
	public class CustomTriviaQuestion
	{
		[Key]
		public int Key { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
	}
}
