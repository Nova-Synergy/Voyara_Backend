# Voyara.Domain Layer Audit Report

## A. Blockers (Must fix before starting Application layer)

| Item | Evidence | Suggested fix (one line) |
|---|---|---|
| Build Failure (CS0050) | `Journey.cs:192` (`Journey.Validate()` returns `internal class ValidationResult` in `ValidationResult.cs:9`) | Change `ValidationResult` to `public` with required members or return `Result` from SharedKernel. |
| Inconsistent `CityId` Type | `City.cs:9` (`BaseEntity.Id` is `Guid`, `BaseEntity.cs:9`) & `Destination.cs:58` (`Guid`), but `GuideProfile.cs:57` (`int`), `CityTransport.cs:36` (`int`), `JourneyItem.cs:39` (`int?`), `RuleScope.cs:37` (`int`) | Standardize `CityId` across all entities, value objects, and repos to `Guid`. |
| `User` is Not an `AggregateRoot` | `User.cs:9` inherits `AuditableEntity`, not `AggregateRoot`, lacking domain event support | Change `User` inheritance to `AggregateRoot`. |
| Empty Core Repository Interfaces | `IUserRepository.cs:9`, `IDestinationRepository.cs:10`, `IGuideProfileRepository.cs:6`, `IGuideRequestRepository.cs:6`, `ICategoryRepository.cs:9` all have 0 members (EMPTY) | Define required query and persistence methods on aggregate root repository interfaces. |
| Missing Repository Interfaces | `ITripRepository`, `IJourneyRepository`, `IReviewRepository`, `IFavoriteRepository`, `IPlaceSuggestionRepository`, `IReportRepository`, `IConversationRepository` are MISSING (`Repositories/*/.gitkeep`) | Create missing repository interfaces for corresponding aggregate roots. |
| Duplicate `LocalRule` Entities | `CultureandRules/LocalRule.cs:23` (`AggregateRoot`) vs `LocalInfo/LocalRule.cs:7` (`AuditableEntity`) both exist | Delete `LocalInfo/LocalRule.cs` and `LocalInfo/LocalRuleTranslation.cs` in favor of `CultureandRules`. |
| Unrestricted Status Transitions | `Trip.ChangeStatus` (`Trip.cs:169`) and `Journey.ChangeStatus` (`Journey.cs:233`) allow arbitrary transitions (`Status = status;`) | Replace raw setters with explicit transition methods enforcing valid lifecycle progression. |
| Public Mutable Collections | `GuideRequest.cs:102` (`public ICollection<Destination> Destinations`), `LocalInfo/LocalRule.cs:21` (`Translations`), `SafetyGuide.cs:9` (`Steps`) | Expose as `IReadOnlyCollection<T>` backed by private lists. |
| Child Entity Modification Bypassing Root | `TripShare.cs:58,67` (`Create`, `Revoke`), `JourneyDay.cs:49,70` (`Create`, `AddItem`), `JourneyItem.cs:111,134` (`Create`, `MarkVisited`) have public mutators | Make child constructors and mutators `internal` so changes only flow through the aggregate root. |
| Empty CostEstimation Subdomain | `CostEstimate.cs:9`, `CostRule.cs:9`, `CostCategory.cs:9`, `CostLine.cs:9` are empty classes; `Journey.cs:60` references `CostEstimate` | Implement `CostEstimate` entities/value objects or decouple until cost estimation is modeled. |

## B. Non-Blockers (Can wait)

