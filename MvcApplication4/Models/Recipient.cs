//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcApplication4.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Recipient
    {
        public int id { get; set; }

        public int idUser { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string surname { get; set; }
        [Required]
        [Display(Name = "Relationship")]
        public string relationship { get; set; }
        [Required]
        [Display(Name = "Duration")]
        public int relationshipLength { get; set; }
        [Required]
        [Display(Name = "Age")]
        public int age { get; set; }
    }
}
