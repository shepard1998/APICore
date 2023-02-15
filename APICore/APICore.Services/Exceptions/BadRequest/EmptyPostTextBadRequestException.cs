﻿using Microsoft.Extensions.Localization;


namespace APICore.Services.Exceptions
{
    public class EmptyPostTextBadRequestException : BaseBadRequestException
    {
        public EmptyPostTextBadRequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400012;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}