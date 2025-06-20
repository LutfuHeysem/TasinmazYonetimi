using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Durumlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DurumAdi = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Durumlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Iller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlAdi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IslemTipleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IslemAdi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IslemTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "text", nullable: false),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.UniqueConstraint("AK_Kullanicilar_Email", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Ilceler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlceAdi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IlId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilceler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilceler_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciAdi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KullaniciId = table.Column<int>(type: "integer", nullable: false),
                    DurumId = table.Column<int>(type: "integer", nullable: false),
                    IslemTipId = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TarihSaat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KullaniciIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loglar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loglar_Durumlar_DurumId",
                        column: x => x.DurumId,
                        principalTable: "Durumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loglar_IslemTipleri_IslemTipId",
                        column: x => x.IslemTipId,
                        principalTable: "IslemTipleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loglar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mahalleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MahalleAdi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IlceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahalleler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mahalleler_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasinmazlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MahalleId = table.Column<int>(type: "integer", nullable: false),
                    Ada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Parsel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nitelik = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KoordinatBilgileri = table.Column<string>(type: "text", nullable: false),
                    KullaniciId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasinmazlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasinmazlar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasinmazlar_Mahalleler_MahalleId",
                        column: x => x.MahalleId,
                        principalTable: "Mahalleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ilce_IlceAdi_IlId",
                table: "Ilceler",
                columns: new[] { "IlceAdi", "IlId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ilceler_IlId",
                table: "Ilceler",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_Il_IlAdi",
                table: "Iller",
                column: "IlAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loglar_DurumId",
                table: "Loglar",
                column: "DurumId");

            migrationBuilder.CreateIndex(
                name: "IX_Loglar_IslemTipId",
                table: "Loglar",
                column: "IslemTipId");

            migrationBuilder.CreateIndex(
                name: "IX_Loglar_KullaniciId",
                table: "Loglar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Mahalle_MahalleAdi_IlceId",
                table: "Mahalleler",
                columns: new[] { "MahalleAdi", "IlceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mahalleler_IlceId",
                table: "Mahalleler",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasinmazlar_Ada_Parsel_Nitelik_KoordinatBilgileri_MahalleId~",
                table: "Tasinmazlar",
                columns: new[] { "Ada", "Parsel", "Nitelik", "KoordinatBilgileri", "MahalleId", "KullaniciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasinmazlar_KullaniciId",
                table: "Tasinmazlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasinmazlar_MahalleId",
                table: "Tasinmazlar",
                column: "MahalleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loglar");

            migrationBuilder.DropTable(
                name: "Tasinmazlar");

            migrationBuilder.DropTable(
                name: "Durumlar");

            migrationBuilder.DropTable(
                name: "IslemTipleri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Mahalleler");

            migrationBuilder.DropTable(
                name: "Ilceler");

            migrationBuilder.DropTable(
                name: "Iller");
        }
    }
}
