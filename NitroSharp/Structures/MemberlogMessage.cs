using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace NitroSharp.Structures
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
            this.Message = Message;
            this.IsEmbed = isEmbed;
            this.IsImage = hasImage;
            this.ImageUrl = imageUrl;
        }
    }
}
