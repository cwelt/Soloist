using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using CW.Soloist.WebApplication.Models;
using System.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Net.Mime;

namespace CW.Soloist.WebApplication
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            string emailRecipient = message.Destination;
            string emailSubject = "Soloist - " + message.Subject;
            string emailBody = "Hello," + Environment.NewLine + 
                "Welcome to Soloist web application." + Environment.NewLine
                + Environment.NewLine + message.Body;
            await SendEmailAsyncWithSendGridService(emailRecipient, emailSubject, emailBody);
        }

        private async Task SendEmailAsyncWithSMTP(string recipient, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                // enable ssl to encrypt the connection 
                smtpClient.EnableSsl = true;

                // set smtp server host domain name 
                smtpClient.Host = ConfigurationManager.AppSettings["SMTPHost"];

                // set the smtp server port 
                bool isPortValid = Int32.TryParse(ConfigurationManager.AppSettings["SMTPPort"], out int port);
                smtpClient.Port = isPortValid ? port : 587;

                // set credentials 
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["SMTPUser"],
                    ConfigurationManager.AppSettings["SMTPPassword"]);

                // set sender email address
                string sender = ConfigurationManager.AppSettings["AdminEmailAddress"];

                // build the email message 
                MailMessage mailMessage = new MailMessage(sender, recipient, subject, body);

                // encode message as html 
                mailMessage.BodyEncoding = Encoding.UTF8;
                string html = HttpUtility.HtmlEncode(body);
                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Plain));
                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // try sending the email via standard smtp 
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch // in case failure send via third party SendGrid service  
                {
                    await SendEmailAsyncWithSendGridService(recipient, subject, body);
                }
            }
        }

        private async Task SendEmailAsyncWithSendGridService(string recipient, string subject, string body)
        {
            // set sender and recipent email addresses 
            string adminEmail = ConfigurationManager.AppSettings["AdminEmailAddress"];
            EmailAddress senderAddress = new EmailAddress(adminEmail, "Admin");
            EmailAddress recipientAddress = new EmailAddress(recipient, "New User");

            // create the email message 
            SendGridMessage email = MailHelper.CreateSingleEmail(
                from: senderAddress,
                to: recipientAddress,
                subject: subject,
                plainTextContent: body,
                htmlContent: body);

            // get api key for email sending service 
            string apiKey = ConfigurationManager.AppSettings["SendGridKey"];

            // set an http client wrapper usign the registered api key 
            SendGridClient httpClient = new SendGridClient(apiKey);

            // use the http client for sending the email
            await httpClient.SendEmailAsync(email);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }



    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
