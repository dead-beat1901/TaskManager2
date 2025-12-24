using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        private const string DllPath = "ConsoleApplication17.dll";
        private int editingId = 0;

        private ObservableCollection<TaskDTO> activeTasks = new ObservableCollection<TaskDTO>();
        private ObservableCollection<TaskDTO> doneTasks = new ObservableCollection<TaskDTO>();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void InitManager();
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void RefreshTasks();
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr GetTasks(out int count);
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void AddTask(TaskDTO task);
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void UpdateTask(TaskDTO task);
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void DeleteTask(int id);
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)] static extern void ChangeStatus(int id, int status);

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                InitManager();
                TasksGrid.ItemsSource = activeTasks;
                DoneGrid.ItemsSource = doneTasks;
                LoadTasks();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message + "\nПроверьте подключение к БД или путь к DLL.");
            }
        }

        void LoadTasks()
        {
            RefreshTasks();
            IntPtr ptr = GetTasks(out int count);
            int size = Marshal.SizeOf(typeof(TaskDTO));

            activeTasks.Clear();
            doneTasks.Clear();

            for (int i = 0; i < count; i++)
            {
                TaskDTO t = (TaskDTO)Marshal.PtrToStructure(IntPtr.Add(ptr, i * size), typeof(TaskDTO));
                if (t.StatusId == 3)
                    doneTasks.Add(t);
                else
                    activeTasks.Add(t);
            }
        }

        private void OnTaskChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is int id)
            {
                ChangeStatus(id, 3);
                LoadTasks();
            }
        }

        void OnTaskSelected(object sender, SelectionChangedEventArgs e)
        {
            if (TasksGrid.SelectedItem is TaskDTO t)
            {
                editingId = t.Id;
                TitleBox.Text = t.Title;
                PriorityBox.SelectedIndex = (t.Priority - 1 >= 0) ? t.Priority - 1 : 2;
                CategoryBox.SelectedIndex = (t.CategoryId - 1 >= 0) ? t.CategoryId - 1 : 0;
                DeadlinePicker.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(t.Deadline).Date;
            }
        }

        void OnSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введите название!");
                return;
            }

            int priority = PriorityBox.SelectedIndex + 1;
            int categoryId = CategoryBox.SelectedIndex + 1;
            DateTime date = DeadlinePicker.SelectedDate ?? DateTime.Today;
            long unix = new DateTimeOffset(date.Year, date.Month, date.Day, 12, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();

            TaskDTO t = new TaskDTO
            {
                Id = editingId,
                Title = TitleBox.Text.Trim(),
                CategoryId = categoryId,
                CategoryName = "",
                Priority = priority,
                StatusId = (editingId == 0) ? 1 : ((TaskDTO)TasksGrid.SelectedItem)?.StatusId ?? 1,
                Deadline = unix
            };

            if (editingId == 0)
            {
                AddTask(t);
                activeTasks.Add(t);
            }
            else
            {
                UpdateTask(t);
                var index = activeTasks.IndexOf((TaskDTO)TasksGrid.SelectedItem);
                if (index >= 0)
                    activeTasks[index] = t;
            }

            LoadTasks();
            ClearForm();
        }

        void OnDelete(object sender, RoutedEventArgs e)
        {
            if (editingId != 0)
            {
                DeleteTask(editingId);
                var taskToRemove = TasksGrid.SelectedItem as TaskDTO;
                if (taskToRemove != null)
                    activeTasks.Remove(taskToRemove);
                LoadTasks();
                ClearForm();
            }
        }

        void OnClear(object sender, RoutedEventArgs e) => ClearForm();

        void ClearForm()
        {
            editingId = 0;
            TitleBox.Text = "";
            PriorityBox.SelectedIndex = 1;
            CategoryBox.SelectedIndex = 0;
            DeadlinePicker.SelectedDate = DateTime.Today.AddDays(1);
            TasksGrid.SelectedItem = null;
        }
    }
}
