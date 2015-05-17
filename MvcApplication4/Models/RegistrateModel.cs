using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MvcApplication4.Models
{
    public class RegistrateModel  
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "User name")]
        [Remote("doesUserNameExist", "Home", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string userName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage = "Passwords does not match.")]
        public string ConfirmPassword { get; set; }

    }
    public class ManageModel
    {

        public string userName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Remote("doesPasswordMatch", "Home", HttpMethod = "POST", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Current password")]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Passwords does not match.")]
        public string ConfirmNewPassword { get; set; }

    }

    public class LogingModel
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string userName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }

    public class SuggestionModel
    {

        public Models.Gift Gift { get; set; }
        public Models.Suggestion Suggestion { get; set; }

    }

    public class RadioValuesModel
    {

        public int Value { get; private set; }


        static Dictionary<int, int> Values = new Dictionary<int, int>()
    {
        { 1 ,1},
        { 2 ,2},
        { 3 ,3},
        {4,4},
        { 5,5},
    };
    }
}
