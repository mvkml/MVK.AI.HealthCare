# Azure DevOps — Personal Access Token (PAT) Setup Guide

Steps to generate a PAT for `https://dev.azure.com/mvishnukiran05/MVK%20AI%20Health%20Care`, scoped
for creating/managing work items (Epics, Features, User Stories) via the Azure DevOps REST API.

## Steps

1. Sign in to `https://dev.azure.com/mvishnukiran05`.
2. Click the **user settings icon** (top-right, next to your profile picture).
3. Select **Personal access tokens**.
4. Click **+ New Token**.
5. Fill in the token details:
   - **Name:** something identifiable, e.g. `mvkhc-work-items`
   - **Organization:** `mvishnukiran05`
   - **Expiration:** pick a short/reasonable window (e.g. 30 or 90 days) — do not use "never expires"
   - **Scopes:** select **Custom defined**, then find **Work Items** and set it to **Read & Write**
     (do not grant broader scopes than needed)
6. Click **Create**.
7. Copy the token value shown on the confirmation screen — **this is the only time it's displayed**;
   Azure DevOps does not let you view it again after you navigate away.
8. Store it somewhere safe (password manager). Do not commit it to source control or paste it into
   any file in this repo.

## Storing the PAT securely (Windows Credential Manager)

Instead of pasting the raw token into chat or a file, store it in Windows Credential Manager and
let PowerShell read it back only when needed.

### Option A — GUI
1. Open **Control Panel** → **Credential Manager** (or press `Win`, type "Credential Manager").
2. Select **Windows Credentials**.
3. Click **Add a generic credential**.
4. Fill in:
   - **Internet or network address:** `AzureDevOps-mvkhc` (any identifiable name)
   - **User name:** `pat` (placeholder — Azure DevOps PAT auth uses Basic auth with a blank/any
     username and the PAT as the password)
   - **Password:** paste the PAT value
5. Click **OK**.

### Option B — PowerShell (repeatable, scriptable)
1. Install the community module once (per user, no admin required):
   ```powershell
   Install-Module -Name CredentialManager -Scope CurrentUser -Force
   ```
2. Store the PAT:
   ```powershell
   $token = Read-Host -AsSecureString "Paste PAT"
   New-StoredCredential -Target "AzureDevOps-mvkhc" -UserName "pat" -SecurePassword $token -Persist LocalMachine
   ```
   `Read-Host -AsSecureString` masks the input on screen so the token is never echoed.

### Retrieving it later (for the REST API calls)
```powershell
$cred = Get-StoredCredential -Target "AzureDevOps-mvkhc"
$pat  = $cred.GetNetworkCredential().Password
$header = @{ Authorization = "Basic " + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$pat")) }
```
This keeps the token out of chat transcripts, scrollback, and any file in the repo — it's pulled
from the Windows credential vault at the moment it's needed and used only in memory for that
session's API calls.

## Notes

- If the token is lost, it cannot be recovered — generate a new one and revoke the old one from the
  **Personal access tokens** list.
- Revoke the token from the same list once the work item setup task is complete, since it's a
  short-lived credential for a one-time setup task, not something that needs to stay live.
- See [[reference_azure_devops_org]] (Claude memory) for how this token is used — REST API calls to
  `https://dev.azure.com/{org}/{project}/_apis/wit/workitems/${type}?api-version=7.1`.
