using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers; // QUAN TRỌNG: Cần thêm thư viện này
using System.Web.Http;

namespace QUANLYDICHVUDULICH
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // --- BẮT ĐẦU SỬA: CẤU HÌNH JSON ---

            // 1. Xóa định dạng XML (Nguyên nhân gây lỗi Serialization)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // 2. Ép trình duyệt (Chrome/Edge) hiển thị JSON đẹp thay vì XML
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            // --- KẾT THÚC SỬA ---

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}