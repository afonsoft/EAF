---
name: design-patterns
license: UNLICENSED
description: 'Use when applying SOLID principles and GoF design patterns in EAF C# code, or refactoring to improve maintainability.'
metadata:
  version: '1.0.0'
---

# Design Patterns Skill

## When to Use This Skill

Use this skill when you need to:
- Implement SOLID principles in your code
- Apply Gang of Four (GoF) design patterns
- Choose appropriate architectural patterns
- Refactor code to improve design and maintainability
- Solve common design problems with proven patterns
- Create flexible and extensible software architecture

## Prerequisites

- Understanding of object-oriented programming concepts
- Familiarity with C# and .NET development
- Knowledge of basic software architecture principles
- Experience with dependency injection and interfaces

## Step-by-Step Workflows

### 1. SOLID Principles Implementation

**Input**: Code that violates SOLID principles  
**Output: Refactored code following SOLID principles

```
Apply SOLID principles to this code:

1. Single Responsibility: Separate concerns
2. Open/Closed: Design for extension, not modification
3. Liskov Substitution: Ensure substitutability
4. Interface Segregation: Create focused interfaces
5. Dependency Inversion: Depend on abstractions
```

### 2. Creational Patterns Implementation

**Input**: Object creation requirements  
**Output: Appropriate creational pattern implementation

```
Implement the right creational pattern for this scenario:

1. Choose between Factory, Abstract Factory, Builder, or Prototype
2. Define interfaces and concrete implementations
3. Implement the pattern with proper abstractions
4. Add configuration and dependency injection
5. Test the implementation with different scenarios
```

### 3. Structural Patterns Implementation

**Input**: System architecture requirements  
**Output: Appropriate structural pattern implementation

```
Implement structural patterns to solve these issues:

1. Choose between Adapter, Decorator, Facade, or Proxy
2. Define class relationships and interfaces
3. Implement the pattern with proper composition
4. Ensure flexibility and maintainability
5. Add comprehensive testing
```

## SOLID Principles

### Single Responsibility Principle (SRP)

**Before (Violates SRP):**
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    // Violates SRP - handles multiple responsibilities
    public void SaveToDatabase() { /* Database logic */ }
    public void SendWelcomeEmail() { /* Email logic */ }
    public string GenerateReport() { /* Report logic */ }
    public bool ValidateUser() { /* Validation logic */ }
}
```

**After (Follows SRP):**
```csharp
// User entity - only data
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Separate responsibilities
public interface IUserRepository
{
    void Save(User user);
    User GetById(int id);
}

public interface IEmailService
{
    void SendWelcomeEmail(User user);
}

public interface IUserValidator
{
    ValidationResult Validate(User user);
}

public interface IUserReportGenerator
{
    string GenerateReport(User user);
}

// Implementations
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void Save(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    public User GetById(int id)
    {
        return _context.Users.Find(id);
    }
}

public class EmailService : IEmailService
{
    public void SendWelcomeEmail(User user)
    {
        // Email sending logic
        Console.WriteLine($"Welcome email sent to {user.Email}");
    }
}
```

### Open/Closed Principle (OCP)

**Before (Violates OCP):**
```csharp
public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        if (order.Type == OrderType.Online)
        {
            // Process online order
        }
        else if (order.Type == OrderType.Phone)
        {
            // Process phone order
        }
        else if (order.Type == OrderType.InStore)
        {
            // Process in-store order
        }
        // Need to modify this class for new order types
    }
}
```

**After (Follows OCP):**
```csharp
// Abstract base class
public abstract class OrderProcessorBase
{
    public abstract void Process(Order order);
    
    // Common processing logic
    protected void ValidateOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));
        
        if (order.Items.Count == 0)
            throw new ArgumentException("Order must have items");
    }
}

// Concrete implementations
public class OnlineOrderProcessor : OrderProcessorBase
{
    public override void Process(Order order)
    {
        ValidateOrder(order);
        
        // Online-specific processing
        ProcessPayment(order);
        SendEmailConfirmation(order);
        ScheduleShipping(order);
    }
    
    private void ProcessPayment(Order order) { /* Payment logic */ }
    private void SendEmailConfirmation(Order order) { /* Email logic */ }
    private void ScheduleShipping(Order order) { /* Shipping logic */ }
}

