using EDSF.Core.Interfaces;
using EDSF.Core.Models;
using EDSF.Data.Data;

namespace EDSF.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<Customer>? _customers;
    private IRepository<Product>? _products;
    private IRepository<Service>? _services;
    private IRepository<Invoice>? _invoices;
    private IRepository<StockMovement>? _stockMovements;
    private IRepository<FinanceRecord>? _financeRecords;
    private IRepository<WarehouseItem>? _warehouseItems;
    private IRepository<TransportGuide>? _transportGuides;
    private IRepository<DebitNote>? _debitNotes;
    private IRepository<CreditNote>? _creditNotes;
    private IRepository<PaymentNote>? _paymentNotes;
    private IRepository<AdvancePayment>? _advancePayments;
    private IRepository<CashRegister>? _cashRegisters;
    private IRepository<CompanyData>? _companyData;
    private IRepository<AppUser>? _appUsers;
    private IRepository<Permission>? _permissions;
    private IRepository<Inventory>? _inventories;
    private IRepository<Supplier>? _suppliers;
    private IRepository<PurchaseOrder>? _purchaseOrders;
    private IRepository<Budget>? _budgets;
    private IRepository<Employee>? _employees;
    private IRepository<AuditLog>? _auditLogs;
    private IRepository<InvoiceSeries>? _invoiceSeries;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
    public IRepository<Service> Services => _services ??= new Repository<Service>(_context);
    public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(_context);
    public IRepository<StockMovement> StockMovements => _stockMovements ??= new Repository<StockMovement>(_context);
    public IRepository<FinanceRecord> FinanceRecords => _financeRecords ??= new Repository<FinanceRecord>(_context);
    public IRepository<WarehouseItem> WarehouseItems => _warehouseItems ??= new Repository<WarehouseItem>(_context);
    public IRepository<TransportGuide> TransportGuides => _transportGuides ??= new Repository<TransportGuide>(_context);
    public IRepository<DebitNote> DebitNotes => _debitNotes ??= new Repository<DebitNote>(_context);
    public IRepository<CreditNote> CreditNotes => _creditNotes ??= new Repository<CreditNote>(_context);
    public IRepository<PaymentNote> PaymentNotes => _paymentNotes ??= new Repository<PaymentNote>(_context);
    public IRepository<AdvancePayment> AdvancePayments => _advancePayments ??= new Repository<AdvancePayment>(_context);
    public IRepository<CashRegister> CashRegisters => _cashRegisters ??= new Repository<CashRegister>(_context);
    public IRepository<CompanyData> CompanyData => _companyData ??= new Repository<CompanyData>(_context);
    public IRepository<AppUser> AppUsers => _appUsers ??= new Repository<AppUser>(_context);
    public IRepository<Permission> Permissions => _permissions ??= new Repository<Permission>(_context);
    public IRepository<Inventory> Inventories => _inventories ??= new Repository<Inventory>(_context);
    public IRepository<Supplier> Suppliers => _suppliers ??= new Repository<Supplier>(_context);
    public IRepository<PurchaseOrder> PurchaseOrders => _purchaseOrders ??= new Repository<PurchaseOrder>(_context);
    public IRepository<Budget> Budgets => _budgets ??= new Repository<Budget>(_context);
    public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
    public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
    public IRepository<InvoiceSeries> InvoiceSeries => _invoiceSeries ??= new Repository<InvoiceSeries>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
