using EDSF.Core.Enums;
using EDSF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EDSF.Data.Data;

public static class DbInitializer
{
    private static string HashPassword(string password)
    {
        var data = $"{password}|edsf-default-secret-change-me";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static void Seed(AppDbContext db)
    {
        if (db.AppUsers.Any()) return;

        // === USERS ===
        var admin = new AppUser
        {
            Username = "admin",
            DisplayName = "Administrador",
            Email = "admin@edsf.pt",
            PasswordHash = HashPassword("admin123"),
            Role = "admin",
            IsActive = true
        };
        var user1 = new AppUser { Username = "joao", DisplayName = "João Silva", Email = "joao@edsf.pt", PasswordHash = HashPassword("1234"), Role = "user", IsActive = true };
        var user2 = new AppUser { Username = "maria", DisplayName = "Maria Santos", Email = "maria@edsf.pt", PasswordHash = HashPassword("1234"), Role = "user", IsActive = true };
        var user3 = new AppUser { Username = "carlos", DisplayName = "Carlos Pereira", Email = "carlos@edsf.pt", PasswordHash = HashPassword("1234"), Role = "manager", IsActive = true };
        db.AppUsers.AddRange(admin, user1, user2, user3);
        db.SaveChanges();

        // === PERMISSIONS ===
        db.Permissions.AddRange(
            new Permission { AppUserId = admin.Id, Module = "Todos", CanRead = true, CanWrite = true, CanDelete = true },
            new Permission { AppUserId = user1.Id, Module = "Clientes", CanRead = true, CanWrite = true, CanDelete = false },
            new Permission { AppUserId = user1.Id, Module = "Faturas", CanRead = true, CanWrite = true, CanDelete = false },
            new Permission { AppUserId = user2.Id, Module = "Tesouraria", CanRead = true, CanWrite = true, CanDelete = false },
            new Permission { AppUserId = user3.Id, Module = "Armazém", CanRead = true, CanWrite = true, CanDelete = true },
            new Permission { AppUserId = user3.Id, Module = "Gestão", CanRead = true, CanWrite = true, CanDelete = true }
        );

        // === COMPANY ===
        db.CompanyData.Add(new CompanyData
        {
            Name = "EDSF - Empresa Exemplo Lda",
            Nif = "123456789",
            Address = "Rua da Tecnologia, 123, 4000-001 Porto",
            Phone = "+351 220 000 000",
            Email = "geral@edsf.pt",
            TaxRegime = TaxRegime.General
        });

        // === EMPLOYEES ===
        var employees = new List<Employee>
        {
            new() { Name = "Ana Costa", Position = "CEO", Department = "Administração", Phone = "911111111", Email = "ana@edsf.pt", HireDate = new DateTime(2020, 1, 15), IsActive = true },
            new() { Name = "Pedro Martins", Position = "CFO", Department = "Finanças", Phone = "922222222", Email = "pedro@edsf.pt", HireDate = new DateTime(2020, 3, 1), IsActive = true },
            new() { Name = "Sofia Almeida", Position = "CTO", Department = "TI", Phone = "933333333", Email = "sofia@edsf.pt", HireDate = new DateTime(2021, 6, 1), IsActive = true },
            new() { Name = "Rui Oliveira", Position = "Vendas", Department = "Comercial", Phone = "944444444", Email = "rui@edsf.pt", HireDate = new DateTime(2022, 1, 10), IsActive = true },
            new() { Name = "Inês Ferreira", Position = "Marketing", Department = "Marketing", Phone = "955555555", Email = "ines@edsf.pt", HireDate = new DateTime(2022, 9, 1), IsActive = true },
            new() { Name = "Tiago Rodrigues", Position = "Programador", Department = "TI", Phone = "966666666", Email = "tiago@edsf.pt", HireDate = new DateTime(2023, 3, 15), IsActive = true },
            new() { Name = "Marta Lopes", Position = "Designer", Department = "Marketing", Phone = "977777777", Email = "marta@edsf.pt", HireDate = new DateTime(2023, 7, 1), IsActive = true },
            new() { Name = "José Nunes", Position = "Armazém", Department = "Logística", Phone = "988888888", Email = "jose@edsf.pt", HireDate = new DateTime(2024, 1, 8), IsActive = true }
        };
        db.Employees.AddRange(employees);
        db.SaveChanges();

        // === CUSTOMERS ===
        var customers = new List<Customer>
        {
            new() { Name = "Tech Solutions Lda", Nif = "123456780", Address = "Av. da Boavista, 1000, Porto", Phone = "912345678", Email = "geral@techsolutions.pt", CreatedAt = new DateTime(2024, 1, 15) },
            new() { Name = "Mega Construções SA", Nif = "234567890", Address = "Rua das Obras, 50, Lisboa", Phone = "913456789", Email = "info@megaconstrucoes.pt", CreatedAt = new DateTime(2024, 2, 1) },
            new() { Name = "Saúde Primeira Unipessoal", Nif = "345678901", Address = "Praceta da Saúde, 10, Coimbra", Phone = "914567890", Email = "admin@saudeprimeira.pt", CreatedAt = new DateTime(2024, 2, 20) },
            new() { Name = "Comércio Global Lda", Nif = "456789012", Address = "Zona Industrial, Lote 5, Braga", Phone = "915678901", Email = "vendas@comercioglobal.pt", CreatedAt = new DateTime(2024, 3, 10) },
            new() { Name = "Restaurante Bom Gosto", Nif = "567890123", Address = "Rua das Flores, 20, Aveiro", Phone = "916789012", Email = "contato@bomgosto.pt", CreatedAt = new DateTime(2024, 3, 25) },
            new() { Name = "Hotel Paraíso SA", Nif = "678901234", Address = "Av. do Mar, 200, Algarve", Phone = "917890123", Email = "reservas@hotelparaiso.pt", CreatedAt = new DateTime(2024, 4, 5) },
            new() { Name = "Auto Peças Rápidas", Nif = "789012345", Address = "Estrada Nacional 10, km 5, Santarém", Phone = "918901234", Email = "pecas@autorepidas.pt", CreatedAt = new DateTime(2024, 4, 20) },
            new() { Name = "Farmácia Central", Nif = "890123456", Address = "Rua Direita, 100, Viseu", Phone = "919012345", Email = "farmacia.central@mail.pt", CreatedAt = new DateTime(2024, 5, 1) },
            new() { Name = "Escola Nova Geração", Nif = "901234567", Address = "Av. da Educação, 300, Setúbal", Phone = "920123456", Email = "secretaria@escolanovageracao.pt", CreatedAt = new DateTime(2024, 5, 15) },
            new() { Name = "Oficina do Saber Lda", Nif = "012345678", Address = "Rua dos Livros, 45, Évora", Phone = "921234567", Email = "geral@oficinadosaber.pt", CreatedAt = new DateTime(2024, 6, 1) }
        };
        db.Customers.AddRange(customers);
        db.SaveChanges();

        // === SUPPLIERS ===
        var suppliers = new List<Supplier>
        {
            new() { Name = "Materiais de Construção Lda", Nif = "111111111", Address = "Rua do Cimento, 1, Porto", Phone = "931234567", Email = "vendas@materiaisconstrucao.pt", ContactPerson = "António Silva" },
            new() { Name = "Papelaria Central", Nif = "222222222", Address = "Rua do Papel, 200, Lisboa", Phone = "932345678", Email = "encomendas@papelariacentral.pt", ContactPerson = "Bárbara Neves" },
            new() { Name = "Equipamentos Informáticos SA", Nif = "333333333", Address = "Parque Tecnológico, Lote 3, Aveiro", Phone = "933456789", Email = "info@equipinf.pt", ContactPerson = "Cristiano Rocha" },
            new() { Name = "Limpeza Total Unipessoal", Nif = "444444444", Address = "Zona Industrial Lt 12, Braga", Phone = "934567890", Email = "geral@limpezatotal.pt", ContactPerson = "Dulce Faria" },
            new() { Name = "Alimentos Fresco Bom", Nif = "555555555", Address = "Mercado Abastecedor, Lisboa", Phone = "935678901", Email = "vendas@alimentosfresco.pt", ContactPerson = "Eduardo Lima" }
        };
        db.Suppliers.AddRange(suppliers);
        db.SaveChanges();

        // === PRODUCTS ===
        var products = new List<Product>
        {
            new() { Code = "PROD-001", Name = "Portátil ProBook 15", Description = "Portátil empresarial 15.6\", i7, 16GB RAM", Price = 1299.99m, Category = "Informática", Unit = "un", StockQuantity = 25 },
            new() { Code = "PROD-002", Name = "Monitor 27\" 4K", Description = "Monitor Ultra HD 27 polegadas", Price = 499.99m, Category = "Informática", Unit = "un", StockQuantity = 40 },
            new() { Code = "PROD-003", Name = "Teclado Mecânico RGB", Description = "Teclado mecânico com iluminação RGB", Price = 89.99m, Category = "Informática", Unit = "un", StockQuantity = 60 },
            new() { Code = "PROD-004", Name = "Rato Sem Fios", Description = "Rato ergonómico sem fios", Price = 39.99m, Category = "Informática", Unit = "un", StockQuantity = 100 },
            new() { Code = "PROD-005", Name = "Webcam HD 1080p", Description = "Webcam com microfone integrado", Price = 59.99m, Category = "Informática", Unit = "un", StockQuantity = 35 },
            new() { Code = "PROD-006", Name = "Cadeira de Escritório Ergo", Description = "Cadeira ergonómica com suporte lombar", Price = 349.99m, Category = "Mobiliário", Unit = "un", StockQuantity = 15 },
            new() { Code = "PROD-007", Name = "Secretária Elétrica Ajustável", Description = "Secretária com regulação elétrica de altura", Price = 599.99m, Category = "Mobiliário", Unit = "un", StockQuantity = 10 },
            new() { Code = "PROD-008", Name = "Resma Papel A4 80g", Description = "Resma 500 folhas papel branco", Price = 5.99m, Category = "Papelaria", Unit = "resma", StockQuantity = 500 },
            new() { Code = "PROD-009", Name = "Caixa Arquivo Morto", Description = "Caixa de arquivo em cartão reforçado", Price = 3.50m, Category = "Papelaria", Unit = "un", StockQuantity = 200 },
            new() { Code = "PROD-010", Name = "Toner HP 26X", Description = "Toner preto alto rendimento 6000 páginas", Price = 89.99m, Category = "Consumíveis", Unit = "un", StockQuantity = 30 },
            new() { Code = "PROD-011", Name = "Tinteiro Canon 545", Description = "Tinteiro preto 400 páginas", Price = 29.99m, Category = "Consumíveis", Unit = "un", StockQuantity = 45 },
            new() { Code = "PROD-012", Name = "Cabo USB-C 2m", Description = "Cabo USB-C para carregamento e dados", Price = 12.99m, Category = "Informática", Unit = "un", StockQuantity = 150 },
            new() { Code = "PROD-013", Name = "Disco SSD 1TB", Description = "Disco SSD interno 1TB NVMe", Price = 119.99m, Category = "Informática", Unit = "un", StockQuantity = 20 },
            new() { Code = "PROD-014", Name = "Switch 8 Portas Gigabit", Description = "Switch de rede 8 portas 10/100/1000", Price = 49.99m, Category = "Redes", Unit = "un", StockQuantity = 12 },
            new() { Code = "PROD-015", Name = "Access Point WiFi 6", Description = "Access Point WiFi 6 AX1800", Price = 89.99m, Category = "Redes", Unit = "un", StockQuantity = 8 }
        };
        db.Products.AddRange(products);
        db.SaveChanges();

        // === SERVICES ===
        db.Services.AddRange(
            new Service { Name = "Consultoria TI", Description = "Consultoria em tecnologias de informação", Price = 95.00m, Category = "Consultoria" },
            new Service { Name = "Desenvolvimento Web", Description = "Criação de sites e aplicações web", Price = 75.00m, Category = "Desenvolvimento" },
            new Service { Name = "Desenvolvimento Mobile", Description = "Aplicações iOS e Android", Price = 85.00m, Category = "Desenvolvimento" },
            new Service { Name = "Suporte Técnico", Description = "Suporte técnico presencial ou remoto (hora)", Price = 55.00m, Category = "Suporte" },
            new Service { Name = "Formação em TI", Description = "Formação empresarial em tecnologias", Price = 120.00m, Category = "Formação" },
            new Service { Name = "Segurança Informática", Description = "Auditoria e implementação de segurança", Price = 110.00m, Category = "Segurança" },
            new Service { Name = "Cloud Computing", Description = "Migração e gestão de cloud", Price = 130.00m, Category = "Cloud" },
            new Service { Name = "Design Gráfico", Description = "Criação de identidade visual e materiais", Price = 65.00m, Category = "Design" }
        );
        db.SaveChanges();

        // === INVOICES ===
        var invoiceData = new[]
        {
            new { Customer = customers[0], Number = "FT-2025/001", Date = new DateTime(2025, 11, 5), Status = InvoiceStatus.Paid, Total = 3499.97m, Lines = new[] { ("Portátil ProBook 15", 2, 1299.99m), ("Monitor 27\" 4K", 2, 499.99m) } },
            new { Customer = customers[1], Number = "FT-2025/002", Date = new DateTime(2025, 12, 10), Status = InvoiceStatus.Paid, Total = 1299.99m, Lines = new[] { ("Portátil ProBook 15", 1, 1299.99m) } },
            new { Customer = customers[2], Number = "FT-2026/001", Date = new DateTime(2026, 1, 15), Status = InvoiceStatus.Paid, Total = 2399.96m, Lines = new[] { ("Cadeira de Escritório Ergo", 4, 349.99m), ("Secretária Elétrica Ajustável", 2, 599.99m) } },
            new { Customer = customers[3], Number = "FT-2026/002", Date = new DateTime(2026, 2, 20), Status = InvoiceStatus.Paid, Total = 1799.97m, Lines = new[] { ("Portátil ProBook 15", 1, 1299.99m), ("Monitor 27\" 4K", 1, 499.99m) } },
            new { Customer = customers[0], Number = "FT-2026/003", Date = new DateTime(2026, 3, 5), Status = InvoiceStatus.Paid, Total = 89.99m, Lines = new[] { ("Teclado Mecânico RGB", 1, 89.99m) } },
            new { Customer = customers[4], Number = "FT-2026/004", Date = new DateTime(2026, 3, 18), Status = InvoiceStatus.Paid, Total = 449.98m, Lines = new[] { ("Tinteiro Canon 545", 10, 29.99m), ("Resma Papel A4 80g", 20, 5.99m) } },
            new { Customer = customers[5], Number = "FT-2026/005", Date = new DateTime(2026, 4, 2), Status = InvoiceStatus.Pending, Total = 799.98m, Lines = new[] { ("Cabo USB-C 2m", 50, 12.99m), ("Disco SSD 1TB", 2, 119.99m) } },
            new { Customer = customers[6], Number = "FT-2026/006", Date = new DateTime(2026, 4, 15), Status = InvoiceStatus.Paid, Total = 2599.98m, Lines = new[] { ("Cadeira de Escritório Ergo", 3, 349.99m), ("Secretária Elétrica Ajustável", 2, 599.99m), ("Monitor 27\" 4K", 1, 499.99m) } },
            new { Customer = customers[7], Number = "FT-2026/007", Date = new DateTime(2026, 5, 3), Status = InvoiceStatus.Paid, Total = 179.98m, Lines = new[] { ("Resma Papel A4 80g", 30, 5.99m) } },
            new { Customer = customers[8], Number = "FT-2026/008", Date = new DateTime(2026, 5, 20), Status = InvoiceStatus.Paid, Total = 1799.97m, Lines = new[] { ("Portátil ProBook 15", 1, 1299.99m), ("Monitor 27\" 4K", 1, 499.99m) } },
            new { Customer = customers[9], Number = "FT-2026/009", Date = new DateTime(2026, 6, 1), Status = InvoiceStatus.Paid, Total = 2450.00m, Lines = new[] { ("Consultoria TI (40h)", 40, 55.00m), ("Desenvolvimento Web (10h)", 10, 75.00m) } },
            new { Customer = customers[0], Number = "FT-2026/010", Date = new DateTime(2026, 6, 5), Status = InvoiceStatus.Paid, Total = 1299.99m, Lines = new[] { ("Portátil ProBook 15", 1, 1299.99m) } },
            new { Customer = customers[1], Number = "FT-2026/011", Date = new DateTime(2026, 6, 8), Status = InvoiceStatus.Pending, Total = 5750.00m, Lines = new[] { ("Servidor Dedicado", 1, 4500.00m), ("Instalação e Configuração", 1, 1250.00m) } },
            new { Customer = customers[2], Number = "FT-2026/012", Date = new DateTime(2026, 6, 10), Status = InvoiceStatus.Draft, Total = 890.00m, Lines = new[] { ("Formação TI (8h)", 8, 85.00m), ("Licenciamento Software", 1, 210.00m) } }
        };

        foreach (var inv in invoiceData)
        {
            var invoice = new Invoice
            {
                CustomerId = inv.Customer.Id,
                Number = inv.Number,
                Date = inv.Date,
                DueDate = inv.Date.AddDays(30),
                Status = inv.Status,
                TotalAmount = inv.Total
            };
            db.Invoices.Add(invoice);
            db.SaveChanges();

            foreach (var (desc, qty, price) in inv.Lines)
            {
                db.InvoiceLines.Add(new InvoiceLine
                {
                    InvoiceId = invoice.Id,
                    Description = desc,
                    Quantity = qty,
                    UnitPrice = price
                });
            }
            db.SaveChanges();
        }

        // === STOCK MOVEMENTS ===
        var stockMoves = new List<StockMovement>
        {
            new() { ProductId = products[0].Id, Type = MovementType.In, Quantity = 30, Date = new DateTime(2026, 1, 10), Notes = "Compra inicial" },
            new() { ProductId = products[1].Id, Type = MovementType.In, Quantity = 50, Date = new DateTime(2026, 1, 10), Notes = "Compra inicial" },
            new() { ProductId = products[7].Id, Type = MovementType.In, Quantity = 1000, Date = new DateTime(2026, 1, 15), Notes = "Stock papel A4" },
            new() { ProductId = products[0].Id, Type = MovementType.Out, Quantity = 5, Date = new DateTime(2026, 2, 5), Notes = "Venda FT-2026/002" },
            new() { ProductId = products[1].Id, Type = MovementType.Out, Quantity = 3, Date = new DateTime(2026, 2, 5), Notes = "Venda FT-2026/002" },
            new() { ProductId = products[5].Id, Type = MovementType.In, Quantity = 20, Date = new DateTime(2026, 2, 10), Notes = "Compra cadeiras" },
            new() { ProductId = products[5].Id, Type = MovementType.Out, Quantity = 4, Date = new DateTime(2026, 3, 1), Notes = "Venda FT-2026/001" },
            new() { ProductId = products[6].Id, Type = MovementType.In, Quantity = 15, Date = new DateTime(2026, 3, 5), Notes = "Compra secretárias" },
            new() { ProductId = products[6].Id, Type = MovementType.Out, Quantity = 2, Date = new DateTime(2026, 3, 10), Notes = "Venda FT-2026/001" },
            new() { ProductId = products[2].Id, Type = MovementType.In, Quantity = 100, Date = new DateTime(2026, 4, 1), Notes = "Compra teclados" },
            new() { ProductId = products[2].Id, Type = MovementType.Out, Quantity = 1, Date = new DateTime(2026, 4, 15), Notes = "Venda FT-2026/003" },
            new() { ProductId = products[11].Id, Type = MovementType.In, Quantity = 200, Date = new DateTime(2026, 4, 10), Notes = "Compra cabos USB-C" }
        };
        db.StockMovements.AddRange(stockMoves);
        db.SaveChanges();

        // === FINANCE RECORDS ===
        db.FinanceRecords.AddRange(
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2025/001 - Tech Solutions", Amount = 3499.97m, Date = new DateTime(2025, 11, 20), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2025/002 - Mega Construções", Amount = 1299.99m, Date = new DateTime(2025, 12, 28), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2026/001 - Saúde Primeira", Amount = 2399.96m, Date = new DateTime(2026, 2, 1), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2026/002 - Comércio Global", Amount = 1799.97m, Date = new DateTime(2026, 3, 5), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2026/003 - Tech Solutions", Amount = 89.99m, Date = new DateTime(2026, 3, 15), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2026/004 - Bom Gosto", Amount = 449.98m, Date = new DateTime(2026, 3, 28), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "FT-2026/007 - Farmácia Central", Amount = 179.98m, Date = new DateTime(2026, 5, 15), Category = "Faturação" },
            new FinanceRecord { Type = FinanceType.Income, Description = "Consultoria - vários clientes", Amount = 3500.00m, Date = new DateTime(2026, 4, 30), Category = "Serviços" },
            new FinanceRecord { Type = FinanceType.Income, Description = "Desenvolvimento web - projetos", Amount = 7200.00m, Date = new DateTime(2026, 5, 31), Category = "Serviços" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Renda escritório", Amount = 1200.00m, Date = new DateTime(2026, 1, 1), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Renda escritório", Amount = 1200.00m, Date = new DateTime(2026, 2, 1), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Renda escritório", Amount = 1200.00m, Date = new DateTime(2026, 3, 1), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Renda escritório", Amount = 1200.00m, Date = new DateTime(2026, 4, 1), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Renda escritório", Amount = 1200.00m, Date = new DateTime(2026, 5, 1), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Salários Janeiro", Amount = 8500.00m, Date = new DateTime(2026, 1, 31), Category = "Salários" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Salários Fevereiro", Amount = 8500.00m, Date = new DateTime(2026, 2, 28), Category = "Salários" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Salários Março", Amount = 9500.00m, Date = new DateTime(2026, 3, 31), Category = "Salários" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Salários Abril", Amount = 9500.00m, Date = new DateTime(2026, 4, 30), Category = "Salários" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Salários Maio", Amount = 9500.00m, Date = new DateTime(2026, 5, 31), Category = "Salários" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Material escritório", Amount = 350.00m, Date = new DateTime(2026, 2, 15), Category = "Material" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Equipamento informático", Amount = 2500.00m, Date = new DateTime(2026, 3, 10), Category = "Investimento" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Serviços cloud (Azure)", Amount = 450.00m, Date = new DateTime(2026, 4, 5), Category = "Cloud" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Eletricidade + Água", Amount = 280.00m, Date = new DateTime(2026, 4, 10), Category = "Instalações" },
            new FinanceRecord { Type = FinanceType.Expense, Description = "Marketing digital", Amount = 600.00m, Date = new DateTime(2026, 5, 10), Category = "Marketing" }
        );
        db.SaveChanges();

        // === WAREHOUSE ITEMS ===
        db.WarehouseItems.AddRange(
            new WarehouseItem { Code = "WH-001", Name = "Palete Papel A4", Description = "Palete com 50 resmas A4", Category = "Papelaria", Location = "A1", Quantity = 50, UnitCost = 4.50m },
            new WarehouseItem { Code = "WH-002", Name = "Caixa Toners HP", Description = "Caixa com 10 toners HP 26X", Category = "Consumíveis", Location = "B2", Quantity = 3, UnitCost = 75.00m },
            new WarehouseItem { Code = "WH-003", Name = "Palete Cadeiras Ergo", Description = "Palete com 5 cadeiras empilhadas", Category = "Mobiliário", Location = "C1", Quantity = 2, UnitCost = 280.00m },
            new WarehouseItem { Code = "WH-004", Name = "Caixa Ratos Sem Fios", Description = "Caixa com 20 ratos", Category = "Informática", Location = "A3", Quantity = 8, UnitCost = 25.00m },
            new WarehouseItem { Code = "WH-005", Name = "Rolo Cabo USB-C 50m", Description = "Rolo com 50m de cabo USB-C", Category = "Informática", Location = "B1", Quantity = 4, UnitCost = 8.50m }
        );
        db.SaveChanges();

        // === BUDGETS ===
        var budgets = new List<Budget>
        {
            new() { Number = "ORC-2026/001", CustomerId = customers[0].Id, Date = new DateTime(2026, 4, 10), ValidUntil = new DateTime(2026, 5, 10), Status = "Accepted", Notes = "Orçamento aprovado", TotalAmount = 22000m },
            new() { Number = "ORC-2026/002", CustomerId = customers[4].Id, Date = new DateTime(2026, 5, 5), ValidUntil = new DateTime(2026, 6, 5), Status = "Sent", Notes = "A aguardar decisão", TotalAmount = 12500m },
            new() { Number = "ORC-2026/003", CustomerId = customers[9].Id, Date = new DateTime(2026, 5, 20), ValidUntil = new DateTime(2026, 6, 20), Status = "Draft", Notes = "Em preparação", TotalAmount = 4500m }
        };
        db.Budgets.AddRange(budgets);
        db.SaveChanges();

        db.BudgetItems.AddRange(
            new BudgetItem { BudgetId = budgets[0].Id, Description = "Migração cloud infraestrutura", Quantity = 1, UnitPrice = 15000m },
            new BudgetItem { BudgetId = budgets[0].Id, Description = "Formação equipa (8h)", Quantity = 8, UnitPrice = 120m },
            new BudgetItem { BudgetId = budgets[0].Id, Description = "Suporte mensal (6 meses)", Quantity = 6, UnitPrice = 1000m },
            new BudgetItem { BudgetId = budgets[1].Id, Description = "App mobile iOS/Android", Quantity = 1, UnitPrice = 10000m },
            new BudgetItem { BudgetId = budgets[1].Id, Description = "Licenciamento anual", Quantity = 1, UnitPrice = 2500m },
            new BudgetItem { BudgetId = budgets[2].Id, Description = "Website institucional", Quantity = 1, UnitPrice = 3500m },
            new BudgetItem { BudgetId = budgets[2].Id, Description = "SEO + Marketing digital", Quantity = 1, UnitPrice = 1000m }
        );
        db.SaveChanges();

        // === PURCHASE ORDERS ===
        var pos = new List<PurchaseOrder>
        {
            new() { Number = "PO-2026/001", SupplierId = suppliers[2].Id, Date = new DateTime(2026, 1, 5), Status = "Received", Notes = "Encomenda equipamentos" },
            new() { Number = "PO-2026/002", SupplierId = suppliers[1].Id, Date = new DateTime(2026, 3, 1), Status = "Received", Notes = "Papel e consumíveis" },
            new() { Number = "PO-2026/003", SupplierId = suppliers[0].Id, Date = new DateTime(2026, 4, 10), Status = "Approved", Notes = "Cadeiras e secretárias" },
            new() { Number = "PO-2026/004", SupplierId = suppliers[4].Id, Date = new DateTime(2026, 6, 1), Status = "Pending", Notes = "Café e snacks escritório" }
        };
        db.PurchaseOrders.AddRange(pos);
        db.SaveChanges();

        db.PurchaseOrderItems.AddRange(
            new PurchaseOrderItem { PurchaseOrderId = pos[0].Id, Description = "Portátil ProBook 15", Quantity = 10, UnitPrice = 1100m },
            new PurchaseOrderItem { PurchaseOrderId = pos[0].Id, Description = "Monitor 27\" 4K", Quantity = 10, UnitPrice = 400m },
            new PurchaseOrderItem { PurchaseOrderId = pos[1].Id, Description = "Resma Papel A4 80g", Quantity = 100, UnitPrice = 4.50m },
            new PurchaseOrderItem { PurchaseOrderId = pos[2].Id, Description = "Cadeira de Escritório Ergo", Quantity = 5, UnitPrice = 280m },
            new PurchaseOrderItem { PurchaseOrderId = pos[2].Id, Description = "Secretária Elétrica Ajustável", Quantity = 3, UnitPrice = 500m },
            new PurchaseOrderItem { PurchaseOrderId = pos[3].Id, Description = "Café grão 1kg", Quantity = 10, UnitPrice = 12m },
            new PurchaseOrderItem { PurchaseOrderId = pos[3].Id, Description = "Bolachas variadas pack", Quantity = 5, UnitPrice = 8m }
        );
        db.SaveChanges();

        // === TRANSPORT GUIDES ===
        var guides = new List<TransportGuide>
        {
            new() { Number = "GT-2026/001", CustomerId = customers[0].Id, Origin = "Armazém Porto", Destination = "Tech Solutions, Av. Boavista", Carrier = "CTT Expresso", Date = new DateTime(2026, 2, 6), Notes = "Entrega portáteis" },
            new() { Number = "GT-2026/002", CustomerId = customers[2].Id, Origin = "Armazém Porto", Destination = "Saúde Primeira, Coimbra", Carrier = "Transportes Rápidos", Date = new DateTime(2026, 3, 2), Notes = "Entrega mobiliário" },
            new() { Number = "GT-2026/003", CustomerId = customers[5].Id, Origin = "Armazém Porto", Destination = "Hotel Paraíso, Algarve", Carrier = "SEUR", Date = new DateTime(2026, 4, 5), Notes = "Material informático" }
        };
        db.TransportGuides.AddRange(guides);
        db.SaveChanges();

        db.TransportGuideItems.AddRange(
            new TransportGuideItem { TransportGuideId = guides[0].Id, Description = "Portátil ProBook 15", Quantity = 2, Unit = "un" },
            new TransportGuideItem { TransportGuideId = guides[0].Id, Description = "Monitor 27\" 4K", Quantity = 2, Unit = "un" },
            new TransportGuideItem { TransportGuideId = guides[1].Id, Description = "Cadeira Escritório Ergo", Quantity = 4, Unit = "un" },
            new TransportGuideItem { TransportGuideId = guides[1].Id, Description = "Secretária Elétrica", Quantity = 2, Unit = "un" },
            new TransportGuideItem { TransportGuideId = guides[2].Id, Description = "Disco SSD 1TB", Quantity = 10, Unit = "un" },
            new TransportGuideItem { TransportGuideId = guides[2].Id, Description = "Cabo USB-C 2m", Quantity = 30, Unit = "un" }
        );
        db.SaveChanges();

        // === DEBIT / CREDIT / PAYMENT NOTES ===
        db.DebitNotes.AddRange(
            new DebitNote { Number = "ND-2026/001", CustomerId = customers[1].Id, Amount = 250.00m, Reason = "Juros de mora FT-2025/002", Date = new DateTime(2026, 3, 1) }
        );

        db.CreditNotes.AddRange(
            new CreditNote { Number = "NC-2026/001", CustomerId = customers[3].Id, Amount = 99.99m, Reason = "Desconto comercial FT-2026/002", Date = new DateTime(2026, 3, 10) }
        );

        db.PaymentNotes.AddRange(
            new PaymentNote { Number = "NP-2026/001", CustomerId = customers[0].Id, Amount = 3499.97m, Method = PaymentMethod.BankTransfer, Date = new DateTime(2025, 11, 25), Notes = "Pagamento FT-2025/001" },
            new PaymentNote { Number = "NP-2026/002", CustomerId = customers[1].Id, Amount = 1299.99m, Method = PaymentMethod.ATM, Date = new DateTime(2025, 12, 30), Notes = "Pagamento FT-2025/002" },
            new PaymentNote { Number = "NP-2026/003", CustomerId = customers[7].Id, Amount = 179.98m, Method = PaymentMethod.Cash, Date = new DateTime(2026, 5, 18), Notes = "Pagamento FT-2026/007" }
        );

        db.AdvancePayments.AddRange(
            new AdvancePayment { EmployeeName = "Tiago Rodrigues", Amount = 300.00m, Date = new DateTime(2026, 5, 10), ExpectedReturnDate = new DateTime(2026, 6, 10), Reason = "Formação online", IsSettled = false },
            new AdvancePayment { EmployeeName = "Rui Oliveira", Amount = 500.00m, Date = new DateTime(2026, 4, 15), ExpectedReturnDate = new DateTime(2026, 5, 15), Reason = "Deslocações clientes", IsSettled = true }
        );

        db.CashRegisters.AddRange(
            new CashRegister { OpeningDate = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc), InitialBalance = 500.00m, ClosingDate = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc), FinalBalance = 1250.00m, Notes = "Abertura diária" },
            new CashRegister { OpeningDate = new DateTime(2026, 6, 2, 9, 0, 0, DateTimeKind.Utc), InitialBalance = 500.00m, Notes = "Abertura diária" }
        );
        db.SaveChanges();

        // === INVENTORY ===
        var inv1 = new Inventory { Name = "Inventário Junho 2026", Date = new DateTime(2026, 6, 1), Notes = "Inventário periódico armazém" };
        db.Inventories.Add(inv1);
        db.SaveChanges();

        db.InventoryItems.AddRange(
            new InventoryItem { InventoryId = inv1.Id, ProductId = products[0].Id, ExpectedQuantity = 25, ActualQuantity = 24 },
            new InventoryItem { InventoryId = inv1.Id, ProductId = products[1].Id, ExpectedQuantity = 40, ActualQuantity = 40 },
            new InventoryItem { InventoryId = inv1.Id, ProductId = products[7].Id, ExpectedQuantity = 500, ActualQuantity = 480 },
            new InventoryItem { InventoryId = inv1.Id, ProductId = products[2].Id, ExpectedQuantity = 60, ActualQuantity = 59 }
        );
        db.SaveChanges();
    }
}
