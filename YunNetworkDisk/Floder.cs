//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YunNetworkDisk
{
    using System;
    using System.Collections.Generic;
    
    public partial class Floder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Floder()
        {
            this.Filedate = new HashSet<Filedate>();
        }
    
        public long ID { get; set; }
        public string Name { get; set; }
        public long FatherDirectory { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public string Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Filedate> Filedate { get; set; }
    }
}
