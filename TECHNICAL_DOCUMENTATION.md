# Peso Based Barcode Printing System API

## Technical Documentation

**Version:** 1.0  
**Last Updated:** February 2026  
**Document Type:** Comprehensive Technical Specification

---

## Table of Contents

1. [Introduction & System Summary](#1-introduction--system-summary)
2. [Technology Stack](#2-technology-stack)
3. [Project Structure Overview](#3-project-structure-overview)
4. [Architecture Overview](#4-architecture-overview)
5. [Business Logic & Workflows](#5-business-logic--workflows)
6. [Database Schema Documentation](#6-database-schema-documentation)
7. [Models & Relationships](#7-models--relationships)
8. [API Endpoints Reference](#8-api-endpoints-reference)
9. [Security & Authentication](#9-security--authentication)
10. [Dependencies & Integrations](#10-dependencies--integrations)
11. [Key Design Decisions](#11-key-design-decisions)

---

## 1. Introduction & System Summary

### 1.1 System Overview

The **Peso Based Barcode Printing System API** is an enterprise-grade backend solution designed for managing barcode generation, product tracking, and dispatch operations in an explosives manufacturing and distribution environment. The system is specifically built for regulatory compliance with PESO (Petroleum and Explosives Safety Organisation) requirements.

### 1.2 Core Purpose

The system provides:
- **Multi-level Barcode Generation**: L1 (Case/Box), L2 (Bundle), and L3 (Unit) barcode hierarchies
- **Magazine (Storage) Management**: Tracking explosives storage and stock levels
- **Dispatch & Transportation**: Managing shipment operations with full traceability
- **Regulatory Compliance**: RE-2, RE-6, RE-11, RE-12 form generation for PESO
- **Real-time Notifications**: WebSocket-based communication for live updates

### 1.3 Key Business Functions

| Function | Description |
|----------|-------------|
| Barcode Generation | Create hierarchical barcodes (L1 → L2 → L3) for products |
| Production Planning | Plan manufacturing schedules and track output |
| Magazine Stock Management | Track inventory in explosive storage magazines |
| Indent Processing | Process RE-11 indent orders from customers |
| Dispatch Management | Handle loading, transportation, and delivery |
| Regulatory Reporting | Generate RE-2, RE-6, RE-11, RE-12 compliance forms |

---

## 2. Technology Stack

### 2.1 Core Framework

| Component | Technology | Version |
|-----------|------------|---------|
| **Runtime** | .NET | 8.0 |
| **Framework** | ASP.NET Core Web API | 8.0 |
| **Language** | C# | Latest |

### 2.2 Data & Persistence

| Component | Technology | Version |
|-----------|------------|---------|
| **Database** | PostgreSQL | N/A |
| **ORM** | Entity Framework Core | 8.0.11 |
| **DB Provider** | Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.11 |
| **Caching** | Redis (Distributed Cache) | 2.1.2 |
| **Bulk Operations** | EFCore.BulkExtensions | 8.1.1 |

### 2.3 Security & Authentication

| Component | Technology | Version |
|-----------|------------|---------|
| **Authentication** | JWT Bearer Tokens | 8.0.11 |
| **Token Library** | Microsoft.IdentityModel.Tokens | Included |
| **Password Hashing** | ASP.NET Identity PasswordHasher | Built-in |

### 2.4 Real-time Communication

| Component | Technology |
|-----------|-------------|
| **WebSockets** | Microsoft.AspNetCore.WebSockets |
| **Message Broker** | Redis Pub/Sub |

### 2.5 Document Processing

| Component | Technology | Version |
|-----------|------------|---------|
| **Excel Processing** | EPPlus | 7.1.0 |
| **PDF Generation** | iText7 | 8.0.5 |
| **PDF Utilities** | PdfSharpCore | 1.3.67 |
| **OpenXML** | DocumentFormat.OpenXml | 3.3.0 |

### 2.6 Utilities & Logging

| Component | Technology | Version |
|-----------|------------|---------|
| **Object Mapping** | AutoMapper | 13.0.1 |
| **Logging** | Serilog | 4.3.0+ |
| **API Documentation** | Swashbuckle (Swagger) | 7.0.0 |
| **JSON Serialization** | Newtonsoft.Json | Via MVC |

---

## 3. Project Structure Overview

```
Peso_Baseed_Barcode_Printing_System_API/
│
├── Configurations/
│   └── AutoMapperConfig.cs          # AutoMapper profile definitions
│
├── Controllers/                      # 65 API Controllers
│   ├── LoginController.cs           # Authentication
│   ├── L1GenerateController.cs      # L1 Barcode Generation
│   ├── DispatchTransactionsController.cs  # Dispatch Management
│   ├── Re11IndentInfosController.cs # Indent Processing
│   ├── ReportsController.cs         # Reporting Engine
│   └── ... (60+ additional controllers)
│
├── DBContext/
│   └── ApplicationDbContext.cs      # EF Core DbContext with all entities
│
├── Interface/
│   ├── IBatchDetailsRepository.cs
│   └── IBatchMasterRepository.cs
│
├── Migrations/                       # 93 EF Core Migrations
│
├── Models/                           # 75 Entity Models
│   ├── L1barcodegeneration.cs       # L1 Barcode Entity
│   ├── L2barcodegeneration.cs       # L2 Barcode Entity
│   ├── L3barcodegeneration.cs       # L3 Barcode Entity
│   ├── ProductMaster.cs             # Product Configuration
│   ├── MagzineMaster.cs             # Magazine/Warehouse
│   ├── DispatchTransaction.cs       # Dispatch Records
│   └── ...
│   │
│   └── DTO/                          # 42 Data Transfer Objects
│       ├── APIResponse.cs           # Standard API Response
│       ├── LoginDTO.cs              # Login Request
│       ├── Re11IndentInfoViewModel.cs
│       └── ...
│
├── PRNFile/                          # Printer Command Files (ZPL/PRN)
│
├── Repositorys/                      # 112 Repository Files (Interfaces + Implementations)
│   ├── GenericRepository.cs         # Base Repository
│   ├── IGenericRepository.cs        # Generic Interface
│   ├── L1barcodegenerationRepository.cs
│   └── ...
│
├── Services/                         # Business Services
│   ├── WebSocketService.cs          # Real-time Communication
│   ├── ProductionPlanService.cs     # Production Planning Logic
│   ├── L1DetailsService.cs          # L1 Barcode Details
│   ├── PdfReaderService.cs          # PDF Processing
│   └── EncryptionHelper.cs          # Password Encryption
│
├── wwwroot/                          # Static Files
│   └── PRNFile/                     # Label Print Templates
│
├── Program.cs                        # Application Entry Point & DI Configuration
├── appsettings.json                  # Application Configuration
└── Peso_Baseed_Barcode_Printing_System_API.csproj
```

---

## 4. Architecture Overview

### 4.1 Architectural Pattern

The application follows a **Repository Pattern** with a **Service Layer Architecture**:

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                             │
│         (Web Applications, Mobile Apps, HHT Devices)            │
└─────────────────────────────────────────────────────────────────┘
                              │
                    ┌─────────▼─────────┐
                    │  WebSocket (/ws)  │  Real-time Updates
                    └─────────┬─────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│                       API LAYER (Controllers)                    │
│   ┌──────────────┬──────────────┬──────────────┬──────────────┐ │
│   │ Login        │ L1Generate   │ Dispatch     │ Reports      │ │
│   │ Controller   │ Controller   │ Controller   │ Controller   │ │
│   └──────────────┴──────────────┴──────────────┴──────────────┘ │
│   JWT Authentication │ CORS │ Authorization                     │
└─────────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│                      SERVICE LAYER                               │
│   ┌────────────────────────────────────────────────────────────┐ │
│   │ ProductionPlanService │ WebSocketService │ PdfReaderService│ │
│   └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│                    REPOSITORY LAYER                              │
│   ┌────────────────────────────────────────────────────────────┐ │
│   │  GenericRepository<T>  │  IGenericRepository<T>            │ │
│   ├────────────────────────────────────────────────────────────┤ │
│   │ L1BarcodeRepository │ DispatchRepository │ XXXRepository   │ │
│   └────────────────────────────────────────────────────────────┘ │
│   AutoMapper (Entity ↔ DTO Mapping)                             │
└─────────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│                      DATA LAYER                                  │
│   ┌─────────────────────────┬─────────────────────────────────┐ │
│   │   ApplicationDbContext  │ Entity Framework Core + Npgsql   │ │
│   └─────────────────────────┴─────────────────────────────────┘ │
│                   EFCore.BulkExtensions                         │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│  PostgreSQL   │    │    Redis      │    │  File System  │
│   Database    │    │    Cache      │    │  (PRN Files)  │
└───────────────┘    └───────────────┘    └───────────────┘
```

### 4.2 Component Interaction Flow

```
Client Request
      │
      ▼
┌─────────────────────────┐
│   JWT Validation        │ ← Validate Token
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│   Controller Action     │ ← Route to Endpoint
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│   Repository Layer      │ ← Business Logic & Data Access
└───────────┬─────────────┘
            │
      ┌─────┴─────┐
      ▼           ▼
┌───────────┐ ┌───────────┐
│ DbContext │ │   Redis   │
└─────┬─────┘ └─────┬─────┘
      │             │
      ▼             ▼
┌───────────┐ ┌───────────┐
│PostgreSQL │ │   Cache   │
└───────────┘ └───────────┘
```

### 4.3 Key Architectural Features

1. **Generic Repository Pattern**: Provides common CRUD operations through `IGenericRepository<T>`
2. **Dependency Injection**: All repositories and services are registered in `Program.cs`
3. **AutoMapper Integration**: Entity-to-DTO mapping via `AutoMapperConfig`
4. **Distributed Caching**: Redis for session and data caching
5. **Real-time Communication**: WebSocket support with Redis Pub/Sub
6. **Bulk Operations**: EFCore.BulkExtensions for high-performance inserts

---

## 5. Business Logic & Workflows

### 5.1 Barcode Hierarchy System

The system implements a three-level barcode hierarchy for product tracking:

```
           ┌─────────────────────────────────────┐
           │          L1 BARCODE (Case)           │
           │  Primary ID: L1Barcode (string)      │
           │  Contains: Product Info, MfgDate     │
           │  Relationships: NoOfL2, NoOfL3       │
           └───────────────┬─────────────────────┘
                          │
          ┌───────────────┼───────────────┐
          ▼               ▼               ▼
    ┌──────────┐    ┌──────────┐    ┌──────────┐
    │L2 Barcode│    │L2 Barcode│    │L2 Barcode│
    │ (Bundle) │    │ (Bundle) │    │ (Bundle) │
    └────┬─────┘    └────┬─────┘    └────┬─────┘
         │               │               │
    ┌────┴────┐     ┌────┴────┐     ┌────┴────┐
    ▼    ▼    ▼     ▼    ▼    ▼     ▼    ▼    ▼
┌────┐┌────┐┌────┐┌────┐┌────┐┌────┐┌────┐┌────┐┌────┐
│ L3 ││ L3 ││ L3 ││ L3 ││ L3 ││ L3 ││ L3 ││ L3 ││ L3 │
│Unit││Unit││Unit││Unit││Unit││Unit││Unit││Unit││Unit│
└────┘└────┘└────┘└────┘└────┘└────┘└────┘└────┘└────┘
```

**L1 Barcode Structure:**
- Format: `{CountryCode}{MfgCode}{PlantCode}{MachineCode}{Shift}{BrandId}{PSizeCode}{Quarter}{Year}{Month}{SerialNo}`
- Example: `IN001ABM10001Q1240100001`

### 5.2 Core Workflows

#### 5.2.1 Barcode Generation Workflow

```
┌──────────────────┐
│ Production Plan  │
│   Created        │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ L1 Barcode       │
│ Generation       │  ← L1GenerateController.CreateL1Generate()
└────────┬─────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌───────┐ ┌───────┐
│L2 Gen │ │BarcodeData│
└───┬───┘ └───┬───┘
    │         │
    ▼         ▼
┌───────┐ ┌────────────┐
│L3 Gen │ │BarcodeDataFinal│
└───────┘ └────────────┘
```

#### 5.2.2 Dispatch Workflow

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 1: RE-11 INDENT CREATION                                   │
│ ┌────────────────┐    ┌─────────────────────┐                   │
│ │ Customer Order │───▶│ Re11IndentInfo     │                   │
│ └────────────────┘    │ + Re11IndentPrdInfo │                   │
│                       └─────────────────────┘                   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 2: LOADING SHEET CREATION                                  │
│ ┌──────────────────┐    ┌────────────────────────────┐          │
│ │ Assign Truck     │───▶│ AllLoadingSheet            │          │
│ │ Select Magazine  │    │ + AllLoadingIndentDeatils  │          │
│ └──────────────────┘    └────────────────────────────┘          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 3: BARCODE SCANNING & DISPATCH                             │
│ ┌─────────────────┐    ┌───────────────────────────┐            │
│ │ Scan L1 Barcodes│───▶│ DispatchTransaction       │            │
│ │ from Magazine   │    │ (Move Stock to Truck)     │            │
│ └─────────────────┘    └───────────────────────────┘            │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 4: RE-12 GENERATION & COMPLETION                           │
│ ┌──────────────────┐    ┌────────────────────────────┐          │
│ │ Mark Dispatch    │───▶│ Update Magazine Stock      │          │
│ │ Complete         │    │ Generate RE-12 Report      │          │
│ └──────────────────┘    └────────────────────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

#### 5.2.3 Magazine Stock Management

```
┌──────────────────────────────────────────────────────────────────┐
│                   MAGAZINE STOCK LIFECYCLE                        │
├──────────────────────────────────────────────────────────────────┤
│                                                                   │
│  Production ──┬──▶ RE-2 Form ──▶ Magazine Stock                   │
│  (L1 Cases)   │                   (Magzine_Stock)                │
│               │                                                   │
│               └──▶ formre2magallot ──▶ Stock Allocation          │
│                                                                   │
│  Magazine Stock ──▶ Dispatch ──▶ DispatchTransaction             │
│  (Available)        Scanning     (Stock Decreases)               │
│                                                                   │
│  DispatchTransaction ──▶ RE-12 Gen ──▶ Completed                 │
│                                                                   │
└──────────────────────────────────────────────────────────────────┘
```

### 5.3 Production Planning

The system supports production planning with the following flow:

1. **Create Production Plan** (`ProductionPlan`)
   - Specify: Plant, Brand, Product Size, Manufacturing Date
   - Set targets: Planned quantity, Shift

2. **Execute Production** (`L1GenerateController.CreateL1Generate`)
   - Generate L1, L2, L3 barcodes based on product configuration
   - Record in `BarcodeData` and `BarcodeDataFinal`

3. **Track Production** (`Getdashboardcard`, `Getproreport`)
   - Dashboard cards for daily/monthly statistics
   - Production reports by brand, size, date

---

## 6. Database Schema Documentation

### 6.1 Entity-Relationship Overview

The database contains approximately **50+ tables** organized into the following domains:

| Domain | Tables | Description |
|--------|--------|-------------|
| **Master Data** | 15+ | Plants, Brands, Products, Customers, Transporters |
| **Barcode** | 6 | L1, L2, L3 generation and tracking |
| **Magazine/Stock** | 4 | Storage and inventory management |
| **Dispatch** | 5 | Transportation and delivery |
| **Indent/Orders** | 4 | RE-11 indent processing |
| **Reporting** | 5+ | Various report tables |
| **System** | 3 | Users, Roles, Page Access |

### 6.2 Core Entities Documentation

#### 6.2.1 User Management

**Table: `Users`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Auto-generated user ID |
| Username | string | Unique, Indexed | Login username (also serves as PK) |
| PasswordHash | string | - | Encrypted password |
| Company_ID | string | - | Associated company identifier |
| Role | string | - | User role reference |

**Table: `RoleMasters`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Role identifier |
| RoleName | string | Indexed | Role name (Admin, User, etc.) |
| pageAccesses | List | Navigation | Child page access records |

**Table: `PageAccess`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Access record ID |
| RoleId | int | FK | Reference to RoleMaster |
| PageName | string | - | Page/Module name |
| IsAdd | bool | Default: false | Add permission |
| IsEdit | bool | Default: false | Edit permission |
| IsDelete | bool | Default: false | Delete permission |

---

#### 6.2.2 Master Data Entities

**Table: `PlantMasters`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Plant ID |
| plant_type | string | - | Type of plant |
| PName | string | Required | Plant name |
| PCode | string | Required | 2-character plant code |
| License | string | Required | PESO license number |
| Company_ID | string | - | Company reference |
| issue_dt | date | - | License issue date |
| validity_dt | date | - | License validity date |

**Table: `BrandMaster`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Brand ID |
| plant_type | string | Required | Associated plant type |
| bname | string | Required | Brand name |
| bid | string | Required, 4 chars | Brand code (e.g., "0001") |
| Class | int | Required | Explosive class (1-7) |
| Division | int | Required | Explosive division |
| unit | string | Required | Unit of measurement |

**Table: `ProductMaster`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Product ID |
| bname | string | Required | Brand name |
| bid | string | Required, 4 chars | Brand ID |
| ptype | string | Required | Plant type name |
| ptypecode | string | Required, 2 chars | Plant type code |
| Class | int | Required | Explosive class |
| Division | int | Required | Explosive division |
| unit | string | Required | Unit of measurement |
| psize | string | Required | Product size description |
| psizecode | string | Required, 3 chars | Size code |
| dimnesion | int | Required | Dimension in MM |
| dimunitwt | double | Required | Unit weight in GM |
| l1netwt | double | Required | Net weight per L1 case |
| noofl2 | int | Required | Number of L2 per L1 |
| noofl3perl2 | int | Required | Number of L3 per L2 |
| noofl3perl1 | int | Required | Total L3 per L1 |
| sdcat | string | Required | SD Category |
| unnoclass | string | Required | UN Number Class |
| act | bool | Required | Active flag |

**Table: `MagzineMasters`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Magazine ID |
| mfgloc | string | Required | Manufacturing location |
| mfgloccode | string | Required | Location code |
| magname | string | Required | Magazine name |
| mcode | string | Required | Magazine code |
| licno | string | Required | License number |
| issuedate | DateTime | Required | License issue date |
| validitydt | date | Required | License validity |
| Totalwt | decimal | Required | Total capacity (weight) |
| margin | int | - | Safety margin |
| autoallot_flag | bool | Default: false | Auto-allocation flag |
| MagzineMasterDetails | List | Navigation | Child details |

---

#### 6.2.3 Barcode Entities

**Table: `L1Barcodegeneration`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| L1Barcode | string | PK | Primary L1 barcode |
| SrNo | int | Required | Serial number |
| Country | string | Required | Country name |
| CountryCode | string | Required, 2 chars | Country code |
| MfgName | string | Required | Manufacturer name |
| MfgLoc | string | Required | Manufacturing location |
| MfgCode | string | Required, 3 chars | Manufacturer code |
| PlantName | string | Required | Plant name |
| PCode | string | Required, 2 chars | Plant code |
| MCode | string | Required, 1 char | Machine code |
| Shift | string | Required, 1 char | Shift (A/B/C) |
| BrandName | string | Required | Brand name |
| BrandId | string | Required, 4 chars | Brand code |
| ProductSize | string | Required | Product size |
| PSizeCode | string | Required, 3 chars | Size code |
| SdCat | string | Required | SD Category |
| UnNoClass | string | Required | UN Number Class |
| MfgDt | date | Required | Manufacturing date |
| Quarter | string | Required, 2 chars | Quarter (Q1-Q4) |
| MfgTime | timestamp | Default: now() | Manufacturing timestamp |
| Month | string | Required, 2 chars | Month (01-12) |
| GenYear | string | Required, 4 chars | Year (YYYY) |
| L1NetWt | double | Required | Net weight |
| L1NetUnit | string | Required | Weight unit |
| NoOfL2 | int | Required | L2 count in this L1 |
| NoOfL3 | int | Required | L3 count in this L1 |
| MFlag | int | Default: 0 | Magazine flag |
| CheckFlag | int | Default: 0 | Check flag |

**Index:** Composite on (PCode, MCode, BrandId, PSizeCode, Month, GenYear, Quarter, MfgDt, L1Barcode)

**Table: `L2Barcodegeneration`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| L2Barcode | string | PK | Primary L2 barcode |
| SrNo | long | Required | Serial number |
| L1Barcode | string | Required | Parent L1 reference |
| *(Same metadata fields as L1)* | | | |

**Table: `L3Barcodegeneration`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| L3Barcode | string | PK | Primary L3 barcode |
| SrNo | long | Required | Serial number |
| L1Barcode | string | Required | Parent L1 reference |
| L2Barcode | string | Required | Parent L2 reference |
| *(Same metadata fields as L1)* | | | |

---

#### 6.2.4 Stock & Magazine Entities

**Table: `Magzine_Stock`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| L1Barcode | string | PK | L1 barcode (unique stock item) |
| MagName | string | Required | Magazine name |
| BrandName | string | Required | Brand name |
| BrandId | string | Required | Brand ID |
| PrdSize | string | Required | Product size |
| PSizeCode | string | Required | Size code |
| Stock | int | Required, Default: 0 | Stock quantity (1 = present) |
| StkDt | date | Required | Stock date |
| Month | string | Required | Month |
| Year | string | Required | Year |
| Re2 | int | Required, Default: 0 | RE-2 flag |
| Re12 | int | Required, Default: 0 | RE-12 flag |

**Index:** Composite on (BrandId, PSizeCode, L1Barcode)

---

#### 6.2.5 Dispatch & Transaction Entities

**Table: `DispatchTransaction`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Tid | long | PK, Identity | Transaction ID |
| IndentNo | string | Required, 50 chars | RE-11 Indent number |
| L1Barcode | string | Required, 50 chars | L1 barcode dispatched |
| Brand | string | Required, 50 chars | Brand name |
| Bid | string | Required, 50 chars | Brand ID |
| PSize | string | Required, 50 chars | Product size |
| PSizeCode | string | Required, 50 chars | Size code |
| TruckNo | string | Required, 50 chars | Vehicle number |
| MagName | string | Required, 50 chars | Source magazine |
| DispDt | date | Required | Dispatch date |
| Month | string | Required, 20 chars | Month |
| Year | string | Required, 20 chars | Year |
| Re12 | int | Required, Default: 0 | RE-12 generated flag |
| L1NetWt | double | Required | Net weight |
| L1NetUnit | string | Required, 50 chars | Weight unit |

**Indexes:**
- (L1Barcode, Bid, PSizeCode)
- (MagName, Re12)

**Table: `AllLoadingSheet`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Loading sheet ID |
| LoadingSheetNo | string | - | Loading sheet number |
| Mfgdt | date | Default: now() | Manufacturing/Creation date |
| TName | string | - | Transporter name |
| TruckNo | string | - | Vehicle number |
| TruckLic | string | - | Truck license |
| LicVal | date | - | License validity |
| CreationDateTime | timestamp | - | Record creation time |
| Month | int | - | Month |
| Year | int | - | Year |
| Quarter | int | - | Quarter |
| Compflag | int | Default: 0 | Completion flag |
| IndentDetails | List | Navigation | Child indent details |

---

#### 6.2.6 Indent/Order Entities

**Table: `Re11IndentInfo`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| IndentNo | string | PK | Indent number |
| IndentDt | date | - | Indent date |
| PesoDt | date | - | PESO approval date |
| CustName | string | - | Customer name |
| ConName | string | - | Contact name |
| ConNo | string | - | Contact number |
| Clic | string | - | Customer license |
| Month | string | Required | Month |
| Year | string | Required | Year |
| CompletedIndent | int | Default: 0 | Completion flag |
| IndentItems | List | Navigation | Child product info |

**Table: `Re11IndentPrdInfo`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Record ID |
| IndentNo | string | FK, Required | Parent indent number |
| IndentDt | date | - | Indent date |
| Ptype | string | Required | Product type |
| PtypeCode | string | - | Product type code |
| Bname | string | Required | Brand name |
| Bid | string | Required, 4 chars | Brand ID |
| Psize | string | Required | Product size |
| SizeCode | string | Required, 3 chars | Size code |
| Class | long | Required | Explosive class |
| Div | long | Required | Division |
| L1NetWt | double | Required | Net weight per case |
| Unit | string | Required | Unit |
| ReqWt | double | Required | Required weight |
| ReqCase | int | Required | Required cases |
| RemWt | double | - | Remaining weight |
| Remcase | int | - | Remaining cases |
| LoadWt | double | - | Loaded weight |
| Loadcase | int | - | Loaded cases |

---

### 6.3 Customer & Transport Entities

**Table: `CustomerMaster`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Customer ID |
| Cid | string | - | Customer code |
| srno | int | - | Serial number |
| CName | string | Required, 100 chars | Customer name |
| Addr | string | 255 chars | Address |
| Gstno | string | - | GST number |
| State | string | 100 chars | State |
| City | string | 100 chars | City |
| District | string | 100 chars | District |
| Tahsil | string | 100 chars | Tahsil |
| Magazines | List | Navigation | Customer magazines |
| Members | List | Navigation | Contact members |

**Table: `TransportMaster`**

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK | Transport ID |
| TName | string | Required, 100 chars | Transporter name |
| Addr | string | 255 chars | Address |
| Gstno | string | - | GST number |
| State | string | 100 chars | State |
| City | string | 100 chars | City |
| District | string | 100 chars | District |
| Tahsil | string | 100 chars | Tahsil |
| Vehicles | List | Navigation | Registered vehicles |
| Members | List | Navigation | Contact members |

---

### 6.4 Database Indexes

The following composite indexes are configured for performance optimization:

| Table | Index Name | Columns |
|-------|------------|---------|
| L1Barcodegeneration | IX_PCode_MCode_BrandId_PSizeCode_Month_GenYear_Quarter_l1 | PCode, MCode, BrandId, PSizeCode, Month, GenYear, Quarter, MfgDt, L1Barcode |
| L2Barcodegeneration | IX_PCode_MCode_BrandId_PSizeCode_Month_GenYear_Quarter_l2 | Same as L1 |
| L3Barcodegeneration | IX_PCode_MCode_BrandId_PSizeCode_Month_GenYear_Quarter_l3 | Same as L1 |
| BarcodeData | IX_PCode_MCode_BrandId_PSizeCode_batch_MfgDt_L1 | PlantCode, BrandId, SizeCode, Batch, MfgDt, L1 |
| Magzine_Stock | IX_BrandId_PSizeCode_L1Barcode | BrandId, PSizeCode, L1Barcode |
| DispatchTransaction | IX_DispatchTransaction_L1BidPSizeCode | L1Barcode, Bid, PSizeCode |
| DispatchTransaction | IX_DispatchTransaction_MagNamere12 | MagName, Re12 |
| ProductionPlan | IX_ProductionPlan_MfgDt_PlantCode_BrandId_PSizeCode | MfgDt, PlantCode, BrandId, PSizeCode |
| BatchMasters | IX_BatchCode_PlantCode_BatchType | BatchCode, PlantCode, BatchType |
| User | IX_User_Username | Username (Unique) |
| RoleMaster | IX_RoleMaster_Page | RoleName |

---

## 7. Models & Relationships

### 7.1 Entity Relationship Diagram (Textual)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           MASTER DATA RELATIONSHIPS                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  PlantMaster (1) ──────────── (Many) ProductMaster                          │
│       │                             │                                        │
│       └────────────────────────────┼──────── (Many) BrandMaster             │
│                                    │                                         │
│  MfgMaster (1) ──── (Many) MfgLocationMaster ──── (Many) MagzineMaster      │
│                                                          │                   │
│                                           ┌──────────────┘                   │
│                                           ▼                                  │
│                                    MagzineMasterDetails                     │
│                                                                              │
│  CustomerMaster (1) ──┬── (Many) CustMemberDetail                           │
│                       └── (Many) CustMagazineDetail                         │
│                                                                              │
│  TransportMaster (1) ──┬── (Many) TransVehicleDetail                        │
│                        └── (Many) TransMemberDetail                          │
│                                                                              │
│  RoleMaster (1) ──── (Many) PageAccess                                      │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                           BARCODE RELATIONSHIPS                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  L1barcodegeneration (1) ──┬── (Many) L2barcodegeneration                   │
│                            └── (Many) L3barcodegeneration                   │
│                                                                              │
│  L2barcodegeneration (1) ──── (Many) L3barcodegeneration                    │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                           DISPATCH RELATIONSHIPS                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Re11IndentInfo (1) ──────────── (Many) Re11IndentPrdInfo                   │
│       │                                                                      │
│       └──────────────────────────────────── AllLoadingSheet                 │
│                                                  │                           │
│  AllLoadingSheet (1) ──── (Many) AllLoadingIndentDeatils                    │
│                                                                              │
│  ProductionTransfer (1) ──── (Many) ProductionTransferCases                 │
│                                                                              │
│  ProductionMagzineAllocation (1) ── (Many) ProductionMagzineAllocationCases │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 7.2 Key Foreign Key Relationships

| Parent Entity | Child Entity | FK Column | Delete Behavior |
|---------------|--------------|-----------|-----------------|
| CustomerMaster | CustMemberDetail | Cid | Cascade |
| CustomerMaster | CustMagazineDetail | Cid | Cascade |
| TransportMaster | TransVehicleDetail | Cid | Cascade |
| TransportMaster | TransMemberDetail | Cid | Cascade |
| Re11IndentInfo | Re11IndentPrdInfo | IndentNo | Cascade |
| AllLoadingSheet | AllLoadingIndentDeatils | LoadingSheetId | Cascade |
| ProductionTransfer | ProductionTransferCases | ProductionTransferId | Cascade |
| ProductionMagzineAllocation | ProductionMagzineAllocationCases | TransferToMazgnieId | Cascade |
| MagzineMaster | MagzineMasterDetails | magzineid | Cascade |
| RoleMaster | PageAccess | RoleId | Cascade |

### 7.3 Unique Constraints

| Table | Column(s) | Description |
|-------|-----------|-------------|
| User | Username | Unique login identifier |
| ProductionTransferCases | L1Barcode | Prevent duplicate barcode transfers |

---

## 8. API Endpoints Reference

### 8.1 Authentication Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/Login/Login` | User authentication | Anonymous |

### 8.2 Master Data Management

| Controller | Base Route | Key Endpoints |
|------------|------------|---------------|
| PlantMastersController | `/api/PlantMasters` | CRUD for plants |
| BrandMastersController | `/api/BrandMasters` | CRUD for brands |
| ProductMastersController | `/api/ProductMasters` | CRUD for products |
| MagzineMastersController | `/api/MagzineMasters` | CRUD for magazines |
| CustomerMastersController | `/api/CustomerMasters` | CRUD for customers |
| TransportMastersController | `/api/TransportMasters` | CRUD for transporters |

### 8.3 Barcode Generation

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/L1Generate` | Get all L1 barcodes |
| POST | `/api/L1Generate` | Create new L1 + L2 + L3 barcodes |
| GET | `/api/L1Generate/{id}` | Get specific L1 details |
| PUT | `/api/L1Generate/{id}` | Update L1 barcode |
| DELETE | `/api/L1Generate/{id}` | Delete L1 barcode |
| GET | `/api/L1Generate/dashboard` | Dashboard statistics |
| GET | `/api/L1Generate/proreport` | Production report |

### 8.4 Dispatch & Transaction

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/DispatchTransactions/GetTruckBrandByIndentNo` | Get dispatch by indent |
| GET | `/api/DispatchTransactions/GetBrandIdAndPSizes` | Get brand/size combos |
| GET | `/api/DispatchTransactions/GetTransMagDetail` | Get magazine transaction details |
| GET | `/api/DispatchTransactions/GetRE11indentedata` | Get RE-11 indent data |

### 8.5 Indent Processing

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Re11IndentInfos` | Get all indents |
| POST | `/api/Re11IndentInfos` | Create new indent |
| GET | `/api/Re11IndentInfos/{indentNo}` | Get specific indent |
| DELETE | `/api/Re11IndentInfos/{id}` | Delete indent |
| POST | `/api/Re11IndentInfos/UploadRe11Pdf` | Upload RE-11 PDF for parsing |

### 8.6 Reports

| Controller | Description |
|------------|-------------|
| ReportsController | Main reporting engine (82KB of report logic) |
| ProductionReportController | Production statistics |
| DispatchReportController | Dispatch analysis |
| StorageMagazineReportController | Magazine stock reports |
| RE11StatusReportController | Indent status tracking |
| RE2StatusReportController | RE-2 form status |

### 8.7 Real-time Communication

| Endpoint | Type | Description |
|----------|------|-------------|
| `/ws` | WebSocket | Real-time notification channel |
| POST `/api/notifications` | REST | Send push notification |

---

## 9. Security & Authentication

### 9.1 Authentication Flow

```
┌───────────────┐         ┌──────────────────┐         ┌─────────────────┐
│    Client     │         │  LoginController │         │  UserRepository │
└───────┬───────┘         └────────┬─────────┘         └────────┬────────┘
        │                          │                            │
        │ POST /api/Login/Login    │                            │
        │ {username, password}     │                            │
        │─────────────────────────▶│                            │
        │                          │ Encrypt Password           │
        │                          │ EncryptionHelper.Encrypt() │
        │                          │                            │
        │                          │ GetUsernamePassword()      │
        │                          │───────────────────────────▶│
        │                          │                            │
        │                          │◀───────────────────────────│
        │                          │   User + Role              │
        │                          │                            │
        │                          │ Generate JWT Token         │
        │                          │ - Claims: Name, Id,        │
        │                          │   CompanyId, Role          │
        │                          │ - Expires: 40 minutes      │
        │                          │                            │
        │◀─────────────────────────│                            │
        │ {user, token}            │                            │
        │                          │                            │
```

### 9.2 JWT Token Structure

**Claims included in JWT:**
- `ClaimTypes.Name`: Username
- `Id`: User ID
- `CompanyId`: Company identifier
- `ClaimTypes.Role`: Role name (default: "Admin")

**Token Configuration:**
- **Expiration**: 40 minutes
- **Algorithm**: HMAC-SHA256
- **Secret Key**: Stored in `appsettings.json` as "JWTSecret"

### 9.3 Authorization

All API endpoints (except Login) require JWT Bearer authentication:

```csharp
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SomeController : ControllerBase { }
```

### 9.4 Password Security

Passwords are encrypted using a custom `EncryptionHelper.Encrypt()` method before storage and comparison.

### 9.5 WebSocket Authentication

WebSocket connections require JWT token validation:
1. Token passed as query parameter: `/ws?token={jwt_token}`
2. Token validated against same JWT configuration
3. User ID extracted from claims for connection tracking

---

## 10. Dependencies & Integrations

### 10.1 NuGet Package Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 8.0.11 | ORM Framework |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.11 | PostgreSQL Provider |
| EFCore.BulkExtensions | 8.1.1 | High-performance bulk operations |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.11 | JWT Authentication |
| AutoMapper | 13.0.1 | Object-to-object mapping |
| Microsoft.Extensions.Caching.Redis | 2.1.2 | Distributed caching |
| Serilog.AspNetCore | 9.0.0 | Structured logging |
| Swashbuckle.AspNetCore | 7.0.0 | Swagger/OpenAPI documentation |
| EPPlus | 7.1.0 | Excel file processing |
| itext7 | 8.0.5 | PDF generation |
| PdfSharpCore | 1.3.67 | PDF manipulation |
| DocumentFormat.OpenXml | 3.3.0 | Office document processing |
| Microsoft.AspNetCore.WebSockets | 2.3.0 | WebSocket support |

### 10.2 External Service Dependencies

| Service | Purpose | Configuration |
|---------|---------|---------------|
| PostgreSQL | Primary database | Connection string in appsettings |
| Redis | Distributed cache & Pub/Sub | localhost:6379 |

### 10.3 Infrastructure Requirements

- **.NET 8.0 Runtime**
- **PostgreSQL 12+** database server
- **Redis 6+** for caching and WebSocket message brokering
- **Network access** to database and Redis servers

---

## 11. Key Design Decisions

### 11.1 Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| **Repository Pattern** | Abstracts data access, enables unit testing, centralizes query logic |
| **Generic Repository** | Reduces boilerplate CRUD code across 50+ entities |
| **Service Layer** | Separates business logic from controllers |
| **Entity Framework Core** | Mature ORM with excellent PostgreSQL support |
| **AutoMapper** | Decouples API contracts from internal entities |

### 11.2 Performance Decisions

| Decision | Rationale |
|----------|-----------|
| **Bulk Extensions** | EFCore.BulkExtensions for batch inserts of L1/L2/L3 barcodes |
| **Composite Indexes** | Optimized frequently-queried barcode lookups |
| **Redis Caching** | Reduces database load for static master data |
| **AsNoTracking()** | Used for read-only queries to improve performance |
| **Multiple DbContext instances** | GenericRepository uses multiple contexts for parallel operations |

### 11.3 Security Decisions

| Decision | Rationale |
|----------|-----------|
| **JWT Bearer Auth** | Stateless, scalable authentication |
| **40-minute token expiry** | Balance between security and usability |
| **CORS: AllowAll** | Configured for development; should be restricted in production |
| **Password Encryption** | Custom encryption for password storage |

### 11.4 Data Model Decisions

| Decision | Rationale |
|----------|-----------|
| **String Primary Keys for Barcodes** | Barcodes are natural keys with business meaning |
| **Cascade Deletes** | Maintains referential integrity for parent-child relationships |
| **Denormalization in Barcode Tables** | Brand/Plant info duplicated for query performance |
| **Soft Flags (CompletedIndent, Compflag)** | Track workflow states without deletion |

### 11.5 Known Assumptions

1. **Single-tenant**: System designed for single company/organization
2. **PostgreSQL-specific**: Some configurations tied to PostgreSQL features
3. **Redis required**: WebSocket functionality depends on Redis Pub/Sub
4. **PESO compliance**: Data models follow Indian explosives regulatory requirements
5. **Barcode format**: Fixed format based on PESO guidelines

---

## Appendix A: Configuration Reference

### A.1 appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Environment": "Development",
  "ConnectionStrings": {
    "DefaultConnection": "Server=<host>;Port=5432;Database=<db>;UID=postgres;PWD=<pwd>;Pooling=true;"
  },
  "JWTSecret": "<secret_key>"
}
```

### A.2 Serilog Configuration

Logs are written to:
- **Console**: All environments
- **File**: `Logs/app_log.txt` with daily rolling interval

---

## Appendix B: Glossary

| Term | Definition |
|------|------------|
| **L1 Barcode** | Primary case/box-level barcode containing product batches |
| **L2 Barcode** | Bundle-level barcode, child of L1 |
| **L3 Barcode** | Unit-level barcode, child of L2 |
| **Magazine** | Licensed storage facility for explosives |
| **PESO** | Petroleum and Explosives Safety Organisation (India) |
| **RE-2** | PESO form for production to magazine transfer |
| **RE-6** | PESO form for transport permit |
| **RE-11** | PESO form for indent/order from customer |
| **RE-12** | PESO form for dispatch confirmation |
| **Indent** | Customer order/requisition |
| **HHT** | Handheld Terminal (scanning device) |

---

**Document End**
