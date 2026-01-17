using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using UI.Models;

namespace UI.Services
{
    public static class InvoicePdfService
    {
        public static void Export(
            PaymentSummary summary,
            string filePath,
            string qrImagePath
        )
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
                    {
                        //  HEADER 
                        col.Item().Text("HÓA ĐƠN THANH TOÁN")
                            .FontSize(16).Bold().AlignCenter();

                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        col.Item().Text($"Nhân viên: {summary.StaffName}");
                        col.Item().Text($"Thời gian: {summary.CreatedTime:HH:mm dd/MM/yyyy}");

                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        //  TABLE
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Tên món").Bold();
                                header.Cell().AlignCenter().Text("Size").Bold();
                                header.Cell().AlignCenter().Text("SL").Bold();
                                header.Cell().AlignRight().Text("Giá").Bold();
                            });

                            foreach (var item in summary.Items)
                            {
                                table.Cell().Text(item.MenuItemName);
                                table.Cell().AlignCenter().Text(item.Size);
                                table.Cell().AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().AlignRight().Text($"{item.Total:N0} đ");
                            }
                        });

                        col.Item().PaddingVertical(5).LineHorizontal(1);

                        // ===== TOTAL =====
                        col.Item().AlignRight().Text($"TỔNG TIỀN: {summary.SubTotal:N0} đ");
                        col.Item().AlignRight().Text($"GIẢM GIÁ: -{summary.Discount:N0} đ");
                        col.Item().AlignRight().Text($"PHẢI TRẢ: {summary.Total:N0} đ")
                            .Bold().FontSize(12);

                        col.Item().PaddingVertical(10);

                        // QR
                        if (!string.IsNullOrEmpty(qrImagePath))
                        {
                            try
                            {
                                col.Item().AlignCenter().Image(qrImagePath, ImageScaling.FitWidth);
                            }
                            catch
                            {
                                col.Item().AlignCenter().Text("[Lỗi hiển thị QR]");
                            }
                        }

                        col.Item().PaddingTop(10)
                            .AlignCenter()
                            .Text("Cảm ơn quý khách!")
                            .Italic();
                    });
                });
            })
            .GeneratePdf(filePath);
        }
    }
}