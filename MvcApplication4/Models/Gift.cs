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
    
    public partial class Gift
    {
        public Gift()
        {
            this.Suggestions = new HashSet<Suggestion>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string shop { get; set; }
        public Nullable<decimal> price { get; set; }
        public string relationship { get; set; }
        public int relationshipLengthFrom { get; set; }
        public int relationshipLengthTo { get; set; }
        public string occasion { get; set; }
        public int ageFrom { get; set; }
        public int ageTo { get; set; }
        public string job { get; set; }
        public string hobbies { get; set; }
        public int repetition { get; set; }
    
        public virtual ICollection<Suggestion> Suggestions { get; set; }
    }
}
