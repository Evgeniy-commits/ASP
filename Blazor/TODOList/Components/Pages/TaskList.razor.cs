namespace TODOList.Components.Pages
{
    public partial class TaskList
    {
        List<TODOList.Task> list =
        [ 
            new TODOList.Task{ Description = "Покормить кота" },
            new TODOList.Task{ Description = "Полить цветы", DONE = true },
            new TODOList.Task{ Description = "Сходить в магазин", DONE = true }
        ];
        //List<TODOList.Task> list = new List<TODOList.Task>();

        string description = "";
        void AddTask()
        {
            if (!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
            {
                TODOList.Task task = new TODOList.Task { Description = description, DONE = false };
                if (!list.Contains(task)) list.Add(task);
                description = "";
            }
        }
    }
}