public class PhoneOrderProcessor : OrderProcessorBase
{
    public override void Process(Order order)
    {
        ValidateOrder(order);
        
        // Phone-specific processing
        VerifyCustomer(order);
        ProcessPayment(order);
        CreateShippingLabel(order);
    }
    
    private void VerifyCustomer(Order order) { /* Verification logic */ }
    private void ProcessPayment(Order order) { /* Payment logic */ }
    private void CreateShippingLabel(Order order) { /* Label logic */ }
}

// Factory for creating processors
public class OrderProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public OrderProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public OrderProcessorBase CreateProcessor(OrderType orderType)
    {
        return orderType switch
        {
            OrderType.Online => _serviceProvider.GetRequiredService<OnlineOrderProcessor>(),
            OrderType.Phone => _serviceProvider.GetRequiredService<PhoneOrderProcessor>(),
            OrderType.InStore => _serviceProvider.GetRequiredService<InStoreOrderProcessor>(),
            _ => throw new NotSupportedException($"Order type {orderType} is not supported")
        };
    }
}
```

### Liskov Substitution Principle (LSP)

**Before (Violates LSP):**
```csharp
public class Bird
{
    public virtual void Fly()
    {
        Console.WriteLine("Bird is flying");
    }
}

public class Ostrich : Bird
{
    public override void Fly()
    {
        throw new InvalidOperationException("Ostriches cannot fly");
    }
}

// Problem: Cannot substitute Ostrich for Bird
Bird bird = new Ostrich();
bird.Fly(); // Throws exception
```

**After (Follows LSP):**
```csharp
// Base class with common behavior
public abstract class Bird
{
    public abstract void MakeSound();
    public abstract void Eat();
}

// Interface for flying capability
public interface IFlyingBird
{
    void Fly();
}

// Concrete implementations
public class Sparrow : Bird, IFlyingBird
{
    public override void MakeSound()
    {
        Console.WriteLine("Sparrow chirps");
    }
    
    public override void Eat()
    {
        Console.WriteLine("Sparrow eats seeds");
    }
    
    public void Fly()
    {
        Console.WriteLine("Sparrow flies");
    }
}

public class Ostrich : Bird
{
    public override void MakeSound()
    {
        Console.WriteLine("Ostrich booms");
    }
    
    public override void Eat()
    {
        Console.WriteLine("Ostrich eats plants");
    }
}

// Safe substitution
Bird bird1 = new Sparrow();
bird1.MakeSound(); // Works

Bird bird2 = new Ostrich();
bird2.MakeSound(); // Works

IFlyingBird flyingBird = new Sparrow();
flyingBird.Fly(); // Works
```

### Interface Segregation Principle (ISP)

**Before (Violates ISP):**
```csharp
public interface IMachine
{
    void Print();
    void Scan();
    void Fax();
    void Copy();
}

public class BasicPrinter : IMachine
{
    public void Print() { /* Implementation */ }
    
    public void Scan()
    {
        throw new NotImplementedException("Basic printer cannot scan");
    }
    
    public void Fax()
    {
        throw new NotImplementedException("Basic printer cannot fax");
    }
    
    public void Copy()
    {
        throw new NotImplementedException("Basic printer cannot copy");
    }
}
```

**After (Follows ISP):**
```csharp
// Segregated interfaces
public interface IPrinter
{
    void Print();
}

public interface IScanner
{
    void Scan();
}

public interface IFaxMachine
{
    void Fax();
}

public interface ICopier
{
    void Copy();
}

// Multi-function machine implements all interfaces
public class MultiFunctionMachine : IPrinter, IScanner, IFaxMachine, ICopier
{
    public void Print() { Console.WriteLine("Printing"); }
    public void Scan() { Console.WriteLine("Scanning"); }
    public void Fax() { Console.WriteLine("Faxing"); }
    public void Copy() { Console.WriteLine("Copying"); }
}

// Basic printer only implements what it needs
public class BasicPrinter : IPrinter
{
    public void Print() { Console.WriteLine("Printing"); }
}
```

### Dependency Inversion Principle (DIP)

**Before (Violates DIP):**
```csharp
public class OrderService
{
    private readonly SqlUserRepository _userRepository;
    private readonly SmtpEmailService _emailService;
    
    public OrderService()
    {
        _userRepository = new SqlUserRepository();
        _emailService = new SmtpEmailService();
    }
    
