using Microsoft.Extensions.Localization;

namespace APICore.Services.Exceptions
{
    public class PostNotFoundException : BaseNotFoundException
    {
        public PostNotFoundException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 404004;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}