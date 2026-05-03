using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitVolunteersDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Volunteers");

            migrationBuilder.CreateTable(
                name: "volunteers",
                schema: "Volunteers",
                columns: table => new
                {
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    experience = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    social_networks = table.Column<string>(type: "jsonb", nullable: false),
                    help_requisites = table.Column<string>(type: "jsonb", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    date_delete = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_volunteers", x => x.volunteer_id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                schema: "Volunteers",
                columns: table => new
                {
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    health_info = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<decimal>(type: "numeric", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    is_sterilized = table.Column<bool>(type: "boolean", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_vaccined = table.Column<bool>(type: "boolean", nullable: false),
                    pet_help_status = table.Column<int>(type: "integer", nullable: false),
                    help_requisites = table.Column<string>(type: "jsonb", nullable: false),
                    created_otc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    date_delete = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    photos = table.Column<string>(type: "jsonb", nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    house = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_pets", x => x.pet_id);
                    table.ForeignKey(
                        name: "fK_pets_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalSchema: "Volunteers",
                        principalTable: "volunteers",
                        principalColumn: "volunteer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "iX_pets_volunteer_id",
                schema: "Volunteers",
                table: "pets",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pets",
                schema: "Volunteers");

            migrationBuilder.DropTable(
                name: "volunteers",
                schema: "Volunteers");
        }
    }
}
