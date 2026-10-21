
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Models;

public class DisciplinesController : Controller
{
    private readonly AcademyContext _context;

    public DisciplinesController(AcademyContext context)
    {
        _context = context;
    }

    // GET: DISCIPLINES
    public async Task<IActionResult> Index()    
    {
        IQueryable<Discipline>? disciplines = _context.Disciplines
                                        .Include(d => d.TeacherDisciplines)
                                            .ThenInclude(td => td.TeacherNavigation)
                                        .Include(d => d.DisciplinesDirections)
                                            .ThenInclude(dd => dd.DirectionNavigation)
                                        .AsNoTracking();
        return View(disciplines);
    }

    // GET: DISCIPLINES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var discipline = await _context.Disciplines
            .FirstOrDefaultAsync(m => m.DisciplineId == id);
        if (discipline == null)
        {
            return NotFound();
        }

        return View(discipline);
    }

    // GET: DISCIPLINES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DISCIPLINES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DisciplineId,DisciplineName,NumberOfLessons")] Discipline discipline)
    {
        if (ModelState.IsValid)
        {
            _context.Add(discipline);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(discipline);
    }

    // GET: DISCIPLINES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var discipline = await _context.Disciplines.FindAsync(id);
        if (discipline == null)
        {
            return NotFound();
        }
        return View(discipline);
    }

    // POST: DISCIPLINES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("DisciplineId,DisciplineName,NumberOfLessons")] Discipline discipline)
    {
        if (id != discipline.DisciplineId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(discipline);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DisciplineExists(discipline.DisciplineId))
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
        return View(discipline);
    }

    // GET: DISCIPLINES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var discipline = await _context.Disciplines
            .FirstOrDefaultAsync(m => m.DisciplineId == id);
        if (discipline == null)
        {
            return NotFound();
        }

        return View(discipline);
    }

    // POST: DISCIPLINES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var discipline = await _context.Disciplines.FindAsync(id);
        if (discipline != null)
        {
            _context.Disciplines.Remove(discipline);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DisciplineExists(int? id)
    {
        return _context.Disciplines.Any(e => e.DisciplineId == id);
    }
}
