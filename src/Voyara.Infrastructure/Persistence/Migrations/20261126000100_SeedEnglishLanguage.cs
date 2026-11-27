using Voyara.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voyara.Infrastructure.Persistence.Migrations;

/// <summary>
/// Adds the English fallback language through EF Core migration operations.
/// The configured provider generates the appropriate SQL; this migration contains no provider-specific SQL.
/// </summary>
[DbContext(typeof(VoyaraDbContext))]
[Migration("20261126000100_SeedEnglishLanguage")]
public sealed class SeedEnglishLanguage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Languages",
            columns: new[] { "Code", "Name", "IsRtl" },
            values: new object[] { "en", "English", false });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // English is a required application fallback. It is intentionally retained on rollback.
    }
}
