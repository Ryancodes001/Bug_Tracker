using System.Web.Mvc;

namespace Bug_tracker.Areas.test
{
    public class testAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "test";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "test_default",
                "test/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}