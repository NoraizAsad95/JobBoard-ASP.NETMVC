# JobPortal-MVC

An ASP.NET MVC-based web application that simulates a real-world job portal. It supports job browsing, posting, applying, and user role management (Employer/User).

---

## 🚀 Features

- 👨‍💻 User Registration & Login (Employer & Job Seeker)
- 📄 Job Listings with Search Functionality
- 📤 Job Application with Resume Upload (PDF/DOCX)
- 💼 Job Posting for Employers
- 💾 Save Jobs for Later
- 🔒 Role-based Access Control (Employer vs User)
- 🧑‍💼 Employer Dashboard (Edit/Delete Jobs)
- 📥 Stored Applications per Job

---

## 🛠 Tech Stack

- **Frontend:** HTML, CSS (Bootstrap), Razor Views
- **Backend:** ASP.NET MVC 5
- **Database:** SQL Server
- **ORM:** Entity Framework
- **Language:** C#

---

## 📁 Folder Structure

/JobPortal
├── Controllers/
├── Models/
├── Views/
│ ├── Home/
│ ├── Job/
│ └── Shared/
├── Content/
├── Scripts/
└── README.md


---


## 🔧 Setup Instructions

1. **Clone the repo**
   ```bash
   git clone https://github.com/yourusername/JobPortal-MVC.git
    
2. Open in Visual Studio
3. Restore NuGet packages

4. Set connection string in Web.config

<connectionStrings>
    <add name="JobApplicationPortalEntities" connectionString="..." providerName="System.Data.SqlClient" />
</connectionStrings>
      
5. Run the app (F5)

🤝 Contributing
This project is for learning and demonstration purposes. Contributions are welcome!

