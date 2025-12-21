using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace TaskManager
{
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

        public DateTime DeadlineDate
        {
            get
            {
                try
                {
                    return Deadline > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(Deadline).DateTime.ToLocalTime()
                        : DateTime.Now;
                }
                catch { return DateTime.Now; }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private const string DllPath = "ConsoleApplication17.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern void InitManager();

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern void RefreshTasks();

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetTasks(out int count);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        public static extern void DisposeManager();

        public MainWindow()
        {
            InitializeComponent();
            try { InitManager(); }
            catch (Exception ex) { MessageBox.Show("DLL Error: " + ex.Message); }
        }

        private void OnLoadTasksClick(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshTasks();
                IntPtr ptr = GetTasks(out int count);

                if (ptr == IntPtr.Zero || count <= 0)
                {
                    MessageBox.Show("Список пуст (база данных не вернула строк).");
                    return;
                }

                var tasks = new List<TaskDTO>();
                int structSize = Marshal.SizeOf(typeof(TaskDTO));

                for (int i = 0; i < count; i++)
                {
                    IntPtr currentPtr = new IntPtr(ptr.ToInt64() + (i * structSize));
                    TaskDTO task = Marshal.PtrToStructure<TaskDTO>(currentPtr);
                    tasks.Add(task);
                }

                TasksGrid.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try { DisposeManager(); } catch { }
            base.OnClosed(e);
        }
    }
}