using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Culture_Rules;

/// <summary>Domain errors raised by the <see cref="LocalRule"/> aggregate and its value objects.</summary>
public static class LocalRuleErrors
{
    /// <summary>The rule type is not a defined value.</summary>
    public static readonly Error InvalidType = Error.Validation(
        "LocalRule.InvalidType", "The rule type is not valid.");

    /// <summary>The scope is missing.</summary>
    public static readonly Error ScopeRequired = Error.Validation(
        "LocalRule.ScopeRequired", "The rule scope is required.");

    /// <summary>The scope identifier is not valid for its scope type.</summary>
    public static readonly Error InvalidScopeId = Error.Validation(
        "LocalRule.InvalidScopeId", "The scope identifier is not valid.");

    /// <summary>An official rule has no source.</summary>
    public static readonly Error OfficialRuleRequiresSource = Error.Validation(
        "LocalRule.OfficialRuleRequiresSource", "An official rule must state its source.");

    /// <summary>The source is too long.</summary>
    public static readonly Error SourceTooLong = Error.Validation(
        "LocalRule.SourceTooLong", "The source must not exceed 500 characters.");

    /// <summary>A rule needs at least one translation.</summary>
    public static readonly Error TranslationRequired = Error.Validation(
        "LocalRule.TranslationRequired", "A rule must have at least one translation.");

    /// <summary>A translation for the language already exists.</summary>
    public static readonly Error DuplicateTranslation = Error.Validation(
        "LocalRule.DuplicateTranslation", "A translation for this language already exists.");

    /// <summary>No translation exists for the language.</summary>
    public static readonly Error TranslationNotFound = Error.Validation(
        "LocalRule.TranslationNotFound", "No translation exists for this language.");

    /// <summary>The last translation cannot be removed.</summary>
    public static readonly Error CannotRemoveLastTranslation = Error.Validation(
        "LocalRule.CannotRemoveLastTranslation", "The last translation of a rule cannot be removed.");

    /// <summary>The language code is not valid.</summary>
    public static readonly Error InvalidLanguageCode = Error.Validation(
        "LocalRule.InvalidLanguageCode", "The language code is not valid (examples: ar, en, ar-EG).");

    /// <summary>The translation title is empty or too long.</summary>
    public static readonly Error InvalidTitle = Error.Validation(
        "LocalRule.InvalidTitle", "The title is required and must not exceed 200 characters.");

    /// <summary>The translation content is empty or too long.</summary>
    public static readonly Error InvalidContent = Error.Validation(
        "LocalRule.InvalidContent", "The content is required and must not exceed 4000 characters.");
}
