using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.ExpireOverdueGuideRequests;

/// <summary>
/// Expires one batch of pending requests whose <c>ExpiresAt</c> has passed. Returns the number expired.
/// Invoked by the background worker; has no user context and is not exposed over HTTP.
/// </summary>
public sealed record ExpireOverdueGuideRequestsCommand(int BatchSize = GuideRequestPolicy.ExpirationBatchSize)
    : ICommand<int>;
