using EDSF.Core.Models;

namespace EDSF.Core.Interfaces;

public interface IUnitOfWork
{
    IRepository<Customer> Customers { get; }
    IRepository<Product> Products { get; }
    IRepository<Service> Services { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<StockMovement> StockMovements { get; }
    IRepository<FinanceRecord> FinanceRecords { get; }
    IRepository<WarehouseItem> WarehouseItems { get; }
    IRepository<TransportGuide> TransportGuides { get; }
    IRepository<DebitNote> DebitNotes { get; }
    IRepository<CreditNote> CreditNotes { get; }
    IRepository<PaymentNote> PaymentNotes { get; }
    IRepository<AdvancePayment> AdvancePayments { get; }
    IRepository<CashRegister> CashRegisters { get; }
    IRepository<CompanyData> CompanyData { get; }
    IRepository<AppUser> AppUsers { get; }
    IRepository<Permission> Permissions { get; }
    IRepository<Inventory> Inventories { get; }
    IRepository<Supplier> Suppliers { get; }
    IRepository<PurchaseOrder> PurchaseOrders { get; }
    IRepository<Budget> Budgets { get; }
    IRepository<Employee> Employees { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<InvoiceSeries> InvoiceSeries { get; }
    Task<int> SaveChangesAsync();
}
