﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DiplomEntities : DbContext
    {
        public DiplomEntities()
            : base("name=DiplomEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Pupil> Pupils { get; set; }
        public virtual DbSet<PupilsVariant> PupilsVariants { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TaskNumber> TaskNumbers { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TasksVariant> TasksVariants { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Variant> Variants { get; set; }
        public virtual DbSet<TaskImage> TaskImages { get; set; }
        public virtual DbSet<TaskFile> TaskFiles { get; set; }
    }
}
