using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class SafetyGuide : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;

    public List<string> Steps { get; private set; } = new();

    private SafetyGuide()
    {
    }

    private SafetyGuide(string title, IEnumerable<string> steps)
    {
        Title = title;
        Steps = steps.ToList();
    }

    public static SafetyGuide Create(string title, IEnumerable<string> steps)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(steps);

        var validatedSteps = steps
            .Select(step =>
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(step);
                return step.Trim();
            })
            .ToList();

        if (validatedSteps.Count == 0)
        {
            throw new ArgumentException(
                "A safety guide must contain at least one step.",
                nameof(steps));
        }

        return new SafetyGuide(title.Trim(), validatedSteps);
    }

    public void Update(string title, IEnumerable<string> steps)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(steps);

        var validatedSteps = steps
            .Select(step =>
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(step);
                return step.Trim();
            })
            .ToList();

        if (validatedSteps.Count == 0)
        {
            throw new ArgumentException(
                "A safety guide must contain at least one step.",
                nameof(steps));
        }

        Title = title.Trim();
        Steps = validatedSteps;
    }
}