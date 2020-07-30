using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpenIddictServer.ViewModels.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}
