# API Documentation: SomosRentWi Backend

## 1. Overview

- **Base URL**: `https://somosrentwi-backend-production.up.railway.app`
- **Authentication**: JWT Bearer Token.
- **Header**: `Authorization: Bearer <token>`
- **Response Format**: JSON.
- **Dates**: All dates are strictly `UTC` (ISO 8601).

### Common Error Format
```json
{
  "error": "Error message details"
}
```

---

## 2. Roles & Access Matrix

| Feature | Admin | Company | Client | Public |
| :--- | :---: | :---: | :---: | :---: |
| **Auth** | Login | Login | Register/Login | - |
| **Clients** | View All, Verify | - | View Own Profile (via Rental) | - |
| **Cars** | - | Create, Update, Delete | View | View |
| **Rentals** | - | View Own, Deliver, Complete | Create, View Own, Cancel | - |
| **Wallets** | View Admin Wallet | View Own Wallet | - | - |
| **Companies** | Create | - | - | - |

---

## 3. Controllers Index

| Controller | Route Prefix | Purpose |
| :--- | :--- | :--- |
| **Auth** | `/api/auth` | Registration and Login |
| **Cars** | `/api/Cars` | Car inventory management |
| **Rentals** | `/api/Rentals` | Rental process lifecycle |
| **CompanyWallet** | `/api/company/wallet` | View company earnings |
| **AdminWallet** | `/api/admin/wallet` | View platform commissions |
| **AdminClients** | `/api/admin/clients` | Client verification management |
| **AdminCompanies** | `/api/admin/companies` | Company onboarding |

---

## 4. Endpoint Reference

### POST /api/auth/register
- **Controller**: `AuthController`
- **Action**: `Register`
- **Source**: `src/SomosRentWi.Api/Controllers/AuthController.cs`
- **Auth**: Public
- **Roles**: N/A
- **Purpose**: Registers a new Client account with required document photos.
- **Request**: `multipart/form-data`
  - `Email` (string, required): Unique email.
  - `Password` (string, required): Plaintext password.
  - `FirstName` (string, required)
  - `LastName` (string, required)
  - `DocumentType` (enum, required): `CC`=0, `CE`=1, `passport`=2
  - `DocumentNumber` (string, required)
  - `BirthDate` (date-time, required)
  - `PrimaryPhone` (string, required)
  - `Address` (string, required)
  - `SelfiePhoto` (file, required): Image file.
  - `DocumentFront` (file, required): Image file.
  - `DocumentBack` (file, required): Image file.
  - `LicenseFront` (file, required): Image file.
  - `LicenseBack` (file, required): Image file.
- **Response**: `200 OK`
```json
{
  "token": "eyJhbG...",
  "userId": 1,
  "role": "Client"
}
```
- **Business Rules**:
  - Email must be unique.
  - All 5 photos are mandatory.
  - Initial `VerificationStatus` is `Pending`.

---

### POST /api/auth/login
- **Controller**: `AuthController`
- **Auth**: Public
- **Request**: `application/json`
```json
{
  "email": "user@example.com",
  "password": "secretPassword"
}
```
- **Response**: `200 OK` or `400 Bad Request` (Invalid credentials/Inactive)
```json
{
  "token": "eyJhbG...",
  "userId": 1,
  "role": "Client" // or Admin, Company
}
```

---

### GET /api/Cars
- **Controller**: `CarsController`
- **Auth**: Public
- **Purpose**: List all cars in the system.
- **Response**: `200 OK` (Array of CarResponse)

### GET /api/Cars/{id}
- **Controller**: `CarsController`
- **Auth**: Public
- **Purpose**: Get details of a specific car.
- **Response**: `200 OK` or `404 Not Found`

### GET /api/Cars/company/{companyId}
- **Controller**: `CarsController`
- **Auth**: Public
- **Purpose**: List all cars belonging to a specific company.

### GET /api/Cars/my-cars
- **Controller**: `CarsController`
- **Auth**: JWT Required
- **Roles**: `Company`
- **Token Usage**: Uses `sub` claim to identify Company User.
- **Response**: `200 OK` (List of cars owned by the authenticated company).

### POST /api/Cars
- **Controller**: `CarsController`
- **Auth**: JWT Required
- **Roles**: `Company`
- **Request**: `multipart/form-data`
  - `Brand` (string)
  - `Model` (string)
  - `Year` (int)
  - `Plate` (string)
  - `Color` (string)
  - `CommercialValue` (decimal)
  - `BasePricePerHour` (decimal)
  - `SoatExpirationDate` (date-time)
  - `TechExpirationDate` (date-time)
  - `MainPhoto` (file, optional): Car image.

### PUT /api/Cars/{id}
- **Controller**: `CarsController`
- **Roles**: `Company`
- **Request**: `multipart/form-data` (Same fields as POST but optional).
- **Business Rules**:
  - Only the owner company can update the car.

