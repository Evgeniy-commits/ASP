@page "/students"
@using Microsoft.AspNetCore.Components.QuickGrid
@using Microsoft.EntityFrameworkCore
@using Academy
@using Academy.Models
@inject IDbContextFactory<PV_521_ImportContext> DbFactory
@inject NavigationManager NavigationManager
@rendermode InteractiveServer

<PageTitle>Студенты</PageTitle>

<h1>Студенты</h1>

<p>
	<a href="students/create">Create New</a>
</p>

<div class="control">
	<div>
		<label>Группа:</label>
		<select @onchange="OnGroupFilterChanged" class="form-select">
			<option value="">Все группы</option>
			@if (Groups != null)
			{
				foreach (var g in Groups)
				{
					<option value="@g.GroupName" selected="@(g.GroupName == GroupFilter)">@g.GroupName</option>

					@*в value option кладётся название группы (оно же используется в фильтре SQL).*@
					@*selected - подсвечивает выбранный пункт, если значение совпадает с текущим фильтром.*@
				}
			}
		</select>
	</div>

	<div>
		<label>Направление:</label>
		<select @onchange="OnDirectionFilterChanged" class="form-select">
			<option value="">Все направления</option>
			@if (Directions != null)
			{
				foreach (var d in Directions)
				{
					<option value="@d.DirectionId" selected="@(d.DirectionId.ToString() == DirectionFilter)">@d.DirectionName</option>
				}
			}
		</select>
	</div>
	<div>
		<label for="items-per-page">Items per page</label>
		<select id="item-per-page" @bind="pagination.ItemsPerPage">
			<option value="3">3</option>
			<option value="5">5</option>
			<option value="8">8</option>
			<option value="10">10</option>
		</select>
	</div>
</div>

<div>
	<QuickGrid TGridItem="Student" Class="table" ItemsProvider="StudentsProvider" Pagination="pagination">
		<PropertyColumn Property="student => student.LastName" />
		<PropertyColumn Property="student => student.FirstName" />
		<PropertyColumn Property="student => student.MiddleName" />
		<PropertyColumn Property="student => student.BirthDate" />
		<PropertyColumn Property="student => student.Email" />
		<PropertyColumn Property="student => student.Phone" />
		<PropertyColumn Property="student => student.Photo" />
		<PropertyColumn Property="student => student.Group" />
		@*<PropertyColumn Property="student => student.GroupNavigation" />*@

		<TemplateColumn Title="Photo" Context="s">
			@{
				var src = ImageHelper.ToBase64ImageSrc(s.Photo, s.PhotoMimeType);
			}
			@if (src != null)
			{
				<img src="@src" alt="Photo" style="max-width: 50px; height: auto; border-radius: 4px;" />
			}
			else
			{
				<span class="text-muted">—</span>
			}
		</TemplateColumn>
		<TemplateColumn Context="student">
			@*<a href="@($"students/edit?studid={student.StudId}")">Edit</a> |*@
			<a href="@($"students/details?studid={student.StudId}")">Details</a> @*|
				<a href="@($"students/delete?studid={student.StudId}")">Delete</a>*@
		</TemplateColumn>
	</QuickGrid>

	<Paginator State="pagination" />
</div>

