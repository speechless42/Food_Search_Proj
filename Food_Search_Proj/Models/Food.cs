//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Food_Search_Proj.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Food
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Food()
        {
            this.Dishes_Contain_Food = new HashSet<Dishes_Contain_Food>();
        }
    
        public int Food_ID { get; set; }
        public string Food_Name { get; set; }
        public int Categories_Of_Food_ID { get; set; }
    
        public virtual Categories_Of_Food Categories_Of_Food { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dishes_Contain_Food> Dishes_Contain_Food { get; set; }
    }
}
