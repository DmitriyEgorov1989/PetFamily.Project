using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PetFamily.Infrastructure.Migrations.AccountDb;

/// <inheritdoc />
public partial class InitAccountDbContext : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "roles",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                normalizedName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                concurrencyStamp = table.Column<string>("text", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pK_roles", x => x.id); });

        migrationBuilder.CreateTable(
            "users",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                userName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                normalizedUserName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                email = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                normalizedEmail = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                emailConfirmed = table.Column<bool>("boolean", nullable: false),
                passwordHash = table.Column<string>("text", nullable: true),
                securityStamp = table.Column<string>("text", nullable: true),
                concurrencyStamp = table.Column<string>("text", nullable: true),
                phoneNumber = table.Column<string>("text", nullable: true),
                phoneNumberConfirmed = table.Column<bool>("boolean", nullable: false),
                twoFactorEnabled = table.Column<bool>("boolean", nullable: false),
                lockoutEnd = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                lockoutEnabled = table.Column<bool>("boolean", nullable: false),
                accessFailedCount = table.Column<int>("integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("pK_users", x => x.id); });

        migrationBuilder.CreateTable(
            "role_claims",
            table => new
            {
                id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                roleId = table.Column<Guid>("uuid", nullable: false),
                claimType = table.Column<string>("text", nullable: true),
                claimValue = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pK_role_claims", x => x.id);
                table.ForeignKey(
                    "fK_role_claims_roles_roleId",
                    x => x.roleId,
                    "roles",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "user_claims",
            table => new
            {
                id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                userId = table.Column<Guid>("uuid", nullable: false),
                claimType = table.Column<string>("text", nullable: true),
                claimValue = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pK_user_claims", x => x.id);
                table.ForeignKey(
                    "fK_user_claims_users_userId",
                    x => x.userId,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "user_logins",
            table => new
            {
                loginProvider = table.Column<string>("text", nullable: false),
                providerKey = table.Column<string>("text", nullable: false),
                providerDisplayName = table.Column<string>("text", nullable: true),
                userId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pK_user_logins", x => new { x.loginProvider, x.providerKey });
                table.ForeignKey(
                    "fK_user_logins_users_userId",
                    x => x.userId,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "user_roles",
            table => new
            {
                userId = table.Column<Guid>("uuid", nullable: false),
                roleId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pK_user_roles", x => new { x.userId, x.roleId });
                table.ForeignKey(
                    "fK_user_roles_roles_roleId",
                    x => x.roleId,
                    "roles",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fK_user_roles_users_userId",
                    x => x.userId,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "user_tokens",
            table => new
            {
                userId = table.Column<Guid>("uuid", nullable: false),
                loginProvider = table.Column<string>("text", nullable: false),
                name = table.Column<string>("text", nullable: false),
                value = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pK_user_tokens", x => new { x.userId, x.loginProvider, x.name });
                table.ForeignKey(
                    "fK_user_tokens_users_userId",
                    x => x.userId,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "iX_role_claims_roleId",
            "role_claims",
            "roleId");

        migrationBuilder.CreateIndex(
            "RoleNameIndex",
            "roles",
            "normalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            "iX_user_claims_userId",
            "user_claims",
            "userId");

        migrationBuilder.CreateIndex(
            "iX_user_logins_userId",
            "user_logins",
            "userId");

        migrationBuilder.CreateIndex(
            "iX_user_roles_roleId",
            "user_roles",
            "roleId");

        migrationBuilder.CreateIndex(
            "EmailIndex",
            "users",
            "normalizedEmail");

        migrationBuilder.CreateIndex(
            "UserNameIndex",
            "users",
            "normalizedUserName",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "role_claims");

        migrationBuilder.DropTable(
            "user_claims");

        migrationBuilder.DropTable(
            "user_logins");

        migrationBuilder.DropTable(
            "user_roles");

        migrationBuilder.DropTable(
            "user_tokens");

        migrationBuilder.DropTable(
            "roles");

        migrationBuilder.DropTable(
            "users");
    }
}