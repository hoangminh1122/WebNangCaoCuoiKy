using System;

namespace QUANLYDICHVUDULICH.API.Models
{
    // DTO dùng để trả về danh sách người chat
    public class ChatUserDTO
    {
        public int MaND { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
    }

    // DTO dùng để trả về nội dung tin nhắn
    public class ChatMessageDTO
    {
        public string Content { get; set; }
        public string Sender { get; set; } // 'User' hoặc 'Admin'
        public string Time { get; set; }
    }

    // DTO dùng để nhận dữ liệu khi gửi tin nhắn mới
    // ĐÃ SỬA: Đổi tên từ ChatMessageRequest thành SendMessageRequest cho khớp với Controller
    public class SendMessageRequest
    {
        public int MaND { get; set; }
        public string Message { get; set; }
    }
}