@code {
	private List<Group>? AllGroups { get; set; }        //полный список групп из БД. Загружается один раз и больше не меняется. Нужен, чтобы при выборе группы найти её направление.
	private List<Group>? Groups { get; set; }           //отфильтрованный список групп (по направлению). Именно он отображается в <select>.
														//Когда выбрано направление, здесь остаются только подходящие группы.
	private List<Direction>? Directions { get; set; }   //справочник направлений (ID + название).

	[SupplyParameterFromQuery]                          //Эти атрибуты связывают свойства с query-параметрами URL.
														//string? для обоих(даже для DirectionFilter, который хранит числовой ID) —
														//потому что SupplyParameterFromQuery не умеет парсить byte? напрямую.
	string? GroupFilter { get; set; }

	[SupplyParameterFromQuery]
	string? DirectionFilter { get; set; }

	// Ссылка на экземпляр QuickGrid, чтобы вызвать RefreshDataAsync
	private QuickGrid<Student>? _studentGrid;

	PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

	protected override async Task OnInitializedAsync()
	{
		// Загружаем справочники один раз при инициализации страницы
		await using var context = DbFactory.CreateDbContext();
		AllGroups = await context.Groups.OrderBy(g => g.GroupName).ToListAsync();
		Groups = await context.Groups.OrderBy(g => g.GroupName).ToListAsync();

		// Загружаем справочник направлений
		Directions = await context.Directions.OrderBy(d => d.DirectionName).ToListAsync();

		UpdateFilteredGroups();
	}

	// Провайдер для QuickGrid: каждый вызов создаёт свой контекст
	// request.StartIndex — сколько записей пропустить (offset).
	// request.Count — сколько записей взять(limit).
	private async ValueTask<GridItemsProviderResult<Student>> StudentsProvider(
	   GridItemsProviderRequest<Student> request)
	{
		await using var context = DbFactory.CreateDbContext();

		var query = context.Students.AsQueryable();

		//Если выбрана группа, добавляется условие: оставить только тех студентов, у которых
		//s.Group (FK на group_id) совпадает с ID группы, чьё GroupName равно GroupFilter
		if (!string.IsNullOrEmpty(GroupFilter))
		{
			query = query.Where(s => context.Groups
				.Any(g => g.GroupId == s.Group && g.GroupName == GroupFilter));
		}

		if (!string.IsNullOrEmpty(DirectionFilter) && byte.TryParse(DirectionFilter, out var dirId))
		{
			query = query.Where(s => context.Groups
				.Any(g => g.GroupId == s.Group && g.Direction == dirId));
		}


		//OrderBy обязателен перед Skip/Take (без него EF Core выдаёт предупреждение, а результаты могут быть непредсказуемыми).
		//CountAsync — общее количество записей(нужно пагинатору, чтобы рассчитать количество страниц).
		//Skip / Take — пагинация: пропускаем startIndex записей, берём pageSize.
		query = query.OrderBy(s => s.StudId);

		var totalCount = await query.CountAsync();
		var items = await query
			.Skip(request.StartIndex)
			.Take(request.Count ?? 100)
			.ToListAsync();

		return new GridItemsProviderResult<Student>
		{
			TotalItemCount = totalCount,
			Items = items
		};
	}

	// Группы, отфильтрованные по выбранному направлению
	// Если направление не выбрано → показываем все группы.
	// Если выбрано → оставляем только группы с совпадающим Direction.
	// Метод не обращается к БД — работает с уже загруженным AllGroups в памяти.
	private void UpdateFilteredGroups()
	{
		if (AllGroups == null)
		{
			Groups = null;
			return;
		}

		if (string.IsNullOrEmpty(DirectionFilter))
		{
			Groups = AllGroups;
		}
		else if (byte.TryParse(DirectionFilter, out var dirId))
		{
			Groups = AllGroups.Where(g => g.Direction == dirId).ToList();
		}
		else
		{
			Groups = AllGroups;
		}
	}


	private void OnGroupFilterChanged(ChangeEventArgs e)
	{
		GroupFilter = e.Value?.ToString();

		if (!string.IsNullOrEmpty(GroupFilter) && AllGroups != null)
		{
			var group = AllGroups.FirstOrDefault(g => g.GroupName == GroupFilter);
			if (group != null)
			{
				DirectionFilter = group.Direction.ToString();
			}
		}

		UpdateFilteredGroups();
		_studentGrid?.RefreshDataAsync();
	}

	private void OnDirectionFilterChanged(ChangeEventArgs e)
	{
		DirectionFilter = e.Value?.ToString();

		// Если выбрано направление — проверяем, что текущая группа ему соответствует
		UpdateFilteredGroups();

		if (!string.IsNullOrEmpty(DirectionFilter) && !string.IsNullOrEmpty(GroupFilter))
		{
			// Если выбранная группа не входит в отфильтрованный список — сбрасываем
			if (Groups != null && !Groups.Any(g => g.GroupName == GroupFilter))
			{
				GroupFilter = null;
			}
		}

		_studentGrid?.RefreshDataAsync();
	}
}
/////////////////////////////////////
@page "/students/edit"
@using Microsoft.EntityFrameworkCore
@using Academy.Models
@inject IDbContextFactory<PV_521_ImportContext> DbFactory
@inject NavigationManager NavigationManager

