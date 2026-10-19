using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerSeyBurada.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialHerSeyBuradaDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eposta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SifreSifirlamaToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenGecerlilikTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kuponlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndirimTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumSepetTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IlkUyelikKuponuMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuponlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adresler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdresBasligi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sehir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ilce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcikAdres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KurumsalMi = table.Column<bool>(type: "bit", nullable: false),
                    FirmaAdı = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VergiNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adresler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Magazalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MagazaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VergiNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VergiDairesi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    KomisyonOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magazalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Magazalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeslimatAdresiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaturaAdresiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KargoTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IndirimTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SiparisDurumu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparisler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MagazaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KategoriId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarkaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PiyasaFiyati = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SatisFiyati = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Urunler_Magazalar_MagazaId",
                        column: x => x.MagazaId,
                        principalTable: "Magazalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunVaryantlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Renk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Beden = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barkod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StokMiktari = table.Column<int>(type: "int", nullable: false),
                    EkFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunVaryantlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunVaryantlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunYorumlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    YorumMetni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunYorumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunYorumlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrunYorumlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisDetaylari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunVaryantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MagazaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SatirDurumu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisDetaylari_Magazalar_MagazaId",
                        column: x => x.MagazaId,
                        principalTable: "Magazalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisDetaylari_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisDetaylari_UrunVaryantlari_UrunVaryantId",
                        column: x => x.UrunVaryantId,
                        principalTable: "UrunVaryantlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adresler_KullaniciId",
                table: "Adresler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Magazalar_KullaniciId",
                table: "Magazalar",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_MagazaId",
                table: "SiparisDetaylari",
                column: "MagazaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_SiparisId",
                table: "SiparisDetaylari",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_UrunVaryantId",
                table: "SiparisDetaylari",
                column: "UrunVaryantId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KullaniciId",
                table: "Siparisler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_MagazaId",
                table: "Urunler",
                column: "MagazaId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunVaryantlari_UrunId",
                table: "UrunVaryantlari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunYorumlari_KullaniciId",
                table: "UrunYorumlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_UrunYorumlari_UrunId",
                table: "UrunYorumlari",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresler");

            migrationBuilder.DropTable(
                name: "Kuponlar");

            migrationBuilder.DropTable(
                name: "SiparisDetaylari");

            migrationBuilder.DropTable(
                name: "UrunYorumlari");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "UrunVaryantlari");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Magazalar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
