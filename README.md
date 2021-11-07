# **ICT2202 Digital Forensic WebServer**

A private blockchain-based chain of custody framework and case management web portal designed to reduce the likelihood of an insider attack within the system.

![Case Management][case-management]

![Block Content][blockchain-content]

## Table of Contents
--------------------
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [User Guide](#user-guide)
  - [Login](#login)
  - [Creating Users](#creating-users)
  - [Creating Case](#creating-case)
  - [Adding Evidence](#adding-evidence)
  - [Download Evidence](#download-evidence)
  - [Adding Assigned Users](#adding-assigned-users)
- [Documentation](#documentation)

## Getting Started
------------------
To clone the git, type:

```bash
git clone https://github.com/liger2020/ICT2202DigitalForensic.git
```

Navigate into the project folder:

## Installation Guide
Download Visual Studio 2019 Community version and run. The link is https://visualstudio.microsoft.com/downloads/

![Download Visual Studio 2019 Community][VisualStudio2019Download]

Select 'ASP.NET and web development' and 'Azure Development' circled in red and install.

![Install 'ASP.NET and web development' and 'Azure Development'][RequiredProgram]

```bash
cd ICT2202DigitalForensic
```

## Configuration
----------------


## User Guide
-------------
After the case management webserver, which is interfacing with the blockchain, and nodes are installed and running, you are able to perform functions such as adding new case and evidence.

### Login
This is the login page

![Login Page for the Website][Login]

### Creating Users
This is for creating user accounts.

![Account Creation using the Website][CreateAccount]

### Creating Case
This is to create new Case based on the current user that is login

![New Case Creation][UploadCase]

### Adding Evidence
This is to add Evidence to existing Case

![Adding Evidence to Case][UploadFile]

### Download Evidence
This is to download evidences from the case

![Download Evidence from the case][DownloadEvidence]

### Adding Assigned Users
This is to assign new users to existing case

![Assign new user to existing case][AssignUser]

## Documentation
----------------
The documentation is available in /docs directory.

[VisualStudio2019Download]: docs/VisualStudio2019Download.jpg

[RequiredProgram]: docs/RequiredProgram.jpg

[case-management]: https://liger2020.github.io/ICT2202DigitalForensic/images/blockchain-case-management-server.png "Case Management System"

[blockchain-content]: https://liger2020.github.io/ICT2202DigitalForensic/images/block-content.png "Contents of Blockchain"

[Login]: docs/LoginPage.jpg

[CreateAccount]: docs/CreateAccount.jpg

[IndexPage]: docs/IndexPage.jpg

[UploadCase]: docs/UploadCase.jpg

[UploadFile]: docs/UploadFile.jpg

[DownloadEvidence]: docs/DownloadEvidence.jpg

[AssignUser]: docs/AssignUser.jpg

[^note]: This is an assignment for ICT2202.

