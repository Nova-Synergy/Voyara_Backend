using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Infrastructure.Persistence.Migrations;

/// <summary>
/// Ensures the English fallback language exists without inserting a duplicate when an environment
/// already contains the language row. Existing language metadata is never overwritten.
/// </summary>
[DbContext(typeof(VoyaraDbContext))]
[Migration("20261126000100_SeedEnglishLanguage")]
public sealed class SeedEnglishLanguage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT 1 FROM [Languages] WHERE [Code] = N'en') " +
                "INSERT INTO [Languages] ([Code], [Name], [IsRtl]) VALUES (N'en', N'English', 0);");
            return;
        }

        if (ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            migrationBuilder.Sql(
                "INSERT INTO \"Languages\" (\"Code\", \"Name\", \"IsRtl\") " +
                "VALUES ('en', 'English', FALSE) ON CONFLICT (\"Code\") DO NOTHING;");
            return;
        }

        throw new NotSupportedException($"The English seed migration does not support provider '{ActiveProvider}'.");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Intentionally left empty: English may have existed before this migration was applied.
        // Removing it would break the application's documented default-language fallback.
    }
}
