
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
            Message = null;
            IsEmbed = false;
            IsImage = false;
            ImageUrl = null;
        }

        public MemberlogMessage(string? msg, bool isEmbed, bool hasImage, string? imageUrl)
        {
            Message = Message;
            IsEmbed = isEmbed;
            IsImage = hasImage;
            ImageUrl = imageUrl;
        }
    }
}
