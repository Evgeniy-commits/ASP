
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

public class EnrollmentsController : Controller
{
    private readonly ContosoUniversityContext _context;

    public EnrollmentsController(ContosoUniversityContext context)
    {
        _context = context;
    }

    // GET: ENROLLMENTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Enrollments.ToListAsync());
    }

    // GET: ENROLLMENTS/Details/5
    public async Task<IActionResult> Details(int? enrollmentid)
    {
        if (enrollmentid == null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(m => m.EnrollmentID == enrollmentid);
        if (enrollment == null)
        {
            return NotFound();
        }

        return View(enrollment);
    }

    // GET: ENROLLMENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ENROLLMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Grade,Course,Student")] Enrollment enrollment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(enrollment);
    }

    // GET: ENROLLMENTS/Edit/5
    public async Task<IActionResult> Edit(int? enrollmentid)
    {
        if (enrollmentid == null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments.FindAsync(enrollmentid);
        if (enrollment == null)
        {
            return NotFound();
        }
        return View(enrollment);
    }

    // POST: ENROLLMENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? enrollmentid, [Bind("EnrollmentID,CourseID,StudentID,Grade,Course,Student")] Enrollment enrollment)
    {
        if (enrollmentid != enrollment.EnrollmentID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(enrollment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(enrollment.EnrollmentID))
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
        return View(enrollment);
    }

    // GET: ENROLLMENTS/Delete/5
    public async Task<IActionResult> Delete(int? enrollmentid)
    {
        if (enrollmentid == null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(m => m.EnrollmentID == enrollmentid);
        if (enrollment == null)
        {
            return NotFound();
        }

        return View(enrollment);
    }

    // POST: ENROLLMENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? enrollmentid)
    {
        var enrollment = await _context.Enrollments.FindAsync(enrollmentid);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EnrollmentExists(int? enrollmentid)
    {
        return _context.Enrollments.Any(e => e.EnrollmentID == enrollmentid);
    }
}
