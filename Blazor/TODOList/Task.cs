namespace TODOList
{
    public class Task
    {
        public string Description { get; set; }
        public bool DONE { get; set; }
        public DateTime AssignTime { get; set; }

        public override bool Equals(object? other)
        {
            return this.Description.Equals
                ((other as TODOList.Task).Description, StringComparison.OrdinalIgnoreCase);
        }
    }
}