    public void ProcessOrder(Order order)
    {
        var user = _userRepository.GetById(order.UserId);
        _emailService.SendEmail(user.Email, "Order processed");
    }
}
```

**After (Follows DIP):**
```csharp
// High-level module depends on abstractions
public class OrderService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    
    public OrderService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }
    
    public void ProcessOrder(Order order)
    {
        var user = _userRepository.GetById(order.UserId);
        _emailService.SendEmail(user.Email, "Order processed");
    }
}

// Abstractions
public interface IUserRepository
{
    User GetById(int id);
}

public interface IEmailService
{
    void SendEmail(string email, string message);
}

// Low-level modules implement abstractions
public class SqlUserRepository : IUserRepository
{
    public User GetById(int id)
    {
        // SQL implementation
        return new User { Id = id, Email = "user@example.com" };
    }
}

public class SmtpEmailService : IEmailService
{
    public void SendEmail(string email, string message)
    {
        // SMTP implementation
        Console.WriteLine($"Email sent to {email}: {message}");
    }
}

// Dependency injection configuration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, SqlUserRepository>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<OrderService>();
        
        return services;
    }
}
```

## Creational Patterns

### Factory Method Pattern

```csharp
// Abstract creator
public abstract class DocumentCreator
{
    public abstract IDocument CreateDocument();
    
    public DocumentOperation CreateOperation()
    {
        var document = CreateDocument();
        return new DocumentOperation(document);
    }
}

// Concrete creators
public class ResumeCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new ResumeDocument();
    }
}

public class ReportCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new ReportDocument();
    }
}

// Products
public interface IDocument
{
    void Open();
    void Save();
    void Print();
}

public class ResumeDocument : IDocument
{
    public void Open() => Console.WriteLine("Opening resume document");
    public void Save() => Console.WriteLine("Saving resume document");
    public void Print() => Console.WriteLine("Printing resume document");
}

public class ReportDocument : IDocument
{
    public void Open() => Console.WriteLine("Opening report document");
    public void Save() => Console.WriteLine("Saving report document");
    public void Print() => Console.WriteLine("Printing report document");
}

// Usage
var creator = new ResumeCreator();
var operation = creator.CreateOperation();
operation.Execute();
```

### Abstract Factory Pattern

```csharp
// Abstract factory
public interface IUIElementFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    ILabel CreateLabel();
}

// Concrete factories
public class WindowsUIElementFactory : IUIElementFactory
{
    public IButton CreateButton()
    {
        return new WindowsButton();
    }
    
    public ITextBox CreateTextBox()
    {
        return new WindowsTextBox();
    }
    
    public ILabel CreateLabel()
    {
        return new WindowsLabel();
    }
}

public class MacUIElementFactory : IUIElementFactory
{
    public IButton CreateButton()
    {
        return new MacButton();
    }
    
    public ITextBox CreateTextBox()
    {
        return new MacTextBox();
    }
    
    public ILabel CreateLabel()
    {
        return new MacLabel();
    }
}

// Abstract products
public interface IButton
{
    void Render();
    void Click();
}

public interface ITextBox
{
    void Render();
    void SetText(string text);
}

public interface ILabel
{
    void Render();
    void SetText(string text);
}

// Concrete products
public class WindowsButton : IButton
{
    public void Render() => Console.WriteLine("Rendering Windows button");
    public void Click() => Console.WriteLine("Windows button clicked");
}

public class MacButton : IButton
{
    public void Render() => Console.WriteLine("Rendering Mac button");
    public void Click() => Console.WriteLine("Mac button clicked");
}

// Usage
IUIElementFactory factory = Platform.IsWindows ? 
    new WindowsUIElementFactory() : 
    new MacUIElementFactory();

var button = factory.CreateButton();
var textBox = factory.CreateTextBox();
var label = factory.CreateLabel();
```

### Builder Pattern

```csharp
// Product
public class Computer
{
    public string CPU { get; set; }
    public string GPU { get; set; }
    public string RAM { get; set; }
    public string Storage { get; set; }
    public bool HasWiFi { get; set; }
    public bool HasBluetooth { get; set; }
    
    public override string ToString()
    {
        return $"Computer: CPU={CPU}, GPU={GPU}, RAM={RAM}, Storage={Storage}, WiFi={HasWiFi}, Bluetooth={HasBluetooth}";
    }
}

