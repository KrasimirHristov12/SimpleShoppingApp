﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;
namespace SimpleShoppingApp.Models.Users
{
    public class AdministrationUserViewModel
    {
        public string Id { get; set; } = null!;

        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        [Display(Name = UsernameDisplay)]
        public string? UserName { get; set; }

        [BindNever]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = PhoneNumberDisplay)]
        public string? PhoneNumber { get; set; }

        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        [Display(Name = FirstNameDisplay)]
        public string? FirstName { get; set; }

        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        [Display(Name = LastNameDisplay)]
        public string? LastName { get; set; }
    }
}
