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
using Class_Relax.Models;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;

namespace Class_Relax
{



    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IIdentityMessageService emailService)
            : base(store)
        {
            this.EmailService = emailService;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()), new SmtpEmailService());
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager);


            {
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {

            };
            return manager;
        }
    }
    public static class IdentityExtensions
    {
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            return (T)Convert.ChangeType(identity.GetUserId(), typeof(T));
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