// Builder interface
public interface IComputerBuilder
{
    IComputerBuilder SetCPU(string cpu);
    IComputerBuilder SetGPU(string gpu);
    IComputerBuilder SetRAM(string ram);
    IComputerBuilder SetStorage(string storage);
    IComputerBuilder AddWiFi();
    IComputerBuilder AddBluetooth();
    Computer Build();
}

// Concrete builder
public class GamingComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new Computer();
    
    public IComputerBuilder SetCPU(string cpu)
    {
        _computer.CPU = cpu;
        return this;
    }
    
    public IComputerBuilder SetGPU(string gpu)
    {
        _computer.GPU = gpu;
        return this;
    }
    
    public IComputerBuilder SetRAM(string ram)
    {
        _computer.RAM = ram;
        return this;
    }
    
    public IComputerBuilder SetStorage(string storage)
    {
        _computer.Storage = storage;
        return this;
    }
    
    public IComputerBuilder AddWiFi()
    {
        _computer.HasWiFi = true;
        return this;
    }
    
    public IComputerBuilder AddBluetooth()
    {
        _computer.HasBluetooth = true;
        return this;
    }
    
    public Computer Build()
    {
        return _computer;
    }
}

// Director
public class ComputerDirector
{
    public Computer BuildGamingComputer(IComputerBuilder builder)
    {
        return builder
            .SetCPU("Intel Core i9-12900K")
            .SetGPU("NVIDIA RTX 4090")
            .SetRAM("32GB DDR4")
            .SetStorage("1TB NVMe SSD")
            .AddWiFi()
            .AddBluetooth()
            .Build();
    }
    
    public Computer BuildOfficeComputer(IComputerBuilder builder)
    {
        return builder
            .SetCPU("Intel Core i5-12400")
            .SetGPU("Intel UHD Graphics")
            .SetRAM("16GB DDR4")
            .SetStorage("512GB SSD")
            .AddWiFi()
            .Build();
    }
}

// Usage
var director = new ComputerDirector();
var builder = new GamingComputerBuilder();

var gamingComputer = director.BuildGamingComputer(builder);
Console.WriteLine(gamingComputer);

var officeComputer = director.BuildOfficeComputer(builder);
Console.WriteLine(officeComputer);
```

### Singleton Pattern

```csharp
// Thread-safe singleton with lazy initialization
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = 
        new Lazy<Logger>(() => new Logger());
    
    private readonly List<string> _logs = new List<string>();
    
    private Logger() { }
    
    public static Logger Instance => _instance.Value;
    
    public void Log(string message)
    {
        var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}";
        _logs.Add(logEntry);
        Console.WriteLine(logEntry);
    }
    
    public IEnumerable<string> GetLogs()
    {
        return _logs.AsReadOnly();
    }
    
    public void ClearLogs()
    {
        _logs.Clear();
    }
}

// Generic singleton base class
public abstract class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
    
    public static T Instance => _instance.Value;
}

// Usage
public class ConfigurationManager : Singleton<ConfigurationManager>
{
    public string GetSetting(string key)
    {
        // Configuration logic
        return "Value";
    }
}
```

## Structural Patterns

### Adapter Pattern

```csharp
// Target interface
public interface IModernPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

// Adaptee (legacy system)
public class LegacyPaymentSystem
{
    public bool ProcessPayment(string accountNumber, decimal amount, string currency)
    {
        Console.WriteLine($"Legacy payment: {accountNumber}, {amount} {currency}");
        return true;
    }
}

// Adapter
public class LegacyPaymentAdapter : IModernPaymentGateway
{
    private readonly LegacyPaymentSystem _legacySystem;
    
    public LegacyPaymentAdapter(LegacyPaymentSystem legacySystem)
    {
        _legacySystem = legacySystem;
    }
    
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        return await Task.Run(() =>
        {
            var success = _legacySystem.ProcessPayment(
                request.AccountNumber,
                request.Amount,
                request.Currency
            );
            
            return new PaymentResult
            {
                Success = success,
                TransactionId = Guid.NewGuid().ToString(),
                Message = success ? "Payment processed" : "Payment failed"
            };
        });
    }
}

// Supporting types
public class PaymentRequest
{
    public string AccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
}

// Usage
var legacySystem = new LegacyPaymentSystem();
var adapter = new LegacyPaymentAdapter(legacySystem);

var request = new PaymentRequest
{
    AccountNumber = "1234567890",
    Amount = 100.00m,
    Currency = "USD"
};

