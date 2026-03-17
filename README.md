# FeatureFlagAPI

SOFTWARE INSTALLATION & SETUP GUIDE
System: Visual Studio 2026 Enterprise/Community
Target Framework: .NET 8.0 (LTS)
Primary Project: HRMS (Human Resource Management System)
Environment: Windows 10/11


Phase 1: Pre-Installation & Requirements
1.	System Update: Ensure Windows is fully updated (Settings > Windows Update).
2.	Hardware Check: Verify at least 16GB RAM and 50GB of SSD storage.
3.	Clean Slate: Uninstall any "Preview" versions of previous .NET SDKs.
4.	Admin Rights: Ensure you are logged in as an Administrator.
5.	Browser Check: Have a browser ready to download the 3MB bootstrapper.


Phase 2: Downloading & Bootstrapping
6.	Navigate to the Visual Studio Official Website.
7.	Hover over "Downloads" and select "Community 2026".
8.	Save the VisualStudioSetup.exe to your local "Downloads" folder.
9.	Right-click the file and select "Run as Administrator".
10.	Click "Continue" on the privacy and terms splash screen.
11.	Wait for the installer to download the core metadata (approx. 80MB).
12.	The Visual Studio Installer dashboard will now appear


Phase 3: Workload Configuration (Critical)
13.	Select the "Workloads" tab.
14.	Check the box for "ASP.NET and web development".
15.	Check the box for "Node.js development" (Essential for your Angular frontend).
16.	(Optional) Check "Data storage and processing" for easier SQL Server management.
17.	Scroll to the right sidebar under "Installation details."
18.	Expand "ASP.NET and web development".
19.	Ensure ".NET 8.0 Runtime (LTS)" is explicitly checked.
20.	Check "Development tools for .NET".


Phase 4: Individual Components & Tools
21.	Click the "Individual Components" tab at the top.
22.	Search for "Git for Windows" and ensure it is checked.
23.	Search for "Entity Framework 6 tools" for your database migrations.
24.	Search for ".NET SDK 8.0" to verify it is included.
25.	Select "Language Packs" and ensure "English" is chosen.
26.	Choose your installation location (Default C:\Program Files is recommended).
27.	Select the radio button "Install while downloading" to save time.
28.	Click the "Install" button at the bottom right.
________________________________________
Phase 5: The Installation Process
29.	Monitor the "Download" progress bar (Total size is usually 5GB to 9GB).
30.	Monitor the "Install" progress bar.
31.	If a "Restart Required" prompt appears, bookmark your work and restart the PC.
33.	Once complete, click the "Launch" button.
________________________________________
Phase 6: Initial Environment Setup
33.	Sign in with your Microsoft Account (Hotmail/Outlook/GitHub).
34.	Select your Development Settings: Choose "Web Development".
35.	Choose your Color Theme: "Dark Mode" is the industry standard for reducing eye strain.
36.	Click "Start Visual Studio".
37.	Go to Tools > Options.
38.	Navigate to Environment > Keyboard.
39.	Confirm your preferred mapping (Default or Visual Studio Code).
________________________________________
Phase 7: Verifying .NET 8 for HRMS Project
40.	Click "Create a new project".
41.	Select "ASP.NET Core Web API".
42.	Click "Next".
43.	Name the project HRMS.API.
44.	In the "Additional Information" window, click the "Framework" dropdown.
45.	Select ".NET 8.0 (Long Term Support)".
46.	Set "Authentication Type" to "None" (you will build custom logic later).
47.	Ensure "Enable OpenAPI (Swagger) support" is checked.
48.	Click "Create".
49.	Press F5 to run the project. If a browser opens with a Swagger page, your installation is 100% successful.
________________________________________
Official Video References
•	Complete Installation Walkthrough: YouTube: Visual Studio 2026 Step-by-Step Guide
•	Setting up .NET 8 for Beginners: YouTube: .NET 8 Fundamentals
•	Connecting SQL Server to VS 2026: YouTube: SQL Server Management in VS


Official Video References
•	Complete Installation Walkthrough: YouTube: Visual Studio 2026 Step-by-Step Guide
•	Setting up .NET 8 for Beginners: YouTube: .NET 8 Fundamentals
•	Connecting SQL Server to VS 2026: YouTube: SQL Server Management in VS