### DELETE /api/Cars/{id}
- **Controller**: `CarsController`
- **Roles**: `Company`
- **Business Rules**:
  - Only the owner company can delete.
  - Hard delete (from code analysis).

---

### POST /api/Rentals
- **Controller**: `RentalsController`
- **Roles**: `Client`
- **Request**: `application/json`
```json
{
  "carId": 5,
  "startDate": "2023-12-20T10:00:00Z",
  "estimatedEndDate": "2023-12-25T10:00:00Z"
}
```
- **Business Rules**:
  - Client `VerificationStatus` must be `Accepted`.
  - `StartDate` cannot be in the past.
  - Car must not have overlapping rentals.
  - **Financial**: 
    - 90% of TotalPrice credited to `CompanyWallet`.
    - 10% credited to `AdminWallet`.
    - Transactions created immediately.
  - Status set to `PendingDelivery`.

### GET /api/Rentals/{id}
- **Controller**: `RentalsController`
- **Auth**: JWT Required (Any role)
- **Response**: `200 OK` (RentalResponse)

### GET /api/Rentals/my-rentals
- **Controller**: `RentalsController`
- **Roles**: `Client`
- **Purpose**: History of rentals for the logged-in client.

### GET /api/Rentals/company-rentals
- **Controller**: `RentalsController`
- **Roles**: `Company`
- **Purpose**: List of all rentals for the company's fleet.

### POST /api/Rentals/{id}/deliver
- **Controller**: `RentalsController`
- **Roles**: `Company`
- **Business Rules**:
  - Source integrity: Company must own the car.
  - Status transition: `PendingDelivery` -> `InProgress`.
  - **Car Status**: Updates Car to `InUse`.

### POST /api/Rentals/{id}/complete
- **Controller**: `RentalsController`
- **Roles**: `Company`
- **Business Rules**:
  - Status transition: `InProgress` -> `FinishedCorrect`.
  - **Recalculation**: `TotalPrice` is recalculated based on *actual* duration vs snapshot price.
  - **Car Status**: Updates Car to `Available`.

### POST /api/Rentals/{id}/cancel
- **Controller**: `RentalsController`
- **Auth**: JWT Required (Any authorized user, likely Client/Company)
- **Request**: `application/json`
```json
{
  "reason": "Changed plans"
}
```
- **Business Rules**:
  - Cannot cancel if already Finished.
  - Frees up the car (`Available`) if it was `InUse`.
  - Status -> `FinishedWithIssue`.

---

### GET /api/company/wallet
- **Controller**: `CompanyWalletController`
- **Roles**: `Company`
- **Response**:
```json
{
  "balance": 1500.00,
  "transactions": [
    {
      "amount": 450.00,
      "description": "Payment for rental #...",
      "type": "RentalIncome",
      "date": "2023-12-01T..."
    }
  ]
}
```

### GET /api/admin/wallet
- **Controller**: `AdminWalletController`
- **Roles**: `Admin`
- **Response**: Same structure as Company Wallet.

---

### POST /api/admin/companies
- **Controller**: `AdminCompaniesController`
- **Roles**: `Admin`
- **Purpose**: Create a new Company account.
- **Request**: `application/json`
  - `Email`, `Password`
  - `TradeName`, `NitNumber`
  - `ContactEmail`, `Phones`, `Address`
  - `CompanyPlan` (enum)

### GET /api/admin/clients
- **Controller**: `AdminClientsController`
- **Roles**: `Admin`
- **Purpose**: List all registered clients for verification.
- **Response**: `List<Client>` (Full Entity exposed).

### PATCH /api/admin/clients/{id}/verification-status
- **Controller**: `AdminClientsController`
- **Roles**: `Admin`
- **Request**: `application/json`
```json
{
  "status": 1 // 0=Pending, 1=Accepted, 2=Rejected
}
```

---

## 5. Enumerations

### UserRole
`Admin`=0, `Company`=1, `Client`=2

### RentalStatus
`PendingDelivery`=0, `InProgress`=1, `FinishedCorrect`=2, `FinishedWithIssue`=3, `Cancelled`=4, `Rejected`=5

### ClientVerificationStatus
`Pending`=0, `Accepted`=1, `Rejected`=2

---

## 6. Known Gaps / Notes

1. **Admin Transactions Date**: `AdminWalletTransaction` entity does not have a `TransactionDate` property (unlike `WalletTransaction`), so dates are returned as null for admin transactions.
2. **Client Exposure**: The endpoint `GET /api/admin/clients` returns the full `Client` domain entity including linked `User` object (which contains password hash). This is efficiently functional but a potential security over-exposure.
3. **Empty Controllers**: `AdminController`, `ClientController`, `CompanyController` exist in the codebase but are empty placeholders with no endpoints.
