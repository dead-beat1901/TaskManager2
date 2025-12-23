using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct TaskDTO
{
    private int _id;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    private string _title;
    private int _categoryId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    private string _categoryName;
    private int _priority;
    private int _statusId;
    private long _deadline;

    public int Id { get => _id; set => _id = value; }
    public string Title { get => _title; set => _title = value; }
    public int CategoryId { get => _categoryId; set => _categoryId = value; }
    public string CategoryName { get => _categoryName; set => _categoryName = value; }
    public int Priority { get => _priority; set => _priority = value; }
    public int StatusId { get => _statusId; set => _statusId = value; }
    public long Deadline { get => _deadline; set => _deadline = value; }

    public DateTime DeadlineDate => Deadline > 0
        ? DateTimeOffset.FromUnixTimeSeconds(Deadline).LocalDateTime
        : DateTime.Now;
}