var result = await adapter.ProcessPaymentAsync(request);
```

### Decorator Pattern

```csharp
// Component interface
public interface INotificationService
{
    void SendNotification(string message, string recipient);
}

// Concrete component
public class EmailNotificationService : INotificationService
{
    public void SendNotification(string message, string recipient)
    {
        Console.WriteLine($"Email sent to {recipient}: {message}");
    }
}

// Base decorator
public abstract class NotificationDecorator : INotificationService
{
    protected readonly INotificationService _notificationService;
    
    protected NotificationDecorator(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    public virtual void SendNotification(string message, string recipient)
    {
        _notificationService.SendNotification(message, recipient);
    }
}

// Concrete decorators
public class SMSNotificationDecorator : NotificationDecorator
{
    public SMSNotificationDecorator(INotificationService notificationService)
        : base(notificationService)
    {
    }
    
    public override void SendNotification(string message, string recipient)
    {
        base.SendNotification(message, recipient);
        SendSMS(message, recipient);
    }
    
    private void SendSMS(string message, string recipient)
    {
        Console.WriteLine($"SMS sent to {recipient}: {message}");
    }
}

public class LoggingNotificationDecorator : NotificationDecorator
{
    public LoggingNotificationDecorator(INotificationService notificationService)
        : base(notificationService)
    {
    }
    
    public override void SendNotification(string message, string recipient)
    {
        Console.WriteLine($"[LOG] Sending notification to {recipient}");
        base.SendNotification(message, recipient);
        Console.WriteLine($"[LOG] Notification sent to {recipient}");
    }
}

// Usage
var emailService = new EmailNotificationService();
var smsDecorator = new SMSNotificationDecorator(emailService);
var loggingDecorator = new LoggingNotificationDecorator(smsDecorator);

loggingDecorator.SendNotification("Hello World", "user@example.com");
```

### Facade Pattern

```csharp
// Complex subsystem
public class DatabaseService
{
    public void Connect() => Console.WriteLine("Database connected");
    public void Disconnect() => Console.WriteLine("Database disconnected");
    public void ExecuteQuery(string query) => Console.WriteLine($"Executing: {query}");
}

public class CacheService
{
    public void Initialize() => Console.WriteLine("Cache initialized");
    public void Set(string key, object value) => Console.WriteLine($"Cache set: {key}");
    public object Get(string key) => Console.WriteLine($"Cache get: {key}");
    public void Clear() => Console.WriteLine("Cache cleared");
}

public class LoggingService
{
    public void Log(string message) => Console.WriteLine($"LOG: {message}");
    public void LogError(string error) => Console.WriteLine($"ERROR: {error}");
}

// Facade
public class DataAccessFacade
{
    private readonly DatabaseService _database;
    private readonly CacheService _cache;
    private readonly LoggingService _logger;
    
    public DataAccessFacade()
    {
        _database = new DatabaseService();
        _cache = new CacheService();
        _logger = new LoggingService();
        
        Initialize();
    }
    
    private void Initialize()
    {
        _logger.Log("Initializing data access facade");
        _database.Connect();
        _cache.Initialize();
        _logger.Log("Data access facade initialized");
    }
    
    public T GetData<T>(string key)
    {
        _logger.Log($"Getting data for key: {key}");
        
        // Try cache first
        var cachedData = _cache.Get(key);
        if (cachedData != null)
        {
            _logger.Log($"Data found in cache for key: {key}");
            return (T)cachedData;
        }
        
        // Get from database
        var query = $"SELECT * FROM Table WHERE Key = '{key}'";
        _database.ExecuteQuery(query);
        
        // Cache the result
        var data = default(T); // Simulate database result
        _cache.Set(key, data);
        
        _logger.Log($"Data retrieved and cached for key: {key}");
        return data;
    }
    
    public void SetData<T>(string key, T value)
    {
        _logger.Log($"Setting data for key: {key}");
        
        // Update database
        var query = $"UPDATE Table SET Value = '{value}' WHERE Key = '{key}'";
        _database.ExecuteQuery(query);
        
        // Update cache
        _cache.Set(key, value);
        
        _logger.Log($"Data set for key: {key}");
    }
    
