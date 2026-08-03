using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MezunBurada.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: false),
                    LongDescription = table.Column<string>(type: "text", nullable: false),
                    DegreeType = table.Column<int>(type: "integer", nullable: false),
                    ExamType = table.Column<int>(type: "integer", nullable: false),
                    EducationDurationYears = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MainTopics = table.Column<string>(type: "text", nullable: false),
                    SuitableFor = table.Column<string>(type: "text", nullable: false),
                    ChallengingFor = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentSkills",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentSkills", x => new { x.DepartmentId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_DepartmentSkills_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Faqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faqs_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestQuestions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false),
                    RequiredSkills = table.Column<string>(type: "text", nullable: false),
                    RecommendedTechnologies = table.Column<string>(type: "text", nullable: false),
                    ExamplePositions = table.Column<string>(type: "text", nullable: false),
                    StartingTopics = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubFields_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CareerPaths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    SubFieldId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerPaths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareerPaths_SubFields_SubFieldId",
                        column: x => x.SubFieldId,
                        principalTable: "SubFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestQuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    InterestQuestionId = table.Column<int>(type: "integer", nullable: false),
                    MapsToSubFieldId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestQuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestQuestionOptions_InterestQuestions_InterestQuestionId",
                        column: x => x.InterestQuestionId,
                        principalTable: "InterestQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestQuestionOptions_SubFields_MapsToSubFieldId",
                        column: x => x.MapsToSubFieldId,
                        principalTable: "SubFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LevelQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    SubFieldId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelQuestions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LevelQuestions_SubFields_SubFieldId",
                        column: x => x.SubFieldId,
                        principalTable: "SubFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CareerPathSkills",
                columns: table => new
                {
                    CareerPathId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerPathSkills", x => new { x.CareerPathId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_CareerPathSkills_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareerPathSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    RequiredSkills = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    CareerPathId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRoles_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    EstimatedDuration = table.Column<string>(type: "text", nullable: false),
                    Technologies = table.Column<string>(type: "text", nullable: false),
                    Steps = table.Column<string>(type: "text", nullable: false),
                    CareerPathId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roadmaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    CareerPathId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roadmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roadmaps_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LevelQuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    LevelQuestionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelQuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelQuestionOptions_LevelQuestions_LevelQuestionId",
                        column: x => x.LevelQuestionId,
                        principalTable: "LevelQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EstimatedDuration = table.Column<string>(type: "text", nullable: false),
                    RoadmapId = table.Column<int>(type: "integer", nullable: false),
                    PrerequisiteStepId = table.Column<int>(type: "integer", nullable: true),
                    SkillId = table.Column<int>(type: "integer", nullable: true),
                    ResourceId = table.Column<int>(type: "integer", nullable: true),
                    SuggestedProjectId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_Projects_SuggestedProjectId",
                        column: x => x.SuggestedProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_RoadmapSteps_PrerequisiteStepId",
                        column: x => x.PrerequisiteStepId,
                        principalTable: "RoadmapSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_Roadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "Roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareerPaths_SubFieldId",
                table: "CareerPaths",
                column: "SubFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CareerPathSkills_SkillId",
                table: "CareerPathSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CategoryId",
                table: "Departments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Slug",
                table: "Departments",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSkills_SkillId",
                table: "DepartmentSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Faqs_DepartmentId",
                table: "Faqs",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestQuestionOptions_InterestQuestionId",
                table: "InterestQuestionOptions",
                column: "InterestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestQuestionOptions_MapsToSubFieldId",
                table: "InterestQuestionOptions",
                column: "MapsToSubFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestQuestions_DepartmentId",
                table: "InterestQuestions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRoles_CareerPathId",
                table: "JobRoles",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelQuestionOptions_LevelQuestionId",
                table: "LevelQuestionOptions",
                column: "LevelQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelQuestions_SkillId",
                table: "LevelQuestions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelQuestions_SubFieldId",
                table: "LevelQuestions",
                column: "SubFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CareerPathId",
                table: "Projects",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_CareerPathId",
                table: "Roadmaps",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_PrerequisiteStepId",
                table: "RoadmapSteps",
                column: "PrerequisiteStepId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_ResourceId",
                table: "RoadmapSteps",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_RoadmapId",
                table: "RoadmapSteps",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_SkillId",
                table: "RoadmapSteps",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_SuggestedProjectId",
                table: "RoadmapSteps",
                column: "SuggestedProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubFields_DepartmentId",
                table: "SubFields",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubFields_Slug",
                table: "SubFields",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareerPathSkills");

            migrationBuilder.DropTable(
                name: "DepartmentSkills");

            migrationBuilder.DropTable(
                name: "Faqs");

            migrationBuilder.DropTable(
                name: "InterestQuestionOptions");

            migrationBuilder.DropTable(
                name: "JobRoles");

            migrationBuilder.DropTable(
                name: "LevelQuestionOptions");

            migrationBuilder.DropTable(
                name: "RoadmapSteps");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "InterestQuestions");

            migrationBuilder.DropTable(
                name: "LevelQuestions");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Roadmaps");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "CareerPaths");

            migrationBuilder.DropTable(
                name: "SubFields");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
