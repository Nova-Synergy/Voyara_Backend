using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Notifications;

/// <summary>Expected business errors of the <see cref="NotificationTemplate"/> entity.</summary>
public static class NotificationTemplateErrors
{
    /// <summary>A placeholder in the template has no matching value, so rendering would leave raw text behind.</summary>
    public static Error MissingValue(string placeholder) =>
        Error.Validation(
            "NotificationTemplate.MissingValue",
            $"No value was supplied for the placeholder '{placeholder}'.");
}