<PageTitle>Edit</PageTitle>

<h1>Edit</h1>

<h2>Student</h2>
<hr />
@if (Student is null)
{
	<p><em>Loading...</em></p>
}
else
{
	<div class="row">
		<div class="col-md-4">
			<EditForm method="post" Model="Student" OnValidSubmit="UpdateStudent" FormName="edit" Enhance>
				<DataAnnotationsValidator />
				<ValidationSummary role="alert" />
				<input type="hidden" name="Student.StudId" value="@Student.StudId" />
				<div class="mb-3">
					<span class="text-danger">*</span>
					<label for="lastname" class="form-label">LastName:</label>
					<InputText id="lastname" @bind-Value="Student.LastName" class="form-control" aria-required="true" />
					<ValidationMessage For="() => Student.LastName" class="text-danger" />
				</div>
				<div class="mb-3">
					<span class="text-danger">*</span>
					<label for="firstname" class="form-label">FirstName:</label>
					<InputText id="firstname" @bind-Value="Student.FirstName" class="form-control" aria-required="true" />
					<ValidationMessage For="() => Student.FirstName" class="text-danger" />
				</div>
				<div class="mb-3">
					<label for="middlename" class="form-label">MiddleName:</label>
					<InputText id="middlename" @bind-Value="Student.MiddleName" class="form-control" />
					<ValidationMessage For="() => Student.MiddleName" class="text-danger" />
				</div>
				<div class="mb-3">
					<label for="birthdate" class="form-label">BirthDate:</label>
					<InputDate id="birthdate" @bind-Value="Student.BirthDate" class="form-control" />
					<ValidationMessage For="() => Student.BirthDate" class="text-danger" />
				</div>
				<div class="mb-3">
					<label for="email" class="form-label">Email:</label>
					<InputText id="email" @bind-Value="Student.Email" class="form-control" />
					<ValidationMessage For="() => Student.Email" class="text-danger" />
				</div>
				<div class="mb-3">
					<label for="phone" class="form-label">Phone:</label>
					<InputText id="phone" @bind-Value="Student.Phone" class="form-control" />
					<ValidationMessage For="() => Student.Phone" class="text-danger" />
				</div>

				<div class="mb-3">
					<label class="form-label">Photo:</label>
					@if (Student.Photo != null && Student.Photo.Length > 0)
					{
						var src = ImageHelper.ToBase64ImageSrc(Student.Photo, Student.PhotoMimeType ?? "image/jpeg");
						<div class="mb-2">
							<img src="@src" alt="Current photo" style="max-height: 150px; border-radius: 4px;" />
						</div>
					}
					<InputFile OnChange="OnPhotoSelected" accept="image/*" class="form-control" />
					<div class="text-muted small mt-1">Выберите файл, чтобы заменить фото.</div>
				</div>
				@*<div class="mb-3">
						<label for="photo" class="form-label">Photo:</label>
						<InputFile id="photo" OnChange="OnPhotoSelected" class="form-control" />
						<ValidationMessage For="() => Student.Photo" class="text-danger" />
					</div>*@
				@*<div class="mb-3">
						<label for="group" class="form-label">Group:</label>
						<InputNumber id="group" @bind-Value="Student.Group" class="form-control" />
						<ValidationMessage For="() => Student.Group" class="text-danger" />
					</div>*@

				<div class="mb-3">
					<label for="group" class="form-label">Group:</label>
					<InputSelect id="group" @bind-Value="Student.Group" class="form-control">
						<option value="">— выберите группу —</option>
						@if (Groups != null)
						{
							foreach (var g in Groups)
							{
								<option value="@g.GroupId">@g.GroupName</option>
							}
						}
					</InputSelect>
					<ValidationMessage For="() => Student.Group" class="text-danger" />
				</div>
				@*<div class="mb-3">
					<label for="groupnavigation" class="form-label">GroupNavigation:</label>
					<InputNumber id="groupnavigation" @bind-Value="Student.GroupNavigation" class="form-control" />
					<ValidationMessage For="() => Student.GroupNavigation" class="text-danger" />
					</div>*@
				<button type="submit" class="btn btn-primary">Save</button>
				<a href="/students" class="btn btn-secondary">Cancel</a>
			</EditForm>
		</div>
	</div>
}

