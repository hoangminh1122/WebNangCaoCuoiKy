using QUANLYDICHVUDULICH.Admin.Controllers;
using QUANLYDICHVUDULICH.ADMIN.Models; // Nhớ using Models
using System.Collections.Generic;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN.Controllers
{
    public class ChatController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        // 1. Lấy danh sách User (Dùng ChatUserViewModel)
        public ActionResult GetUsers()
        {
            // Gọi API và ép kiểu về List<ChatUserViewModel>
            var users = GetFromApi<List<ChatUserViewModel>>("api/adminchat/users");

            // Nếu null thì khởi tạo list rỗng để không lỗi JS
            if (users == null) users = new List<ChatUserViewModel>();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        // 2. Lấy lịch sử chat (Dùng ChatMessageViewModel)
        public ActionResult GetHistory(int id)
        {
            var history = GetFromApi<List<ChatMessageViewModel>>("api/adminchat/history/" + id);

            if (history == null) history = new List<ChatMessageViewModel>();

            return Json(history, JsonRequestBehavior.AllowGet);
        }

        // 3. Gửi tin nhắn (Giữ nguyên)
        [HttpPost]
        public ActionResult SendMessage(int maND, string message)
        {
            var data = new { MaND = maND, Message = message };
            if (PostToApi("api/adminchat/send", data))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}