using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        private const string DllPath = "ConsoleApplication17.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitManager();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RefreshTasks();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTasks(out int count);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisposeManager();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddTask(TaskDTO task);

        public MainWindow()
        {
            InitializeComponent();
            InitManager();
            LoadTasks();
        }

        private void OnLoadTasksClick(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void LoadTasks()
        {
            RefreshTasks();
            IntPtr ptr = GetTasks(out int count);

            var list = new List<TaskDTO>();
            if (ptr != IntPtr.Zero && count > 0)
            {
                int size = Marshal.SizeOf<TaskDTO>();
                for (int i = 0; i < count; i++)
                {
                    IntPtr itemPtr = IntPtr.Add(ptr, i * size);
                    list.Add(Marshal.PtrToStructure<TaskDTO>(itemPtr));
                }
            }
            TasksGrid.ItemsSource = list;
        }

        private void OnAddTaskClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle.Text))
            {
                MessageBox.Show("Введите название задачи");
                return;
            }

            TaskDTO newTask = new TaskDTO
            {
                Title = NewTaskTitle.Text,
                CategoryId = 1,
                CategoryName = "General",
                Priority = 2,
                StatusId = 1,
                Deadline = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds()
            };

            AddTask(newTask);
            NewTaskTitle.Text = "";
            LoadTasks();
        }

        protected override void OnClosed(EventArgs e)
        {
            DisposeManager();
            base.OnClosed(e);
        }
    }
}