<div>
	<a href="/students">Back to List</a>
</div>

@code {

	private List<Group>? Groups { get; set; }
	private Student? Student { get; set; }

	[SupplyParameterFromQuery]
	private int StudId { get; set; }

	protected override async Task OnInitializedAsync()
	{
		using var context = DbFactory.CreateDbContext();
		Student = await context.Students.FirstOrDefaultAsync(m => m.StudId == StudId);

		// Загружаем список групп для выпадающего списка
		Groups = await context.Groups.OrderBy(g => g.GroupName).ToListAsync();

		if (Student is null)
		{
			NavigationManager.NavigateTo("notfound");
			return;
		}
	}

	private async Task OnPhotoSelected(InputFileChangeEventArgs e)
	{
		var file = e.File;
		if (file != null && file.Size > 0)
		{
			using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			Student!.Photo = ms.ToArray();
			Student.PhotoMimeType = file.ContentType;
		}
	}

	private async Task UpdateStudent()
	{
		await using var context = DbFactory.CreateDbContext();
		context.Attach(Student!).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
		await context.SaveChangesAsync();
		NavigationManager.NavigateTo("/students");
	}
}

/////////////////////////////////
@page "/students/details"
@using Microsoft.EntityFrameworkCore
@using Academy.Models
@inject IDbContextFactory<PV_521_ImportContext> DbFactory
@inject NavigationManager NavigationManager

<PageTitle>Details</PageTitle>

<h1>Details</h1>

<div>
	<h2>Student</h2>
	<hr />
	@if (student is null)
	{
		<p><em>Loading...</em></p>
	}
	else
	{
		<dl class="row">
			<dt class="col-sm-2">LastName</dt>
			<dd class="col-sm-10">@student.LastName</dd>
			<dt class="col-sm-2">FirstName</dt>
			<dd class="col-sm-10">@student.FirstName</dd>
			<dt class="col-sm-2">MiddleName</dt>
			<dd class="col-sm-10">@student.MiddleName</dd>
			<dt class="col-sm-2">BirthDate</dt>
			<dd class="col-sm-10">@student.BirthDate</dd>
			<dt class="col-sm-2">Email</dt>
			<dd class="col-sm-10">@student.Email</dd>
			<dt class="col-sm-2">Phone</dt>
			<dd class="col-sm-10">@student.Phone</dd>
			<dt class="col-sm-2">Photo</dt>
			<dd class="col-sm-10">
				@{
					var src = ImageHelper.ToBase64ImageSrc(student.Photo, student.PhotoMimeType);
				}
				@if (src != null)
				{
					<img src="@src" alt="Photo" style="max-height: 200px; border-radius: 8px;" />
				}
				else
				{
					<span class="text-muted">No photo</span>
				}
			</dd>
			@*<dt class="col-sm-2">Photo</dt>
				<dd class="col-sm-10">@student.Photo</dd>*@
			<dt class="col-sm-2">Group</dt>
			<dd class="col-sm-10">@student.Group</dd>
			@*<dt class="col-sm-2">GroupNavigation</dt>
				<dd class="col-sm-10">@student.GroupNavigation</dd>*@
		</dl>
		<div>
			<a href="@($"/students/edit?studid={student.StudId}")">Edit</a> |
			<a href="@($"/students")">Back to List</a>
		</div>
	}
