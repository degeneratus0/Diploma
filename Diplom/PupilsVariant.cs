//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Diplom
{
    using System;
    using System.Collections.Generic;
    
    public partial class PupilsVariant
    {
        public int ID { get; set; }
        public int PupilId { get; set; }
        public int VariantId { get; set; }
        public Nullable<bool> Completed { get; set; }
        public string Result { get; set; }
    
        public virtual Pupil Pupil { get; set; }
        public virtual Variant Variant { get; set; }
    }
}
