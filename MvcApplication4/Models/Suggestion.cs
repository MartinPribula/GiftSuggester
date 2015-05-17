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
    
    public partial class Suggestion
    {
        public int id { get; set; }
        public Nullable<int> idUser { get; set; }
        [Display(Name = "Gift")]
        public int idGift { get; set; }
        [Required]
        [Display(Name = "Relationship")]
        public string relationship { get; set; }
        [Required]
        [Display(Name = "Duration")]
        public int relationshipLength { get; set; }
        [Required]
        [Display(Name = "Occasion")]
        public string occasion { get; set; }
        [Display(Name = "Age")]
        public int age { get; set; }
        [Display(Name = "Job")]
        public string job { get; set; }
        [Display(Name = "Hobbies")]
        public string hobbies { get; set; }
        [Display(Name = "Rating")]
        public Nullable<int> rating { get; set; }
        [Required]
        [Display(Name = "Im willing to pay this much")]
        public decimal priceTo { get; set; }
        public Nullable<int> idRecipient { get; set; }
        [Display(Name = "Date")]
        public Nullable<System.DateTime> timeStamp { get; set; }
    
        public virtual Gift Gift { get; set; }
        public virtual Recipient Recipient { get; set; }
        public virtual User User { get; set; }
    }
}
