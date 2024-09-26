# SourceFuse.Assessment

# Music API

This is an API for managing songs, including uploading files to AWS S3 and generating pre-signed URLs.

## Requirements

- .NET 5.0 or higher
- Visual Studio 2019 or higher / Visual Studio Code

## Setup

### Step 1: Clone the Repository

```sh
git clone https://github.com/your-username/music-api.git
cd music-api
```

### Step 2: Configure appsettings.json

Create an appsettings.json file in the API folder (src/Api) and paste the following content, replacing the AWS credentials and other values as needed:

```sh
{
    "AWS": {
        "Region": "us-west-2",
        "BucketName": "your-bucket-name",
        "AccessKey": "your_access_key",
        "SecretKey": "your_secret_key"
    },
    "ConnectionStrings": {
        "DefaultConnection": "Host=your_host;Port=5432;Database=MusicDB;Username=your_user;Password=your_password"
    },
    "Jwt": {
        "Key": "2285a2c53b070f6a20fcf0601a89a897d85705fcea10ed3b1224e88",
        "Issuer": "https://myapp.com",
        "Audience": "https://myapp.com",
        "ExpirationMinutes": "60"
    }
}
```

## Setup

### Step 3: Restore Packages

Navigate to the API project folder and restore the NuGet packages:

```sh
cd SourceFuse.Assessment.Api
dotnet restore
```

### Step 4: Run the Application

Run the application using the following command:

```sh
dotnet run
```

### Step 5: Run Unit Tests

Navigate to the test project folder and run the unit tests:

```sh
cd SourceFuse.Assessment.Tests
dotnet test
```

### Step 6: Authentication

There is an endpoint for logging in. Use the following credentials:

- **Username**: `spuertab1`
- **Password**: `123`

## Endpoints

### Login

- **URL**: `/api/auth/login`
- **Method**: `POST`
- **Body**:
  ```json
  {
      "username": "spuertab1",
      "password": "123"
  }
  ```
### Step 7: Using the Token with Swagger

If you want to use the token with Swagger, you can pass it as a Bearer token.

1. **Login**: Obtain the JWT token using the login endpoint.
2. **Authorize in Swagger**:
   - Open Swagger UI (usually at `/swagger`).
   - Click on the "Authorize" button.
   - Enter the token in the following format: `Bearer {token}`.
   - Click "Authorize".

With these steps, you should be able to use the JWT token to make authenticated requests via Swagger.

Notes
Ensure that the appsettings.json file is placed in the src/SourceFuse.Assessment.Api folder.
Replace placeholder values in the appsettings.json file with your actual configuration values.
With these steps, you should be able to set up, run, and test the Music API application.
