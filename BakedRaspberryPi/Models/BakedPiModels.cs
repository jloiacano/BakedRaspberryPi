namespace BakedRaspberryPi.Models
{
    using MySql.Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BakedPiModels : DbContext
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
        public int Id { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    public class Accessory
    {
        public int Id { get; set; }
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class PiCase
    {
        public int Id { get; set; }
        public string UPC { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    public class WholePi
    {
        public int Id { get; set; }
        public virtual Pi Pi { get; set; }
        public virtual OS Filling { get; set; }
        public virtual PiCase Crust { get; set; }
        public virtual ICollection<Accessory> ALaModes { get; set; }
        public decimal Price { get; set; }
    }

    public class Cart
    {
        public int Id { get; set; }

        public ICollection<WholePi> CurrentPis { get; set; }
       
        public int CurrentIndex { get; set; }

    }
}