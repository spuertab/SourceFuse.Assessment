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




