using System;
using System.Configuration; // Để đọc Web.config
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace QUANLYDICHVUDULICH.API.Controllers
{
    // Controller này kế thừa ApiController
    // Các Controller con sẽ kế thừa lại Controller này
    public class BaseApiController : ApiController
    {
        // 1. Hàm lấy chuỗi kết nối từ Web.config
        protected string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["WebDuLichConn"].ConnectionString;
        }

        // 2. Hàm trả về một kết nối SQL đang mở (Dùng cho các lệnh phức tạp)
        protected SqlConnection GetOpenConnection()
        {
            SqlConnection con = new SqlConnection(GetConnectionString());
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            return con;
        }

        // 3. Hàm tiện ích: Thực thi câu lệnh SELECT và trả về DataTable (Dùng cho việc lấy danh sách)
        protected DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // 4. Hàm tiện ích: Thực thi INSERT/UPDATE/DELETE (Trả về số dòng bị ảnh hưởng)
        protected int ExecuteNonQuery(string query, SqlParameter[] parameters = null, bool isStoredProcedure = false)
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (isStoredProcedure)
                        cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}