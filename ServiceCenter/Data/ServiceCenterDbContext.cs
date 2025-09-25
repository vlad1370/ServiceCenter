using Microsoft.EntityFrameworkCore;
using ServiceCenter.Models;

namespace ServiceCenter.Data
{
    public class ServiceCenterDbContext : DbContext
    {
        public ServiceCenterDbContext(DbContextOptions<ServiceCenterDbContext> options)
            : base(options)
        {
        }

        // Таблицы базы данных
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RepairableModel> RepairableModels { get; set; }
        public DbSet<FaultType> FaultTypes { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderFault> OrderFaults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей и ограничений

            // 1. Связь многие-ко-многим между Order и FaultType через OrderFault
            modelBuilder.Entity<OrderFault>()
                .HasKey(of => of.Id);

            modelBuilder.Entity<OrderFault>()
                .HasOne(of => of.Order)
                .WithMany(o => o.OrderFaults)
                .HasForeignKey(of => of.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderFault>()
                .HasOne(of => of.FaultType)
                .WithMany(ft => ft.OrderFaults)
                .HasForeignKey(of => of.FaultTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Связь FaultType с RepairableModel
            modelBuilder.Entity<FaultType>()
                .HasOne(ft => ft.Model)
                .WithMany(rm => rm.FaultTypes)
                .HasForeignKey(ft => ft.ModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Связь Car с RepairableModel и Customer
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Model)
                .WithMany(rm => rm.Cars)
                .HasForeignKey(c => c.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Customer)
                .WithMany(cust => cust.Cars)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Связь Order с Customer и Employee
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Уникальные ограничения
            modelBuilder.Entity<Car>()
                .HasIndex(c => c.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<RepairableModel>()
                .HasIndex(rm => new { rm.Name, rm.Manufacturer })
                .IsUnique();

            // 6. Значения по умолчанию
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.HasWarranty)
                .HasDefaultValue(false);

            // 7. Индексы для улучшения производительности
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CarSerialNumber);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Phone)
                .IsUnique();
        }
    }
}