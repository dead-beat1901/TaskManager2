using System.Runtime.InteropServices;

namespace TaskManager
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class TaskDTO
    {
        public int Id { get; set; }

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title { get; set; }

        public int CategoryId { get; set; }

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CategoryName { get; set; }

        public int Priority { get; set; }
        public int StatusId { get; set; }
        public long Deadline { get; set; }
    }
}
