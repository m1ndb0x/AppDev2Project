# Examina

A C# ASP.NET project designed to be a exam creation and distribution website for teachers and students.  

This project uses C#to handle all API calls and TailwindCSS and HTML to display an appealing webpage for the user


Troubleshooting: 
when the database schema changes, especially if the 'users' table is renamed to 'AspNetUsers' or modified:
1. Update model to ensure the UserName property is properly mapped.
2. Ensure the database reflects the required fields for ASP.NET Identity.
3. Migrations should be used to keep the schema in sync with any model changes.
