# Project Name

## Description
Provide a brief description of your project here.

## Getting Started

Follow these steps to set up and use this repository:

### 1. Clone the Repository
```sh
git clone https://github.com/your-username/your-repository.git
cd your-repository
```

### 2. Add `LibDB` Project Reference on `ADO.Lib`
- Open the solution in your preferred IDE (e.g., Visual Studio).
- Navigate to the `ADO.Lib` project.
- Add a project reference to `LibDB`.

### 3. Configure the Connection String in `LibraryContext` on `EFLibrary`
- Locate the `LibraryContext` class in the `EFLibrary` project.
- Set the connection string to your database, ensuring you specify the target database name.
- If the database does not exist, Entity Framework Core will create it for you.

### 4. Add Migration on `EFLibrary`
Run the following command to add a new migration in the `EFLibrary` project:
```sh
dotnet ef migrations add InitialMigration --project EFLibrary
```
Replace `InitialMigration` with a suitable name if necessary.

### 5. Update Database on `EFLibrary`
Apply the migration to update the database:
```sh
dotnet ef database update --project EFLibrary
```

### 6. Configure Environment Variable in `WebAPI`
Modify the `launchSettings.json` file located in the `Properties` folder of the `WebAPI` project:
- Add or update the following environment variable:
```json
"EnvironmentVariables": {
  "CONNECTION_STRING": "your-database-connection-string"
}
```

### 7. Seed Data in `AdminMPA`
- Start your project and navigate to your localhost.
- Append `/seed` to the URL and load the page to seed the initial data.

### 8. Access the Website
- Use the following credentials to log in:
  - **Email:** `admin@biblioteca.com`
  - **Password:** Check your local database for confirmation.

## Additional Information
- Ensure that you have the required dependencies installed.
- Verify that Entity Framework Core is correctly configured in your project.
- Modify connection strings if needed before running the migration.

## Contributing
Feel free to contribute by submitting issues or pull requests.

## License
This project is licensed under the [MIT License](LICENSE).

