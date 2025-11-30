using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration; // Để đọc Web.config
using System.Web.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYDICHVUDULICH.Admin.Controllers
{
    // Controller này kế thừa từ Controller gốc của MVC
    public class BaseController : Controller
    {
        // Khởi tạo HttpClient dùng chung
        protected readonly HttpClient _client;

        public BaseController()
        {
            _client = new HttpClient();
            // Lấy địa chỉ từ Web.config
            string baseUrl = WebConfigurationManager.AppSettings["ApiBaseUrl"];
            _client.BaseAddress = new Uri(baseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // HÀM DÙNG CHUNG 1: Lấy dữ liệu (GET)
        // T là kiểu dữ liệu bạn muốn nhận về (Ví dụ: List<TourViewModel>, TourViewModel...)
        protected T GetFromApi<T>(string uri)
        {
            var response = _client.GetAsync(uri).Result; // Gọi API
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<T>().Result; // Đọc kết quả
            }
            return default(T); // Nếu lỗi trả về null
        }

        // HÀM DÙNG CHUNG 2: Gửi dữ liệu (POST)
        protected bool PostToApi<T>(string uri, T data)
        {
            var response = _client.PostAsJsonAsync(uri, data).Result;
            return response.IsSuccessStatusCode;
        }

        // HÀM DÙNG CHUNG 3: Cập nhật dữ liệu (PUT)
        protected bool PutToApi<T>(string uri, T data)
        {
            var response = _client.PutAsJsonAsync(uri, data).Result;
            return response.IsSuccessStatusCode;
        }
        protected bool DeleteFromApi(string uri)
        {
            try
            {
                var response = _client.DeleteAsync(uri).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}