    public void Dispose()
    {
        _logger.Log("Disposing data access facade");
        _cache.Clear();
        _database.Disconnect();
        _logger.Log("Data access facade disposed");
    }
}

// Usage
using var facade = new DataAccessFacade();
var data = facade.GetData<string>("user:123");
facade.SetData("user:123", "John Doe");
```

## Behavioral Patterns

### Strategy Pattern

```csharp
// Strategy interface
public interface IPaymentStrategy
{
    PaymentResult ProcessPayment(PaymentRequest request);
}

// Concrete strategies
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        Console.WriteLine("Processing credit card payment");
        // Credit card processing logic
        return new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString() };
    }
}

public class PayPalPaymentStrategy : IPaymentStrategy
{
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        Console.WriteLine("Processing PayPal payment");
        // PayPal processing logic
        return new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString() };
    }
}

public class BankTransferPaymentStrategy : IPaymentStrategy
{
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        Console.WriteLine("Processing bank transfer payment");
        // Bank transfer processing logic
        return new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString() };
    }
}

// Context
public class PaymentProcessor
{
    private IPaymentStrategy _paymentStrategy;
    
    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
    }
    
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        if (_paymentStrategy == null)
            throw new InvalidOperationException("Payment strategy not set");
            
        return _paymentStrategy.ProcessPayment(request);
    }
}

// Usage
var processor = new PaymentProcessor();
var request = new PaymentRequest { Amount = 100.00m, Currency = "USD" };

processor.SetPaymentStrategy(new CreditCardPaymentStrategy());
var result1 = processor.ProcessPayment(request);

processor.SetPaymentStrategy(new PayPalPaymentStrategy());
var result2 = processor.ProcessPayment(request);
```

### Observer Pattern

```csharp
// Subject interface
public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void UnregisterObserver(IObserver observer);
    void NotifyObservers();
}

// Observer interface
public interface IObserver
{
    void Update(ISubject subject);
}

// Concrete subject
public class Stock : ISubject
{
    private readonly List<IObserver> _observers = new List<IObserver>();
    private decimal _price;
    
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                NotifyObservers();
            }
        }
    }
    
    public string Symbol { get; }
    
    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }
    
    public void RegisterObserver(IObserver observer)
    {
        _observers.Add(observer);
    }
    
    public void UnregisterObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }
    
    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
}

// Concrete observers
public class Investor : IObserver
{
    public string Name { get; }
    private readonly List<Stock> _stocks = new List<Stock>();
    
    public Investor(string name)
    {
        Name = name;
    }
    
    public void Update(ISubject subject)
    {
        if (subject is Stock stock)
        {
            Console.WriteLine($"{Name} notified: {stock.Symbol} price changed to ${stock.Price}");
            
            // Auto-sell logic
            if (stock.Price > 100)
            {
                Console.WriteLine($"{Name} selling {stock.Symbol} at ${stock.Price}");
                stock.UnregisterObserver(this);
                _stocks.Remove(stock);
            }
        }
    }
    
    public void BuyStock(Stock stock)
    {
        _stocks.Add(stock);
        stock.RegisterObserver(this);
        Console.WriteLine($"{Name} bought {stock.Symbol} at ${stock.Price}");
    }
}

public class TradingBot : IObserver
{
    public void Update(ISubject subject)
    {
        if (subject is Stock stock)
        {
            Console.WriteLine($"Bot analyzing: {stock.Symbol} at ${stock.Price}");
            
            // Trading logic
            if (stock.Price < 50)
            {
                Console.WriteLine($"Bot recommends buying {stock.Symbol}");
            }
            else if (stock.Price > 150)
            {
                Console.WriteLine($"Bot recommends selling {stock.Symbol}");
            }
        }
    }
}

// Usage
var appleStock = new Stock("AAPL", 150.00m);
var googleStock = new Stock("GOOGL", 100.00m);

var investor1 = new Investor("John");
var investor2 = new Investor("Jane");
var tradingBot = new TradingBot();

investor1.BuyStock(appleStock);
investor2.BuyStock(appleStock);
tradingBot.Update(appleStock);

appleStock.Price = 120.00m; // Triggers notifications
appleStock.Price = 160.00m; // Triggers notifications and auto-sell
```

### Command Pattern

```csharp
// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver
public class Light
{
    public void TurnOn() => Console.WriteLine("Light is ON");
    public void TurnOff() => Console.WriteLine("Light is OFF");
    public void Dim(int level) => Console.WriteLine($"Light dimmed to {level}%");
}

// Concrete commands
public class LightOnCommand : ICommand
{
    private readonly Light _light;
    
