CREATE TABLE Customers (
    Id VARCHAR(500) PRIMARY KEY ,

    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),

    Email NVARCHAR(255),
    Phone NVARCHAR(20),

    Gender INT, -- 0: Unknown, 1: Male, 2: Female, 3: Other
    DateOfBirth DATE,

    AcceptMarketing BIT DEFAULT 0,
    Note nvarchar(2000),
    CustomerGroupId  VARCHAR(255),

    CreatedAt datetime,
    CreatedBy NVARCHAR(500),

    UpdatedAt datetime NULL,
    UpdatedBy NVARCHAR(500)
);
CREATE TABLE CustomerGroups (
   Id VARCHAR(500) PRIMARY KEY ,
    Name NVARCHAR(255) NOT NULL,
    Note NVARCHAR(2550),
    Type varchar(350)
  
);

CREATE TABLE CustomerAddresses (
      Id VARCHAR(500) PRIMARY KEY ,

    CustomerId VARCHAR(500) NOT NULL,

    FirstName NVARCHAR(1000),
    LastName NVARCHAR(1000),
    Company NVARCHAR(255),

    Phone NVARCHAR(20),

    Country NVARCHAR(1000),
    Province NVARCHAR(1000),
    District NVARCHAR(1000),
    Ward NVARCHAR(1000),

    AddressLine NVARCHAR(500),
    PostalCode NVARCHAR(20),

    IsDefault BIT DEFAULT 0,

    CreatedAt datetime,
    CreatedBy NVARCHAR(500),

    UpdatedAt datetime NULL,
    UpdatedBy NVARCHAR(500)

    
);



CREATE TABLE CustomerTaxInfos (
    Id VARCHAR(500) PRIMARY KEY,

    CustomerId VARCHAR(500) NOT NULL,

    -- 🧾 Thông tin hóa đơn
    CompanyName NVARCHAR(500),       -- Tên công ty
    TaxCode NVARCHAR(50),            -- Mã số thuế
    Address NVARCHAR(500),           -- Địa chỉ công ty

    BuyerName NVARCHAR(255),         -- Tên người mua
    CardId NVARCHAR(50),          -- CCCD / CMND

    BudgetCode NVARCHAR(100),        -- Mã đơn vị quan hệ ngân sách

    Phone NVARCHAR(20),              -- SĐT nhận hóa đơn
    Email NVARCHAR(255),             -- Email nhận hóa đơn

    -- ⚙️ control
    IsActive BIT DEFAULT 1,          -- có bật xuất hóa đơn không

    CreatedAt DATETIME,
    CreatedBy NVARCHAR(500),

    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(500)
);
CREATE INDEX IX_Customers_Email ON Customers(Email);
CREATE INDEX IX_Customers_Phone ON Customers(Phone);

CREATE INDEX IX_CustomerAddresses_CustomerId 
ON CustomerAddresses(CustomerId);


-- 1. Bảng Đơn hàng
-- 1. Bảng Đơn hàng (Bổ sung BranchId, SourceId, DeliveryDate)
CREATE TABLE Orders (
    Id nvarchar(50) NOT NULL PRIMARY KEY,
    OrderCode nvarchar(20) NOT NULL,
    CustomerId nvarchar(50) NULL,
    WarehouseId nvarchar(50) NULL,
    BranchId nvarchar(50) NULL,      -- THIẾU: Chi nhánh
    SourceId nvarchar(50) NULL,      -- THIẾU: Nguồn đơn hàng (Facebook, Website, POS...)
    StaffId nvarchar(50) NULL,
    TotalAmount decimal(18, 2) DEFAULT 0,
    Discount decimal(18, 2) DEFAULT 0,
    ShippingFee decimal(18, 2) DEFAULT 0,
    FinalAmount decimal(18, 2) DEFAULT 0,
    PaidAmount decimal(18, 2) DEFAULT 0,
    PaymentStatus nvarchar(20) NULL, 
    PaymentMethod nvarchar(50) NULL,
    ShippingMethod nvarchar(50) NULL,
    Note nvarchar(MAX) NULL,
    OrderDate datetime NULL,
    DeliveryDate datetime NULL,      -- THIẾU: Ngày hẹn giao
    CreatedAt datetime DEFAULT GETDATE(),
    CreatedBy nvarchar(50) NULL
);

-- 2. Bảng Chi tiết đơn hàng (Giữ nguyên)
CREATE TABLE OrderItems (
    Id nvarchar(50) NOT NULL PRIMARY KEY,
    OrderId nvarchar(50) NOT NULL,
    ProductId nvarchar(50) NOT NULL,
    VariantId nvarchar(50) NULL,
    Quantity int NOT NULL,
    UnitPrice decimal(18, 2) NOT NULL,
    TotalPrice decimal(18, 2) NOT NULL
);

-- 3. Bảng Thông tin giao hàng (Sửa lại tên cột cho khớp với Entity)
CREATE TABLE OrderShippingInfo (
    OrderId nvarchar(50) NOT NULL PRIMARY KEY,
    CodAmount decimal(18, 2) DEFAULT 0, -- Cột này đã có trong báo lỗi
    Weight float DEFAULT 0,
    Length float DEFAULT 0,
    Width float DEFAULT 0,
    Height float DEFAULT 0,
    ShippingNote nvarchar(500) NULL,    -- SỬA: Từ DeliveryNote thành ShippingNote
    DeliveryRequirement nvarchar(100) NULL -- SỬA: Từ Requirement thành DeliveryRequirement
);

-- 4. Bảng Menu (Giữ nguyên)
CREATE TABLE MenuItems (
    Id nvarchar(150) NOT NULL PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Path NVARCHAR(1500),
    Icon NVARCHAR(100),
    ParentId nvarchar(150) NULL,
    SortOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1
);

-- 2. Bảng Chi tiết đơn hàng
CREATE TABLE OrderItems (
    Id nvarchar(50) NOT NULL PRIMARY KEY,
    OrderId nvarchar(50) NOT NULL,
    ProductId nvarchar(50) NOT NULL,
    VariantId nvarchar(50) NULL,
    Quantity int NOT NULL,
    UnitPrice decimal(18, 2) NOT NULL,
    TotalPrice decimal(18, 2) NOT NULL
    
);

-- 3. Bảng Thông tin giao hàng
CREATE TABLE OrderShippingInfo (
    OrderId nvarchar(50) NOT NULL PRIMARY KEY,
    CodAmount decimal(18, 2) DEFAULT 0,
    Weight float DEFAULT 0,
    Length float DEFAULT 0,
    Width float DEFAULT 0,
    Height float DEFAULT 0,
    DeliveryNote nvarchar(500) NULL,
    Requirement nvarchar(100) NULL
);

-- tao menu
CREATE TABLE MenuItems (
    Id nvarchar(150) NOT NULL PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,      -- Tên hiển thị (VD: Đơn hàng)
    Path NVARCHAR(1500),               -- Đường dẫn URL (VD: /admin/orders)
    Icon NVARCHAR(100),               -- Class icon (FontAwesome hoặc SVG name)
    ParentId nvarchar(150) NULL,                -- ID của menu cha (NULL nếu là menu cấp cao nhất)
    SortOrder INT DEFAULT 0,          -- Thứ tự hiển thị
    IsActive BIT DEFAULT 1
);