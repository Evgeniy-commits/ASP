
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CoursesController : Controller
{
    private readonly ContosoUniversityContext _context;

    public CoursesController(ContosoUniversityContext context)
    {
        _context = context;
    }

    // GET: COURSES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Courses.ToListAsync());
    }

    // GET: COURSES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.CourseID == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // GET: COURSES/Create
    public IActionResult Create()
    {
        PopulateDepartmentsDropDownList();
        return View();
    }

    // POST: COURSES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] Course course)
    {
        if (ModelState.IsValid)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateDepartmentsDropDownList(course.DepartmentID);
        return View(course);
    }

    // GET: COURSES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        PopulateDepartmentsDropDownList(course.DepartmentID);
        return View(course);
    }

    // POST: COURSES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("CourseID,Title,Credits,Enrollments")] Course course)
    {
        if (id != course.CourseID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.CourseID))
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
        PopulateDepartmentsDropDownList(course.DepartmentID);
        return View(course);
    }

    void PopulateDepartmentsDropDownList(object selectedDepartment = null)
    {
        IQueryable<Department> departments = 
            from department in _context.Departments
            orderby department.Name
            select department;
        ViewBag.DepartmentID = new SelectList(departments.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
    }

    // GET: COURSES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.CourseID == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: COURSES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int? id)
    {
        return _context.Courses.Any(e => e.CourseID == id);
    }
}
