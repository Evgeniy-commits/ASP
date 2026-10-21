
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Models;
using Academy;

public class StudentsController : Controller
{
    private readonly AcademyContext _context;

    public StudentsController(AcademyContext context)
    {
        _context = context;
    }

    public IActionResult GetPhoto(int id)
    {
        var student = _context.Students.FirstOrDefault(s => s.StudId == id);

        if (student == null || student.Photo == null || student.Photo.Length == 0)
        {
            return NotFound();
        }

        string mimeType = "image/jpeg"; // дефолтное значение

        // Простая детекция по первым байтам 
        if (student.Photo.Length >= 2)
        {
            if (student.Photo[0] == 0xFF && student.Photo[1] == 0xD8)
                mimeType = "image/jpeg";
            else if (student.Photo[0] == 0x89 && student.Photo[1] == 0x50)
                mimeType = "image/png";
            else if (student.Photo[0] == 0x47 && student.Photo[1] == 0x49)
                mimeType = "image/gif";
        }

        return File(student.Photo, mimeType);
    }

    // GET: STUDENTS
    public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
    {
        ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
        if (searchString != null) pageNumber = 1;
        ViewData["CurrentFilter"] = searchString;

        IQueryable<Student> students = _context.Students
                                .Include(s => s.GroupNavigation)
                                    .ThenInclude(g => g.DirectionNavigation);

        //IQueryable<Student> students = from student in _context.Students select student;

        if (!String.IsNullOrEmpty(searchString))
        {
            students = students.Where
            (
                s =>
                s.LastName.Contains(searchString) ||
                s.FirstName.Contains(searchString)
            );
        }

        switch (sortOrder)
        {
            case "name_desc": students = students.OrderByDescending(s => s.LastName); break;
            case "date_desc": students = students.OrderByDescending(s => s.BirthDate); break;
            case "Date": students = students.OrderBy(s => s.BirthDate); break;
            default: students = students.OrderBy(s => s.LastName); break;
        }

        int pageSize = 5;
        return View
            (
                await PaginatedList<Student>.CreateAsync
                (
                    students.AsNoTracking(),
                    pageNumber ?? 1,
                    pageSize
                )
            );
        //return View(await students.AsNoTracking().ToListAsync());
        //return View(await _context.Students.ToListAsync());
    }

    // GET: STUDENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.StudId == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: STUDENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: STUDENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StudId,LastName,FirstName,MiddleName,BirthDate,Email,Phone,Photo,Group,GroupNavigation")] Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: STUDENTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: STUDENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("StudId,LastName,FirstName,MiddleName,BirthDate,Email,Phone,Photo,Group,GroupNavigation")] Student student)
    {
        if (id != student.StudId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudId))
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
        return View(student);
    }

    // GET: STUDENTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.StudId == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: STUDENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int? id)
    {
        return _context.Students.Any(e => e.StudId == id);
    }
}
