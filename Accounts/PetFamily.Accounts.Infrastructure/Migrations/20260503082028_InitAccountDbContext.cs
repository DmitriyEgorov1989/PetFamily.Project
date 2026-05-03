using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PetFamily.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitAccountDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "accounts");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    passwordHash = table.Column<string>(type: "text", nullable: true),
                    securityStamp = table.Column<string>(type: "text", nullable: true),
                    concurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    phoneNumber = table.Column<string>(type: "text", nullable: true),
                    phoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleId = table.Column<Guid>(type: "uuid", nullable: false),
                    claimType = table.Column<string>(type: "text", nullable: true),
                    claimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fK_role_claims_roles_roleId",
                        column: x => x.roleId,
                        principalSchema: "accounts",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    claimType = table.Column<string>(type: "text", nullable: true),
                    claimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fK_user_claims_users_userId",
                        column: x => x.userId,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                schema: "accounts",
                columns: table => new
                {
                    loginProvider = table.Column<string>(type: "text", nullable: false),
                    providerKey = table.Column<string>(type: "text", nullable: false),
                    providerDisplayName = table.Column<string>(type: "text", nullable: true),
                    userId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_user_logins", x => new { x.loginProvider, x.providerKey });
                    table.ForeignKey(
                        name: "fK_user_logins_users_userId",
                        column: x => x.userId,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "accounts",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    roleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_user_roles", x => new { x.userId, x.roleId });
                    table.ForeignKey(
                        name: "fK_user_roles_roles_roleId",
                        column: x => x.roleId,
                        principalSchema: "accounts",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_user_roles_users_userId",
                        column: x => x.userId,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                schema: "accounts",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    loginProvider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_user_tokens", x => new { x.userId, x.loginProvider, x.name });
                    table.ForeignKey(
                        name: "fK_user_tokens_users_userId",
                        column: x => x.userId,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "iX_role_claims_roleId",
                schema: "accounts",
                table: "role_claims",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "accounts",
                table: "roles",
                column: "normalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "iX_user_claims_userId",
                schema: "accounts",
                table: "user_claims",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "iX_user_logins_userId",
                schema: "accounts",
                table: "user_logins",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "iX_user_roles_roleId",
                schema: "accounts",
                table: "user_roles",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "accounts",
                table: "users",
                column: "normalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "accounts",
                table: "users",
                column: "normalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "user_claims",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "user_logins",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "user_tokens",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "users",
                schema: "accounts");
        }
    }
}
