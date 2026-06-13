using Attendace_Tracking_Sytem.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Attendace_Tracking_Sytem.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        public async Task<string> RenderToStringAsync(string ViewName, object model)
        {
            var actionContext = GetActionContext();

            using var sw = new StringWriter();

            var viewResult = _razorViewEngine.FindView(actionContext,ViewName,false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"View {ViewName} was not found");
            }

            var viewDictionary = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary()
            )
            {
                Model = model,
            };

            var viewContext = new ViewContext(actionContext,viewResult.View,viewDictionary,new TempDataDictionary(
                actionContext.HttpContext,_tempDataProvider),sw,new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider,
            };

            return new ActionContext(
              httpContext,
              new RouteData(),
              new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );
          
        }
    }
}
