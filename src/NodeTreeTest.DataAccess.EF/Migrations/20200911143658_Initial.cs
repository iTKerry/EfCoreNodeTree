using Microsoft.EntityFrameworkCore.Migrations;

namespace NodeTreeTest.DataAccess.EF.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Dbo");

            migrationBuilder.CreateTable(
                name: "Application",
                schema: "Dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                schema: "Dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Query = table.Column<string>(nullable: false),
                    QueryParameter = table.Column<string>(nullable: true),
                    DocumentType = table.Column<short>(nullable: true),
                    NodeType = table.Column<short>(nullable: false),
                    ApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Dbo",
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenNode",
                schema: "Dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: false),
                    ChildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenNode_Token_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "Dbo",
                        principalTable: "Token",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TokenNode_Token_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Dbo",
                        principalTable: "Token",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Token_ApplicationId",
                schema: "Dbo",
                table: "Token",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenNode_ChildId",
                schema: "Dbo",
                table: "TokenNode",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenNode_ParentId",
                schema: "Dbo",
                table: "TokenNode",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenNode",
                schema: "Dbo");

            migrationBuilder.DropTable(
                name: "Token",
                schema: "Dbo");

            migrationBuilder.DropTable(
                name: "Application",
                schema: "Dbo");
        }
    }
}
