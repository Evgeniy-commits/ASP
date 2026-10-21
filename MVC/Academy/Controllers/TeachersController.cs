
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Models;
using Academy;

public class TeachersController : Controller
{
    private readonly AcademyContext _context;

    public TeachersController(AcademyContext context)
    {
        _context = context;
    }

    public IActionResult GetPhoto(int id)
    {
        var teacher = _context.Teachers.FirstOrDefault(s => s.TeacherId == id);

        if (teacher == null || teacher.Photo == null || teacher.Photo.Length == 0)
        {
            return NotFound();
        }

        string mimeType = "image/jpeg"; // дефолтное значение

        // Простая детекция по первым байтам 
        if (teacher.Photo.Length >= 2)
        {
            if (teacher.Photo[0] == 0xFF && teacher.Photo[1] == 0xD8)
                mimeType = "image/jpeg";
            else if (teacher.Photo[0] == 0x89 && teacher.Photo[1] == 0x50)
                mimeType = "image/png";
            else if (teacher.Photo[0] == 0x47 && teacher.Photo[1] == 0x49)
                mimeType = "image/gif";
        }

        return File(teacher.Photo, mimeType);
    }

    // GET: TEACHERS
    public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
    {
        ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
        if (searchString != null) pageNumber = 1;
        ViewData["CurrentFilter"] = searchString;

        //IQueryable<Teacher> teachers = from teacher in _context.Teachers select teacher;
        IQueryable<Teacher> teachers = _context.Teachers
                                .Include(t => t.TeacherDisciplines)
                                    .ThenInclude(td => td.DisciplineNavigation);

        if (!String.IsNullOrEmpty(searchString))
        {
            teachers = teachers.Where
            (
                s =>
                s.LastName.Contains(searchString) ||
                s.FirstName.Contains(searchString)
            );
        }

        switch (sortOrder)
        {
            case "name_desc": teachers = teachers.OrderByDescending(s => s.LastName); break;
            case "date_desc": teachers = teachers.OrderByDescending(s => s.BirthDate); break;
            case "Date": teachers = teachers.OrderBy(s => s.BirthDate); break;
            default: teachers = teachers.OrderBy(s => s.LastName); break;
        }

        int pageSize = 5;
        return View
            (
                await PaginatedList<Teacher>.CreateAsync
                (
                    teachers.AsNoTracking(),
                    pageNumber ?? 1,
                    pageSize
                )
            );
        //return View(await students.AsNoTracking().ToListAsync());
        //return View(await _context.Students.ToListAsync());
    }

    // GET: TEACHERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(m => m.TeacherId == id);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // GET: TEACHERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TEACHERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeacherId,LastName,FirstName,MiddleName,BirthDate,Email,Phone,Photo,WorkSince,Rate")] Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            _context.Add(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(teacher);
    }

    // GET: TEACHERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }
        return View(teacher);
    }

    // POST: TEACHERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("TeacherId,LastName,FirstName,MiddleName,BirthDate,Email,Phone,Photo,WorkSince,Rate")] Teacher teacher)
    {
        if (id != teacher.TeacherId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(teacher.TeacherId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(teacher);
    }

    // GET: TEACHERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(m => m.TeacherId == id);
        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }

    // POST: TEACHERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher != null)
        {
            _context.Teachers.Remove(teacher);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TeacherExists(int? id)
    {
        return _context.Teachers.Any(e => e.TeacherId == id);
    }
}
