using System.Web;
using System.Web.Mvc;

namespace QUANLYDICHVUDULICH.ADMIN
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