</div>

@code {
	private Student? student;

	[SupplyParameterFromQuery]
	private int StudId { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await using var context = DbFactory.CreateDbContext();
		student = await context.Students.FirstOrDefaultAsync(m => m.StudId == StudId);

		if (student is null)
		{
			NavigationManager.NavigateTo("notfound");
			return;
		}
	}
}


/////////////////////////////////////////

@page "/students/create"
@using Microsoft.EntityFrameworkCore
@using Academy.Models
@inject IDbContextFactory<Academy.Models.PV_521_ImportContext> DbFactory
@inject NavigationManager NavigationManager

<PageTitle>Create</PageTitle>

<h1>Create</h1>

<h2>Student</h2>
<hr />
<div class="row">
	<div class="col-md-4">
		<EditForm method="post" Model="Student" OnValidSubmit="AddStudent" FormName="create" Enhance>
			<DataAnnotationsValidator />
			<ValidationSummary class="text-danger" role="alert" />
			<div class="mb-3">
				<span class="text-danger">*</span>
				<label for="lastname" class="form-label">LastName:</label>
				<InputText id="lastname" @bind-Value="Student.LastName" class="form-control" aria-required="true" />
				<ValidationMessage For="() => Student.LastName" class="text-danger" />
			</div>
			<div class="mb-3">
				<span class="text-danger">*</span>
				<label for="firstname" class="form-label">FirstName:</label>
				<InputText id="firstname" @bind-Value="Student.FirstName" class="form-control" aria-required="true" />
				<ValidationMessage For="() => Student.FirstName" class="text-danger" />
			</div>
			<div class="mb-3">
				<label for="middlename" class="form-label">MiddleName:</label>
				<InputText id="middlename" @bind-Value="Student.MiddleName" class="form-control" />
				<ValidationMessage For="() => Student.MiddleName" class="text-danger" />
			</div>
			<div class="mb-3">
				<label for="birthdate" class="form-label">BirthDate:</label>
				<InputDate id="birthdate" @bind-Value="Student.BirthDate" class="form-control" />
				<ValidationMessage For="() => Student.BirthDate" class="text-danger" />
			</div>
			<div class="mb-3">
				<label for="email" class="form-label">Email:</label>
				<InputText id="email" @bind-Value="Student.Email" class="form-control" />
				<ValidationMessage For="() => Student.Email" class="text-danger" />
			</div>
			<div class="mb-3">
				<label for="phone" class="form-label">Phone:</label>
				<InputText id="phone" @bind-Value="Student.Phone" class="form-control" />
				<ValidationMessage For="() => Student.Phone" class="text-danger" />
			</div>

			<div class="mb-3">
				<label class="form-label">Photo:</label>
				@if (Student.Photo != null && Student.Photo.Length > 0)
				{
					var src = ImageHelper.ToBase64ImageSrc(Student.Photo, Student.PhotoMimeType);
					<div class="mb-2">
						<img src="@src" alt="Selected photo" style="max-height: 150px; border-radius: 4px;" />
					</div>
				}
				<InputFile OnChange="OnPhotoSelected" accept="image/*" class="form-control" />
			</div>

			@*<div class="mb-3">
					<label for="photo" class="form-label">Photo:</label>
					<InputFile id="photo" OnChange="OnPhotoSelected" class="form-control" />
					<ValidationMessage For="() => Student.Photo" class="text-danger" />
				</div>*@
			<div class="mb-3">
				<label for="group" class="form-label">Group:</label>
				<InputNumber id="group" @bind-Value="Student.Group" class="form-control" />
				<ValidationMessage For="() => Student.Group" class="text-danger" />
			</div>
			@*<div class="mb-3">
					<label for="groupnavigation" class="form-label">GroupNavigation:</label>
					<InputSelect id="groupnavigation" @bind-Value="Student.GroupNavigation" class="form-control" >
						<option value="">— выберите группу —</option>
							@if (Groups != null)
							{
								foreach (var g in Groups)
								{
									<option value="@g.GroupId">@g.GroupName</option>
								}
							}
					</InputSelect>
					<ValidationMessage For="() => Student.GroupNavigation" class="text-danger" />
				</div>*@
			<button type="submit" class="btn btn-primary">Create</button>
			<a href="/students" class="btn btn-secondary">Cancel</a>
		</EditForm>
	</div>
