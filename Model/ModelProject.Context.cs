﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class jsEntities : DbContext
    {
        public jsEntities()
            : base("name=jsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<usertable> usertables { get; set; }
        public virtual DbSet<Transformer> Transformers { get; set; }
        public virtual DbSet<MutualTranslator> MutualTranslators { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
    }
}
