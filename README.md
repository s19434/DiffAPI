

# DiffAPI

DiffAPI is a web application that provides endpoints to compare two base64-encoded binary data strings. The API supports storing and comparing the data and returning the differences in a structured format.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
  - [Endpoints](#endpoints)
- [Running Tests](#running-tests)
- [Project Structure](#project-structure)

## Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/s19434/DiffAPI.git
   cd DiffAPI


2. **Restore the dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

   The application will start and be accessible at `http://localhost:5002`.

## Usage

### Endpoints

#### PUT /v1/diff/{id}/left
Stores the left binary data for a given ID.

- **Request:**
  ```http
  PUT /v1/diff/{id}/left
  Content-Type: application/json

  {
    "data": "base64EncodedString"
  }
  ```

- **Response:**
    - `201 Created` on success
    - `400 Bad Request` if the data is invalid

#### PUT /v1/diff/{id}/right
Stores the right binary data for a given ID.

- **Request:**
  ```http
  PUT /v1/diff/{id}/right
  Content-Type: application/json

  {
    "data": "base64EncodedString"
  }
  ```

- **Response:**
    - `201 Created` on success
    - `400 Bad Request` if the data is invalid

#### GET /v1/diff/{id}
Retrieves the differences between the left and right data for a given ID.

- **Response:**
    - `200 OK` with the diff result
    - `404 Not Found` if the data for the given ID does not exist

  ```json
  {
    "diffResultType": "Equals" | "SizeDoNotMatch" | "ContentDoNotMatch",
    "diffs": [
      {
        "offset": int,
        "length": int
      }
    ]
  }
  ```

## Running Tests

1. **Run unit tests:**
   ```bash
   dotnet test
   ```

2. **Run integration tests:**
   ```bash
   dotnet test IntegrationTests/IntegrationTests.csproj
   ```
   
3. **Run integration tests:**
   ```bash
   dotnet test IntegrationTests/IntegrationTests.csproj
   ```
   
## Project Structure

```
DiffAPI
├── Controllers
│   └── DiffController.cs       # API controller for handling diff operations
├── Interfaces
│   └── IDiffService.cs         # Interface for the diff service
├── Models
│   ├── Diff.cs                 # Model representing a diff
│   └── DiffData.cs             # Model representing the diff data
├── Services
    └── DiffService.cs          # Service implementation for diff operations

IntegrationTests
   ├── Factories
   │   └── CustomWebApplicationFactory.cs # Factory for integration tests
   └── IntegrationTests
       └── DiffIntegrationTests.cs       # Integration tests for the API

UnitTests
   ├── Helpers
   │   └── DiffServiceHelper.cs # Helper methods for unit tests
   └── Services
       └── DiffServiceTests.cs  # Unit tests for the DiffService

```
