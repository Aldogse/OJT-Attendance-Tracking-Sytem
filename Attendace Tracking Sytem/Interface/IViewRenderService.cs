namespace Attendace_Tracking_Sytem.Interface
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string ViewName,object model);
    }
}
