using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        const string DLL = "ConsoleApplication17.dll";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void InitManager();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void RefreshTasks();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetTasks(out int count);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void AddTask(TaskDTO task);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void UpdateTask(TaskDTO task);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void DeleteTask(int id);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void ChangeStatus(int id, int status);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetCategories(out int count);


        TaskDTO selectedTask;
        bool editMode = false;

        public MainWindow()
        {
            InitializeComponent();
            InitManager();
            LoadTasks();
        }

        void LoadTasks()
        {
            RefreshTasks();
            IntPtr ptr = GetTasks(out int count);

            var active = new List<TaskDTO>();
            var done = new List<TaskDTO>();
            var deferred = new List<TaskDTO>();

            int size = Marshal.SizeOf(typeof(TaskDTO));

            for (int i = 0; i < count; i++)
            {
                IntPtr p = new IntPtr(ptr.ToInt64() + i * size);
                TaskDTO t = (TaskDTO)Marshal.PtrToStructure(p, typeof(TaskDTO));

                if (t.StatusId == 3)
                    done.Add(t);
                else if (t.StatusId == 4)
                    deferred.Add(t);
                else
                    active.Add(t);
            }

            TasksGrid.ItemsSource = active;
            DoneGrid.ItemsSource = done;
            DeferredGrid.ItemsSource = deferred;
        }

        void OnTaskDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid == null || grid.SelectedItem == null)
                return;

            selectedTask = (TaskDTO)grid.SelectedItem;
            editMode = true;

            TitleBox.Text = selectedTask.Title;

            DeadlinePicker.SelectedDate =
                DateTimeOffset.FromUnixTimeSeconds(selectedTask.Deadline)
                              .LocalDateTime
                              .Date;

            SelectCombo(CategoryBox, selectedTask.CategoryId);
            SelectCombo(PriorityBox, selectedTask.Priority);
            SelectCombo(StatusBox, selectedTask.StatusId);
        }

        void OnSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введите название задачи");
                return;
            }

            if (!DeadlinePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату");
                return;
            }

            TaskDTO t = editMode ? selectedTask : new TaskDTO();

            t.Title = TitleBox.Text;
            t.CategoryId = GetTag(CategoryBox);
            t.Priority = GetTag(PriorityBox);
            t.StatusId = editMode ? selectedTask.StatusId : 1;
            t.Deadline = new DateTimeOffset(DeadlinePicker.SelectedDate.Value).ToUnixTimeSeconds();
            t.StatusId = GetTag(StatusBox);


            if (editMode)
                UpdateTask(t);
            else
                AddTask(t);

            ClearForm();
            LoadTasks();
        }

        void OnDelete(object sender, RoutedEventArgs e)
        {
            if (!editMode) return;
            DeleteTask(selectedTask.Id);
            ClearForm();
            LoadTasks();
        }

        void OnTaskChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int id = (int)cb.Tag;
            ChangeStatus(id, 3);
            LoadTasks();
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        void ClearForm()
        {
            editMode = false;
            TitleBox.Text = "";
            DeadlinePicker.SelectedDate = null;
            CategoryBox.SelectedIndex = -1;
            PriorityBox.SelectedIndex = -1;
        }

        int GetTag(ComboBox box)
        {
            ComboBoxItem item = box.SelectedItem as ComboBoxItem;
            return item == null ? 1 : int.Parse(item.Tag.ToString());
        }

        void SelectCombo(ComboBox box, int tag)
        {
            foreach (ComboBoxItem i in box.Items)
                if (int.Parse(i.Tag.ToString()) == tag)
                    box.SelectedItem = i;
        }
    }
}
