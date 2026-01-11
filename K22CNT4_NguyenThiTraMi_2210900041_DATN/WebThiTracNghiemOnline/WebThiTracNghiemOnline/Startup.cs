using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebThiTracNghiemOnline.Startup))]
namespace WebThiTracNghiemOnline
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình OWIN middleware, chẳng hạn như Hangfire Dashboard
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}