using System;

namespace APICore.Common.DTO.Response
{
    public class PostResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
    }
}