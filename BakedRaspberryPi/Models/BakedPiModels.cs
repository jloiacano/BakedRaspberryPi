namespace BakedRaspberryPi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using MySql.Data.Entity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    
    public class BakedPiModels : IdentityDbContext
    {
        // Your context has been configured to use a 'BakedPiModels' connection string from your application's
        // configuration file (App.config or Web.config). By default, this connection string targets the
        // 'BakedRaspberryPi.Models.BakedPiModels' database on your LocalDb instance.
        //
        // If you wish to target a different database and/or database provider, modify the 'BakedPiModels'
        // connection string in the application configuration file.
        public BakedPiModels()
            : base("name=BakedPiModels")
        {
            Database.SetInitializer<BakedPiModels>(new DropCreateDatabaseAlways<BakedPiModels>());
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Pi> Pis { get; set; }

        public virtual DbSet<Accessory> Accessories { get; set; }
        public virtual DbSet<OS> OSs { get; set; }
        public virtual DbSet<PiCase> PiCases { get; set; }
        public virtual DbSet<WholePi> WholePis { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class Pi
    {
        public int PiId { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    public class Accessory
    {
        public int AccessoryId { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public virtual ICollection<WholePi> WholePis { get; set; }
    }

    public class OS
    {
        public int OSId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class PiCase
    {
        public int PiCaseId { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    public class WholePi
    {
        [Key]
        public int WholePiId { get; set; }

        public virtual Pi Pi { get; set; }
        public virtual OS Filling { get; set; }
        public virtual PiCase Crust { get; set; }
        public virtual ICollection<Accessory> ALaModes { get; set; }
        public decimal Price { get; set; }
        
        public virtual Cart cart { get; set; }
    }
    
    public class Cart
    {
        [Key]
        public System.Guid CartId { get; set; }

        public virtual ICollection<WholePi> WholePis { get; set; }

        public int CurrentPiId { get; set; }

    }
}