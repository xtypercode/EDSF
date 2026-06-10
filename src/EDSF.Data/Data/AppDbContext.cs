using EDSF.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Data.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<FinanceRecord> FinanceRecords => Set<FinanceRecord>();
    public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
    public DbSet<TransportGuide> TransportGuides => Set<TransportGuide>();
    public DbSet<TransportGuideItem> TransportGuideItems => Set<TransportGuideItem>();
    public DbSet<DebitNote> DebitNotes => Set<DebitNote>();
    public DbSet<CreditNote> CreditNotes => Set<CreditNote>();
    public DbSet<PaymentNote> PaymentNotes => Set<PaymentNote>();
    public DbSet<AdvancePayment> AdvancePayments => Set<AdvancePayment>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CompanyData> CompanyData => Set<CompanyData>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<InvoiceSeries> InvoiceSeries => Set<InvoiceSeries>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.Nif).HasMaxLength(20);
            e.Property(c => c.Phone).HasMaxLength(50);
            e.Property(c => c.Email).HasMaxLength(200);
            e.Property(c => c.Municipality).HasMaxLength(100);
            e.Property(c => c.Commune).HasMaxLength(100);
            e.Property(c => c.CustomerType).HasConversion<string>().HasMaxLength(15);
            e.Property(c => c.Province).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Code).HasMaxLength(50).IsRequired();
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Category).HasMaxLength(100);
            e.Property(p => p.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<Service>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(200).IsRequired();
            e.Property(s => s.Category).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.Property(i => i.Number).HasMaxLength(50).IsRequired();
            e.Property(i => i.Series).HasMaxLength(20);
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(i => i.DocumentType).HasConversion<string>().HasMaxLength(3);
            e.Property(i => i.Currency).HasConversion<string>().HasMaxLength(3);
            e.HasOne(i => i.Customer).WithMany(c => c.Invoices).HasForeignKey(i => i.CustomerId);
        });

        modelBuilder.Entity<InvoiceLine>(e =>
        {
            e.Property(il => il.Description).HasMaxLength(500).IsRequired();
            e.Property(il => il.ProductCode).HasMaxLength(50);
            e.Property(il => il.TaxRate).HasConversion<string>().HasMaxLength(20);
            e.Property(il => il.ExemptionReason).HasConversion<string>().HasMaxLength(3);
            e.HasOne(il => il.Invoice).WithMany(i => i.Lines).HasForeignKey(il => il.InvoiceId);
        });

        modelBuilder.Entity<InvoiceSeries>(e =>
        {
            e.Property(s => s.Series).HasMaxLength(20).IsRequired();
            e.Property(s => s.DocumentType).HasConversion<string>().HasMaxLength(3);
            e.HasIndex(s => new { s.Series, s.FiscalYear }).IsUnique();
        });

        modelBuilder.Entity<StockMovement>(e =>
        {
            e.Property(sm => sm.Type).HasConversion<string>().HasMaxLength(10);
            e.HasOne(sm => sm.Product).WithMany(p => p.StockMovements).HasForeignKey(sm => sm.ProductId);
        });

        modelBuilder.Entity<FinanceRecord>(e =>
        {
            e.Property(fr => fr.Type).HasConversion<string>().HasMaxLength(10);
            e.Property(fr => fr.Description).HasMaxLength(500).IsRequired();
            e.Property(fr => fr.Category).HasMaxLength(100);
        });

        modelBuilder.Entity<WarehouseItem>(e =>
        {
            e.Property(w => w.Code).HasMaxLength(50).IsRequired();
            e.Property(w => w.Name).HasMaxLength(200).IsRequired();
            e.Property(w => w.Category).HasMaxLength(100);
            e.Property(w => w.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<TransportGuide>(e =>
        {
            e.Property(t => t.Number).HasMaxLength(50).IsRequired();
            e.Property(t => t.Origin).HasMaxLength(200);
            e.Property(t => t.Destination).HasMaxLength(200);
            e.Property(t => t.Carrier).HasMaxLength(200);
            e.HasOne(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TransportGuideItem>(e =>
        {
            e.Property(t => t.Description).HasMaxLength(500).IsRequired();
            e.Property(t => t.Unit).HasMaxLength(20);
            e.HasOne(t => t.TransportGuide).WithMany(tg => tg.Items).HasForeignKey(t => t.TransportGuideId);
        });

        modelBuilder.Entity<DebitNote>(e =>
        {
            e.Property(d => d.Number).HasMaxLength(50).IsRequired();
            e.Property(d => d.Reason).HasMaxLength(500);
            e.HasOne(d => d.Customer).WithMany().HasForeignKey(d => d.CustomerId);
            e.HasOne(d => d.Invoice).WithMany().HasForeignKey(d => d.InvoiceId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CreditNote>(e =>
        {
            e.Property(c => c.Number).HasMaxLength(50).IsRequired();
            e.Property(c => c.Reason).HasMaxLength(500);
            e.HasOne(c => c.Customer).WithMany().HasForeignKey(c => c.CustomerId);
            e.HasOne(c => c.Invoice).WithMany().HasForeignKey(c => c.InvoiceId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PaymentNote>(e =>
        {
            e.Property(p => p.Number).HasMaxLength(50).IsRequired();
            e.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
            e.HasOne(p => p.Customer).WithMany().HasForeignKey(p => p.CustomerId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AdvancePayment>(e =>
        {
            e.Property(a => a.EmployeeName).HasMaxLength(200);
            e.Property(a => a.Reason).HasMaxLength(500);
        });

        modelBuilder.Entity<CashRegister>(e =>
        {
            e.Property(c => c.Notes).HasMaxLength(500);
        });

        modelBuilder.Entity<CompanyData>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.Nif).HasMaxLength(20);
            e.Property(c => c.Phone).HasMaxLength(50);
            e.Property(c => c.Email).HasMaxLength(200);
            e.Property(c => c.Municipality).HasMaxLength(100);
            e.Property(c => c.Commune).HasMaxLength(100);
            e.Property(c => c.CommercialReg).HasMaxLength(50);
            e.Property(c => c.Cae).HasMaxLength(20);
            e.Property(c => c.TaxRegime).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Province).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Permission>(e =>
        {
            e.Property(p => p.Module).HasMaxLength(100).IsRequired();
            e.HasOne(p => p.AppUser).WithMany().HasForeignKey(p => p.AppUserId);
        });

        modelBuilder.Entity<Inventory>(e =>
        {
            e.Property(i => i.Name).HasMaxLength(200).IsRequired();
            e.Property(i => i.Notes).HasMaxLength(500);
        });

        modelBuilder.Entity<InventoryItem>(e =>
        {
            e.HasOne(i => i.Inventory).WithMany(inv => inv.Items).HasForeignKey(i => i.InventoryId);
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId);
        });

        modelBuilder.Entity<Supplier>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(200).IsRequired();
            e.Property(s => s.Nif).HasMaxLength(20);
            e.Property(s => s.Phone).HasMaxLength(20);
            e.Property(s => s.Email).HasMaxLength(200);
            e.Property(s => s.ContactPerson).HasMaxLength(200);
        });

        modelBuilder.Entity<PurchaseOrder>(e =>
        {
            e.Property(p => p.Number).HasMaxLength(50).IsRequired();
            e.Property(p => p.Status).HasMaxLength(20);
            e.Property(p => p.Notes).HasMaxLength(500);
            e.HasOne(p => p.Supplier).WithMany().HasForeignKey(p => p.SupplierId);
        });

        modelBuilder.Entity<PurchaseOrderItem>(e =>
        {
            e.Property(i => i.Description).HasMaxLength(500).IsRequired();
            e.HasOne(i => i.PurchaseOrder).WithMany(po => po.Items).HasForeignKey(i => i.PurchaseOrderId);
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.Property(b => b.Number).HasMaxLength(50).IsRequired();
            e.Property(b => b.Status).HasMaxLength(20);
            e.Property(b => b.Notes).HasMaxLength(500);
            e.HasOne(b => b.Customer).WithMany().HasForeignKey(b => b.CustomerId);
        });

        modelBuilder.Entity<BudgetItem>(e =>
        {
            e.Property(i => i.Description).HasMaxLength(500).IsRequired();
            e.HasOne(i => i.Budget).WithMany(b => b.Items).HasForeignKey(i => i.BudgetId);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.Property(emp => emp.Name).HasMaxLength(200).IsRequired();
            e.Property(emp => emp.Position).HasMaxLength(100);
            e.Property(emp => emp.Department).HasMaxLength(100);
            e.Property(emp => emp.Phone).HasMaxLength(20);
            e.Property(emp => emp.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.Property(a => a.EntityName).HasMaxLength(100).IsRequired();
            e.Property(a => a.Action).HasMaxLength(10).IsRequired();
            e.Property(a => a.UserName).HasMaxLength(100);
            e.Property(a => a.IpAddress).HasMaxLength(50);
            e.Property(a => a.OldValues).HasColumnType("TEXT");
            e.Property(a => a.NewValues).HasColumnType("TEXT");
            e.HasIndex(a => a.Timestamp);
            e.HasIndex(a => new { a.EntityName, a.EntityId });
        });
    }
}