    public LightOnCommand(Light light)
    {
        _light = light;
    }
    
    public void Execute()
    {
        _light.TurnOn();
    }
    
    public void Undo()
    {
        _light.TurnOff();
    }
}

public class LightOffCommand : ICommand
{
    private readonly Light _light;
    
    public LightOffCommand(Light light)
    {
        _light = light;
    }
    
    public void Execute()
    {
        _light.TurnOff();
    }
    
    public void Undo()
    {
        _light.TurnOn();
    }
}

public class LightDimCommand : ICommand
{
    private readonly Light _light;
    private readonly int _level;
    private int _previousLevel;
    
    public LightDimCommand(Light light, int level)
    {
        _light = light;
        _level = level;
    }
    
    public void Execute()
    {
        _previousLevel = GetCurrentLightLevel(); // Simulate getting current level
        _light.Dim(_level);
    }
    
    public void Undo()
    {
        _light.Dim(_previousLevel);
    }
    
    private int GetCurrentLightLevel()
    {
        // Simulate getting current light level
        return 100;
    }
}

// Invoker
public class RemoteControl
{
    private ICommand? _command;
    private Stack<ICommand> _commandHistory = new Stack<ICommand>();
    
    public void SetCommand(ICommand command)
    {
        _command = command;
    }
    
    public void PressButton()
    {
        if (_command != null)
        {
            _command.Execute();
            _commandHistory.Push(_command);
        }
    }
    
    public void PressUndo()
    {
        if (_commandHistory.Count > 0)
        {
            var lastCommand = _commandHistory.Pop();
            lastCommand.Undo();
        }
    }
}

// Macro command
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands;
    
    public MacroCommand(List<ICommand> commands)
    {
        _commands = commands;
    }
    
    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }
    
    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}

// Usage
var light = new Light();
var lightOn = new LightOnCommand(light);
var lightOff = new LightOffCommand(light);
var lightDim = new LightDimCommand(light, 50);

var remote = new RemoteControl();

remote.SetCommand(lightOn);
remote.PressButton();

remote.SetCommand(lightDim);
remote.PressButton();

remote.PressUndo();

// Macro command
var partyCommands = new List<ICommand>
{
    new LightOnCommand(light),
    new LightDimCommand(light, 75)
};

var partyMode = new MacroCommand(partyCommands);
remote.SetCommand(partyMode);
remote.PressButton();
```

## Architectural Patterns

### Repository Pattern

```csharp
// Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Generic repository implementation
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

// Specific repository interface
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}

// Specific repository implementation
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet.Where(u => u.IsActive).ToListAsync();
    }
}
```

### Unit of Work Pattern

```csharp
// Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

// Unit of Work implementation
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Orders = new OrderRepository(_context);
        Products = new ProductRepository(_context);
    }
    
    public IUserRepository Users { get; }
    public IOrderRepository Orders { get; }
    public IProductRepository Products { get; }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

// Usage
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found");
            
            var order = new Order
            {
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };
            
            order = await _unitOfWork.Orders.AddAsync(order);
            
            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product {item.ProductId} not found");
                
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                
                await _unitOfWork.OrderItems.AddAsync(orderItem);
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            
            return order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

## Best Practices

### Do's
- ✅ Choose the right pattern for the problem
- ✅ Implement patterns with proper abstractions
- ✅ Use dependency injection for loose coupling
- ✅ Follow SOLID principles consistently
- ✅ Test pattern implementations thoroughly
- ✅ Document pattern usage and intent
- ✅ Consider performance implications
- ✅ Keep patterns simple and focused
- ✅ Use established naming conventions
- ✅ Refactor code incrementally

### Don'ts
- ❌ Over-engineer with unnecessary patterns
- ❌ Mix multiple patterns inappropriately
- ❌ Ignore performance considerations
- ❌ Create tightly coupled implementations
- ❌ Skip testing pattern implementations
- ❌ Use patterns without understanding them
- ❌ Ignore SOLID principles
- ❌ Create overly complex abstractions
- ❌ Forget to document pattern usage
- ❌ Apply patterns universally

## References

- [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Microsoft Design Pattern Guidelines](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff650760(v=msp.10))
- [C# Design Patterns](https://www.dofactory.com/net/design-patterns)

---

**This skill provides comprehensive guidance for implementing design patterns and SOLID principles in .NET applications. Use it to create maintainable, extensible, and well-architected software solutions.**
