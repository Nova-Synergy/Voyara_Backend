using FluentValidation;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Application.Guides.Commands.UploadGuideDocument;

public sealed class UploadGuideDocumentCommandValidator : AbstractValidator<UploadGuideDocumentCommand>
{
    public UploadGuideDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(GuideDocument.MaxDocumentTypeLength);
        RuleFor(x => x.OriginalFileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(GuideDocument.MaxContentTypeLength);
        RuleFor(x => x.Length).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}
