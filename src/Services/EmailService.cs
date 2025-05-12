using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Check_Inn.Entities;

namespace Check_Inn.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            _smtpHost = System.Configuration.ConfigurationManager.AppSettings["EmailHost"];
            _smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);
            _smtpUsername = System.Configuration.ConfigurationManager.AppSettings["EmailUsername"];
            _smtpPassword = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
            _fromEmail = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            _fromName = System.Configuration.ConfigurationManager.AppSettings["EmailFromName"];
        }

        public bool SendBookingConfirmation(Booking booking, Accomodation accomodation, AccomodationPackage package)
        {
            try
            {
                var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = $"Your booking confirmation for {accomodation.Name}",
                    Body = BuildBookingConfirmationEmail(booking, accomodation, package),
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(booking.Email, booking.GuestName));
                
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendBookingConfirmationAsync(Booking booking, Accomodation accomodation, AccomodationPackage package)
        {
            try
            {
                var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = $"Your booking confirmation for {accomodation.Name}",
                    Body = BuildBookingConfirmationEmail(booking, accomodation, package),
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(booking.Email, booking.GuestName));
                
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        private string BuildBookingConfirmationEmail(Booking booking, Accomodation accomodation, AccomodationPackage package)
        {
            var checkInDate = booking.FromDate.ToString("dddd, MMMM dd, yyyy");
            var checkOutDate = booking.FromDate.AddDays(booking.Duration).ToString("dddd, MMMM dd, yyyy");
            var totalPrice = package.FeePerNight * booking.Duration;

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<title>Booking Confirmation</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            sb.AppendLine(".header { background-color: #4a90e2; color: white; padding: 15px; text-align: center; }");
            sb.AppendLine(".content { padding: 20px; border: 1px solid #ddd; }");
            sb.AppendLine(".booking-details { background-color: #f9f9f9; padding: 15px; margin: 15px 0; }");
            sb.AppendLine(".footer { text-align: center; margin-top: 20px; font-size: 12px; color: #777; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"container\">");
            
            sb.AppendLine("<div class=\"header\">");
            sb.AppendLine("<h1>Booking Confirmation</h1>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("<div class=\"content\">");
            sb.AppendLine($"<p>Dear {booking.GuestName},</p>");
            sb.AppendLine("<p>Thank you for choosing Check-Inn Hotel. Your booking has been confirmed.</p>");
            
            sb.AppendLine("<div class=\"booking-details\">");
            sb.AppendLine("<h2>Booking Details</h2>");
            sb.AppendLine($"<p><strong>Booking ID:</strong> #{booking.ID}</p>");
            sb.AppendLine($"<p><strong>Room:</strong> {accomodation.Name} ({package.Name})</p>");
            sb.AppendLine($"<p><strong>Check-in:</strong> {checkInDate}</p>");
            sb.AppendLine($"<p><strong>Check-out:</strong> {checkOutDate}</p>");
            sb.AppendLine($"<p><strong>Duration:</strong> {booking.Duration} night(s)</p>");
            sb.AppendLine($"<p><strong>Guests:</strong> {booking.NoOfAdults} Adult(s), {booking.NoOfChildren} Children</p>");
            sb.AppendLine($"<p><strong>Total Price:</strong> ${totalPrice:F2}</p>");
            sb.AppendLine("</div>");
            
            if (!string.IsNullOrEmpty(booking.AdditionalInfo))
            {
                sb.AppendLine("<div class=\"special-requests\">");
                sb.AppendLine("<h2>Special Requests</h2>");
                sb.AppendLine($"<p>{booking.AdditionalInfo}</p>");
                sb.AppendLine("</div>");
            }
            
            sb.AppendLine("<p>If you have any questions regarding your reservation, please contact our front desk.</p>");
            sb.AppendLine("<p>We look forward to welcoming you!</p>");
            sb.AppendLine("<p>Best regards,<br>Check-Inn Hotel Team</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("<div class=\"footer\">");
            sb.AppendLine("<p>This is an automated message. Please do not reply to this email.</p>");
            sb.AppendLine("<p>© 2025 Check-Inn Hotel. All rights reserved.</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
