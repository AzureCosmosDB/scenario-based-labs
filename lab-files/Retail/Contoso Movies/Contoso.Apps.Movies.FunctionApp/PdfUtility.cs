using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DinkToPdf;
using IPdfConverter = DinkToPdf.Contracts.IConverter;

namespace ContosoFunctionApp
{
    public static class PdfUtility
    {
        static IPdfConverter pdfConverter = new SynchronizedConverter(new PdfTools());

        public static async Task<byte[]> CreatePdfReport(OrderViewModel order, string fileName, ILogger log)
        {
            try
            {
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Landscape,
                        PaperSize = PaperKind.A4Plus,
                    },
                                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = $"<div style='border:solid; border - width: thin; background - color:lightgrey'>Contoso Sport League - Purchase Receipt</div><br/>" +
                            $"<div style = 'text-align:right;font-weight:bold'>Order ID: {order.OrderId}</ div >" +
                            $"<div style = 'text-align:right;font-weight:bold'>Total: ${order.Total}</ div >" +
                            $"<div style = 'text-align:center;font-weight:bold'>Thank you for your order {order.FirstName}</ div >" ,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                        }
                    }
                };

                var converter = new SynchronizedConverter(new PdfTools());
                return converter.Convert(doc);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
