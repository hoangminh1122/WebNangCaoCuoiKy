using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using QUANLYDICHVUDULICH.API.Models;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    [RoutePrefix("api/adminchat")]
    public class AdminChatController : BaseApiController
    {
        // 1. Lấy danh sách những khách hàng đã từng nhắn tin
        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetChatUsers()
        {
            // SỬA: Dùng List<ChatUserDTO> thay vì List<object>
            List<ChatUserDTO> users = new List<ChatUserDTO>();
            try
            {
                string sql = @"
                    SELECT DISTINCT c.MaND, u.HoTen, u.Email 
                    FROM ChatLichSu c
                    JOIN NguoiDung u ON c.MaND = u.MaND
                    WHERE c.MaND IS NOT NULL";

                DataTable dt = ExecuteQuery(sql);

                foreach (DataRow row in dt.Rows)
                {
                    users.Add(new ChatUserDTO
                    {
                        MaND = Convert.ToInt32(row["MaND"]),
                        HoTen = row["HoTen"].ToString(),
                        Email = row["Email"].ToString()
                    });
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi lấy danh sách chat: " + ex.Message);
            }
        }

        // 2. Lấy lịch sử tin nhắn của 1 khách hàng
        [HttpGet]
        [Route("history/{id}")]
        public IHttpActionResult GetHistory(int id)
        {
            // SỬA: Dùng List<ChatMessageDTO> thay vì List<object>
            List<ChatMessageDTO> messages = new List<ChatMessageDTO>();
            try
            {
                string sql = "SELECT * FROM ChatLichSu WHERE MaND = @MaND ORDER BY ThoiGian ASC";
                SqlParameter[] param = { new SqlParameter("@MaND", id) };

                DataTable dt = ExecuteQuery(sql, param);

                foreach (DataRow row in dt.Rows)
                {
                    string noiDung = row["NoiDung"] != DBNull.Value ? row["NoiDung"].ToString() : "";
                    string traLoi = row["TraLoi"] != DBNull.Value ? row["TraLoi"].ToString() : "";
                    string loaiTin = row["LoaiTin"].ToString();
                    DateTime time = Convert.ToDateTime(row["ThoiGian"]);

                    // 1. Nếu là tin nhắn của Khách (User)
                    if (loaiTin == "User")
                    {
                        messages.Add(new ChatMessageDTO
                        {
                            Content = noiDung,
                            Sender = "User",
                            Time = time.ToString("HH:mm")
                        });

                        // Nếu có câu trả lời cũ
                        if (!string.IsNullOrEmpty(traLoi))
                        {
                            messages.Add(new ChatMessageDTO
                            {
                                Content = traLoi,
                                Sender = "Admin",
                                Time = time.ToString("HH:mm")
                            });
                        }
                    }
                    // 2. Nếu là tin nhắn của Admin
                    else
                    {
                        messages.Add(new ChatMessageDTO
                        {
                            Content = noiDung,
                            Sender = "Admin",
                            Time = time.ToString("HH:mm")
                        });
                    }
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi lấy lịch sử: " + ex.Message);
            }
        }

        // 3. Gửi tin nhắn mới (Admin trả lời)
        [HttpPost]
        [Route("send")]
        // SỬA: Nhận class SendMessageRequest thay vì dynamic
        public IHttpActionResult SendMessage([FromBody] SendMessageRequest data)
        {
            if (data == null) return BadRequest("Dữ liệu rỗng");

            try
            {
                string sql = @"INSERT INTO ChatLichSu (MaND, NoiDung, LoaiTin, ThoiGian, Kenh) 
                               VALUES (@MaND, @NoiDung, 'Admin', GETDATE(), 'Web')";

                SqlParameter[] param = {
                    new SqlParameter("@MaND", data.MaND),
                    new SqlParameter("@NoiDung", data.Message)
                };

                ExecuteNonQuery(sql, param, false);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi gửi tin: " + ex.Message);
            }
        }
    }
}