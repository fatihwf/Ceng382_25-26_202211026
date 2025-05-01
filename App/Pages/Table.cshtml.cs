using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.Data;

namespace App.Pages
{
    public class TableModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public TableModel(SchoolDbContext context)
        {
            _context = context;
        }

        // Binding for Add/Edit form
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string ClassName { get; set; }
        [BindProperty]
        public int StudentCount { get; set; }
        [BindProperty]
        public string Description { get; set; }

        // EF Core üzerinden çekilen liste
        public IList<Class> ClassList { get; set; }

        // TempData ile edit modu
        [TempData]
        public bool IsEditMode { get; set; } = false;

        // Query string’den gelen filtre ve sayfa numarası
        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        // Sabit sayfa başına kayıt sayısı
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Session / Cookie kontrolü 
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieToken  = Request.Cookies["token"];
            if (string.IsNullOrEmpty(sessionToken) || cookieToken != sessionToken)
                return RedirectToPage("/Index", new { error = "Session expired." });

            // Edit modu için TempData’dan değerleri al
            if (TempData.ContainsKey("EditingId"))
            {
                Id = (int)TempData.Peek("EditingId");
                var rec = await _context.Classes.FindAsync(Id);
                if (rec != null)
                {
                    ClassName    = rec.Name;
                    StudentCount = rec.PersonCount;
                    Description  = rec.Description;
                }
            }
            if (TempData.ContainsKey("IsEditMode"))
            {
                IsEditMode = (bool)TempData["IsEditMode"];
                TempData.Keep("IsEditMode");
            }

            // EF Core sorgusu: önce tüm kayıtları al, sonra filtre uygula
            IQueryable<Class> query = _context.Classes.AsNoTracking();
            if (!string.IsNullOrEmpty(Filter))
            {
                query = query.Where(c => 
                    EF.Functions.Like(c.Name, $"%{Filter}%"));
            }

            // Toplam sayfa sayısını hesapla
            var totalItems = await query.CountAsync();
            TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);
            PageNumber = System.Math.Clamp(PageNumber, 1, TotalPages > 0 ? TotalPages : 1);

            // Sayfalama
            ClassList = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            _context.Classes.Add(new Class {
                Name         = ClassName,
                PersonCount  = StudentCount,
                Description  = Description,
                IsActive     = true  // veya form’dan gelecekse BindProperty ekleyin
            });
            await _context.SaveChangesAsync();
            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostEdit(int id)
        {
            TempData["EditingId"]  = id;
            TempData["IsEditMode"] = true;
            return RedirectToPage(new { Filter, PageNumber });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var entity = await _context.Classes.FindAsync(id);
            if (entity != null)
            {
                _context.Classes.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { Filter, PageNumber });
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var entity = await _context.Classes.FindAsync(Id);
            if (entity != null)
            {
                entity.Name        = ClassName;
                entity.PersonCount = StudentCount;
                entity.Description = Description;
                await _context.SaveChangesAsync();
            }
            TempData.Remove("EditingId");
            TempData.Remove("IsEditMode");
            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostCancel()
        {
            TempData.Remove("EditingId");
            TempData.Remove("IsEditMode");
            return RedirectToPage(new { Filter, PageNumber });
        }

        public async Task<IActionResult> OnPostExportAsync(string selectedColumns)
        {
            IQueryable<Class> query = _context.Classes;
            if (!string.IsNullOrEmpty(Filter))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{Filter}%"));
            }
            var list = await query.ToListAsync();

            var cols = System.Text.Json.JsonSerializer
                         .Deserialize<List<string>>(selectedColumns);
            var json = Utils.Instance.ExportToJson(list, cols);
            return File(System.Text.Encoding.UTF8.GetBytes(json),
                        "application/json", "ExportedData.json");
        }
    }
}
