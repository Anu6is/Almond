<h1 align="center" style="font-size:28px; line-height:1"><b>Almond</b></h1>

<div align="center">
	<img alt="Icon" src="images/almond.jpg" width="150px">
</div>

---

Almond is a companion application developed to integrate with the [Cashew](https://github.com/jameskokoska/Cashew) budgeting app. Built with C# and powered by GitHub Actions, Almond generates [AppLinks](https://github.com/jameskokoska/Cashew?tab=readme-ov-file#app-links) from financial transaction emails, enabling quick and easy recording in Cashew.

---

> [!NOTE] 
> 
> ### This was developed for personal use and the required Google App has not been published.
> ### As such, you would need to create your own Google Cloud Project.
> 

## How It Works 
**Almond** operates on a GitHub workflow, running at regular intervals to automatically scan your emails for financial transaction messages based on personalized filters. It extracts key details using a customizable regex, then creates a **Cashew** AppLink. This AppLink is packaged as a notification and sent to your chosen destination. By default, Almond integrates with **GMail** for email scanning and **Pushbullet** for notifications.

## Getting Started

### Part 1: Create a Google Cloud project

- Go to the [Google Cloud Console](https://console.cloud.google.com/)
- Click "Create Project" named Almond
- Navigate to "APIs & Services" > "Library"
- Search for "Gmail API" and enable it
- Go to "APIs & Services" > "OAuth consent screen"
- Choose User Type
- Fill in required information (app name, user support email, etc.)
- Add the Gmail API scope: https://www.googleapis.com/auth/gmail.modify
- Complete the OAuth consent screen setup
- Go to "APIs & Services" > "Credentials"
- Click "Create Credentials" and select "OAuth client ID"
- Choose "Desktop app" as the application type
- Download the client configuration file

### Part 2: Configure the Application

- Clone this repository.
- Update the `appsettings.json` file locally.
- Build and run the solution.
- After authorizing with Google, save the retrieved token data.
   - The token data will be displayed in the console output.

### Part 3: Set Up GitHub Secrets and Variables
- Navigate to the **Setting** tab in the your repository
- Expand **Secrets and Variables** in the left panel
- Select **Actions**
- Select **New Enviornment Secret**
- Create a **Production** environment
- Add the required **Secrets** and **Variables**

### Configurations 
#### Appsettings 
```json
{
  "EMAIL_QUERY": "is:unread from:xxx@mybank.com subject:(credit card)",
  "EMAIL_TOKEN": //left empty until initial authorization with Google,
  "EMAIL_CLIENT_ID": "xxx-123.apps.googleusercontent.com",
  "EMAIL_CLIENT_SECRET": "G-xxx",
  "NOTIFICATION_TOKEN": "o.xxx",
  "NOTIFICATION_DESTINATION": "personal@gmail.com",
  "REGEX_PATTERN_TITLE": ".*?at\\s+(.+?)\\s+on",
  "REGEX_PATTERN_AMOUNT": "(?:for |made.*?for )\\$(\\d+(?:\\.\\d{2})?)\\s+\\w+",
  "REGEX_PATTERN_ACCOUNT": ".*?(?:Credit Card|card) \\*{3}(\\d+)",
  "ACCOUNT_MAP": "{\"1234\": \"Chase\", \"5678\": \"VISA\", \"9999\": \"MasterCard\"}" 
}
```
#### Secrets 
```yml
EMAIL_TOKEN: {"AccessToken":"xxx...","TokenType":"Bearer","ExpiresInSeconds":3599,"RefreshToken":"...","Scope":"https://www.googleapis.com/auth/gmail.modify",...}"
EMAIL_CLIENT_ID: xxx-123.apps.googleusercontent.com
EMAIL_CLIENT_SECRET: G-xxx
NOTIFICATION_TOKEN: o.xxx
NOTIFICATION_DESTINATION: personal@gmail.com
ACCOUNT_MAP: {"1234": "Chase", "5678": "VISA", "9999": "MasterCard"}
```		
#### Variables
```yml
EMAIL_QUERY: ${{ vars.EMAIL_QUERY }}        
REGEX_PATTERN_TITLE: ${{ vars.REGEX_PATTERN_TITLE }}
REGEX_PATTERN_AMOUNT: ${{ vars.REGEX_PATTERN_AMOUNT }}
REGEX_PATTERN_ACCOUNT: ${{ vars.REGEX_PATTERN_ACCOUNT }}
```