</div>

<div>
	<a href="/students">Back to List</a>
</div>

@code
{
	[SupplyParameterFromForm]
	private Student Student { get; set; } = new();

	protected override void OnInitialized() => Student ??= new();

	private async Task AddStudent()
	{
		using var context = DbFactory.CreateDbContext();
		context.Students.Add(Student);
		await context.SaveChangesAsync();
		NavigationManager.NavigateTo("/students");
	}

	private async Task OnPhotoSelected(InputFileChangeEventArgs e)
	{
		var file = e.File;
		if (file != null && file.Size > 0)
		{
			using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			Student!.Photo = ms.ToArray();
			Student.PhotoMimeType = file.ContentType;
		}
	}

	private async Task UpdateStudent()
	{
		await using var context = DbFactory.CreateDbContext();
		context.Attach(Student!).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
		await context.SaveChangesAsync();
		NavigationManager.NavigateTo("/students");
	}
}





















//////////////////////////////////////////////

<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">Academy</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="nav flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
            </NavLink>
        </div>

        @*<div class="nav-item px-3">
            <NavLink class="nav-link" href="counter">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Counter
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="weather">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Weather
            </NavLink>
        </div>*@

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="disciplines">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Disciplines
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="groups">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Groups
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="students">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Students
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="teachers">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> Teachers
            </NavLink>
        </div>
    </nav>
</div>

////////////////////////////////////////////
////////////////////////////////////////////
///////////////////////////////////////////

// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Academy.Models;

public partial class PV_521_ImportContext : DbContext
{
    public PV_521_ImportContext(DbContextOptions<PV_521_ImportContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Direction> Directions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.Property(e => e.DisciplineId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.GroupId).ValueGeneratedNever();
            entity.Property(e => e.GroupName).IsFixedLength();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.Phone).IsFixedLength();

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Students).HasConstraintName("FK_Students_Groups");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.Phone).IsFixedLength();
        });

        modelBuilder.Entity<Direction>(entity =>
        {
            entity.HasKey(e => e.DirectionId);
            entity.ToTable("Directions");
            entity.Property(e => e.DirectionId).HasColumnName("direction_id");
            entity.Property(e => e.DirectionName)
                .HasColumnName("direction_name")
                .HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
} 

/////////////////////////////////
/////////////////////////////////
///////////////////////////////

using Academy.Components;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using Academy.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PV_521_ImportContext") ?? throw new InvalidOperationException("Connection string 'PV_521_ImportContext' not found.");

builder.Services.AddDbContextFactory<PV_521_ImportContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

///////////////////////////////////////////
///////////////////////////////////////////
//////////////////////////////////////////

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "PV_521_ImportContext": "Server=DESKTOP-8URQBFL\\SQLEXPRESS;Database=PV_521_Import;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=true"
  }
}