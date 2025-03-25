/*prompt1:
On the left side of the page, there will be a form that collects: o Class Name o Student Count o Description • On the right side, there will be a table that displays all the submitted class data. • The table will have the following columns: o Id o Class Name o Student Count o Description o Actions (Edit and Delete) The data should be validated and added to a static list each time the form is submitted. The data will then be displayed in the table razor page

prompt2:
Step 4 – Requirements and Constraints • Use Bootstrap to create a responsive layout with two columns (form on the left, table on the right). • Use Razor Pages only; no JavaScript is allowed. • All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel. • Form validation should be done using C# attributes like [Required], [Range], etc. • When editing, pre-fill the form with the selected item's data. • After deletion or editing, refresh the page and update the table accordingly. Reminders Add asp-page-handler to Buttons Each submit button and action button (Edit and Delete) must use asp-page-handler to connect to the correct handler method in the PageModel (e.g., OnPostAdd, OnPostDelete, etc.). Example: <button type="submit" asp-page-handler="Add">Add Class</button>
<form method="post"><input type="hidden" name="id" value="@item.Id" /><button type="submit" asp-page-handler="Delete">Delete</button></form>This tells Razor which C# method to run when clicking the button. For example, asp-pagehandler="Delete" triggers the OnPostDelete(int id) method in the PageModel.You need to bind the input and submit tags to C# IActionResults.Inside the cshtml.cs file, write these functions for the given example.public IActionResult OnPostAdd(){}public IActionResult OnPostDelete(int id){}    bunları yapacak şekilde bütün dosyaları güncelle
prompt3:
update class sadece edite basıldığında ortaya çıksın,edite basıldığında add class da görünmez olsun ve cancel butonu getir, bu edit halinden çıkmasını sağlamalı, add butonu geri gelir

*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class ClassInformationModel
    {
        private static int _nextId = 1;
        private static List<ClassInformationModel> _classDatabase = new List<ClassInformationModel>();

        // Properties
        public int Id { get; private set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be greater than 0.")]
        public int StudentCount { get; set; }

        [Required]
        public string Description { get; set; }

        public ClassInformationModel()
        {
        }

        public ClassInformationModel(string className, int studentCount, string description)
        {
            Id = _nextId++;
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }

        public static void AddClass(ClassInformationModel newClass)
        {
            newClass.Id = _nextId++;
            _classDatabase.Add(newClass);
        }

        public static void EditClass(int id, ClassInformationModel updatedClass)
        {
            var existingClass = _classDatabase.Find(c => c.Id == id);
            if (existingClass != null)
            {
                existingClass.ClassName = updatedClass.ClassName;
                existingClass.StudentCount = updatedClass.StudentCount;
                existingClass.Description = updatedClass.Description;
            }
        }

        public static void DeleteClass(int id)
        {
            var classToDelete = _classDatabase.Find(c => c.Id == id);
            if (classToDelete != null)
            {
                _classDatabase.Remove(classToDelete);
            }
        }

        public static List<ClassInformationModel> GetAllClasses()
        {
            return _classDatabase;
        }

        public static ClassInformationModel GetClassById(int id)
        {
            return _classDatabase.Find(c => c.Id == id);
        }
    }
}
