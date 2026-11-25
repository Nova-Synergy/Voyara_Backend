using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Infrastructure.Persistence.Migrations;

/// <summary>
/// Adds the English fallback language to databases that were created before it was configured with HasData.
/// The migration uses provider-neutral migration operations and does not change the configured database provider.
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
        migrationBuilder.DeleteData(
            table: "Languages",
            keyColumn: "Code",
            keyValue: "en");
    }
}
