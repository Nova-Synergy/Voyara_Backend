namespace Voyara.Application.DTOs;

/// <summary>Read model of a local rule translation.</summary>
public sealed record LocalRuleTranslationResponse(string LanguageCode, string Title, string Content);
