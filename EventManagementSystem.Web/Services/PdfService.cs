using EventManagementSystem.Core.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace EventManagementSystem.Web.Services
{
    public class PdfService
    {
        public byte[] GenerateTicket(Registration registration)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.Postcard, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                document.Add(new Paragraph("Event Ticket", titleFont));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph($"Event: {registration.Event?.Name ?? "N/A"}", normalFont));
                document.Add(new Paragraph($"Date: {registration.Event?.DateAndTime.ToString("g") ?? "N/A"}", normalFont));
                document.Add(new Paragraph($"Location: {registration.Event?.Location ?? "N/A"}", normalFont));
                document.Add(new Paragraph($"Attendee: {registration.User?.UserName ?? "N/A"}", normalFont));
                document.Add(new Paragraph($"Registration ID: {registration.Id}", normalFont));

                document.Close();
                return ms.ToArray();
            }
        }
    }
}