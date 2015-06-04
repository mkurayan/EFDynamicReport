using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using SchoolReport.DB;

namespace SchoolReport
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Database.SetInitializer(new SchoolDbInitializer());
            using (var context = new SchoolDbContext())
            {
                context.Database.Initialize(true);
            }
        }
    }
}