- **Zero Domain Events on 8 Aggregates**: `Trip.cs`, `Journey.cs`, `Conversation.cs`, `GuideProfile.cs`, `GuideRequest.cs`, `User.cs`, `CityTransport.cs`, `CultureandRules/LocalRule.cs` raise 0 domain events.
- **Audit Field Setters Public**: `AuditableEntity.cs:9-21` (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted` all have `{ get; set; }`).
- **Redundant Child Entity Repositories**: `IGuideSessionRepository.cs:6`, `IGuideReviewRepository.cs:6`, `IChatThreadRepository.cs:6`, `IChatMessageRepository.cs:6`, `IGuideAvailabilityRepository.cs:6`, `IGuideExpertiseRepository.cs:6`, `IGuideLanguageRepository.cs:6`, `IGuidePricingRepository.cs:6`, `IGuideTimeOffRepository.cs:6` exist despite being child entities of `GuideProfile`/`GuideRequest`.
- **System.Text.Json Dependency in Value Objects**: `AuditDetails.cs:1` and `NotificationPayload.cs:1` use `System.Text.Json` directly for JSON schema parsing.
- **Zero Domain Tests**: No test project for `Voyara.Domain` exists; test projects `Voyara.Application.UnitTests` and `Voyara.ArchitectureTests` have 0 test files (`Get-ChildItem tests -Filter *.cs` returned 0 tests).

## C. Verified OK

- **Pure Dependencies**: `Voyara.Domain.csproj:6-8` contains 0 `PackageReference` and only 1 `ProjectReference` (`Voyara.SharedKernel`).
- **No Infrastructure Leaks**: 0 hits for `using Microsoft.EntityFrameworkCore`, `using System.Data`, or `using Newtonsoft` across all Domain `.cs` files.
- **No Unimplemented Stubs/Hacks**: 0 hits for `NotImplementedException`, `TODO`, `FIXME`, or `HACK` across `src/Voyara.Domain`.
- **Verified Aggregate Invariants & Events**:
  - `Review.cs:12` validates transitions (`Approve` L127, `Reject` L149, `MarkAsReported` L171, `Hide` L188) and raises 5 events (`CommunityDomainEvents.cs:6-47`).
  - `PlaceSuggestion.cs:12` validates transitions (`Approve` L120, `Reject` L143, `LinkDestination` L162) and raises 4 events (`CommunityDomainEvents.cs:50-85`).
  - `Favorite.cs:20` validates status (`Remove` L86, `Restore` L102) and raises 2 events (`CommunityDomainEvents.cs:88-101`).
  - `Report.cs:24` validates transitions (`StartInvestigation` L134, `Resolve` L170, `RecordOutcome` L196, `Dismiss` L222) and raises 4 events (`ModerationDomainEvents.cs:10-45`).
  - `Destination.cs:150` validates status on `Publish` (L152) and raises `DestinationPublishedDomainEvent` (`DestinationPublishedDomainEvent.cs:6`).
- **Destination Stats Updaters**: `Destination.ApplyReviewStatistics` (`Destination.cs:204`) updates `AverageRating` (L211) & `ReviewCount` (L212); `Destination.ChangePopularity` (`Destination.cs:216`) updates `PopularityScore` (L219).
- **Active Repositories**: `ILocalRuleRepository.cs:6` (3 methods: `GetByIdAsync`, `ListByScopesAsync`, `Add`) and `ICityTransportRepository.cs:6` (3 methods: `GetByIdAsync`, `ExistsAsync`, `Add`) are COMPLETE.
- **GuideRequest/GuideSession Topology**: `GuideRequest.cs:11` is an independent `AggregateRoot`; `GuideSession.cs:13` is a child entity instantiated solely via `GuideRequest.Accept` (`GuideRequest.cs:184`), not a child of `GuideProfile`.

## D. UNVERIFIED Items

- **Password Verification**: UNVERIFIED in Domain (only `User.PasswordHash` property exists at `User.cs:29`; verification logic belongs to Application/Security services).
- **AI Planning Lineage**: UNVERIFIED in Domain (generation workflows for `Journey` and prompt tokens in `Conversation` belong to Application/Infrastructure).
- **Outbox Persistence & Event Dispatch**: UNVERIFIED (Domain models only invoke `RaiseDomainEvent`; database outbox interception is an Infrastructure concern).

## E. Use-Case Readiness Table

| Use Case | Aggregate Root | Domain Method(s) | Repository Interface | Event(s) | Status |
|---|---|---|---|---|---|
| Register / Login / Refresh token | `User` (`User.cs:9`, AuditableEntity) | `User.Create` (L69), `IssueRefreshToken` (L147), `FindActiveRefreshToken` (L158), `RevokeRefreshToken` (L163) | `IUserRepository` (EMPTY, 0 members) | MISSING | INCOMPLETE |
| Create Trip | `Trip` (`Trip.cs:15`) | `Trip.Create` (L108) | `ITripRepository` (MISSING) | MISSING | INCOMPLETE |
| Generate Journey | `Journey` (`Journey.cs:15`) | `Journey.Create` (L88), `AddItem` (L102) [Blocked: CS0050 in `Validate` L192] | `IJourneyRepository` (MISSING) | MISSING | INCOMPLETE |
| Browse Destinations | `Destination` (`Destination.cs:15`) | Read queries / `IsOpenAt` (L349) | `IDestinationRepository` (EMPTY, 0 members) | N/A (query) | INCOMPLETE |
| Submit Review | `Review` (`Review.cs:12`) | `Review.Create` (L64), `AddPhoto` (L97) | `IReviewRepository` (MISSING) | `ReviewSubmittedDomainEvent` (COMPLETE, L89) | INCOMPLETE |
| Add Favorite | `Favorite` (`Favorite.cs:20`) | `Favorite.Create` (L57), `Restore` (L104) | `IFavoriteRepository` (MISSING) | `FavoriteAddedDomainEvent` (COMPLETE, L74, 109) | INCOMPLETE |
| Suggest Place | `PlaceSuggestion` (`PlaceSuggestion.cs:12`) | `PlaceSuggestion.Create` (L61), `AddPhoto` (L93) | `IPlaceSuggestionRepository` (MISSING) | `PlaceSuggestionSubmittedDomainEvent` (COMPLETE, L86) | INCOMPLETE |
| Request Guide | `GuideRequest` (`GuideRequest.cs:11`) | `GuideRequest.Create` (L136) | `IGuideRequestRepository` (EMPTY, 0 members) | MISSING | INCOMPLETE |
| File Report | `Report` (`Report.cs:24`) | `Report.Create` (L81) | `IReportRepository` (MISSING) | `ReportFiledDomainEvent` (COMPLETE, L119) | INCOMPLETE |
| AI Conversation | `Conversation` (`Conversation.cs:15`) | `Conversation.Create` (L85), `AddMessage` (L95), `ApplyRequirements` (L107) | `IConversationRepository` (MISSING) | MISSING | INCOMPLETE |

