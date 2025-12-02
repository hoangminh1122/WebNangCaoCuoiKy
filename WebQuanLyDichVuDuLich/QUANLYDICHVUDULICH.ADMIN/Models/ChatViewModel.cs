using System;

namespace QUANLYDICHVUDULICH.ADMIN.Models
{
    // Model hứng danh sách người chat
    public class ChatUserViewModel
    {
        public int MaND { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
    }

    // Model hứng nội dung tin nhắn
    public class ChatMessageViewModel
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public string Time { get; set; }
    }
}