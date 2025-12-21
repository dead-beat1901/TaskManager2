using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct TaskDTO
{
    public int Id { get; set; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    private string _title;
    public string Title
    {
        get => _title;
        set => _title = value;
    }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
    private string _categoryName;
    public string CategoryName
    {
        get => _categoryName;
        set => _categoryName = value;
    }

    public int Priority { get; set; }
    public int StatusId { get; set; }
    public long Deadline { get; set; }

}