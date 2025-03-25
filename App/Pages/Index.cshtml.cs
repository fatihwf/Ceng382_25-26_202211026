/*prompt1:
On the left side of the page, there will be a form that collects: o Class Name o Student Count o Description • On the right side, there will be a table that displays all the submitted class data. • The table will have the following columns: o Id o Class Name o Student Count o Description o Actions (Edit and Delete) The data should be validated and added to a static list each time the form is submitted. The data will then be displayed in the table razor page

prompt2:
Step 4 – Requirements and Constraints • Use Bootstrap to create a responsive layout with two columns (form on the left, table on the right). • Use Razor Pages only; no JavaScript is allowed. • All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel. • Form validation should be done using C# attributes like [Required], [Range], etc. • When editing, pre-fill the form with the selected item's data. • After deletion or editing, refresh the page and update the table accordingly. Reminders Add asp-page-handler to Buttons Each submit button and action button (Edit and Delete) must use asp-page-handler to connect to the correct handler method in the PageModel (e.g., OnPostAdd, OnPostDelete, etc.). Example: <button type="submit" asp-page-handler="Add">Add Class</button>
<form method="post"><input type="hidden" name="id" value="@item.Id" /><button type="submit" asp-page-handler="Delete">Delete</button></form>This tells Razor which C# method to run when clicking the button. For example, asp-pagehandler="Delete" triggers the OnPostDelete(int id) method in the PageModel.You need to bind the input and submit tags to C# IActionResults.Inside the cshtml.cs file, write these functions for the given example.public IActionResult OnPostAdd(){}public IActionResult OnPostDelete(int id){}    bunları yapacak şekilde bütün dosyaları güncelle
prompt3:
update class sadece edite basıldığında ortaya çıksın,edite basıldığında add class da görünmez olsun ve cancel butonu getir, bu edit halinden çıkmasını sağlamalı, add butonu geri gelir

*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using App.Models;

public class IndexModel : PageModel
{
    [BindProperty]
    public int Id { get; set; }
    [BindProperty]
    public string ClassName { get; set; }
    [BindProperty]
    public int StudentCount { get; set; }
    [BindProperty]
    public string Description { get; set; }
    public List<ClassInformationModel> ClassList { get; private set; } = new List<ClassInformationModel>();

    public bool IsEditMode { get; set; } = false;

    

    public void OnGet()
    {
        // Populate the table with existing classes
        ClassList = ClassInformationModel.GetAllClasses();
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            ClassList = ClassInformationModel.GetAllClasses();
            return Page();
        }

        var newClass = new ClassInformationModel
        {
            ClassName = ClassName,
            StudentCount = StudentCount,
            Description = Description
        };
        ClassInformationModel.AddClass(newClass);

        return RedirectToPage();
    }

    public IActionResult OnPostEdit()
    {
        IsEditMode = true;
        if (!ModelState.IsValid)
        {
            ClassList = ClassInformationModel.GetAllClasses();
            return Page();
        }

        var updatedClass = new ClassInformationModel
        {
            ClassName = ClassName,
            StudentCount = StudentCount,
            Description = Description
        };
        ClassInformationModel.EditClass(Id, updatedClass);

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        ClassInformationModel.DeleteClass(id);
        return RedirectToPage();
    }
    public IActionResult OnPostCancel()
    {
        IsEditMode = false;
        ClassList = ClassInformationModel.GetAllClasses();
        return Page();
    }
}
