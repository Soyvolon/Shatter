
using Microsoft.EntityFrameworkCore;

namespace Shatter.Core.Structures
{
	[Owned]
    public class MemberlogMessage
    {
        public string? Message { get; set; }
        public bool IsEmbed { get; set; }
        public bool IsImage { get; set; }
        public string? ImageUrl { get; set; }

        public MemberlogMessage()
        {
			this.Message = null;
			this.IsEmbed = false;
			this.IsImage = false;
			this.ImageUrl = null;
        }

        public MemberlogMessage(string? msg, bool isEmbed, bool hasImage, string? imageUrl)
        {
			this.Message = msg;
			this.IsEmbed = isEmbed;
			this.IsImage = hasImage;
			this.ImageUrl = imageUrl;
        }
    }
}
