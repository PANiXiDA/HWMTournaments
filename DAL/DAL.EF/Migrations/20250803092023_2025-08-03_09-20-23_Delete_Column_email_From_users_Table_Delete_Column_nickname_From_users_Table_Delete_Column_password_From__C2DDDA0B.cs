using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.EF.Migrations;

/// <inheritdoc />
public partial class _20250803_092023_Delete_Column_email_From_users_Table_Delete_Column_nickname_From_users_Table_Delete_Column_password_From__C2DDDA0B : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "email",
            table: "users");

        migrationBuilder.DropColumn(
            name: "nickname",
            table: "users");

        migrationBuilder.DropColumn(
            name: "password",
            table: "users");

        migrationBuilder.EnsureSchema(
            name: "identity_service");

        migrationBuilder.RenameTable(
            name: "data_protection_keys",
            newName: "data_protection_keys",
            newSchema: "identity_service");

        migrationBuilder.AddColumn<int>(
            name: "application_user_id",
            table: "users",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "application_users",
            schema: "identity_service",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: true),
                security_stamp = table.Column<string>(type: "text", nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                phone_number = table.Column<string>(type: "text", nullable: true),
                phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                access_failed_count = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_application_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "identity_roles",
            schema: "identity_service",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_identity_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "identity_user_claims",
            schema: "identity_service",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_id = table.Column<int>(type: "integer", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_identity_user_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_identity_user_claims_application_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "identity_service",
                    principalTable: "application_users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "identity_user_logins",
            schema: "identity_service",
            columns: table => new
            {
                login_provider = table.Column<string>(type: "text", nullable: false),
                provider_key = table.Column<string>(type: "text", nullable: false),
                provider_display_name = table.Column<string>(type: "text", nullable: true),
                user_id = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_identity_user_logins", x => new { x.login_provider, x.provider_key });
                table.ForeignKey(
                    name: "fk_identity_user_logins_application_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "identity_service",
                    principalTable: "application_users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "identity_user_tokens",
            schema: "identity_service",
            columns: table => new
            {
                user_id = table.Column<int>(type: "integer", nullable: false),
                login_provider = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_identity_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                table.ForeignKey(
                    name: "fk_identity_user_tokens_application_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "identity_service",
                    principalTable: "application_users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "application_user_role_scopes",
            schema: "identity_service",
            columns: table => new
            {
                application_user_id = table.Column<int>(type: "integer", nullable: false),
                role_id = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_application_user_role_scopes", x => new { x.application_user_id, x.role_id });
                table.ForeignKey(
                    name: "fk_application_user_role_scopes_asp_net_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "identity_service",
                    principalTable: "identity_roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_application_user_role_scopes_asp_net_users_user_id",
                    column: x => x.application_user_id,
                    principalSchema: "identity_service",
                    principalTable: "application_users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "identity_role_claims",
            schema: "identity_service",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                role_id = table.Column<int>(type: "integer", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_identity_role_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_identity_role_claims_identity_roles_role_id",
                    column: x => x.role_id,
                    principalSchema: "identity_service",
                    principalTable: "identity_roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_users_application_user_id",
            table: "users",
            column: "application_user_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_application_user_role_scopes_role_id",
            schema: "identity_service",
            table: "application_user_role_scopes",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            schema: "identity_service",
            table: "application_users",
            column: "normalized_email");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: "identity_service",
            table: "application_users",
            column: "normalized_user_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_identity_role_claims_role_id",
            schema: "identity_service",
            table: "identity_role_claims",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: "identity_service",
            table: "identity_roles",
            column: "normalized_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_identity_user_claims_user_id",
            schema: "identity_service",
            table: "identity_user_claims",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_identity_user_logins_user_id",
            schema: "identity_service",
            table: "identity_user_logins",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "fk_users_asp_net_users_application_user_id",
            table: "users",
            column: "application_user_id",
            principalSchema: "identity_service",
            principalTable: "application_users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_users_asp_net_users_application_user_id",
            table: "users");

        migrationBuilder.DropTable(
            name: "application_user_role_scopes",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "identity_role_claims",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "identity_user_claims",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "identity_user_logins",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "identity_user_tokens",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "identity_roles",
            schema: "identity_service");

        migrationBuilder.DropTable(
            name: "application_users",
            schema: "identity_service");

        migrationBuilder.DropIndex(
            name: "ix_users_application_user_id",
            table: "users");

        migrationBuilder.DropColumn(
            name: "application_user_id",
            table: "users");

        migrationBuilder.RenameTable(
            name: "data_protection_keys",
            schema: "identity_service",
            newName: "data_protection_keys");

        migrationBuilder.AddColumn<string>(
            name: "email",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "nickname",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "password",
            table: "users",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
