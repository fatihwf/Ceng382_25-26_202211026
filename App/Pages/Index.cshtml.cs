using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using App.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using System;
using System.Text.Json;
// prompt: pagination ve filter özellikleri ekle, sayfa numarasını ve filter durumunu kaydet
// other done manually, using TempData features
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

    // Sayfada gösterilecek kayıtları tutan liste
    public List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

    
    [TempData]
    public bool IsEditMode { get; set; } = false;
    
    
    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public int TotalPages { get; set; }

    public string ErrorMessage {get; set; }

    public IActionResult OnGet()
    {
        string sessionUsername = HttpContext.Session.GetString("username");
        string sessionToken = HttpContext.Session.GetString("token");

        // Çerezden verileri oku
        var cookieUsername = Request.Cookies["username"];
        var cookieToken = Request.Cookies["token"];

        // Her iki yöntemle de alınan verileri kontrol et
        // Eğer session token boşsa, direkt çık (logout yönlendirmesi)
        if (string.IsNullOrEmpty(sessionToken))
        {
            ErrorMessage = "Session expired.";
            return RedirectToPage("/Login");
        }
        else
        {
            // Session token dolu, şimdi cookie token boş mu kontrol et
            if (string.IsNullOrEmpty(cookieToken))
            {
                ErrorMessage = "Session expired.";
                return RedirectToPage("/Login");
            }
            else
            {
                // Her iki token dolu; eşleşip eşleşmediğini kontrol et
                if (sessionToken != cookieToken)
                {
                    ErrorMessage = "Session expired.";
                    return RedirectToPage("/Login");
                }
            }
        }
            

        if (TempData.ContainsKey("EditingId"))
        {
            int editingId = Convert.ToInt32(TempData.Peek("EditingId"));
            Id = editingId;
            var record = ClassInformationTable.GetClassById(Id);
            if (record != null)
            {
                ClassName = record.ClassName;
                StudentCount = record.StudentCount;
                Description = record.Description;
            }
        }
        
        
        if (TempData.ContainsKey("IsEditMode"))
        {
            IsEditMode = Convert.ToBoolean(TempData["IsEditMode"]);
            TempData.Keep("IsEditMode");
        }
        
        Console.WriteLine($"OnGet - IsEditMode: {IsEditMode}");

        var allRecords = ClassInformationTable.GetAllClasses();

        // Filter
        IEnumerable<ClassInformationModel> filtered = allRecords;
        if (!string.IsNullOrEmpty(Filter))
        {
            filtered = filtered.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase));
        }

        // Pagination 
        int totalItems = filtered.Count();
        TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);

        if (PageNumber < 1)
            PageNumber = 1;
        if (PageNumber > TotalPages)
            PageNumber = TotalPages > 0 ? TotalPages : 1;

        filtered = filtered.Skip((PageNumber - 1) * PageSize).Take(PageSize);
        ClassList = filtered.ToList();

        return Page()
    }

    public IActionResult OnPostAdd()
    {
        var newClass = new ClassInformationModel
        {
            ClassName = ClassName,
            StudentCount = StudentCount,
            Description = Description
        };

        ClassInformationTable.AddClass(newClass);
        
        return RedirectToPage(new { Filter = Filter, PageNumber = PageNumber });
    }

    public IActionResult OnPostEdit(int id)
    {
        Console.WriteLine($"OnPostEdit: Received id from parameter: {id}");
        var record = ClassInformationTable.GetClassById(id);
        if (record != null)
        {
            TempData["EditingId"] = id;          
            TempData["IsEditMode"] = true;         
        }
        
        return RedirectToPage(new { Filter = Filter, PageNumber = PageNumber });
    }

    public IActionResult OnPostDelete(int id)
    {
        ClassInformationTable.DeleteClass(id);
        
        return RedirectToPage(new { Filter = Filter, PageNumber = PageNumber });
    }

    public IActionResult OnPostUpdate()
    {
        Console.WriteLine($"OnPostUpdate: Received Id is: {Id}");

        var updatedClass = new ClassInformationModel
        {
            ClassName = ClassName,
            StudentCount = StudentCount,
            Description = Description
        };

        ClassInformationTable.EditClass(Id, updatedClass);
       
        TempData.Remove("EditingId");
        TempData.Remove("IsEditMode");

        
        return RedirectToPage(new { Filter = Filter, PageNumber = PageNumber });
    }

    public IActionResult OnPostCancel()
    {
        TempData.Remove("EditingId");
        TempData.Remove("IsEditMode");
        
        return RedirectToPage(new { Filter = Filter, PageNumber = PageNumber });
    }

    public IActionResult OnPostExport(string selectedColumns)
    {
        var allRecords = ClassInformationTable.GetAllClasses();
        IEnumerable<ClassInformationModel> filteredRecords = allRecords;

        if (!string.IsNullOrEmpty(Filter))
        {
            filteredRecords = filteredRecords.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase));
        }

        // Deserialize selected columns from JSON
        var selectedColumnList = JsonSerializer.Deserialize<List<string>>(selectedColumns);

        var jsonResult = Utils.Instance.ExportToJson(filteredRecords, selectedColumnList);

        return File(System.Text.Encoding.UTF8.GetBytes(jsonResult), "application/json", "ExportedData.json");
    }

}
